using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.ShaderToyBrowser
{
   
    [Serializable]
    public class RenderPassData {
        public string Code { get { return code; } }

        [SerializeField]
        public string code;
        [SerializeField]
        public string name;
        [SerializeField]
        public string description;
        [SerializeField]
        public string type;
    }

    [Serializable]
    public class ShaderInfo {
        public string Name { get { return name; } }

        [SerializeField]
        public string id;
        [SerializeField]
        public string date;
        [SerializeField]
        public int viewed;
        [SerializeField]
        public string name;
        [SerializeField]
        public string description;
        [SerializeField]
        public int likes;
        [SerializeField]
        public string published;
        [SerializeField]
        public List<string> tags;
    }

    [Serializable]
    public class ShaderData {
        public string Ver { get { return ver; } }
        public RenderPassData Renderpass0 { get { return renderpass.Count>0?renderpass[0]:null; } }
        public ShaderInfo Info { get { return info; } }

        [SerializeField]
        public string ver;
        [SerializeField]
        public ShaderInfo info;
        [SerializeField]
        public List<RenderPassData> renderpass;

        public ShaderData() {
            info = new ShaderInfo() { name = "new shader" };
            renderpass = new List<RenderPassData>();
            renderpass.Add(new RenderPassData() { code = @"void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
                    // Normalized pixel coordinates (from 0 to 1)
                    vec2 uv = fragCoord / iResolution.xy;

            // Time varying pixel color
            vec3 col = 0.5 + 0.5 * cos(iTime + uv.xyx + vec3(0, 2, 4));

            // Output to screen
            fragColor = vec4(col, 1.0);
        }"});
        }
    }

    [Serializable]
    public class UserData
    {
        [SerializeField]
        public string userName;
        [SerializeField]
        public string date;
        [SerializeField]
        public int numShaders;
        [SerializeField]
        public List<ShaderData> shaders;
    }
}
