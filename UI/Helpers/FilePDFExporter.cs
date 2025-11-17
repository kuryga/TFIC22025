using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using BLL.Audit;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using BE;

namespace Utils.Reporting
{
    public sealed class FilePDFExporter
    {
        public static string GenerarPdfTemporal(
            DateTime? desde, DateTime? hasta, int? page, int? pageSize, string criticidad,
            bool exportarTodos, string empresa, byte[] logo, string tempRootDir = null)
        {
            string baseDir = !string.IsNullOrWhiteSpace(tempRootDir)
                ? tempRootDir
                : Path.Combine(Path.GetTempPath(), AppDomain.CurrentDomain.FriendlyName);

            Directory.CreateDirectory(baseDir);

            string rutaJson = BitacoraBLL.GetInstance()
                .ExportarReporte(desde, hasta, page, pageSize, criticidad, baseDir, exportarTodos);

            if (string.IsNullOrWhiteSpace(rutaJson) || !File.Exists(rutaJson))
                return null;

            string tempPdf = Path.Combine(baseDir, "Bitacora_" + Guid.NewGuid().ToString("N") + ".pdf");
            ExportFromJsonFile(rutaJson, tempPdf, empresa, logo);
            return tempPdf;
        }

        public static void ExportFromJsonFile(string inputJsonPath, string outputPdfPath, string empresa, byte[] logoBytes = null)
        {
            var L = new Func<string, string, string>((code, fb) => ParametrizacionBLL.GetInstance().GetLocalizable(code) ?? fb);

            if (string.IsNullOrWhiteSpace(inputJsonPath))
                throw new ArgumentException(L("input_path_empty_error", "Ruta de entrada vacía."));

            var json = File.ReadAllText(inputJsonPath);
            ExportFromJsonText(json, outputPdfPath, empresa, logoBytes);
        }

        public static void ExportFromJsonText(string json, string outputPdfPath, string empresa, byte[] logoBytes = null)
        {
            var L = new Func<string, string, string>((code, fb) => ParametrizacionBLL.GetInstance().GetLocalizable(code) ?? fb);

            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException(L("json_empty_error", "JSON vacío."));
            if (string.IsNullOrWhiteSpace(outputPdfPath))
                throw new ArgumentException(L("output_path_empty_error", "Ruta de salida vacía."));

            var data = JsonConvert.DeserializeObject<BitacoraExport>(json) ?? new BitacoraExport
            {
                generadoEn = DateTime.Now,
                registros = new List<Registro>(),
                filtros = new Filtros()
            };

            GenerarPdf(data, outputPdfPath, empresa, logoBytes);
        }

        private class BitacoraExport
        {
            public DateTime generadoEn { get; set; }
            public Filtros filtros { get; set; }
            public int? totalRegistros { get; set; }
            public List<Registro> registros { get; set; }
        }

        private class Filtros
        {
            public string desde { get; set; }
            public string hasta { get; set; }
            public string criticidad { get; set; }
            public bool exportarTodos { get; set; }
            public int? pagina { get; set; }
            public int? tamanoPagina { get; set; }
        }

        private class Registro
        {
            public int? IdRegistro { get; set; }
            public string Fecha { get; set; }
            public string Criticidad { get; set; }
            public string Accion { get; set; }
            public string Mensaje { get; set; }
            public int? IdEjecutor { get; set; }
            public string UsuarioEjecutor { get; set; }
        }

        private static readonly BaseColor CorporateGreen = new BaseColor(0, 102, 64);
        private static readonly BaseColor RowAlt = new BaseColor(234, 242, 234);
        private static readonly BaseColor LightGray = new BaseColor(245, 245, 245);

        private static void GenerarPdf(BitacoraExport data, string rutaSalida, string empresa, byte[] logoBytes)
        {
            var L = new Func<string, string, string>((code, fb) => ParametrizacionBLL.GetInstance().GetLocalizable(code) ?? fb);

            var dir = Path.GetDirectoryName(rutaSalida);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            using (var fs = new FileStream(rutaSalida, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var doc = new Document(PageSize.A4, 36, 36, 100, 50))
            {
                var writer = PdfWriter.GetInstance(doc, fs);

                Image logo = null;
                if (logoBytes != null && logoBytes.Length > 0)
                {
                    logo = Image.GetInstance(logoBytes);
                    logo.ScaleToFit(110f, 110f);
                    logo.Alignment = Image.ALIGN_LEFT;
                }

                string reportTitle = L("report_title", "Reporte de Bitácora del Sistema");
                string generatedAtLabel = L("report_generated_at_label", "Generado");
                string subtitle = $"{empresa} | {generatedAtLabel}: {data.generadoEn:yyyy-MM-dd HH:mm}";
                string footer = $"{empresa} • {DateTime.Now:yyyy-MM-dd HH:mm}";

                var pageEvents = new HeaderFooterEvent
                {
                    Titulo = reportTitle,
                    Subtitulo = subtitle,
                    Logo = logo,
                    Pie = footer,
                    PageLabel = L("page_label", "Página")
                };
                writer.PageEvent = pageEvents;

                doc.Open();

                var fLbl = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
                var fTxt = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                var tablaFiltros = new PdfPTable(6) { WidthPercentage = 100 };
                tablaFiltros.SetWidths(new float[] { 1.1f, 1.6f, 1.0f, 1.0f, 1.0f, 1.2f });

                Action<string, string> AddPair = (l, v) =>
                {
                    tablaFiltros.AddCell(new PdfPCell(new Phrase(l, fLbl)) { BackgroundColor = LightGray, Padding = 4 });
                    tablaFiltros.AddCell(new PdfPCell(new Phrase(v ?? string.Empty, fTxt)) { Padding = 4 });
                };

                AddPair(L("filter_from_label", "Desde"), data.filtros?.desde);
                AddPair(L("filter_to_label", "Hasta"), data.filtros?.hasta);

                var allVal = L("filter_all_value", "Todas");
                AddPair(L("filter_criticality_label", "Criticidad"),
                        !string.IsNullOrWhiteSpace(data.filtros?.criticidad) ? data.filtros.criticidad : allVal);

                string yesTxt = L("yes_label", "Sí");
                string noTxt = L("no_label", "No");
                AddPair(L("filter_export_all_label", "Exportar todos"), data.filtros?.exportarTodos == true ? yesTxt : noTxt);

                AddPair(L("page_label", "Página"), ((data.filtros?.pagina ?? 1)).ToString());
                AddPair(L("page_size_label", "Tamaño pág."), ((data.filtros?.tamanoPagina ?? 0)).ToString());
                doc.Add(tablaFiltros);

                doc.Add(new Paragraph(" "));

                var table = new PdfPTable(6) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 1.0f, 1.5f, 0.8f, 1.6f, 3.6f, 1.8f });

                var fHead = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);

                Th(table, L("report_col_id", "ID"), fHead);
                Th(table, L("report_col_date", "Fecha"), fHead);
                Th(table, L("report_col_criticality", "Crit."), fHead);
                Th(table, L("report_col_action", "Acción"), fHead);
                Th(table, L("report_col_message", "Mensaje"), fHead);
                Th(table, L("report_col_user", "Usuario"), fHead);

                var fRow = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                bool zebra = false;

                foreach (var r in data.registros ?? Enumerable.Empty<Registro>())
                {
                    var bg = zebra ? RowAlt : BaseColor.WHITE;
                    zebra = !zebra;

                    table.AddCell(Td(r.IdRegistro, fRow, Element.ALIGN_CENTER, bg));
                    table.AddCell(Td(r.Fecha, fRow, Element.ALIGN_CENTER, bg));
                    table.AddCell(Td(r.Criticidad, fRow, Element.ALIGN_CENTER, bg));
                    table.AddCell(Td(r.Accion, fRow, Element.ALIGN_LEFT, bg));
                    table.AddCell(Td(r.Mensaje, fRow, Element.ALIGN_LEFT, bg));
                    table.AddCell(Td(r.UsuarioEjecutor, fRow, Element.ALIGN_LEFT, bg));
                }

                doc.Add(table);
                doc.Add(new Paragraph(" "));

                var fRes = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var total = (data.totalRegistros ?? 0) > 0 ? data.totalRegistros.Value : (data.registros?.Count ?? 0);
                string totalLabel = L("report_total_label", "Total de registros");
                doc.Add(new Paragraph($"{totalLabel}: {total}", fRes));

                doc.Close();
            }
        }

        private static void Th(PdfPTable t, string text, Font f)
        {
            t.AddCell(new PdfPCell(new Phrase(text, f))
            {
                BackgroundColor = CorporateGreen,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            });
        }

        private static PdfPCell Td(object val, Font f, int align, BaseColor bg)
        {
            return new PdfPCell(new Phrase(val?.ToString() ?? string.Empty, f))
            {
                BackgroundColor = bg,
                Padding = 4,
                HorizontalAlignment = align
            };
        }

        private class HeaderFooterEvent : PdfPageEventHelper
        {
            public string Titulo { get; set; }
            public string Subtitulo { get; set; }
            public Image Logo { get; set; }
            public string Pie { get; set; }
            public string PageLabel { get; set; } = "Página";

            private readonly Font fTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15);
            private readonly Font fSub = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.DARK_GRAY);
            private readonly Font fPie = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                var cb = writer.DirectContent;
                float width = document.PageSize.Width - document.LeftMargin - document.RightMargin;

                var header = new PdfPTable(2) { TotalWidth = width };
                header.SetWidths(new float[] { 1.6f, 4f });

                PdfPCell cLogo = Logo != null
                    ? new PdfPCell(Logo, false)
                    {
                        Border = Rectangle.NO_BORDER,
                        Rowspan = 2,
                        PaddingRight = 12f,
                        PaddingTop = -20f,
                        VerticalAlignment = Element.ALIGN_TOP
                    }
                    : new PdfPCell(new Phrase(string.Empty)) { Border = Rectangle.NO_BORDER };

                header.AddCell(cLogo);

                header.AddCell(new PdfPCell(new Phrase(Titulo ?? string.Empty, fTitulo))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    PaddingBottom = 6f
                });

                header.AddCell(new PdfPCell(new Phrase(Subtitulo ?? string.Empty, fSub))
                {
                    Border = Rectangle.NO_BORDER,
                    PaddingTop = 2f
                });

                header.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 16, cb);

                var footer = new PdfPTable(2) { TotalWidth = width };
                footer.SetWidths(new float[] { 4f, 1f });
                footer.AddCell(new PdfPCell(new Phrase(Pie ?? string.Empty, fPie)) { Border = Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_LEFT });
                footer.AddCell(new PdfPCell(new Phrase($"{PageLabel} {writer.PageNumber}", fPie)) { Border = Rectangle.TOP_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });
                footer.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin - 5, cb);
            }
        }

        public static string GenerarPdfCotizacion(Cotizacion ctz, string empresa, byte[] logoBytes = null, string tempRootDir = null)
        {
            if (ctz == null) throw new ArgumentNullException("ctz");

            string baseDir = !string.IsNullOrWhiteSpace(tempRootDir)
                ? tempRootDir
                : Path.Combine(Path.GetTempPath(), AppDomain.CurrentDomain.FriendlyName);

            Directory.CreateDirectory(baseDir);

            string nombre = string.Format("Cotizacion_{0}_{1}.pdf",
                ctz.IdCotizacion.ToString("000000"),
                DateTime.Now.ToString("yyyyMMddHHmmss"));

            string rutaPdf = Path.Combine(baseDir, nombre);

            GenerarPdfCotizacionInterno(ctz, rutaPdf, empresa, logoBytes);

            return rutaPdf;
        }

        private static void GenerarPdfCotizacionInterno(Cotizacion ctz, string rutaSalida, string empresa, byte[] logoBytes)
        {
            var L = new Func<string, string, string>((code, fb) => ParametrizacionBLL.GetInstance().GetLocalizable(code) ?? fb);

            var dir = Path.GetDirectoryName(rutaSalida);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            using (var fs = new FileStream(rutaSalida, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var doc = new Document(PageSize.A4, 36, 36, 100, 50))
            {
                var writer = PdfWriter.GetInstance(doc, fs);

                Image logo = null;
                if (logoBytes != null && logoBytes.Length > 0)
                {
                    logo = Image.GetInstance(logoBytes);
                    logo.ScaleToFit(110f, 110f);
                    logo.Alignment = Image.ALIGN_LEFT;
                }

                string reportTitle = L("cotizacion_report_title", "Detalle de Cotización");
                string subtitle = $"{empresa} | {DateTime.Now:yyyy-MM-dd HH:mm}";
                string footer = $"{empresa} • {DateTime.Now:yyyy-MM-dd HH:mm}";

                var pageEvents = new HeaderFooterEvent
                {
                    Titulo = reportTitle,
                    Subtitulo = subtitle,
                    Logo = logo,
                    Pie = footer,
                    PageLabel = L("page_label", "Página")
                };

                writer.PageEvent = pageEvents;

                doc.Open();

                var fLbl = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
                var fTxt = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                var tablaHeader = new PdfPTable(4) { WidthPercentage = 100 };
                tablaHeader.SetWidths(new float[] { 1.3f, 2.7f, 1.3f, 2.0f });

                tablaHeader.AddCell(new PdfPCell(new Phrase(L("cotizacion_id_label", "N° Cotización"), fLbl)) { BackgroundColor = LightGray, Padding = 4 });
                tablaHeader.AddCell(new PdfPCell(new Phrase(ctz.IdCotizacion.ToString(), fTxt)) { Padding = 4 });

                tablaHeader.AddCell(new PdfPCell(new Phrase(L("cotizacion_date_label", "Fecha"), fLbl)) { BackgroundColor = LightGray, Padding = 4 });
                tablaHeader.AddCell(new PdfPCell(new Phrase(ctz.FechaCreacion.ToString("yyyy-MM-dd"), fTxt)) { Padding = 4 });

                tablaHeader.AddCell(new PdfPCell(new Phrase(L("cotizacion_type_label", "Tipo de edificación"), fLbl)) { BackgroundColor = LightGray, Padding = 4 });
                tablaHeader.AddCell(new PdfPCell(new Phrase(ctz.TipoEdificacion?.Descripcion ?? "", fTxt)) { Padding = 4 });

                tablaHeader.AddCell(new PdfPCell(new Phrase(L("cotizacion_currency_label", "Moneda"), fLbl)) { BackgroundColor = LightGray, Padding = 4 });
                tablaHeader.AddCell(new PdfPCell(new Phrase(ctz.Moneda?.NombreMoneda ?? "", fTxt)) { Padding = 4 });

                doc.Add(tablaHeader);
                doc.Add(new Paragraph(" "));

                var fSection = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                var fHead = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                var fRow = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                decimal totalMateriales = 0m;
                decimal totalMaquinaria = 0m;
                decimal totalServicios = 0m;

                if (ctz.ListaMateriales != null && ctz.ListaMateriales.Count > 0)
                {
                    doc.Add(new Paragraph(L("cotizacion_materials_section", "Materiales"), fSection));
                    doc.Add(new Paragraph(" "));

                    var tableMat = new PdfPTable(5) { WidthPercentage = 100 };
                    tableMat.SetWidths(new float[] { 2.4f, 1.0f, 1.2f, 1.2f, 1.2f });

                    Th(tableMat, L("cotizacion_material_name_col", "Material"), fHead);
                    Th(tableMat, L("cotizacion_material_unit_col", "Unidad"), fHead);
                    Th(tableMat, L("cotizacion_material_price_col", "Precio unidad"), fHead);
                    Th(tableMat, L("cotizacion_material_qty_col", "Cantidad"), fHead);
                    Th(tableMat, L("cotizacion_material_subtotal_col", "Subtotal"), fHead);

                    bool zebra = false;
                    foreach (var it in ctz.ListaMateriales)
                    {
                        var bg = zebra ? RowAlt : BaseColor.WHITE;
                        zebra = !zebra;

                        string nombre = it.Material?.Nombre ?? "";
                        string unidad = it.Material?.UnidadMedida ?? "";
                        decimal precio = it.Material?.PrecioUnidad ?? 0m;
                        decimal cant = it.Cantidad;
                        decimal subtotal = precio * cant;
                        totalMateriales += subtotal;

                        tableMat.AddCell(Td(nombre, fRow, Element.ALIGN_LEFT, bg));
                        tableMat.AddCell(Td(unidad, fRow, Element.ALIGN_CENTER, bg));
                        tableMat.AddCell(Td(precio.ToString("N2"), fRow, Element.ALIGN_RIGHT, bg));
                        tableMat.AddCell(Td(cant.ToString("N2"), fRow, Element.ALIGN_RIGHT, bg));
                        tableMat.AddCell(Td(subtotal.ToString("N2"), fRow, Element.ALIGN_RIGHT, bg));
                    }

                    doc.Add(tableMat);
                    doc.Add(new Paragraph(" "));
                }

                if (ctz.ListaMaquinaria != null && ctz.ListaMaquinaria.Count > 0)
                {
                    doc.Add(new Paragraph(L("cotizacion_machinery_section", "Maquinaria"), fSection));
                    doc.Add(new Paragraph(" "));

                    var tableMaq = new PdfPTable(4) { WidthPercentage = 100 };
                    tableMaq.SetWidths(new float[] { 2.4f, 1.2f, 1.2f, 1.2f });

                    Th(tableMaq, L("cotizacion_machinery_name_col", "Maquinaria"), fHead);
                    Th(tableMaq, L("cotizacion_machinery_cost_col", "Costo/hora"), fHead);
                    Th(tableMaq, L("cotizacion_machinery_hours_col", "Horas uso"), fHead);
                    Th(tableMaq, L("cotizacion_machinery_subtotal_col", "Subtotal"), fHead);

                    bool zebra = false;
                    foreach (var it in ctz.ListaMaquinaria)
                    {
                        var bg = zebra ? RowAlt : BaseColor.WHITE;
                        zebra = !zebra;

                        string nombre = it.Maquinaria?.Nombre ?? "";
                        decimal costoHora = it.Maquinaria?.CostoPorHora ?? 0m;
                        decimal horas = it.HorasUso;
                        decimal subtotal = costoHora * horas;
                        totalMaquinaria += subtotal;

                        tableMaq.AddCell(Td(nombre, fRow, Element.ALIGN_LEFT, bg));
                        tableMaq.AddCell(Td(costoHora.ToString("N2"), fRow, Element.ALIGN_RIGHT, bg));
                        tableMaq.AddCell(Td(horas.ToString("N2"), fRow, Element.ALIGN_RIGHT, bg));
                        tableMaq.AddCell(Td(subtotal.ToString("N2"), fRow, Element.ALIGN_RIGHT, bg));
                    }

                    doc.Add(tableMaq);
                    doc.Add(new Paragraph(" "));
                }

                if (ctz.ListaServicios != null && ctz.ListaServicios.Count > 0)
                {
                    doc.Add(new Paragraph(L("cotizacion_services_section", "Servicios adicionales"), fSection));
                    doc.Add(new Paragraph(" "));

                    var tableSrv = new PdfPTable(2) { WidthPercentage = 100 };
                    tableSrv.SetWidths(new float[] { 3.4f, 1.2f });

                    Th(tableSrv, L("cotizacion_service_desc_col", "Servicio"), fHead);
                    Th(tableSrv, L("cotizacion_service_price_col", "Precio"), fHead);

                    bool zebra = false;
                    foreach (var it in ctz.ListaServicios)
                    {
                        var bg = zebra ? RowAlt : BaseColor.WHITE;
                        zebra = !zebra;

                        string desc = it.Servicio?.Descripcion ?? "";
                        decimal precio = it.Servicio?.Precio ?? 0m;
                        totalServicios += precio;

                        tableSrv.AddCell(Td(desc, fRow, Element.ALIGN_LEFT, bg));
                        tableSrv.AddCell(Td(precio.ToString("N2"), fRow, Element.ALIGN_RIGHT, bg));
                    }

                    doc.Add(tableSrv);
                    doc.Add(new Paragraph(" "));
                }

                var fRes = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);

                decimal totalGeneral = totalMateriales + totalMaquinaria + totalServicios;

                string totMatLbl = L("cotizacion_materials_total_label", "Total materiales");
                string totMaqLbl = L("cotizacion_machinery_total_label", "Total maquinaria");
                string totSrvLbl = L("cotizacion_services_total_label", "Total servicios");
                string totGenLbl = L("cotizacion_grand_total_label", "TOTAL COTIZACIÓN");

                doc.Add(new Paragraph($"{totMatLbl}: {totalMateriales:N2}", fRes));
                doc.Add(new Paragraph($"{totMaqLbl}: {totalMaquinaria:N2}", fRes));
                doc.Add(new Paragraph($"{totSrvLbl}: {totalServicios:N2}", fRes));
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph($"{totGenLbl}: {totalGeneral:N2}", fRes));

                doc.Close();
            }
        }
    }
}
