using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[System.Serializable]
public class Flow : MonoBehaviour
{

    GameObject PointCloudMesh;
    GameObject Controller;
	Mesh mesh;

    int num_flows;
    int time_instants;
    int num_vertex;
    Vector3[] vertex_pos;
    float[] intensity;

    // Dynamic visualization - particle system
    ParticleSystem ps;
    int time_idx = 0;
    [SerializeField] public float waitTime = 0.01f;
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        PointCloudMesh = GameObject.Find("FlowMesh");
		Controller = GameObject.Find("Controller");

        // Access to loaded variable
        num_flows = SceneController.num_flows;
        time_instants = SceneController.time_instants;
		num_vertex = SceneController.num_vertex_f;
		vertex_pos = Controller.GetComponent<SceneController>().vertex_pos_f;
		intensity = Controller.GetComponent<SceneController>().intensity;


        //StartCoroutine("CreateMesh", 0);
        //CreateMesh(0);

        // start with particle system off
        ps.Stop();
        PointCloudMesh.transform.Rotate(-90f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        ParticleAnimation();
    }


    public void CreateMesh(int render_color)
    {
        // Init mesh params
        int[] indecies = new int[num_vertex];
        Color[] colors = new Color[num_vertex];

        // Init mesh
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Custom/VertexColor"));

        if(render_color == 0)
		{
            for(int i=0; i<num_vertex; i++)
            {
                indecies[i] = i;
                colors[i] = Color.Lerp(Color.white, Color.black, intensity[i]);
            }
        }
        else if(render_color == 1)
        {
            // Render Color Gradient mesh
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

                particles[i].startSize = 0.1f;
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

                particles[i].startSize = 0.1f;
            }         
        }

        // set the particles back
        ps.SetParticles(particles, particleCount);
    }

}
