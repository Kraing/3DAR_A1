using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Model : MonoBehaviour
{
    GameObject PointCloudMesh;
	GameObject Controller;
	Mesh mesh;
	// Define number of vertex
	int num_vertex;
	Vector3[] vertex_pos;
	float[] pressure;

    // Start is called before the first frame update
    void Start()
    {
        // Reference object
		PointCloudMesh = GameObject.Find("ModelMesh");
		Controller = GameObject.Find("Controller");

		// Access to loaded variable
		num_vertex = SceneController.num_vertex_m;
		vertex_pos = Controller.GetComponent<SceneController>().vertex_pos_m;
		pressure = Controller.GetComponent<SceneController>().pressure;

        // Create mesh
		//StartCoroutine("CreateMesh", 0);
        CreateMesh(0);

		// Rotate object
		PointCloudMesh.transform.Rotate(-90f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMesh(int render_color)
	{
		int[] indecies = new int[num_vertex];
		Color[] colors = new Color[num_vertex];

		mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Custom/VertexColor"));
		
		if(render_color == 0)
		{
			// Render white-gradient mesh
			for(int i=0; i<num_vertex; i++)
			{
				indecies[i] = i;
				colors[i] = Color.Lerp(Color.white, Color.black, pressure[i]);
			}
		}
		else if(render_color == 1)
		{
			// Render Color Gradient mesh
			for(int i=0; i<num_vertex; i++)
			{
				indecies[i] = i;

				if(pressure[i] < 0.6f)
				{
					colors[i] = Color.Lerp(Color.magenta, Color.blue, pressure[i] / 0.6f);
				}
				else if(pressure[i] >= 0.6f && pressure[i] < 0.65f)
				{
					colors[i] = Color.Lerp(Color.blue, Color.green, (pressure[i] - 0.6f) / 0.05f);
				}
				else if(pressure[i] >= 0.65f && pressure[i] < 0.675f)
				{
					colors[i] = Color.Lerp(Color.green, Color.yellow, (pressure[i] - 0.65f) / 0.025f);
				}
				else if(pressure[i] >= 0.675f && pressure[i] < 0.7f)
				{
					colors[i] = Color.Lerp(Color.yellow, Color.red, (pressure[i] - 0.675f) / 0.025f);
				}
				else
				{
					colors[i] = Color.Lerp(Color.red, Color.black, (pressure[i] - 0.7f) / 0.3f);
				}
			}
		}
		
		mesh.vertices = vertex_pos;
		mesh.colors = colors;
		mesh.SetIndices(indecies, MeshTopology.Points,0);
	}
}
