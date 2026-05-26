using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace StockSense.API.OptionsSetup;

public sealed class RabbitMqOptionsSetup(IConfiguration configuration) : IConfigureOptions<RabbitMqOptions>
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly string _sectionName = "RabbitMQ";

    public void Configure(RabbitMqOptions options)
    {
        _configuration.GetSection(_sectionName).Bind(options);
    }
}
