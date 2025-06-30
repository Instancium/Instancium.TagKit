using Instancium.TagKit.Core.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Core.Runtime
{
    public class CssLinksViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var manifest = ResourceManifest.Current;
            var tags = new StringBuilder();

            foreach (var hash in manifest.StyleHashes)
                tags.AppendLine($"<link rel=\"stylesheet\" href=\"/instancium/resources/style-{hash}.css?v={hash}\" />");

            return new HtmlContentViewComponentResult(new HtmlString(tags.ToString()));
        }
    }
}
