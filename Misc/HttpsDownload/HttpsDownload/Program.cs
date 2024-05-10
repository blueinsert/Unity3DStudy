using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpsDownload
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static List<List<string>> s_argRangeList = new List<List<string>>();

        static string s_urlPath = "";

        static string s_outputPath = "./download/";

        static int ParseRange(string arg)
        {
            if(!(arg.StartsWith("[") && arg.EndsWith("]")))
            {
                Console.WriteLine(string.Format("range string:{0} format error!", arg));
                return 1;
            }
            string rangeStart = null;
            string rangeEnd = null;
            if (arg.Contains("-"))
            {
                arg = arg.Substring(1, arg.Length - 2);
                var splits = arg.Split(new char[] { '-' });
                if (splits.Length != 2)
                {
                    Console.WriteLine(string.Format("range string:{0} format error!", arg));
                    return 1;
                }
                rangeStart = splits[0];
                rangeEnd = splits[1];
            }
            else
            {
                rangeEnd = rangeStart = arg.Substring(1, arg.Length - 2);
            }
            int temp;
            bool isInt = int.TryParse(rangeStart, out temp) && int.TryParse(rangeEnd, out temp);
            if (isInt)
            {
                List<string> range = new List<string>();
                int rangeStartInt = 0;
                int rangeEndInt = 0;
                int.TryParse(rangeStart, out rangeStartInt);
                int.TryParse(rangeEnd, out rangeEndInt);
                for (int i = rangeStartInt; i <= rangeEndInt; i++)
                {
                    range.Add(i.ToString());
                }
                s_argRangeList.Add(range);
            }
            else
            {
                if(rangeStart.Length == 1 && rangeEnd.Length == 1)
                {
                    List<string> range = new List<string>();
                    char cStart = rangeStart[0];
                    char cEnd = rangeEnd[0];
                    for(char c = cStart; c <= cEnd; c = (char)((int)c + 1))
                    {
                        range.Add(c.ToString());
                    }
                    s_argRangeList.Add(range);
                }
                else
                {
                    Console.WriteLine(string.Format("range string:{0} format error!", arg));
                    return 1;
                }
            }
            return 0;
        }

        static int ParseArgs(string[] args)
        {
            int i = 0;
            for (; i < args.Length;)
            {
                var arg = args[i];
                if(arg == "-r")
                {
                    int count = int.Parse(args[i + 1]);
                    for(int j = 0; j < count; j++)
                    {
                        var rangeArg = args[i + 2 + j];
                        ParseRange(rangeArg);
                    }
                    i += count + 2;
                }else if(arg == "-o")
                {
                    s_outputPath = args[i + 1];
                    i += 2;
                }else if(arg == "-p")
                {
                    s_urlPath = args[i + 1];
                    i += 2;
                }
            }
            return 0;
        }

        static void DebugOutput(List<int> layIndexList)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < layIndexList.Count; i++)
            {
                sb.Append(layIndexList[i].ToString()).Append(" ");
            }
            Console.WriteLine(sb.ToString());
        }

        static void Traverse_Recurse(List<int> layIndexList, int curLayer)
        {
            if (curLayer == layIndexList.Count - 1)
            {
                for(int i = 0; i < s_argRangeList[curLayer].Count; i++)
                {
                    layIndexList[curLayer] = i;
                    DebugOutput(layIndexList);
                }
            }
            else
            {
                for (int i = 0; i < s_argRangeList[curLayer].Count; i++)
                {
                    layIndexList[curLayer] = i;
                    Traverse_Recurse(layIndexList, curLayer + 1);
                }
            }
        }

        static void TraverseArgRange()
        {
            List<int> layIndexList = new List<int>();
            int count = s_argRangeList.Count;
            for(int i = 0; i < count; i++)
            {
                layIndexList.Add(0);
            }
            Traverse_Recurse(layIndexList, 0);
        }

        static int Main(string[] args)
        {
            int res = 0;
            if((res = ParseArgs(args)) != 0)
            {
                return res;
            }
            TraverseArgRange();
            return 0;
            var urlPath = "https://data.lhncbc.nlm.nih.gov/public/Visible-Human/Female-Images/70mm/2K_PNG-Images/";
            var localPath = "./download/";
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            var fileNameT = "{0}{1}.png";
            List<string> urlPathList = new List<string>();
            List<string> localPathList = new List<string>();
            List<string> array1 = new List<string> { "",};
            for (int i = 3566; i <= 6189; i++)
            {
                var s1 = i.ToString();
                for(int j = 0; j < array1.Count; j++)
                {
                    var s2 = array1[j];
                    var fileName = string.Format(fileNameT, s1, s2);
                    urlPathList.Add(urlPath + fileName);
                    localPathList.Add(localPath + fileName);
                }
            }
            for(int i = 0; i < urlPathList.Count; i++)
            {
                var url = urlPathList[i];
                var savePath = localPathList[i];
                DownloadAsset(url, savePath);
            }
            Console.WriteLine("all completed!");
            var exitEvent = new System.Threading.ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();
            return 0;
        }

        static void  DownloadAsset(string fileUrl, string savePath)
        {
            int retryMax = 3;
            int retryCount = 0;

            DownloadStart:
            try {
                Console.WriteLine(string.Format("start download {0} ...", fileUrl));
                DownloadAsset_Async(fileUrl, savePath).Wait();
                Console.Write("  ok \n");
            }catch(Exception e)
            {
                retryCount++;
                if (retryCount < retryMax)
                {
                    goto DownloadStart;
                }
            }
        }

        static async Task DownloadAsset_Async(string fileUrl, string savePath)
        {
            string localFilePath = savePath; 

            try
            {
                HttpResponseMessage response = await client.GetAsync(fileUrl);
                response.EnsureSuccessStatusCode(); // 确保HTTP响应状态码表示成功  

                Stream contentStream = await response.Content.ReadAsStreamAsync();

                // 创建一个文件流来写入下载的数据  
                using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    await contentStream.CopyToAsync(fileStream);
                }

                //Console.WriteLine("文件下载完成并已保存到: " + localFilePath);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\n异常捕获: {0}", e.Message);
                throw;
            }
        }
    }
}
