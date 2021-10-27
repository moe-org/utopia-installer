using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UtopiaInstaller
{
    /// <summary>
    /// git tags. 支持序列化
    /// </summary>
    internal class GitTag
    {
        /// <summary>
        /// commit信息
        /// </summary>
        public class Commit
        {
            [JsonPropertyName("sha")]
            public string? Sha { get; set; } = null;

            [JsonPropertyName("url")]
            public string? Url { get; set; } = null;
        }

        [JsonPropertyName("name")]
        public string? Name { get; set; } = null;

        [JsonPropertyName("zipball_url")]
        public string? ZipballUrl { get; set; } = null;

        [JsonPropertyName("tarball_url")]
        public string? TarballUrl { get; set; } = null;

        [JsonPropertyName("node_id")]
        public string? NodeId { get; set; } = null;

        [JsonPropertyName("commit")]
        public Commit? GitCommit { get; set; } = null;

        public override string ToString() 
        {
            return Name ?? "null";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;


            if (obj is not GitTag other)
                return false;

            if (other.Name == Name)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return string.GetHashCode(Name);
        }
    }
}
