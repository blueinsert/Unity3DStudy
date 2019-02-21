using System;

namespace bluebean
{
    public class eNode
    {
        public int id { get; set; }
        public float x { get; set; }
        public float y { get; set; }
    }

    public class eEdge
    {
        public int start { get; set; }
        public int end { get; set; }
    }

    public class ExportConfig
    {
        public eNode[] nodes { get; set; }
        public eEdge[] edges { get; set; }
    }

}
