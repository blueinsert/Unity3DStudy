using UnityEngine;
using UnityEngine.Rendering;
//与之前相同的命名空间
namespace SRPStudy
{
    //定义右键添加渲染管线的功能路径
    [CreateAssetMenu(menuName = "Rendering/MyPipeline")]
    public class MyPipelineAsset : RenderPipelineAsset
    {  
        protected override RenderPipeline CreatePipeline()
        {
            //返回上一个脚本自定义的管线
            return new MyPipeline();
        }
    }
}