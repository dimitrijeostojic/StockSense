using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace StockSense.API.OptionsSetup;

public sealed class AppOptionsSetup(IConfiguration configuration) : IConfigureOptions<AppOptions>
{
    public const string _sectionName = "App";
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public void Configure(AppOptions options)
    {
        _configuration.GetSection(_sectionName).Bind(options);
    }
}
