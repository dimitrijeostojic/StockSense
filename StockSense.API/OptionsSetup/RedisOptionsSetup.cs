using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace StockSense.API.OptionsSetup;

public class RedisOptionsSetup(IConfiguration configuration) : IConfigureOptions<RedisOptions>
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly string _sectionName = "Redis";

    public void Configure(RedisOptions options)
    {
        _configuration.GetSection(_sectionName).Bind(options);
    }
}