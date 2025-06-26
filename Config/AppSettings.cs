
namespace Instancium.TagKit.Core.Config
{
    /// <summary>
    /// Concrete implementation of <see cref="IAppSettings"/> used
    /// for binding configuration values from appsettings.json or DI.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        /// <inheritdoc/>
        public virtual string BaseUrl { get; set; } = "/";
    }
}
