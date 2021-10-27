using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows;
using System.Net.Http.Headers;

namespace UtopiaInstaller
{
    /// <summary>
    /// 处理github访问
    /// </summary>
    internal static class GithubAccesser
    {

        public const string OWNER = "moe-org";
        public const string REPO = "utopia";

        private class GitTags
        {
            public List<GitTag>? Tags { get; set; } = null;
        };

        /// <summary>
        /// 获取版本号列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<GitTag>> GetVersionList()
        {
            using (HttpClient? client = new())
            {

                client
                    .DefaultRequestHeaders
                    .Accept.Clear();
                client
               .DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
                    new ProductHeaderValue("Utopia-Installer")));

                using (MemoryStream stream = new())
                {
                    await DownloadTool.DownloadAsync(
                        client,
                        $"https://api.github.com/repos/{OWNER}/{REPO}/tags",
                        stream,
                        null);


                    // 获取字符串
                    string json = Encoding.UTF8.GetString(stream.ToArray());

                    json = "{\"Tags\":" + json;
                    json += "}";

                    GitTags? tags = JsonSerializer.Deserialize<GitTags>(json);

                    return tags?.Tags ?? throw new NullReferenceException();
                }
            }
        }

    }
}
