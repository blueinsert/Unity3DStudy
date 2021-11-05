using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestConfig:ConfigDataBase
{
    public int ID;
    public string Desc;
    public float Field1;
    public int[] Field2;
}
public partial class ConfigDataManager : MonoBehaviour {
    public TestConfig GetConfigDataTestConfig(int id)
    {
        TestConfig data = null;
        m_TestConfigDic.TryGetValue(id, out data);
        return data;
    }
    public List<TestConfig> GetAllConfigDataTestConfig()
    {
        return new List<TestConfig>(m_TestConfigDic.Values);
    }
    private Dictionary<int,TestConfig> m_TestConfigDic = new Dictionary<int,TestConfig>();
}
