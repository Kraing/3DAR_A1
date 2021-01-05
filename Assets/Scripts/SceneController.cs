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

    // static fields can't be serialized
    static bool load_model = false;
    static bool load_flow = false;

    [SerializeField] float progress = 0f;
    [SerializeField] public bool model_loaded = load_model;
    [SerializeField] public bool flow_loaded = load_flow;


    // Car model field
	static int num_vertex = 8981484;
	Vector3[] vertex_pos = new Vector3[num_vertex];
	float[] pressure = new float[num_vertex];
	float max_p = 0f;
	float min_p = 0f;

    // Flow field


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
        if(!load_model)
            StartCoroutine("ReadVertexPos");
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

    /*
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("application");
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }*/

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

            // Every 20 points return to main to not freeze the scene
            if (i%10000 == 0)
            {
                progress = i*1f / num_vertex * 1f;
                yield return null;
            }
		}

        // Set model loaded flag
        load_model = true;
        model_loaded = load_model;
	}


	// Read vertex pressure
	void ReadPressure()
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

		for (int i=0; i<num_vertex ; i++)
		{
			tmp = reader.ReadBytes(4);
			pressure[i] = System.BitConverter.ToSingle(tmp, 0);
		}

		max_p = pressure.Max();
		min_p = pressure.Min();

		// Normalize the pressure from (0-1)
		float delta = (max_p - min_p) / 1f;
		for (int i=0; i<num_vertex ; i++)
		{
			pressure[i] = (pressure[i] - min_p) / delta;
		}
	}
}
