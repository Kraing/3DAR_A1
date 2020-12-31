using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MainApp : MonoBehaviour
{
	private GameObject PointCloudMesh;
	private Mesh mesh;
	// Define number of vertex
	static int num_vertex = 8981484;
	private Vector3[] vertex_pos = new Vector3[num_vertex];

	void ReadVertexPos()
	{
		// Load binary file
    	byte[] fileBytes = File.ReadAllBytes("Assets/Dataset/vertex_pos.bin");
		MemoryStream stream = new MemoryStream(fileBytes);
		BinaryReader reader = new BinaryReader(stream);

		float tmp_x;
		float tmp_y;
		float tmp_z;
		byte[] tmp;

		// Read vetex value [x y z]
		for (int i=0; i<num_vertex ; i++)
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

			// store values
			vertex_pos[i] = new Vector3(tmp_x, tmp_y, tmp_z);
		}
	}

	
	void CreateMesh()
	{
		int[] indecies = new int[num_vertex];
		Color[] colors = new Color[num_vertex];

		for(int i=0; i<num_vertex; i++)
		{
			indecies[i] = i;
			colors[i] = new Color(255, 255, 255, 1.0f);
		}

		mesh.vertices = vertex_pos;
		mesh.colors = colors;
		mesh.SetIndices(indecies, MeshTopology.Points,0);
	}
	

    // Start is called before the first frame update
    void Start()
    {
		PointCloudMesh = GameObject.Find("PointCloudMesh");
		// Load vertex pos
		ReadVertexPos();
		//Debug.Log("CloudPoints loaded");
		/*
		for(int i=0; i<10; i++)
		{
			Debug.Log("Vertex: " + vertex_pos[i][0] + vertex_pos[i][1] + vertex_pos[i][2]);
		}*/
		// Create Mesh
		mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Custom/VertexColor"));
		CreateMesh();
		Debug.Log("Mesh Created");
		PointCloudMesh.transform.Rotate(-90f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void ReadMesh()
	{
		// Load binary file
    	byte[] fileBytes = File.ReadAllBytes("Assets/Dataset/mesh.bin");
		MemoryStream stream = new MemoryStream(fileBytes);
		BinaryReader reader = new BinaryReader(stream);

		// read number of triangles
		int num_triangle = reader.ReadInt32();
		int num_t1 = 6637495;
		int num_t2 = num_triangle - num_t1;
		Debug.Log("Triangles: " + num_triangle);

		// read triangle data
		int[,] tri_mat1 = new int[3, num_t1];
		int[,] tri_mat2 = new int[3, num_t2];

		for (int j=0; j<num_t1; j++)
		{
			for(int i=0; i<3; i++)
			{
				tri_mat1[i, j] = reader.ReadInt32();
			}
		}

		for (int j=0; j<num_t2; j++)
		{
			for(int i=0; i<3; i++)
			{
				tri_mat2[i, j] = reader.ReadInt32();
			}
		}

		// Read dummy string long 40
		char[] dummy_str_1 = reader.ReadChars(8);
		char[] dummy_str_2 = reader.ReadChars(32);
		
		// Read min/max pressure
		byte[] b1 = reader.ReadBytes(4);
		byte[] b2 = reader.ReadBytes(4);
		float min_v = System.BitConverter.ToSingle(b1, 0);
		float max_v = System.BitConverter.ToSingle(b2, 0);

		// Read number of vertices
		int num_vertex = reader.ReadInt32();
		Debug.Log("Vertex: " + num_vertex);
		// Read vetex value [x y z nx ny nz pressure]
		float[,] ver_mat = new float[7, num_vertex];
		for (int j=0; j<num_vertex ; j++)
		{
			for(int i=0; i<7; i++)
			{
				byte[] tmp = reader.ReadBytes(4);
				ver_mat[i, j] = System.BitConverter.ToSingle(tmp, 0);
			}
		}

		Debug.Log("Finished to parse the data.");
	}

	(int[,], int[,]) ReadTriangles()
	{
		// Load binary file
    	byte[] fileBytes = File.ReadAllBytes("Assets/Dataset/triangles.bin");
		MemoryStream stream = new MemoryStream(fileBytes);
		BinaryReader reader = new BinaryReader(stream);

		// Set matrix length
		int num_t1 = 6637495;
		int num_t2 = 6637494;

		// read triangle data
		int[,] tri_mat1 = new int[3, num_t1];
		int[,] tri_mat2 = new int[3, num_t2];

		for (int j=0; j<num_t1; j++)
		{
			for(int i=0; i<3; i++)
			{
				tri_mat1[i, j] = reader.ReadInt32();
			}
		}

		for (int j=0; j<num_t2; j++)
		{
			for(int i=0; i<3; i++)
			{
				tri_mat2[i, j] = reader.ReadInt32();
			}
		}

		return (tri_mat1, tri_mat2);
	}
}