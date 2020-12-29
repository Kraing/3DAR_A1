using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainApp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    	// Load binary file
    	byte[] fileBytes = File.ReadAllBytes("Assets/Dataset/mesh.bin");
		MemoryStream stream = new MemoryStream(fileBytes);
		BinaryReader reader = new BinaryReader(stream);

		int num_triangle = reader.ReadInt32();
		Debug.Log("Triangles: " + num_triangle);

		// Read and print dummy strings
		char[] dummy_str = reader.ReadChars(40);
		Debug.Log("Dummy string 1: " + dummy_str);

		double min_v = reader.ReadDouble();
		Debug.Log("minimun_speed: " + min_v);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
