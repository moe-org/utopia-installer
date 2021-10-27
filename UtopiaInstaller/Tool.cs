using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaInstaller
{
    /// <summary>
    /// 工具类
    /// </summary>
    internal static class Tool
    {

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取一个目录下的唯一一个目录
        /// </summary>
        /// <returns></returns>
        public static string GetOnlyDir(string basePath)
        {
            var dirs = Directory.GetDirectories(basePath);

            if (dirs.Length == 0)
            {
                throw new DirectoryNotFoundException($"the base dir `{basePath}` not found");
            }
            if (dirs.Length >= 2)
            {
                logger.Error("Found much dirs");

                foreach (var dir in dirs)
                {
                    logger.Error("dir list:{dir}", dir);
                }
            }

            return dirs.First();
        }










    }
}
