using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[System.Serializable]
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

    // Dynamic visualization - particle system
    private ParticleSystem ps;
    private int time_idx = 0;
    [SerializeField] float waitTime = 0.01f;
    private float timer = 0.0f;

	
    // Start is called before the first frame update
    void Start()
    {
        /*
        // Reference object
		PointCloudMesh = GameObject.Find("FlowMesh");

        // Load data from file
        ReadFlowPos();

        // Create Mesh
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Custom/VertexColor"));
        
        // Normalize the intensity value
        NormalizeIntensity();
        
        // whole flow static visualization
        CreateMesh();

        // Rotate object
        PointCloudMesh.transform.Rotate(-90f, 0f, 0f);
        ps = GetComponent<ParticleSystem>();
        */
    }

    // Update is called once per frame
    void Update()
    {
        // Update dynamic visualization
        //ParticleAnimation();
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


    void NormalizeIntensity()
    {
        max_i = intensity.Max();
		min_i = intensity.Min();
        // Normalize the pressure from (0-1)
		float delta = (max_i - min_i) / 1f;
		for (int i=0; i<num_vertex ; i++)
		{
			intensity[i] = (intensity[i] - min_i) / delta;
		}
    }


    void CreateMesh()
    {
        // Init mesh params
        int[] indecies = new int[num_vertex];
        Color[] colors = new Color[num_vertex];

        // Init mesh
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Custom/VertexColor"));

        // Loop over data
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

        // Define mesh params
        mesh.vertices = vertex_pos;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points,0);
    }


    void ParticleAnimation()
    {
        // Init particle-system variables
        int particleCount = ps.particleCount;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        ps.GetParticles(particles);

        // Update intra-frame timer
        timer += Time.deltaTime;

        // Check if update time index
        if(timer > waitTime)
        {
            // set position for each particle and color
            for( int i = 0; i < particles.Length; i++)
            {
                // Update position by delta_T
                //particles[i].position = new Vector3(vertex_pos[time_idx*num_flows + i].x, vertex_pos[time_idx*num_flows + i].y, vertex_pos[time_idx*num_flows + i].z);

                // Update particles position
                if(vertex_pos[(time_idx + 1)*num_flows + i] == new Vector3(0f,0f,0f))
                {
                    // Set position to zero if in the next time-step is zero
                    particles[i].position = new Vector3(vertex_pos[(time_idx + 1)*num_flows + i].x, vertex_pos[(time_idx + 1)*num_flows + i].y, vertex_pos[(time_idx + 1)*num_flows + i].z);
                }
                else
                {
                    // Lerp the particles position between two time-steps
                    particles[i].position = Vector3.Lerp(vertex_pos[time_idx*num_flows + i], vertex_pos[(time_idx + 1)*num_flows + i ], timer/waitTime);
                }

                // Update particles color
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

            // reset timer index if needed
            if(time_idx == time_instants - 1)
                time_idx = 0;

            // reset intra-frame timer
            timer = timer - waitTime;
        }
        else
        {
            // set position for each particle and color
            for( int i = 0; i < particles.Length; i++)
            {
                // Position Update
                if(vertex_pos[(time_idx + 1)*num_flows + i] == new Vector3(0f,0f,0f))
                {
                    particles[i].position = new Vector3(vertex_pos[(time_idx + 1)*num_flows + i].x, vertex_pos[(time_idx + 1)*num_flows + i].y, vertex_pos[(time_idx + 1)*num_flows + i].z);
                }
                else
                {
                    particles[i].position = Vector3.Lerp(vertex_pos[time_idx*num_flows + i], vertex_pos[(time_idx + 1)*num_flows + i ], timer/waitTime);
                }

                // Color Update
                if(intensity[time_idx*num_flows + i] < 0.1f)
                {
                    particles[i].startColor = Color.Lerp(Color.clear, Color.white, intensity[time_idx*num_flows + i] / 0.1f);
                }
                else
                {
                    particles[i].startColor = Color.Lerp(Color.white, Color.red, (intensity[time_idx*num_flows + i] - 0.1f) / 0.9f);
                }
            }         
        }

        // set the particles back
        ps.SetParticles(particles, particleCount);
    }
}
