using Instancium.TagKit.Core.Config;
using Instancium.TagKit.Core.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Instancium.TagKit.Core
{
    /// <summary>
    /// Provides a foundation for TagHelpers that render HTML from embedded resources with optional localization and language switching.
    /// </summary>
    public abstract class TagHelperBase : TagHelper
    {
        /// <summary>Accessor for the current HTTP context, used to retrieve headers for localization.</summary>
        protected readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>Factory used to generate localizers for resolving resource keys.</summary>
        protected readonly IStringLocalizerFactory _localizerFactory;

        /// <summary>Localizer for the current TagHelper type.</summary>
        protected IStringLocalizer? _localizer;

        /// <summary>Configuration settings injected via AppSettings.</summary>
        protected readonly IAppSettings _settings;

        private static readonly Regex LocalizationPattern = new(@"@(_[A-Za-z0-9]+_)", RegexOptions.Compiled);

        /// <summary>
        /// Optional language code (e.g., "en", "it") used to override culture detection.
        /// </summary>
        [HtmlAttributeName("lang-code")]
        public string LanguageCode { get; set; } = "en";


        /// <summary>
        /// Unique HTML element identifier for the tag helper root. This ID is injected into the
        /// rendered output as the element's DOM `id` and used for instance registration, external
        /// scripting, and hook resolution (e.g., ComponentHooks[id]).
        /// </summary>
        [HtmlAttributeName("id")]
        public string ElementId { get; set; } = $"tag-{Guid.NewGuid():N}";


        /// <summary>
        /// Initializes the TagHelper with culture resolution support and injected dependencies.
        /// </summary>
        protected TagHelperBase(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettings> options,
            IStringLocalizerFactory localizerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _settings = options.Value;
            _localizerFactory = localizerFactory;
        }

        /// <summary>
        /// Final sealed entry point for TagHelper rendering. Applies localization to generated HTML if necessary.
        /// </summary>
        public sealed override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            InitializeCulture(context);
            string html = await RenderHtmlAsync(context, output);
            output.Attributes.SetAttribute("id", ElementId);

            if (!string.IsNullOrWhiteSpace(html))
            {
                string localized = Localize(html);
                OnComponentRendered();
                output.Content.SetHtmlContent(localized);
            }
        }

        /// <summary>
        /// Resolves culture from explicit attributes or request headers and initializes the localizer.
        /// </summary>
        protected virtual void InitializeCulture(TagHelperContext context)
        {

            LanguageCode = ResolveLanguageCode(context);
            var cultureInfo = new CultureInfo(LanguageCode);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            _localizer = _localizerFactory.Create(GetType());
        }

        /// <summary>
        /// Determines the language code from context or HTTP request headers.
        /// </summary>
        protected virtual string ResolveLanguageCode(TagHelperContext context)
        {
            if (context.Items.TryGetValue("lang-code", out var val) && val is string code)
                return code;

            var header = _httpContextAccessor?.HttpContext?.Request?.Headers["X-Language-Code"].FirstOrDefault();
            return string.IsNullOrWhiteSpace(header) ? "en" : header;
        }

        /// <summary>
        /// Applies localization replacement using @(_Key_) syntax in the given HTML content.
        /// </summary>
        protected string Localize(string htmlContent)
        {
            return LocalizationPattern.Replace(htmlContent, match =>
            {
                string key = match.Groups[1].Value;
                return _localizer?[key] ?? $"[MISSING {_localizer?.GetType().Name ?? "Localizer"}: {key}]";
            });
        }

        /// <summary>
        /// Loads the HTML resource associated with the current component (auto-resolves file name by type).
        /// </summary>
        protected async Task<string> ReadOwnHtmlAsync()
        {
            var viewBuilder = new TagViewBuilder()
            {
                ComponentId = ElementId,
                UseResourceLink = _settings.UseResourceLink,
            };
            return await viewBuilder.ReadFromResourceAsync(GetHtmlResourceOwnerType());
        }

        /// <summary>
        /// Returns the type that owns the embedded view resource. Can be overridden in derived classes.
        /// </summary>
        protected virtual Type GetHtmlResourceOwnerType()
        {
            return GetType();
        }

        /// <summary>
        /// Must be implemented to generate raw HTML content to be localized and rendered.
        /// </summary>
        protected abstract Task<string> RenderHtmlAsync(TagHelperContext context, TagHelperOutput output);

        protected virtual string? GetComponentCss() => null;
        protected virtual string? GetComponentJs() => null;

        private static string ToKebabCase(string input)
        {
            return Regex.Replace(input, "(?<!^)([A-Z])", "-$1").ToLowerInvariant();
        }

        protected virtual string? ResolveTagName()
        {
            return GetType().GetCustomAttribute<HtmlTargetElementAttribute>()?.Tag
                   ?? ToKebabCase(GetType().Name.Replace("TagHelper", ""));
        }

        protected virtual void OnComponentRendered()
        {
            var tag = ResolveTagName();
            if (!string.IsNullOrWhiteSpace(tag))
                ComponentRegistry.Register(tag, GetType());
        }

    }
}

