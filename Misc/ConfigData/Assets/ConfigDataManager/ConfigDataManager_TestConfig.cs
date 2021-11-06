using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestConfig:ConfigDataBase
{
    //ID
    public int ID;
    //说明
    public string Desc;
    //浮点
    public float Field1;
    //整形数组
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

    private void InitTestConfigDic(List<string[]> strGrid)
    {
        ParseConfigDataDic<TestConfig>(strGrid, m_TestConfigDic);
    }

    private Dictionary<int,TestConfig> m_TestConfigDic = new Dictionary<int,TestConfig>();
}
