using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Example : MonoBehaviour
{
    public int agentCount = 100;

    public enum RVOExampleType
    {
        Circle,
        Line,
        Point,
        RandomStreams,
        Crossing
    }

    public RVOExampleType type = RVOExampleType.Circle;

    public float radius = 3;
    public float maxSpeed = 2;
    public int maxNeighbours = 10;

    /// <summary>
    /// Offset from the agent position the actual drawn postition.
    /// Used to get rid of z-buffer issues
    /// </summary>
    public Vector3 renderingOffset = Vector3.up * 0.1f;
    public float exampleScale = 100;

    /// <summary>Mesh for rendering</summary>
    Mesh mesh;

    /// <summary>Reference to the simulator in the scene</summary>
    RVO.Simulator sim;

    /// <summary>Goals for each agent</summary>
    List<Vector3> goals;

    /// <summary>Color for each agent</summary>
    List<Color> colors;

    Vector3[] verts;
    Vector2[] uv;
    int[] tris;
    Color[] meshColors;

    public float m_timeSum;

    public void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        sim = RVO.Simulator.Instance;
        sim.SetTimeStep(0.25f);
        sim.SetAgentDefaults(saftyFactor: 1, velSampleCount: 250, neighborDist: 15, maxNeighbors: 10, radius: 2, prefSpeed: 2, maxSpeed: 3, maxAccel:20);
        CreateAgents(agentCount);
    }

    public void OnGUI()
    {
        if (GUILayout.Button("2")) CreateAgents(2);
        if (GUILayout.Button("10")) CreateAgents(10);
        if (GUILayout.Button("100")) CreateAgents(100);
        if (GUILayout.Button("500")) CreateAgents(500);
        if (GUILayout.Button("1000")) CreateAgents(1000);
        if (GUILayout.Button("5000")) CreateAgents(5000);

        GUILayout.Space(5);

        if (GUILayout.Button("Random Streams"))
        {
            type = RVOExampleType.RandomStreams;
            CreateAgents(agentCount);
        }

        if (GUILayout.Button("Line"))
        {
            type = RVOExampleType.Line;
            CreateAgents(agentCount);
        }

        if (GUILayout.Button("Circle"))
        {
            type = RVOExampleType.Circle;
            CreateAgents(agentCount);
        }

        if (GUILayout.Button("Point"))
        {
            type = RVOExampleType.Point;
            CreateAgents(agentCount);
        }

        if (GUILayout.Button("Crossing"))
        {
            type = RVOExampleType.Crossing;
            CreateAgents(agentCount);
        }
    }

    private float uniformDistance(float radius)
    {
        float v = Random.value + Random.value;

        if (v > 1) return radius * (2 - v);
        else return radius * v;
    }

    /// <summary>Create a number of agents in circle and restart simulation</summary>
    public void CreateAgents(int num)
    {
        this.agentCount = num;

        goals = new List<Vector3>(agentCount);
        colors = new List<Color>(agentCount);

        sim.ClearAgents();

        if (type == RVOExampleType.Circle)
        {
            float circleRad = Mathf.Sqrt(agentCount * radius * radius * 4 / Mathf.PI) * exampleScale * 0.05f;

            for (int i = 0; i < agentCount; i++)
            {
                Vector3 pos = new Vector3(Mathf.Cos(i * Mathf.PI * 2.0f / agentCount), 0, Mathf.Sin(i * Mathf.PI * 2.0f / agentCount)) * circleRad * (1 + Random.value * 0.01f);
                sim.AddAgent(new Vector2(pos.x, pos.z));
                goals.Add(-pos);
                colors.Add(ColorUtility.HSVToRGB(i * 360.0f / agentCount, 0.8f, 0.6f));
            }
        }
        else if (type == RVOExampleType.Line)
        {
            for (int i = 0; i < agentCount; i++)
            {
                Vector3 pos = new Vector3((i % 2 == 0 ? 1 : -1) * exampleScale, 0, (i / 2) * radius * 2.5f);
                sim.AddAgent(new Vector2(pos.x, pos.z));
                goals.Add(new Vector3(-pos.x, pos.y, pos.z));
                colors.Add(i % 2 == 0 ? Color.red : Color.blue);
            }
        }
        else if (type == RVOExampleType.Point)
        {
            for (int i = 0; i < agentCount; i++)
            {
                Vector3 pos = new Vector3(Mathf.Cos(i * Mathf.PI * 2.0f / agentCount), 0, Mathf.Sin(i * Mathf.PI * 2.0f / agentCount)) * exampleScale;
                sim.AddAgent(new Vector2(pos.x, pos.z));
                goals.Add(new Vector3(0, pos.y, 0));
                colors.Add(ColorUtility.HSVToRGB(i * 360.0f / agentCount, 0.8f, 0.6f));
            }
        }
        else if (type == RVOExampleType.RandomStreams)
        {
            float circleRad = Mathf.Sqrt(agentCount * radius * radius * 4 / Mathf.PI) * exampleScale * 0.05f;

            for (int i = 0; i < agentCount; i++)
            {
                float angle = Random.value * Mathf.PI * 2.0f;
                float targetAngle = Random.value * Mathf.PI * 2.0f;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * uniformDistance(circleRad);
                sim.AddAgent(new Vector2(pos.x, pos.z));
                goals.Add(new Vector3(Mathf.Cos(targetAngle), 0, Mathf.Sin(targetAngle)) * uniformDistance(circleRad));
                colors.Add(ColorUtility.HSVToRGB(targetAngle * Mathf.Rad2Deg, 0.8f, 0.6f));
            }
        }
        else if (type == RVOExampleType.Crossing)
        {
            float distanceBetweenGroups = exampleScale * radius * 0.5f;
            int directions = (int)Mathf.Sqrt(agentCount / 25f);
            directions = Mathf.Max(directions, 2);

            const int AgentsPerDistance = 10;
            for (int i = 0; i < agentCount; i++)
            {
                float angle = ((i % directions) / (float)directions) * Mathf.PI * 2.0f;
                var dist = distanceBetweenGroups * ((i / (directions * AgentsPerDistance) + 1) + 0.3f * Random.value);
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;
                sim.AddAgent(new Vector2(pos.x, pos.z));
                goals.Add(-pos.normalized * distanceBetweenGroups * 3);
                colors.Add(ColorUtility.HSVToRGB(angle * Mathf.Rad2Deg, 0.8f, 0.6f));
            }
        }

        verts = new Vector3[4 * agentCount];
        uv = new Vector2[verts.Length];
        tris = new int[agentCount * 2 * 3];
        meshColors = new Color[verts.Length];
    }


    public void Update()
    {
        m_timeSum += Time.deltaTime;
        if (sim != null)
        {    if(m_timeSum > sim.m_timeStep)
            {
                sim.DoStep();
                m_timeSum -= sim.m_timeStep;
            }
            
        }
        for (int i = 0; i < agentCount; i++)
        {

            Vector2 pos = RVO.Simulator.Instance.GetAgentPos(i);
            var radius = RVO.Simulator.Instance.GetAgentRadius(i);

            var target = new Vector2(goals[i].x, goals[i].z);
            RVO.Simulator.Instance.SetAgentTarget(i, target);
            float orient = RVO.Simulator.Instance.GetAgentOrient(i);

            Vector3 forward = new Vector3(Mathf.Cos(orient), 0, Mathf.Sin(orient)).normalized * radius;
            if (forward == Vector3.zero) forward = new Vector3(0, 0, radius);
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            Vector3 orig = new Vector3(pos.x, 0, pos.y) + renderingOffset;

            int vc = 4 * i;
            int tc = 2 * 3 * i;
            verts[vc + 0] = (orig + forward - right);
            verts[vc + 1] = (orig + forward + right);
            verts[vc + 2] = (orig - forward + right);
            verts[vc + 3] = (orig - forward - right);

            uv[vc + 0] = (new Vector2(0, 1));
            uv[vc + 1] = (new Vector2(1, 1));
            uv[vc + 2] = (new Vector2(1, 0));
            uv[vc + 3] = (new Vector2(0, 0));

            meshColors[vc + 0] = colors[i];
            meshColors[vc + 1] = colors[i];
            meshColors[vc + 2] = colors[i];
            meshColors[vc + 3] = colors[i];

            tris[tc + 0] = (vc + 0);
            tris[tc + 1] = (vc + 1);
            tris[tc + 2] = (vc + 2);

            tris[tc + 3] = (vc + 0);
            tris[tc + 4] = (vc + 2);
            tris[tc + 5] = (vc + 3);
        }

        //Update the mesh
        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.colors = meshColors;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }
}
