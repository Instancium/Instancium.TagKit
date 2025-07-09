using System.Collections.Concurrent;

namespace Instancium.TagKit.Core.Rendering
{
    public static class ResourceRegistry
    {
        private static readonly ConcurrentDictionary<string, string> Styles = new();
        private static readonly ConcurrentDictionary<string, string> Scripts = new();

        public static string RegisterStyle(string content)
        {
            var hash = ResourceHash.FromContent(content);
            Styles.TryAdd(hash, content);
            return hash;
        }

        public static string RegisterScript(string content)
        {
            var hash = ResourceHash.FromContent(content);
            Scripts.TryAdd(hash, content);
            return hash;
        }

        public static string? GetStyle(string hash) => Styles.TryGetValue(hash, out var c) ? c : null;
        public static string? GetScript(string hash) => Scripts.TryGetValue(hash, out var c) ? c : null;
    }

}
