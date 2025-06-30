using Microsoft.Extensions.Options;


namespace Instancium.TagKit.Core.Config
{
    /// <summary>
    /// Adapter that exposes <see cref="AppSettings"/> as a safe read-only <see cref="IAppSettings"/> instance.
    /// </summary>
    public class AppSettingsAdapter : IAppSettings
    {
        private readonly AppSettings _options;

        /// <summary>
        /// Initializes the adapter using configured options.
        /// </summary>
        /// <param name="options">Injected options provider for <see cref="AppSettings"/>.</param>
        public AppSettingsAdapter(IOptions<AppSettings> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        public string BaseUrl => _options.BaseUrl ?? "";

        public bool UseResourceLink => _options.UseResourceLink;
    }
}

