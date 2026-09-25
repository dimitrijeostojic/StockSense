namespace Application.OrderManagement.ExportPurchaseOrderPdf;

public sealed record ExportPurchaseOrderPdfResponse(byte[] FileContent, string FileName, string MimeType);
