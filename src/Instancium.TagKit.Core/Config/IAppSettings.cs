
namespace Instancium.TagKit.Core.Config
{
    /// <summary>
    /// Represents application-level configuration settings
    /// required by the TagKit rendering engine.
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets the base URL of the application (e.g., "/").
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// Determines whether CSS and JS resources should be linked as external URLs
        /// (served via the resource controller) instead of being inlined directly into the HTML output.
        /// 
        /// When set to <c>true</c>, component styles and scripts will be registered in the runtime
        /// registry and inserted as references (e.g., &lt;link&gt; / &lt;script src&gt;).
        /// When <c>false</c>, the assets will be inlined into the markup for self-contained rendering.
        /// </summary>
        bool UseResourceLink { get; }
    }
}



