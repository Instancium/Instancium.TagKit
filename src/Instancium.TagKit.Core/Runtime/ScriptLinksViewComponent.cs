using Instancium.TagKit.Core.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Text;


namespace Instancium.TagKit.Core.Runtime
{
    public class ScriptLinksViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var manifest = ResourceManifest.Current;
            var tags = new StringBuilder();

            tags.AppendLine("<script src=\"/instancium/resources/inst.js?v=1\" defer></script>");

            foreach (var hash in manifest.ScriptHashes)
                tags.AppendLine($"<script src=\"/instancium/resources/script-{hash}.js\" defer></script>");

            return new HtmlContentViewComponentResult(new HtmlString(tags.ToString()));
        }
    }
}
