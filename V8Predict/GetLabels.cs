using Microsoft.ML.OnnxRuntime;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace V8Predict
{
    public class GetLabels
    {
        public string[]? GetOnnxLabels(string modelPath)
        {
            string[]? resultArray = null;
            using (var session = new InferenceSession(modelPath))
            {
                
                // 获取模型元数据
                var modelMetadata = session.ModelMetadata;


                if (modelMetadata.CustomMetadataMap.ContainsKey("names"))
                {
                    string labels = modelMetadata.CustomMetadataMap["names"];

                    // 正则表达式匹配单引号中的内容
                    Regex regex = new Regex("'([^']*)'");
                    MatchCollection matches = regex.Matches(labels);

                    // 创建一个列表来存储匹配的结果
                    List<string> resultList = new List<string>();

                    // 遍历所有的匹配项
                    foreach (Match match in matches)
                    {
                        // 将匹配的内容添加到结果列表中
                        resultList.Add(match.Groups[1].Value);
                    }

                    // 将结果列表转换为数组
                    resultArray = resultList.ToArray();

                    // 打印结果数组
                    foreach (string item in resultArray)
                    {
                        Debug.WriteLine(item);
                    }

                }
                else
                {
                    Debug.WriteLine("No label information found in the model metadata.");
                }
            }
            return resultArray;
        }
    }
}
