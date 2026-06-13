using System.Globalization;
using Business.Application.Common.Interfaces;
using Business.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Business.Infrastructure.Services;

public class AcceptanceProtocolPdfGenerator : IAcceptanceProtocolPdfGenerator
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-DE");

    public byte[] Generate(Order order, OrderAcceptance acceptance)
    {
        var signatureBytes = DecodeSignatureImage(acceptance.SignatureImage);

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text("Abnahmeprotokoll")
                    .FontSize(20).Bold();

                page.Content().Column(column =>
                {
                    column.Spacing(12);

                    column.Item().PaddingTop(10).Text("Auftragsdetails").FontSize(14).Bold();
                    column.Item().Text($"Auftrag: {order.Title}");

                    if (!string.IsNullOrWhiteSpace(order.Customer))
                    {
                        column.Item().Text($"Kunde / Baustelle: {order.Customer}");
                    }

                    if (!string.IsNullOrWhiteSpace(order.Description))
                    {
                        column.Item().Text($"Beschreibung: {order.Description}");
                    }

                    if (order.PlannedStartDate.HasValue || order.PlannedEndDate.HasValue)
                    {
                        column.Item().Text($"Zeitraum: {FormatDate(order.PlannedStartDate)} - {FormatDate(order.PlannedEndDate)}");
                    }

                    column.Item().PaddingTop(20).Text("Kundenabnahme").FontSize(14).Bold();
                    column.Item().Text($"Datum/Uhrzeit: {acceptance.SignedAt.ToLocalTime():dd.MM.yyyy HH:mm}");
                    column.Item().Text($"Unterzeichner: {acceptance.SignerName}");
                    column.Item().PaddingTop(5).Height(120).Image(signatureBytes).FitArea();
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static string FormatDate(DateOnly? date) =>
        date?.ToString("dd.MM.yyyy", Culture) ?? "-";

    private static byte[] DecodeSignatureImage(string signatureImage)
    {
        var commaIndex = signatureImage.IndexOf(',');
        var base64 = commaIndex >= 0 ? signatureImage[(commaIndex + 1)..] : signatureImage;
        return Convert.FromBase64String(base64);
    }
}
