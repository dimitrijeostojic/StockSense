using Application.Abstractions.Services;
using Application.Common.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Infrastructure.Pdf;

public sealed class GoodsReceiptPdfGenerator : IGoodsReceiptPdfGenerator
{
    public byte[] Generate(GoodsReceiptPdfData data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            if (data.TenantLogo is { Length: > 0 })
                            {
                                col.Item().Height(50).Image(data.TenantLogo).FitArea();
                                col.Item().PaddingTop(4);
                            }

                            col.Item().Text(data.TenantName).FontSize(13).Bold();
                            if (!string.IsNullOrWhiteSpace(data.TenantPib))
                                col.Item().Text($"PIB: {data.TenantPib}");
                            if (!string.IsNullOrWhiteSpace(data.TenantAddress))
                                col.Item().Text(data.TenantAddress);

                            col.Item().PaddingTop(8);
                            col.Item().Text(data.SupplierName).Bold();
                            if (!string.IsNullOrWhiteSpace(data.SupplierAddress))
                                col.Item().Text(data.SupplierAddress);
                            if (!string.IsNullOrWhiteSpace(data.SupplierCity))
                                col.Item().Text(data.SupplierCity);
                            if (!string.IsNullOrWhiteSpace(data.SupplierCountry))
                                col.Item().Text(data.SupplierCountry);
                        });

                        row.ConstantItem(180).Column(col =>
                        {
                            col.Item().AlignRight().Text("PRIJEMNICA").FontSize(16).Bold();
                            col.Item().AlignRight().Text($"Porudzbenica: {data.OrderPublicId[..8]}");
                            col.Item().AlignRight().Text($"Datum porudzbine: {data.OrderDate:dd.MM.yyyy}");
                            col.Item().AlignRight().Text($"Datum prijema: {data.ReceivedAt:dd.MM.yyyy}").Bold();
                        });
                    });

                    column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(15);

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Šifra artikla").Bold();
                            header.Cell().Text("Opis").Bold();
                            header.Cell().AlignCenter().Text("JM").Bold();
                            header.Cell().AlignRight().Text("Naručeno").Bold();
                            header.Cell().AlignRight().Text("Primljeno").Bold();
                            header.Cell().AlignRight().Text("Razlika").Bold();

                            header.Cell().ColumnSpan(6).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                        });

                        foreach (var item in data.Items)
                        {
                            var diffColor = item.Difference == 0
                                ? Colors.Black
                                : item.Difference < 0
                                    ? Colors.Red.Medium
                                    : Colors.Green.Medium;

                            table.Cell().PaddingVertical(4).Text(item.Sku);
                            table.Cell().PaddingVertical(4).Text(item.ProductName);
                            table.Cell().PaddingVertical(4).AlignCenter().Text(item.UnitOfMeasure);
                            table.Cell().PaddingVertical(4).AlignRight().Text(item.OrderedQuantity.ToString());
                            table.Cell().PaddingVertical(4).AlignRight().Text(item.ReceivedQuantity.ToString());
                            table.Cell().PaddingVertical(4).AlignRight().Text(item.Difference.ToString()).FontColor(diffColor);
                        }
                    });

                    if (!string.IsNullOrWhiteSpace(data.Notes))
                    {
                        column.Item().Column(col =>
                        {
                            col.Item().Text("Napomena:").Bold();
                            col.Item().Text(data.Notes);
                        });
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated by StockSense").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });

        return document.GeneratePdf();
    }
}
