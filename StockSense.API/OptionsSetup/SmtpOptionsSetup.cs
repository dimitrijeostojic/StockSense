using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace StockSense.API.OptionsSetup;

public class SmtpOptionsSetup(
    IConfiguration configuration) : IConfigureOptions<SmtpOptions>
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private const string _sectionName = "Smtp";

    public void Configure(SmtpOptions options)
    {
        _configuration.GetSection(_sectionName).Bind(options);
    }
}
