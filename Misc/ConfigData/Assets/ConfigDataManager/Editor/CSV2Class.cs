using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

public static class CSV2Class
{
    const string SaveFolder = "ConfigDataManager/";
    const string Space4 = "    ";
    const string Space8 = "        ";
    const string Space12 = "            ";

    [MenuItem("Assets/CSV2Class")]
    public static void TranslateCSV2Class()
    {
        if (Selection.activeObject == null)
            return;

        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(string.Format("name:{0} type:{1} path:{2}", Selection.activeObject.name, Selection.activeObject.GetType(), path));
        if (Selection.activeObject.GetType() != typeof(TextAsset))
            return;
        var content = File.ReadAllText(path);
        var fileName = Selection.activeObject.name + "Config";
        var saveFilePath = EditorUtility.SaveFilePanel("save class file", Application.dataPath + "/" + SaveFolder, "ConfigDataManager_" + fileName, "cs");
        var csvContent = CsvParser.Parse(content);
        var res = GetManagerClassContent(csvContent, fileName);
        File.WriteAllText(saveFilePath, res);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    private static string GetClassContent(List<string[]> content, string className)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(string.Format("public class {0}:ConfigDataBase", className));
        sb.AppendLine("{");
        var fieldNames = content[0];
        var fieldDescs = content[1];
        var fieldTypes = content[2];
        for (int i = 0; i < fieldNames.Length; i++)
        {
            var fieldName = fieldNames[i];
            if (string.IsNullOrEmpty(fieldName))
                continue;
            sb.AppendLine(string.Format("{1}//{0}", fieldDescs[i], Space4));
            sb.AppendLine(string.Format("{2}public {0} {1};", fieldTypes[i], fieldNames[i],Space4));
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetManagerClassContent(List<string[]> content, string className)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;");
        sb.AppendLine("");
        sb.Append(GetClassContent(content, className));


        sb.AppendLine("public partial class ConfigDataManager : MonoBehaviour {");
        //GetConfigData
        sb.AppendLine(string.Format("{1}public {0} GetConfigData{0}(int id)", className, Space4));
        sb.AppendLine(string.Format("{0}{1}", Space4, "{"));
        sb.AppendLine(string.Format("{1}{0} data = null;", className, Space8));
        sb.AppendLine(string.Format("{1}m_{0}Dic.TryGetValue(id, out data);", className, Space8));
        sb.AppendLine(string.Format("{1}return data;", className, Space8));
        sb.AppendLine(string.Format("{0}{1}", Space4, "}"));
        sb.AppendLine("");
        //GetAllConfigData
        sb.AppendLine(string.Format("    public List<{0}> GetAllConfigData{0}()", className));
        sb.AppendLine("    {");
        sb.AppendLine(string.Format("        return new List<{0}>(m_{0}Dic.Values);", className));
        sb.AppendLine("    }");
        sb.AppendLine("");
        //Init Dic
        sb.AppendLine(string.Format("{1}private void Init{0}Dic(List<string[]> strGrid)", className, Space4));
        sb.AppendLine(string.Format("{0}{1}", Space4, "{"));
        sb.AppendLine(string.Format("{1}ParseConfigDataDic<{0}>(strGrid, m_{0}Dic);", className, Space8));
        sb.AppendLine(string.Format("{0}{1}", Space4, "}"));
        sb.AppendLine("");
        //m_dic
        sb.AppendLine(string.Format("    private Dictionary<int,{0}> m_{0}Dic = new Dictionary<int,{0}>();", className));
        sb.AppendLine("}");


        return sb.ToString();
    }
}
