using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bluebean.ShaderToyBrowser
{
    public class ShaderHelper
    {
        public static Shader LoadShader(ShaderData shaderData) {
            Shader shader = null;
            try
            {
                var template = AssetLoader.Load<TextAsset>("Assets/ShaderToyBrowser/Shaders/ShaderToyFramework.shader.txt").text;
                var source = TemplateHelper.Parse(template, "shaderData", shaderData);
                Debug.Log(source);
                shader = ShaderUtil.CreateShaderAsset(source);
            }
            catch(Exception e) {
                Debug.Log(e);
            }
            return shader;
        }

        public static void ReplaceShader(Shader shader, ShaderData shaderData) {
            var template = AssetLoader.Load<TextAsset>("Assets/ShaderToyBrowser/Shaders/ShaderToyFramework.shader.txt").text;
            var source = TemplateHelper.Parse(template, "shaderData", shaderData);
            ShaderUtil.UpdateShaderAsset(shader, source);
        }
       
    }
}
