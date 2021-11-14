using UnityEngine;
using System.Collections.Generic;
using System.Text;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Example : MonoBehaviour
{

    public int m_agentCount = 2;

    public enum RVOExampleType
    {
        Circle,
        Line,
        Point,
        RandomStreams,
        Crossing
    }

    public RVOExampleType type = RVOExampleType.Crossing;
    public float m_timeStep = 1 / 60f;
    public float m_safetyFactor = 1.0f;
    public float m_radius = 3;
    public float m_maxSpeed = 2;
    public float m_maxAccel = 10;
    public int m_maxNeighbours = 10;
    public float m_neighbourDist = 15;
    public int m_velSampleCount = 10;
    public int m_angleSampleCount = 36;

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

    public List<int> m_agentIds;
    /// <summary>Goals for each agent</summary>
    Dictionary<int,Vector3> goals;

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
       
        CreateAgents(m_agentCount);
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
            CreateAgents(m_agentCount);
        }

        if (GUILayout.Button("Line"))
        {
            type = RVOExampleType.Line;
            CreateAgents(m_agentCount);
        }

        if (GUILayout.Button("Circle"))
        {
            type = RVOExampleType.Circle;
            CreateAgents(m_agentCount);
        }

        if (GUILayout.Button("Point"))
        {
            type = RVOExampleType.Point;
            CreateAgents(m_agentCount);
        }

        if (GUILayout.Button("Crossing"))
        {
            type = RVOExampleType.Crossing;
            CreateAgents(m_agentCount);
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
        this.m_agentCount = num;

        goals = new Dictionary<int, Vector3>();
        colors = new List<Color>(m_agentCount);
        m_agentIds = new List<int>(num);

        sim.Clear();
        sim.setTimeStep(m_timeStep);
        sim.setAgentDefaults(neighborDist: m_neighbourDist, maxNeighbors: m_maxNeighbours, radius: m_radius, maxSpeed: m_maxSpeed, timeHorizon: 2, timeHorizonObst: 2,velocity:new RVO.Vector2(1,1));
        
        if (type == RVOExampleType.Circle)
        {
            float circleRad = Mathf.Sqrt(m_agentCount * m_radius * m_radius * 4 / Mathf.PI) * exampleScale * 0.05f;

            for (int i = 0; i < m_agentCount; i++)
            {
                Vector3 pos = new Vector3(Mathf.Cos(i * Mathf.PI * 2.0f / m_agentCount), 0, Mathf.Sin(i * Mathf.PI * 2.0f / m_agentCount)) * circleRad * (1 + Random.value * 0.01f);
                var id = sim.addAgent(new RVO.Vector2(pos.x,pos.z));
                m_agentIds.Add(id);
                goals.Add(id, -pos);
                colors.Add(ColorUtility.HSVToRGB(i * 360.0f / m_agentCount, 0.8f, 0.6f));
            }
        }
        else if (type == RVOExampleType.Line)
        {
            for (int i = 0; i < m_agentCount; i++)
            {
                Vector3 pos = new Vector3((i % 2 == 0 ? 1 : -1) * exampleScale, 0, (i / 2) * m_radius * 2.5f);
                var id = sim.addAgent(new RVO.Vector2(pos.x, pos.z));
                m_agentIds.Add(id);
                goals.Add(id,new Vector3(-pos.x, pos.y, pos.z));
                colors.Add(i % 2 == 0 ? Color.red : Color.blue);
            }
        }
        else if (type == RVOExampleType.Point)
        {
            for (int i = 0; i < m_agentCount; i++)
            {
                Vector3 pos = new Vector3(Mathf.Cos(i * Mathf.PI * 2.0f / m_agentCount), 0, Mathf.Sin(i * Mathf.PI * 2.0f / m_agentCount)) * m_radius;
                var id = sim.addAgent(new RVO.Vector2(pos.x, pos.z));
                m_agentIds.Add(id);
                //sim.AddAgent(new Vector2(0, 0));
                goals.Add(id, new Vector3(0, pos.y, 0));
                colors.Add(ColorUtility.HSVToRGB(i * 360.0f / m_agentCount, 0.8f, 0.6f));
            }
        }
        else if (type == RVOExampleType.RandomStreams)
        {
            float circleRad = Mathf.Sqrt(m_agentCount * m_radius * m_radius * 4 / Mathf.PI) * exampleScale * 0.05f;

            for (int i = 0; i < m_agentCount; i++)
            {
                float angle = Random.value * Mathf.PI * 2.0f;
                float targetAngle = Random.value * Mathf.PI * 2.0f;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * uniformDistance(circleRad);
                var id = sim.addAgent(new RVO.Vector2(pos.x, pos.z));
                m_agentIds.Add(id);
                goals.Add(id, new Vector3(Mathf.Cos(targetAngle), 0, Mathf.Sin(targetAngle)) * uniformDistance(circleRad));
                colors.Add(ColorUtility.HSVToRGB(targetAngle * Mathf.Rad2Deg, 0.8f, 0.6f));
            }
        }
        else if (type == RVOExampleType.Crossing)
        {
            float distanceBetweenGroups = exampleScale * m_radius * 0.5f;
            int directions = (int)Mathf.Sqrt(m_agentCount / 25f);
            directions = Mathf.Max(directions, 2);

            const int AgentsPerDistance = 10;
            for (int i = 0; i < m_agentCount; i++)
            {
                float angle = ((i % directions) / (float)directions) * Mathf.PI * 2.0f;
                var dist = distanceBetweenGroups * ((i / (directions * AgentsPerDistance) + 1) + 0.3f * Random.value);
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * dist;
                var id = sim.addAgent(new RVO.Vector2(pos.x, pos.z));
                m_agentIds.Add(id);
                goals.Add(id, -pos.normalized * distanceBetweenGroups * 3);
                colors.Add(ColorUtility.HSVToRGB(angle * Mathf.Rad2Deg, 0.8f, 0.6f));
            }
        }

        verts = new Vector3[4 * m_agentCount];
        uv = new Vector2[verts.Length];
        tris = new int[m_agentCount * 2 * 3];
        meshColors = new Color[verts.Length];
    }

    private StringBuilder m_sb = new StringBuilder();

    public void Update()
    {
        m_timeSum += Time.deltaTime;
        if (sim != null)
        {    if(m_timeSum > sim.timeStep_)
            {
                sim.doStep();
                m_timeSum -= sim.timeStep_;
            }
        }
        m_sb.Length = 0;
        foreach (var i in m_agentIds)
        {
            var pos = RVO.Simulator.Instance.getAgentPosition(i);
            var vel = RVO.Simulator.Instance.getAgentVelocity(i);
            var radius = RVO.Simulator.Instance.getAgentRadius(i);
            float orient = Mathf.Atan2(vel.y(), vel.x());
            m_sb.Append(string.Format("id:{0} pos:{1} vel:{2} orieng:{3}", i, pos, vel, orient));
            m_sb.AppendLine("");
            var target = new RVO.Vector2(goals[i].x, goals[i].z);
            RVO.Simulator.Instance.setAgentPrefVelocity(i, RVO.RVOMath.normalize(target-pos)*m_maxSpeed);
            

            Vector3 forward = new Vector3(Mathf.Cos(orient), 0, Mathf.Sin(orient)).normalized * radius;
            if (forward == Vector3.zero) forward = new Vector3(0, 0, radius);
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            Vector3 orig = new Vector3(pos.x(), 0, pos.y()) + renderingOffset;

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
        Debug.Log(m_sb.ToString());
        //Update the mesh
        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.colors = meshColors;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }
}
