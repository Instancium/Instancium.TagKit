using System.Collections.Concurrent;

namespace Instancium.TagKit.Core.Rendering
{
    public static class ComponentRegistry
    {
        private static readonly ConcurrentDictionary<string, Type> Map = new();

        public static void Register(string tag, Type type)
            => Map[tag.ToLowerInvariant()] = type;

        public static Type? Resolve(string tag)
            => Map.TryGetValue(tag.ToLowerInvariant(), out var t) ? t : null;
    }

}
