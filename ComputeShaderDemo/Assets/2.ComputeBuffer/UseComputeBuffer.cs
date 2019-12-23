using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseComputeBuffer : MonoBehaviour
{
    public ComputeShader shader;

    struct Data
    {
        public float A;
        public float B;
        public float C;
    }

    void Start()
    {
        Data[] inputData = new Data[3];
        Data[] outputData = new Data[3];

        print("輸入 --------------------------------------------");
        for (int i = 0; i < inputData.Length; i++)
        {
            inputData[i].A = i * 3 + 1;
            inputData[i].B = i * 3 + 2;
            inputData[i].C = i * 3 + 3;
            print(inputData[i].A + ", " + inputData[i].B + ", " + inputData[i].C);
        }

        // Data 有3個float，一個 float 有 4 Byte，所以 3 * 4 = 12
        ComputeBuffer inputbuffer = new ComputeBuffer(outputData.Length, 12);
        ComputeBuffer outputbuffer = new ComputeBuffer(outputData.Length, 12);

        int k = shader.FindKernel("CSMain");

        // 寫入 GPU
        inputbuffer.SetData(inputData);
        shader.SetBuffer(k, "inputData", inputbuffer);

        // 計算，並輸出至 CPU
        shader.SetBuffer(k, "outputData", outputbuffer);
        shader.Dispatch(k, outputData.Length, 1, 1);
        outputbuffer.GetData(outputData);


        print("輸出 --------------------------------------------");
        // 打印結果
        for (int i = 0; i < outputData.Length; i++)
        {
            print(outputData[i].A + ", " + outputData[i].B + ", " + outputData[i].C);
        }

        // 釋放
        inputbuffer.Dispose();
        outputbuffer.Dispose();
    }
}
