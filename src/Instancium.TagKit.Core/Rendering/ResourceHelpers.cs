namespace Instancium.TagKit.Core.Rendering
{
    public static class ResourceHelpers
    {
        public static string RegisterAndTrackScript(string content)
        {
            var hash = ResourceRegistry.RegisterScript(content);
            ResourceManifest.Current.AddScript(hash);
            return hash;
        }

        public static string RegisterAndTrackStyle(string content)
        {
            var hash = ResourceRegistry.RegisterStyle(content);
            ResourceManifest.Current.AddStyle(hash);
            return hash;
        }
    }

}
