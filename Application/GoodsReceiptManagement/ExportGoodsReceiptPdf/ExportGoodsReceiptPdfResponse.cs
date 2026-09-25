namespace Application.GoodsReceiptManagement.ExportGoodsReceiptPdf;

public sealed record ExportGoodsReceiptPdfResponse(byte[] FileContent, string FileName, string MimeType);
