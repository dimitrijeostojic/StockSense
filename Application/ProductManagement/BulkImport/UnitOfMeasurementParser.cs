using Domain.Enums;

namespace Application.ProductManagement.BulkImport;

internal static class UnitOfMeasurementParser
{
    private static readonly Dictionary<string, UnitOfMeasurement> _aliases =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["kom"] = UnitOfMeasurement.Piece,
            ["komad"] = UnitOfMeasurement.Piece,
            ["pcs"] = UnitOfMeasurement.Piece,
            ["kg"] = UnitOfMeasurement.Kilogram,
            ["sm"] = UnitOfMeasurement.SquareMeter,
            ["lm"] = UnitOfMeasurement.LinearMeter,
            ["kutija"] = UnitOfMeasurement.Box,
            ["pakovanje"] = UnitOfMeasurement.Box,
            ["set"] = UnitOfMeasurement.Set,
            ["komplet"] = UnitOfMeasurement.Set
            // dopuni prema svom enumu
        };

    public static bool TryParse(string? raw, out UnitOfMeasurement unit)
    {
        unit = default;

        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var key = raw.Trim();

        if (_aliases.TryGetValue(key, out unit))
            return true;

        return Enum.TryParse(key, ignoreCase: true, out unit) && Enum.IsDefined(unit);
    }

    public static string AllowedValues =>
        string.Join(", ", _aliases.Keys);
}