using Domain.Enums;

namespace Application.ProductManagement.BulkImport;

internal sealed class CurrencyParser
{
    private static readonly Dictionary<string, Currency> _aliases =
    new(StringComparer.OrdinalIgnoreCase)
    {
        ["rsd"] = Currency.RSD,
        ["din"] = Currency.RSD,
        ["dinara"] = Currency.RSD,
        ["eur"] = Currency.EUR,
        ["eura"] = Currency.EUR,
        ["evra"] = Currency.EUR,
        ["€"] = Currency.EUR,
        ["usd"] = Currency.USD,
        ["dolara"] = Currency.USD,
        ["$"] = Currency.USD
    };


    public static bool TryParse(string? raw, out Currency currency)
    {
        currency = default;

        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var key = raw.Trim();

        if (_aliases.TryGetValue(key, out currency))
            return true;

        return Enum.TryParse(key, ignoreCase: true, out currency) && Enum.IsDefined(currency);
    }

    public static string AllowedValues =>
        string.Join(", ", _aliases.Keys);
}
