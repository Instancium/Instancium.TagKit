
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
    }
}



