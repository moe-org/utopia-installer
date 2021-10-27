using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UtopiaInstaller
{
    /// <summary>
    /// 一些设置
    /// </summary>
    internal class GameArchive
    {
        public const string SETTINGS_FILE = "./settings.json";

        /// <summary>
        /// 游戏设置
        /// </summary>
        public static GameArchive archive = new();

        /// <summary>
        /// 保存配置
        /// </summary>
        public static void SaveSettings()
        {
            File.WriteAllText(SETTINGS_FILE,
                JsonSerializer.Serialize(archive), Encoding.UTF8);
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        public static void ReadSettings()
        {
            if (!File.Exists(SETTINGS_FILE))
            {
                File.WriteAllText(SETTINGS_FILE,"{}",Encoding.UTF8);
            }
            else
            {
                GameArchive archive = 
                    JsonSerializer.Deserialize<GameArchive>(File.ReadAllText(SETTINGS_FILE,Encoding.UTF8))!;

                GameArchive.archive = archive;
            }
        }

        /// <summary>
        /// 游戏参数
        /// </summary>
        public class GameVersion
        {
            public GitTag? Tag { get; set; } = null;
            public string Path { get; set; } = string.Empty;

            public override string ToString()
{
                return Tag?.Name ?? "null";
            }

            public override bool Equals(object? obj)
            {
                if (obj == null)
                    return false;


                if (obj is not GameVersion other)
                    return false;

                if (other.Tag?.Name == Tag?.Name)
                    return true;
                else
                    return false;
            }

            public override int GetHashCode()
{
                return string.GetHashCode(Tag?.Name);
            }
        }

        /// <summary>
        /// jvm参数
        /// </summary>
        public class JvmSetting
        {
            public string[] Arguments {  get; set; } = Array.Empty<string>();
        }

        /// <summary>
        /// 游戏版本
        /// </summary>
        public List<GameVersion> GameVersions { get; set; } = new List<GameVersion>();


    }
}
