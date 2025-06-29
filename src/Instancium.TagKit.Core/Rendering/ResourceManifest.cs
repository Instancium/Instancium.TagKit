using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Core.Rendering
{
    public class ResourceManifest
    {
        public HashSet<string> StyleHashes { get; } = new();
        public HashSet<string> ScriptHashes { get; } = new();

        public void AddStyle(string hash) => StyleHashes.Add(hash);
        public void AddScript(string hash) => ScriptHashes.Add(hash);

        public static ResourceManifest Current
        {
            get
            {
                var context = new HttpContextAccessor().HttpContext;
                const string key = "__inst_resource_manifest";

                if (context == null) return new ResourceManifest();

                if (!context.Items.TryGetValue(key, out var existing) || existing is not ResourceManifest manifest)
                {
                    manifest = new ResourceManifest();
                    context.Items[key] = manifest;
                }

                return manifest;
            }
        }
    }
}
