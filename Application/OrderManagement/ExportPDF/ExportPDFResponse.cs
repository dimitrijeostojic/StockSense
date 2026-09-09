namespace Application.OrderManagement.ExportPDF;

public sealed record ExportPDFResponse(byte[] FileContent, string FileName, string MimeType);
