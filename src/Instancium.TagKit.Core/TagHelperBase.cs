using Instancium.TagKit.Core.Config;
using Instancium.TagKit.Core.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Instancium.TagKit.Core
{
    /// <summary>
    /// Provides a base class for TagHelpers that render HTML from embedded resources,
    /// with support for localization, language switching, and dynamic component registration.
    /// </summary>
    public abstract class TagHelperBase : TagHelper
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IAppSettings _settings;

        /// <summary>
        /// Language code used for localization (e.g. "en", "fr", "de").
        /// Can be set via the "lang-code" attribute or resolved from headers.
        /// </summary>
        [HtmlAttributeName("lang-code")]
        public string LanguageCode { get; set; } = "en";

        /// <summary>
        /// Unique ID for the rendered component. Can be overridden via the "id" attribute.
        /// </summary>
        [HtmlAttributeName("id")]
        public string ElementId { get; set; } = $"tag-{Guid.NewGuid():N}";

        protected TagHelperBase(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettings> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _settings = options.Value;
        }

        /// <summary>
        /// Main entry point for rendering the component.
        /// Initializes culture, renders HTML, and registers the component.
        /// </summary>
        public sealed override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            InitializeCulture(context);
            string html = await RenderHtmlAsync(context, output);
            output.Attributes.SetAttribute("id", ElementId);

            if (!string.IsNullOrWhiteSpace(html))
            {
                OnComponentRendered();
                output.Content.SetHtmlContent(html);
            }
        }

        /// <summary>
        /// Sets the current thread culture based on the resolved language code.
        /// </summary>
        protected virtual void InitializeCulture(TagHelperContext context)
        {
            LanguageCode = ResolveLanguageCode(context);
        }

        /// <summary>
        /// Resolves the language code from the "lang-code" attribute or request headers.
        /// </summary>
        protected virtual string ResolveLanguageCode(TagHelperContext context)
        {
            var attr = context.AllAttributes["lang-code"];
            if (attr is not null && attr.Value is string code && !string.IsNullOrWhiteSpace(code))
                return code;

            var header = _httpContextAccessor?.HttpContext?.Request?.Headers["X-Language-Code"].FirstOrDefault();
            return string.IsNullOrWhiteSpace(header) ? "en" : header;
        }

        /// <summary>
        /// Reads the component's HTML template from an embedded resource.
        /// </summary>
        protected async Task<string> ReadOwnHtmlAsync()
        {
            var viewBuilder = new TagViewBuilder()
            {
                ComponentId = ElementId,
                UseResourceLink = _settings.UseResourceLink,
                LanguageCode = LanguageCode,
            };

            return await viewBuilder.ReadFromResourceAsync(GetHtmlResourceOwnerType());
        }

        /// <summary>
        /// Returns the type that owns the embedded HTML resource.
        /// Defaults to the current component type.
        /// </summary>
        protected virtual Type GetHtmlResourceOwnerType() => GetType();

        /// <summary>
        /// Must be implemented by derived components to render their HTML.
        /// </summary>
        protected abstract Task<string> RenderHtmlAsync(TagHelperContext context, TagHelperOutput output);

        /// <summary>
        /// Optionally returns the component's CSS resource path or inline content.
        /// </summary>
        protected virtual string? GetComponentCss() => null;

        /// <summary>
        /// Optionally returns the component's JavaScript resource path or inline content.
        /// </summary>
        protected virtual string? GetComponentJs() => null;

        /// <summary>
        /// Resolves the tag name for the component, either from the HtmlTargetElement attribute or class name.
        /// </summary>
        protected virtual string? ResolveTagName()
        {
            return GetType().GetCustomAttribute<HtmlTargetElementAttribute>()?.Tag
                   ?? ToKebabCase(GetType().Name.Replace("TagHelper", ""));
        }

        /// <summary>
        /// Registers the component in the runtime registry after rendering.
        /// </summary>
        protected virtual void OnComponentRendered()
        {
            var tag = ResolveTagName();
            if (!string.IsNullOrWhiteSpace(tag))
                ComponentRegistry.Register(tag, GetType());
        }

        /// <summary>
        /// Converts a PascalCase name to kebab-case.
        /// </summary>
        private static string ToKebabCase(string input)
        {
            return Regex.Replace(input, "(?<!^)([A-Z])", "-$1").ToLowerInvariant();
        }
    }
}
