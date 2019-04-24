using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.Properties;
using FastReport;
using System.IO;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class CFDIsConsecutivos : SaariReport, IReport, IBackgroundReport
    {
        private string idInmobiliaria = string.Empty, idConjunto = string.Empty, usuario = string.Empty, rutaFormato = string.Empty;
        private DateTime fechaInicio = new DateTime(), fechaFin = DateTime.Now.Date;
        private bool esPdf = true;
       
        public CFDIsConsecutivos(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, string usuario, string rutaFormato, bool esPdf)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.fechaInicio = fechaInicio.Date;
            this.fechaFin = fechaFin.Date;
            this.usuario = usuario;
            this.rutaFormato = rutaFormato;
            this.esPdf = esPdf;
        }

        public string generar()
        {
            try
            {
                string resultValidar = validar(idInmobiliaria, idConjunto, fechaInicio, fechaFin, rutaFormato);
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                if (string.IsNullOrEmpty(resultValidar))
                {
                    List<SubtipoEntity> listaSubtipos = SaariDB.getSubTiposOI();
                    if (listaSubtipos != null)
                    {
                        OnCambioProgreso(20);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";
                        List<CFDIConsecutivoEntity> listaCfdisBD = SaariDB.getRegistrosCFDISConsec(idInmobiliaria, idConjunto, fechaInicio.Date, fechaFin.Date);

                        var list = SaariE.GetDescripcion();

                        if (listaCfdisBD != null)
                        {
                           
                            foreach (var list1 in listaCfdisBD)
                            {
                                List<ClaveMetodoPagoEntity> listaDetalle = list.Where(c => c.Clave == list1.MetodoPago).ToList();

                                foreach (var Detalle in listaDetalle)
                                {
                                    if (Detalle.Clave == "1" || Detalle.Clave == "01")
                                    {
                                        list1.MetodoPago = Detalle.Descripcion;
                                    }
                                    else if (Detalle.Clave == "2")
                                    {
                                        list1.MetodoPago = Detalle.Descripcion;
                                    }
                                    else if (Detalle.Clave == "3" || Detalle.Clave == "03")
                                    {
                                        list1.MetodoPago = Detalle.Descripcion;
                                    }
                                    else if (Detalle.Clave == "99")
                                    {
                                        list1.MetodoPago = Detalle.Descripcion;
                                    }
                                }

                            }

                            OnCambioProgreso(30);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";

                            int porcentaje = 30;
                            decimal factor = 50 / listaCfdisBD.Count;
                            factor = factor >= 1 ? factor : 1;

                            foreach (CFDIConsecutivoEntity c in listaCfdisBD)
                            {
                                if (porcentaje <= 80)
                                    porcentaje += Convert.ToInt32(factor);
                                OnCambioProgreso(porcentaje);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                decimal importe = c.ImporteFac;
                                decimal descuento = c.DescuentoRenta;
                                decimal descuentoNegativo = SaariDB.getImporteDescuentoNegativo(c.IDHistRec);
                                decimal totalOtrosCargos = c.TotalCargos;
                                if (!c.EsCancelada)
                                {
                                    if (c.Moneda != "P")
                                    {
                                        importe = c.ImporteFac * c.TipoDeCambio;
                                        descuento = c.DescuentoRenta * c.TipoDeCambio;
                                        totalOtrosCargos = c.TotalCargos * c.TipoDeCambio;
                                        descuentoNegativo = descuentoNegativo * c.TipoDeCambio;
                                    }

                                    if (c.EsRentaAnticipada)
                                    {
                                        c.ImporteRentasAnticipadas = importe;
                                        break;
                                    }

                                    if (c.TipoDocumento == "R" || c.TipoDocumento == "X")
                                    {
                                        decimal otrosImportes = 0;
                                        c.ImporteRenta = importe - totalOtrosCargos - descuentoNegativo;
                                        c.DescuentoRenta = descuento - descuentoNegativo;                                        
                                        if (totalOtrosCargos != 0)
                                        {
                                            int numImporte = 1;
                                            foreach (SubtipoEntity s in listaSubtipos)
                                            {
                                                decimal importeVariable = SaariDB.getImporteCargo(c.IDHistRec, s.Identificador);
                                                if (numImporte == 1)
                                                    c.ImporteVariable1 = importeVariable;
                                                else if (numImporte == 2)
                                                    c.ImporteVariable2 = importeVariable;
                                                else if (numImporte == 3)
                                                    c.ImporteVariable3 = importeVariable;
                                                else if (numImporte == 4)
                                                    c.ImporteVariable4 = importeVariable;
                                                else if (numImporte == 5)
                                                    c.ImporteVariable5 = importeVariable;
                                                else if (numImporte == 6)
                                                    c.ImporteVariable6 = importeVariable;
                                                else if (numImporte == 7)
                                                    c.ImporteVariable7 = importeVariable;
                                                else if (numImporte == 8)
                                                    c.ImporteVariable8 = importeVariable;
                                                else if (numImporte == 9)
                                                    c.ImporteVariable9 = importeVariable;
                                                else if (numImporte == 10)
                                                    c.ImporteVariable10 = importeVariable;
                                                else if (numImporte == 11)
                                                    c.ImporteVariable11 = importeVariable;
                                                else if (numImporte == 12)
                                                    c.ImporteVariable12 = importeVariable;
                                                else if (numImporte == 13)
                                                    c.ImporteVariable13 = importeVariable;
                                                else if (numImporte == 14)
                                                    c.ImporteVariable14 = importeVariable;
                                                else if (numImporte == 15)
                                                    c.ImporteVariable15 = importeVariable;
                                                else if (numImporte == 16)
                                                    c.ImporteVariable16 = importeVariable;
                                                else if (numImporte == 17)
                                                    c.ImporteVariable17 = importeVariable;
                                                else if (numImporte == 18)
                                                    c.ImporteVariable18 = importeVariable;
                                                else if (numImporte == 19)
                                                    c.ImporteVariable19 = importeVariable;
                                                else if (numImporte == 20)
                                                    c.ImporteVariable20 = importeVariable;
                                                else if (numImporte == 21)
                                                    c.ImporteVariable21 = importeVariable;
                                                else if (numImporte == 22)
                                                    c.ImporteVariable22 = importeVariable;
                                                else if (numImporte == 23)
                                                    c.ImporteVariable23 = importeVariable;
                                                else if (numImporte == 24)
                                                    c.ImporteVariable24 = importeVariable;
                                                else if (numImporte == 25)
                                                    c.ImporteVariable25 = importeVariable;
                                                else
                                                    otrosImportes += importeVariable;
                                                numImporte++;
                                            }
                                            otrosImportes += SaariDB.getImporteCargo(c.IDHistRec, string.Empty);
                                            c.OtrosImportes = otrosImportes;
                                        }
                                    }
                                    else
                                    {
                                        decimal otrosImportes = 0;
                                        otrosImportes -= descuento;
                                        if (string.IsNullOrEmpty(c.IDSubtipo))
                                            otrosImportes += importe + descuento;
                                        else
                                        {
                                            int numImporte = (from s in listaSubtipos
                                                                where s.Identificador == c.IDSubtipo
                                                                select s.Orden).FirstOrDefault();
                                            if (numImporte > 0)
                                            {
                                                if (numImporte == 1)
                                                    c.ImporteVariable1 = importe + descuento;
                                                else if (numImporte == 2)
                                                    c.ImporteVariable2 = importe + descuento;
                                                else if (numImporte == 3)
                                                    c.ImporteVariable3 = importe + descuento;
                                                else if (numImporte == 4)
                                                    c.ImporteVariable4 = importe + descuento;
                                                else if (numImporte == 5)
                                                    c.ImporteVariable5 = importe + descuento;
                                                else if (numImporte == 6)
                                                    c.ImporteVariable6 = importe + descuento;
                                                else if (numImporte == 7)
                                                    c.ImporteVariable7 = importe + descuento;
                                                else if (numImporte == 8)
                                                    c.ImporteVariable8 = importe + descuento;
                                                else if (numImporte == 9)
                                                    c.ImporteVariable9 = importe + descuento;
                                                else if (numImporte == 10)
                                                    c.ImporteVariable10 = importe + descuento;
                                                else if (numImporte == 11)
                                                    c.ImporteVariable11 = importe + descuento;
                                                else if (numImporte == 12)
                                                    c.ImporteVariable12 = importe + descuento;
                                                else if (numImporte == 13)
                                                    c.ImporteVariable13 = importe + descuento;
                                                else if (numImporte == 14)
                                                    c.ImporteVariable14 = importe + descuento;
                                                else if (numImporte == 15)
                                                    c.ImporteVariable15 = importe + descuento;
                                                else if (numImporte == 16)
                                                    c.ImporteVariable16 = importe + descuento;
                                                else if (numImporte == 17)
                                                    c.ImporteVariable17 = importe + descuento;
                                                else if (numImporte == 18)
                                                    c.ImporteVariable18 = importe + descuento;
                                                else if (numImporte == 19)
                                                    c.ImporteVariable19 = importe + descuento;
                                                else if (numImporte == 20)
                                                    c.ImporteVariable20 = importe + descuento;
                                                else if (numImporte == 21)
                                                    c.ImporteVariable21 = importe + descuento;
                                                else if (numImporte == 22)
                                                    c.ImporteVariable22 = importe + descuento;
                                                else if (numImporte == 23)
                                                    c.ImporteVariable23 = importe + descuento;
                                                else if (numImporte == 24)
                                                    c.ImporteVariable24 = importe + descuento;
                                                else if (numImporte == 25)
                                                    c.ImporteVariable25 = importe + descuento;
                                                else
                                                    otrosImportes += importe + descuento;
                                            }
                                            else
                                                otrosImportes += importe + descuento;
                                        }      
                                        c.OtrosImportes = otrosImportes;
                                    }
                                }
                                else
                                    c.NombreCliente = "CANCELADA";
                            }

                            string nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idInmobiliaria);
                            string nombreConjunto = SaariDB.getNombreConjuntoByID(idConjunto);
                            EncabezadoEntity encabezado = new EncabezadoEntity()
                            {
                                Inmobiliaria = nombreInmobiliaria,
                                FechaInicio = fechaInicio.ToString("dd/MM/yyyy"),
                                FechaFin = fechaFin.ToString("dd/MM/yyyy"),
                                Usuario = usuario,
                                Conjunto = nombreConjunto
                            };
                            List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                            listaEncabezado.Add(encabezado);
                            
                            OnCambioProgreso(90);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";

                            if (File.Exists(rutaFormato))
                            {          
                                Report report = new Report();
                                report.Load(rutaFormato);
                                report.RegisterData(listaEncabezado, "Encabezado");
                                report.RegisterData(listaCfdisBD, "Registro");
                                DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                                bandaRecibos.DataSource = report.GetDataSource("Registro");

                                int cont = 1;
                                TextObject conceptos = report.FindObject("conceptos") as TextObject;
                                foreach (SubtipoEntity s in listaSubtipos)
                                {
                                    TextObject text = report.FindObject("importeVariable" + cont) as TextObject;
                                    if (text != null)
                                    {
                                        text.Text = s.Nombre;                                        
                                    }
                                    TextObject importe = report.FindObject("importe" + cont) as TextObject;
                                    if (importe != null)
                                        importe.Text = "[Registro.ImporteVariable" + cont + "]";
                                    cont++;
                                }

                                TextObject otrosImp = report.FindObject("otrosImportes") as TextObject;
                                TextObject otrosImpC = report.FindObject("otrosImportesC") as TextObject;
                                TextObject rentasAntic = report.FindObject("rentasAntic") as TextObject;
                                TextObject rentasAnticC = report.FindObject("rentasAnticC") as TextObject;
                                TextObject subtotal = report.FindObject("subtotal") as TextObject;
                                TextObject subtotalC = report.FindObject("subtotalC") as TextObject;
                                TextObject iva = report.FindObject("iva") as TextObject;
                                TextObject ivaC = report.FindObject("ivaC") as TextObject;
                                TextObject total = report.FindObject("total") as TextObject;
                                TextObject totalC = report.FindObject("totalC") as TextObject;
                                TextObject enc1 = report.FindObject("encabezado1") as TextObject;
                                TextObject enc2 = report.FindObject("encabezado2") as TextObject;
                                TextObject enc3 = report.FindObject("encabezado3") as TextObject;
                                TextObject enc4 = report.FindObject("encabezado4") as TextObject;
                                ReportPage rp = report.FindObject("Page1") as ReportPage;
                                for (int i = cont; i <= 25; i++)
                                {
                                    if (conceptos != null)
                                        conceptos.Width -= 94.5f;
                                    TextObject text = report.FindObject("importeVariable" + i) as TextObject;
                                    if (text != null)
                                        text.Visible = false;
                                    TextObject importe = report.FindObject("importe" + cont) as TextObject;
                                    if (importe != null)
                                        importe.Visible = false;
                                    if (otrosImp != null)
                                        otrosImp.Left -= 94.5f;
                                    if (rentasAntic != null)
                                        rentasAntic.Left -= 94.5f;
                                    if (otrosImpC != null)
                                        otrosImpC.Left -= 94.5f;
                                    if (rentasAnticC != null)
                                        rentasAnticC.Left -= 94.5f;
                                    if (subtotal != null)
                                        subtotal.Left -= 94.5f;
                                    if (subtotalC != null)
                                        subtotalC.Left -= 94.5f;
                                    if (iva != null)
                                        iva.Left -= 94.5f;
                                    if (ivaC != null)
                                        ivaC.Left -= 94.5f;
                                    if (total != null)
                                        total.Left -= 94.5f;
                                    if (totalC != null)
                                        totalC.Left -= 94.5f;
                                    if (enc1 != null)
                                        enc1.Width -= 94.5f;
                                    if (enc2 != null)
                                        enc2.Width -= 94.5f;
                                    if (enc3 != null)
                                        enc3.Width -= 94.5f;
                                    if (enc4 != null)
                                        enc4.Width -= 94.5f;
                                    if (rp != null)
                                        rp.PaperWidth -= 25f;
                                }
                                return exportar(report, esPdf, "CFDIsConsecutivos");
                            }
                            else
                                return "No se encontro el formato " + rutaFormato + Environment.NewLine;
                        }
                        else
                            return "Error al obtener la lista de CFDIs almacenados";
                    }
                    else
                        return "Error al obtener la lista de subtipos registrados";
                }
                else
                    return resultValidar;
            }
            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte:" + Environment.NewLine + ex.Message;
            }
        }

        public string validar(string idInmobiliaria, string idConjunto, DateTime fechaInicio, DateTime fechaFin, string rutaFormato)
        {
            string error = string.Empty;
            if (string.IsNullOrEmpty(idInmobiliaria))
                error += "Debe seleccionar una inmobiliaria" + Environment.NewLine;
            if (string.IsNullOrEmpty(idConjunto))
                error += "Debe seleccionar un conjunto" + Environment.NewLine;
            if (fechaInicio > fechaFin)
                error += "La fecha de fin debe ser mayor o igual a la fecha de inicio" + Environment.NewLine;
            if (!File.Exists(rutaFormato))
                error += "No se encontró el formato " + rutaFormato + Environment.NewLine;
            return error;
        }
    }
}
