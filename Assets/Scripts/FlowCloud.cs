using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FlowCloud : MonoBehaviour
{
    private GameObject PointCloudMesh;
	private Mesh mesh;
    static int num_flows = 603;
    static int time_instants = 125;

    static int num_vertex = num_flows * time_instants;
    private Vector3[] vertex_pos = new Vector3[num_vertex];
    private float[] intensity = new float[num_vertex];
    private float max_i = 0f;
	private float min_i = 0f;

    private Vector3[] dynamic_lines = new Vector3[num_flows];
    int time_counter = 1;


    // particle system
    private ParticleSystem ps;
    private int time_idx = 0;
    private float waitTime = 0.1f;
    private float timer = 0.0f;

	
    // Start is called before the first frame update
    void Start()
    {
        // Reference object
		PointCloudMesh = GameObject.Find("FlowMesh");
        ReadFlowPos();

        // Create Mesh
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Custom/VertexColor"));
        
        // whole flow static visualization
        //CreateMesh();

        max_i = intensity.Max();
		min_i = intensity.Min();
        // Normalize the pressure from (0-1)
		float delta = (max_i - min_i) / 1f;
		for (int i=0; i<num_vertex ; i++)
		{
			intensity[i] = (intensity[i] - min_i) / delta;
		}

        // dynamic visualization with mesh regeneration
        //CreateLineMesh();
        

        Debug.Log("Mesh Created");
        // Rotate object
        PointCloudMesh.transform.Rotate(-90f, 0f, 0f);
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        particle_animation();
        /* Mesh regeneration method
        int[] indecies = new int[num_flows];
        Color[] colors = new Color[num_flows];


        for(int i=0; i<num_flows; i++)
        {
            if(time_counter == 125)
                time_counter = 0;

            indecies[i] = i;

            if(intensity[time_counter*num_flows + i] < 0.1f)
            {
                colors[i] = Color.Lerp(Color.clear, Color.white, intensity[time_counter * num_flows + i] / 0.1f);
            }
            else
            {
                colors[i] = Color.Lerp(Color.white, Color.red, (intensity[time_counter * num_flows + i] - 0.1f) / 0.9f);
            }
                
            dynamic_lines[i].x = vertex_pos[time_counter*num_flows + i].x;
            dynamic_lines[i].y = vertex_pos[time_counter*num_flows + i].y;
            dynamic_lines[i].z = vertex_pos[time_counter*num_flows + i].z;
            time_counter ++;
            //Debug.Log(time_counter);
        }

        mesh.vertices = dynamic_lines;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);
        */
    }

    // Read flowpoints coordinates
    void ReadFlowPos()
    {
        // Load binary file
        string file_name = "flow_lines2.bytes";
        string tmp_path = Path.Combine(Application.streamingAssetsPath, file_name);
        byte[] fileBytes;

        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(tmp_path);
            www.SendWebRequest();
            while (!www.isDone) ;
            fileBytes = www.downloadHandler.data;

        }
        else
        {
            fileBytes = File.ReadAllBytes(tmp_path);
        }

        MemoryStream stream = new MemoryStream(fileBytes);
        BinaryReader reader = new BinaryReader(stream);

        float tmp_x;
        float tmp_y;
        float tmp_z;
        byte[] tmp;

        int idx = 0;
        // Read vetex value [x y z]
        for (int i=0; i<time_instants ; i++)
        {
            for (int j=0; j<num_flows ; j++)
            {

                // save x
                tmp = reader.ReadBytes(4);
                tmp_x = System.BitConverter.ToSingle(tmp, 0);

                // save y
                tmp = reader.ReadBytes(4);
                tmp_y = System.BitConverter.ToSingle(tmp, 0);

                // save z
                tmp = reader.ReadBytes(4);
                tmp_z = System.BitConverter.ToSingle(tmp, 0);

                // throw away time data
                tmp = reader.ReadBytes(4);

                // read intensity value
                tmp = reader.ReadBytes(4);
                intensity[idx] = System.BitConverter.ToSingle(tmp, 0);

                // store [x,y,z] values
                vertex_pos[idx] = new Vector3(tmp_x, tmp_y, tmp_z);
                idx ++;

            }
        }
    }

    void CreateMesh()
    {
        int[] indecies = new int[num_vertex];
        Color[] colors = new Color[num_vertex];

        // normalize intensity
        max_i = intensity.Max();
		min_i = intensity.Min();
        // Normalize the pressure from (0-1)
		float delta = (max_i - min_i) / 1f;
		for (int i=0; i<num_vertex ; i++)
		{
			intensity[i] = (intensity[i] - min_i) / delta;
		}


        for(int i=0; i<num_vertex; i++)
        {
            indecies[i] = i;

            if(intensity[i] < 0.1f)
			{
				colors[i] = Color.Lerp(Color.clear, Color.white, intensity[i] / 0.1f);
			}
            else
            {
                colors[i] = Color.Lerp(Color.white, Color.red, (intensity[i] - 0.1f) / 0.9f);
            }
        }

        mesh.vertices = vertex_pos;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points,0);
    }


    // Create line mesh
    void CreateLineMesh()
    {
        int[] indecies = new int[num_flows];
        Color[] colors = new Color[num_flows];


        for(int i=0; i<num_flows; i++)
        {
            indecies[i] = i;
            dynamic_lines[i] = new Vector3(vertex_pos[i].x, vertex_pos[i].y, vertex_pos[i].z);
            colors[i] = new Color(1f, 1f, 1f, 1f);
        }
        mesh.vertices = dynamic_lines;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points,0);
    }


    // Dynamic visualization of flow vertex
    IEnumerator dynamic_view()
    {
        for(int i=0; i<num_flows; i++)
        {
            if(time_counter == 125)
                time_counter = 0;

            dynamic_lines[i].x = vertex_pos[time_counter*num_flows + i].x;
            dynamic_lines[i].y = vertex_pos[time_counter*num_flows + i].y;
            dynamic_lines[i].z = vertex_pos[time_counter*num_flows + i].z;
            time_counter ++;
        }
        yield return null;
    }



    void particle_animation()
    {
        timer += Time.deltaTime;

        if(timer > waitTime)
        {
            int particleCount = ps.particleCount;
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
            ps.GetParticles(particles);

            // reset timer if exceed a value
            if(time_idx == time_instants)
                time_idx = 0;


            // set position for each particle and color
            for( int i = 0; i < particles.Length; i++)
            {
                particles[i].position = new Vector3(vertex_pos[time_idx*num_flows + i].x, vertex_pos[time_idx*num_flows + i].y, vertex_pos[time_idx*num_flows + i].z);

                if(intensity[time_idx*num_flows + i] < 0.1f)
                {
                    particles[i].startColor = Color.Lerp(Color.clear, Color.white, intensity[time_idx*num_flows + i] / 0.1f);
                }
                else
                {
                    particles[i].startColor = Color.Lerp(Color.white, Color.red, (intensity[time_idx*num_flows + i] - 0.1f) / 0.9f);
                }
            }

            // update timer index
            time_idx ++;

            // set the particles back
            ps.SetParticles(particles, particleCount);
            timer = timer - waitTime;
        }
    }
}
