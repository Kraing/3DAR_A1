using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;
using System.Linq;


[System.Serializable]
public class SceneController : MonoBehaviour
{
    // Controller field
    [SerializeField] GameObject Controller;

    // Loading progress var
    [SerializeField] public float progress_model;
    [SerializeField] public float progress_pressure;
    [SerializeField] public float progress_flow;

    // Car model field
	static int num_vertex_m = 8981484;
	Vector3[] vertex_pos_m = new Vector3[num_vertex_m];
	float[] pressure = new float[num_vertex_m];
	float max_p = 0f;
	float min_p = 0f;

    // Flow field
	static int num_flows = 603;
    static int time_instants = 125;
	static int num_vertex_f = num_flows * time_instants;
	Vector3[] vertex_pos_f = new Vector3[num_vertex_f];
    float[] intensity = new float[num_vertex_f];
    float max_i = 0f;
	float min_i = 0f;


	// Start is called before the first frame update
    void Start()
    {
		// Init progressbar
		progress_model = 0f;
    	progress_pressure = 0f;
    	progress_flow = 0f;
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }*/


    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");

        // Keep only one Controller per scene
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        // Let the gameobject persist over the scenes
        DontDestroyOnLoad(Controller);
    }


	// Menu scene swithcer
    public void LoadingApp()
    {
        SceneManager.LoadScene("loading");

        // Load model data at first startup
        if(progress_model == 0f)
            StartCoroutine("ReadVertexPos");

        if(progress_pressure == 0f)
            StartCoroutine("ReadPressure");
        
		if(progress_flow == 0f)
            StartCoroutine("ReadFlowPos");

    }

	public void StartApp()
    {
    	//SceneManager.LoadScene("application");
        SceneManager.LoadSceneAsync("application");
    }
    
    public void CreditsApp()
    {
    	SceneManager.LoadScene("credits");
    }

    public void ExitApp()
    {
    	Debug.Log("exit");  
        Application.Quit(); 
    }

    // Credits scene switcher
    public void BackMainMenu()
    {
    	Debug.Log("menu");  
        SceneManager.LoadScene("menu");
    }


    // Read vertex coordinates
	IEnumerator ReadVertexPos()
	{
		// Load binary file
		string file_name = "vertex_pos.bytes";
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

		// Read vetex value [x y z]
		for (int i=0; i<num_vertex_m ; i++)
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
			vertex_pos_m[i] = new Vector3(tmp_x, tmp_y, tmp_z);

            // Every 100000 points return to main to not freeze the scene
            if (i%10000 == 0)
            {
                progress_model = ((i * 1f) / (num_vertex_m * 1f)) * 100;
                yield return null;
            }
		}

        // Update progress
        progress_model = 100f;
	}


	// Read vertex pressure
	IEnumerator ReadPressure()
	{
		string file_name = "vertex_pressure.bytes";
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
		byte[] tmp;

		for (int i=0; i<num_vertex_m ; i++)
		{
			tmp = reader.ReadBytes(4);
			pressure[i] = System.BitConverter.ToSingle(tmp, 0);

            // Every 20 points return to main to not freeze the scene
            if (i%10000 == 0)
            {
                progress_pressure = ((i*1f) / (num_vertex_m * 1f)) * 100;
                yield return null;
            }
		}

		NormalizeValues("model");
		// Update progress
        progress_pressure = 100f;
	}


	IEnumerator ReadFlowPos()
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
                vertex_pos_f[idx] = new Vector3(tmp_x, tmp_y, tmp_z);
                idx ++;

            }

			// Every 10 time steps return to no freeze the scene
            if (i%10 == 0)
            {
                progress_flow = ((i*num_flows*1f) / (num_vertex_f * 1f)) * 100;
                yield return null;
            }

			NormalizeValues("flow");
			progress_flow = 100f;
        }
    }

	void NormalizeValues(string data_type)
    {
		if(data_type == "model")
		{
			max_p = pressure.Max();
			min_p = pressure.Min();

			// Normalize the pressure from (0-1)
			float delta = (max_p - min_p) / 1f;
			for (int i=0; i<num_vertex_m ; i++)
			{
				pressure[i] = (pressure[i] - min_p) / delta;
			}
		}

		if(data_type == "flow")
		{
			max_i = intensity.Max();
			min_i = intensity.Min();
			// Normalize the pressure from (0-1)
			float delta = (max_i - min_i) / 1f;
			for (int i=0; i<num_vertex_f; i++)
			{
				intensity[i] = (intensity[i] - min_i) / delta;
			}
		}
    }

    
}
