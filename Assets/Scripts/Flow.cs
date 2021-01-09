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

    // Define colors
    Color c1;
    Color c2;
    Color c3;
    Color c4;
    Color c5;
    Color c6;


    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        PointCloudMesh = GameObject.Find("FlowMesh");
		Controller = GameObject.Find("Controller");

        // Init color palete
        c1 = new Color(114f/255f, 37f/255f, 135f/255f, 1);
        c2 = new Color(8f/255f, 29f/255f, 88f/255f, 1);
        c3 = new Color(34f/255f, 94f/255f, 168f/255f, 1);
        c4 = new Color(65f/255f, 182f/255f, 196f/255f, 1);
        c5 = new Color(199f/255f, 233f/255f, 180f/255f, 1);
        c6 = new Color(255f/255f, 255f/255f, 217f/255f, 1);

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

                // Old colors
                /*
                if(intensity[i] < 0.1f)
                {
                    colors[i] = Color.Lerp(Color.clear, Color.white, intensity[i] / 0.1f);
                }
                else
                {
                    colors[i] = Color.Lerp(Color.white, Color.red, (intensity[i] - 0.1f) / 0.9f);
                }
                */

                // new color palette
                if(intensity[i] < 0.1f)
				{
					colors[i] = Color.Lerp(c6, c5, intensity[i] / 0.1f);
				}
				else if(intensity[i] >= 0.1f && intensity[i] < 0.4f)
				{
					colors[i] = Color.Lerp(c5, c4, (intensity[i] - 0.1f) / 0.3f);
				}
				else if(intensity[i] >= 0.4f && intensity[i] < 0.7f)
				{
					colors[i] = Color.Lerp(c4, c3, (intensity[i] - 0.4f) / 0.3f);
				}
				else if(intensity[i] >= 0.7f && intensity[i] < 0.85f)
				{
					colors[i] = Color.Lerp(c3, c2, (intensity[i] - 0.7f) / 0.15f);
				}
				else
				{
					colors[i] = Color.Lerp(c2, c1, (intensity[i] - 0.85f) / 0.15f);
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

                // Color Update olds
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
