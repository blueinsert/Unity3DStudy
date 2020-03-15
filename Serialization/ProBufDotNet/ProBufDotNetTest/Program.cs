using System.Collections.Generic;
using System.IO;

namespace ProtoTest_1
{
    [ProtoBuf.ProtoContract]
    class MyClass
    {
        [ProtoBuf.ProtoMember(1)]
        public int _nNumber;
        [ProtoBuf.ProtoMember(2)]
        public string _strName;
        [ProtoBuf.ProtoMember(3)]
        public List<string> _lstInfo;
        [ProtoBuf.ProtoMember(4)]
        public Dictionary<int, string> _dictInfo;
    }

    class Program
    {
        static void Main(string[] args)
        {
            //测试用数据
            MyClass my = new MyClass();
            my._nNumber = 0;
            my._strName = "test";
            my._lstInfo = new List<string>();
            my._lstInfo.Add("a");
            my._lstInfo.Add("b");
            my._lstInfo.Add("c");
            my._dictInfo = new Dictionary<int, string>();
            my._dictInfo.Add(1, "a");
            my._dictInfo.Add(2, "b");
            my._dictInfo.Add(3, "c");

            using (FileStream stream = File.OpenWrite("test.dat"))
            {
                //序列化后的数据存入文件
                ProtoBuf.Serializer.Serialize<MyClass>(stream, my);
            }

            MyClass my2 = null;
            using (FileStream stream = File.OpenRead("test.dat"))
            {
                //从文件中读取数据并反序列化
                my2 = ProtoBuf.Serializer.Deserialize<MyClass>(stream);
            }
        }
    }
}