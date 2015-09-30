using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aufen.PortalReportes.Core;
using iTextSharp.text;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class Header : PdfPageEventHelper
    {
        private EMPRESA _Empresa { get; set; }
        private string _path { get; set; }
        private Font ChicaNegrita { get; set; }

        public Header(EMPRESA empresa, string path)
        {
            _Empresa = empresa;    
            _path = path;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            Configurar();
            //base.OnEndPage(writer, document);
            PdfPTable tabla = new PdfPTable(new float[]{4,1});
            tabla.AddCell(new PdfPCell(new Phrase(_Empresa.Descripcion,ChicaNegrita)) { Border = Rectangle.NO_BORDER });
            //logo
            Image imagen = Image.GetInstance(String.Format(@"{0}\imagenes\LogosEmpresas\logo{1}.jpg",_path,_Empresa.Codigo.Trim()));
            tabla.AddCell(new PdfPCell(imagen,true) { Rowspan = 3,  Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(_Empresa.Direccion, ChicaNegrita)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(
                new Phrase(
                    String.Format("Fono: {0} Fax: {1}", (_Empresa.Fono ?? String.Empty).Trim(), (_Empresa.Fax ?? String.Empty).Trim()), ChicaNegrita)
                    ) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(String.Format("Fecha Informe: {0}", DateTime.Now.ToShortDateString()), ChicaNegrita)) { Border = Rectangle.NO_BORDER });
            tabla.TotalWidth = document.Right - document.Left- 20;
            tabla.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 18, writer.DirectContent);
        }

        private void Configurar()
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            ChicaNegrita = new Font(bf, 8, Font.BOLD, BaseColor.BLACK);
        }
    }
}