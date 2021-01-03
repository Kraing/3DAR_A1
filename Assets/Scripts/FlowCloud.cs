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
        CreateMesh();
        Debug.Log("Mesh Created");
        // Rotate object
        PointCloudMesh.transform.Rotate(-90f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
