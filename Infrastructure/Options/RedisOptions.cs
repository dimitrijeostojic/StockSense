namespace Infrastructure.Options;

public sealed class RedisOptions
{
    public required string ConnectionString { get; init; }
}
