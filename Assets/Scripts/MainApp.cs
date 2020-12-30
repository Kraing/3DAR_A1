using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainApp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    	(int[,] tri_mat1, int[,] tri_mat2) = ReadTriangles();
		
		Debug.Log(tri_mat1[0 , 0]);

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
