// part of project utopia
// under MIT license
// mode by moe-org
// All Rights Reserved
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UtopiaInstaller
{
    /// <summary>
    /// 下载工具
    /// </summary>
    internal class DownloadTool
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public const string TEMP_DIR = "./temp";

        public const string RUNTIME_TEMP_FILE = $"{TEMP_DIR}/runtime-17.zip";

        public const string RUNTIME_DIR = "./runtime";

        public const string RUNTIME_DOWNLOAD_URL = "https://download.oracle.com/java/17/latest/jdk-17_windows-x64_bin.zip";

        public static bool CheckRuntime()
        {
            return Directory.Exists(RUNTIME_DIR);
        }

        /// <remarks>
        /// note:进度条范围0-1
        /// </remarks>
        private static async Task CopyToAsync(
            Stream source,
            Stream destination,
            int bufferSize,
            IProgress<long>? progress = null,
            CancellationToken cancellationToken = default)
        {
            // 检查参数
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            // 循环读取缓冲区
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                // 写入到目标
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                // 更新进度
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        /// <remarks>
        /// note:进度条范围0-1
        /// </remarks>
        public static async Task DownloadAsync(
            HttpClient client,
            string requestUri,
            Stream destination,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default)
        {
            // 获取链接
            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                var contentLength = response.Content.Headers.ContentLength;

                // 获取异步流
                using (var download = await response.Content.ReadAsStreamAsync())
                {
                    // 不需要进度条 || 未知长度
                    if (progress == null || !contentLength.HasValue)
                    {
                        // 直接copy
                        var buffer = new byte[8192];
                        var bytesRead = 0;
                        while ((bytesRead=await download.ReadAsync(buffer).ConfigureAwait(false)) != 0)
                        {
                            // 写入到目标
                            await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        // 带进度条的copy
                        var relativeProgress =
                            new Progress<long>(totalBytes => progress.Report((double)totalBytes / contentLength.Value));

                        await CopyToAsync(download, destination, 81920, relativeProgress, cancellationToken);

                        // 读取完成，报告1
                        progress.Report(1);
                    }
                }
            }
        }

        /// <remarks>
        /// note:进度条范围0-1。
        /// 
        /// 下载到临时文件->解压文件(zip)->删除临时文件
        /// </remarks>
        public static async Task DownloadAndExtractAndDeleteTemp(
            string url,
            string tempFile,
            string outputDir,
            IProgress<double> progress)
        {
            using var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 64,
            };

            using (HttpClient? client = new(handler)) {

                // 设置client的一些信息
                client
                    .DefaultRequestHeaders
                    .Accept.Clear();
                client
               .DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
                    new ProductHeaderValue("Utopia-Installer")));

                // 创建临时目录
                if (!Directory.Exists(TEMP_DIR))
                {
                    _ = Directory.CreateDirectory(TEMP_DIR);
                }

                using FileStream fs = File.Create(tempFile, 4096);

                // 下载
                await DownloadAsync(client, url, fs, progress);
            }

            // 解压
            ZipFile.ExtractToDirectory(tempFile, outputDir,Encoding.UTF8,true);

            // 删除临时文件
            File.Delete(tempFile);

            return;
        }

        /// <remarks>
        /// note:进度条范围0-1
        /// </remarks>
        public static async Task DownloadRuntime(
            string url,
            IProgress<double> progress)
        {
            await DownloadAndExtractAndDeleteTemp(
                url,
                RUNTIME_TEMP_FILE,
                RUNTIME_DIR,
                progress);
        }

        public static void RemoveRuntime()
        {
            Directory.Delete(RUNTIME_DIR,true);
        }

        /// <summary>
        /// 获取JAVA_HOME
        /// </summary>
        /// <returns></returns>
        public static string? GetRuntime()
        {
            if (!CheckRuntime())
                return null;

            return Tool.GetOnlyDir(RUNTIME_DIR);
        }

    }
}
