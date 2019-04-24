﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Export.OoXML;
using System.Diagnostics;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;
using GestorReportes.BusinessLayer.Helpers;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class CarteraVencida306090 : SaariReport, IReport, IBackgroundReport
    {
        private string idInmobiliaria = string.Empty, idConjunto = string.Empty, rutaFormato = string.Empty, usuario = string.Empty;
        private DateTime fechaCorte = DateTime.Now.Date;
        private bool esPdf = true, incluirIVA = true, esDetallado = false, tomarTcEmision = true, esPorContrato = false;
        private decimal tipoCambio = 1;

        public CarteraVencida306090() { }

        public CarteraVencida306090(string idInmobiliaria, string idConjunto, DateTime fechaCorte, bool esPdf, bool incluirIVA, bool esDetallado, string rutaFormato, string usuario, bool tomarTcEmision, decimal tipoCambio, bool esPorContrato)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.idConjunto = idConjunto;
            this.fechaCorte = fechaCorte.Date;
            this.esPdf = esPdf;
            this.incluirIVA = incluirIVA;
            this.esDetallado = esDetallado;
            this.rutaFormato = rutaFormato;
            this.usuario = usuario;
            this.tomarTcEmision = tomarTcEmision;
            this.tipoCambio = tipoCambio;
            this.esPorContrato = esPorContrato;
        }

        public static List<InmobiliariaEntity> obtenerInmobiliarias()
        {
            return SaariDB.getListaInmobiliarias();
        }

        public static List<ConjuntoEntity> obtenerConjuntos(string idInmobiliaria)
        {
            return SaariDB.getConjuntos(idInmobiliaria);
        }

        public string generar()
        {
            if (esPorContrato)
                return generarPorContratos();
            else
                return generarPorCliente();
        }

        private string generarPorCliente()
        {
            try
            {
                List<ReciboEntity> listaRecibos = null;
                OnCambioProgreso(10);

                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                if (idConjunto == "Todos")
                    listaRecibos = SaariDB.getListaRecibos(idInmobiliaria, fechaCorte);
                else
                    listaRecibos = SaariDB.getListaRecibos(idInmobiliaria, idConjunto, fechaCorte);

                OnCambioProgreso(20);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";

                if (listaRecibos != null)
                {
                    if (listaRecibos.Count > 0)
                    {
                        List<string> listaIDsClientes = listaRecibos.Select(r => r.IDCliente).Distinct().ToList();

                        OnCambioProgreso(25);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        int porcentaje = 25;
                        decimal factor = 50 / listaIDsClientes.Count;
                        factor = factor >= 1 ? factor : 1;

                        List<Cartera306090Entity> listaCartera = new List<Cartera306090Entity>();

                        foreach (string idCliente in listaIDsClientes)
                        {
                            if (porcentaje <= 75)
                                porcentaje += Convert.ToInt32(factor);
                            OnCambioProgreso(porcentaje);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";

                            Cartera306090Entity registroCartera = new Cartera306090Entity();
                            registroCartera.IDCliente = idCliente;

                            List<ReciboEntity> lista30 = (from r in listaRecibos
                                                          where ((r.TipoDoc == "R" || r.TipoDoc == "X" || r.TipoDoc == "T" || r.TipoDoc == "Z" || r.TipoDoc == "W") &&
                                                            r.FechaEmision >= fechaCorte.Date.AddDays(-30) && r.FechaEmision <= fechaCorte.Date &&
                                                            r.IDCliente == idCliente && (r.Estatus == "1" || (r.Estatus == "2" && r.FechaPago > fechaCorte.Date) || (r.Estatus == "3" && r.FechaPago > fechaCorte.Date)) ||
                                                            (r.IDCliente == idCliente && r.TipoDoc == "T" && r.Estatus == "0" && r.VencimientoPago <= fechaCorte.Date))
                                                          select r).ToList();

                            List<ReciboEntity> lista60 = (from r in listaRecibos
                                                          where ((r.TipoDoc == "R" || r.TipoDoc == "X" || r.TipoDoc == "T" || r.TipoDoc == "Z" || r.TipoDoc == "W") &&
                                                            r.FechaEmision >= fechaCorte.Date.AddDays(-60) && r.FechaEmision <= fechaCorte.Date.AddDays(-31) &&
                                                            r.IDCliente == idCliente && (r.Estatus == "1" || (r.Estatus == "2" && r.FechaPago > fechaCorte.Date) || (r.Estatus == "3" && r.FechaPago > fechaCorte.Date)) ||
                                                            (r.IDCliente == idCliente && r.TipoDoc == "T" && r.Estatus == "0" && r.VencimientoPago <= fechaCorte.Date.AddDays(-31)))
                                                          select r).ToList();

                            List<ReciboEntity> lista90 = (from r in listaRecibos
                                                          where ((r.TipoDoc == "R" || r.TipoDoc == "X" || r.TipoDoc == "T" || r.TipoDoc == "Z" || r.TipoDoc == "W") &&
                                                          r.FechaEmision >= fechaCorte.Date.AddDays(-90) && r.FechaEmision <= fechaCorte.Date.AddDays(-61) &&
                                                          r.IDCliente == idCliente && (r.Estatus == "1" || (r.Estatus == "2" && r.FechaPago > fechaCorte.Date) || (r.Estatus == "3" && r.FechaPago > fechaCorte.Date)) ||
                                                          (r.IDCliente == idCliente && r.TipoDoc == "T" && r.Estatus == "0" && r.VencimientoPago <= fechaCorte.Date.AddDays(-61)))
                                                          select r).ToList();

                            List<ReciboEntity> listaMas90 = (from r in listaRecibos
                                                             where ((r.TipoDoc == "R" || r.TipoDoc == "X" || r.TipoDoc == "T" || r.TipoDoc == "Z" || r.TipoDoc == "W") &&
                                                             r.FechaEmision <= fechaCorte.Date.AddDays(-91) &&
                                                             r.IDCliente == idCliente && (r.Estatus == "1" || (r.Estatus == "2" && r.FechaPago > fechaCorte.Date) || (r.Estatus == "3" && r.FechaPago > fechaCorte.Date)) ||
                                                             (r.IDCliente == idCliente && r.TipoDoc == "T" && r.Estatus == "0" && r.VencimientoPago <= fechaCorte.Date.AddDays(-61)))
                                                             select r).ToList();

                            ClienteEntity cliente = SaariDB.getClienteByID(idCliente);
                            if (cliente != null)
                            {
                                registroCartera.RazonSocialCliente = cliente.Nombre;
                                registroCartera.NombreComercialCliente = cliente.NombreComercial;
                            }

                            if (incluirIVA)
                            {
                                decimal total30 = 0, total60 = 0, total90 = 0, totalMas90 = 0;
                                foreach (var l in lista30)
                                {
                                    bool busquedaDoble = l.IDHistRec != l.Numero;
                                    decimal cantidad30 = 0;
                                    if (l.Moneda == "D")
                                    {
                                        if (tomarTcEmision)
                                        {
                                            cantidad30 = l.Total * l.TipoCambio;
                                            cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true);
                                            if (busquedaDoble)
                                                cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria);
                                            total30 += cantidad30;
                                        }
                                        else
                                        {
                                            cantidad30 = l.Total * tipoCambio;
                                            cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio);
                                            if (busquedaDoble)
                                                cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio);
                                            total30 += cantidad30;
                                        }
                                    }
                                    else
                                    {
                                        cantidad30 = l.Total;
                                        cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true);
                                        if (busquedaDoble)
                                            cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria);
                                        total30 += cantidad30;
                                    }
                                    l.Importe30 = cantidad30;
                                }

                                foreach (var l in lista60)
                                {
                                    bool busquedaDoble = l.IDHistRec != l.Numero;
                                    decimal cantidad60 = 0;
                                    if (l.Moneda == "D")
                                    {
                                        if (tomarTcEmision)
                                        {
                                            cantidad60 = l.Total * l.TipoCambio;
                                            cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true);
                                            if (busquedaDoble)
                                                cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria);
                                            total60 += cantidad60;
                                        }
                                        else
                                        {
                                            cantidad60 = l.Total * tipoCambio;
                                            cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio);
                                            if (busquedaDoble)
                                                cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio);
                                            total60 += cantidad60;
                                        }
                                    }
                                    else
                                    {
                                        cantidad60 = l.Total;
                                        cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true);
                                        if (busquedaDoble)
                                            cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria);
                                        total60 += cantidad60;
                                    }
                                    l.Importe60 = cantidad60;
                                }

                                foreach (var l in lista90)
                                {
                                    bool busquedaDoble = l.IDHistRec != l.Numero;
                                    decimal cantidad90 = 0;
                                    if (l.Moneda == "D")
                                    {
                                        if (tomarTcEmision)
                                        {
                                            cantidad90 = l.Total * l.TipoCambio;
                                            cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true);
                                            if (busquedaDoble)
                                                cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria);
                                            total90 += cantidad90;
                                        }
                                        else
                                        {
                                            cantidad90 = l.Total * tipoCambio;
                                            cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio);
                                            if (busquedaDoble)
                                                cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio);
                                            total90 += cantidad90;
                                        }
                                    }
                                    else
                                    {
                                        cantidad90 = l.Total;
                                        cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true);
                                        if (busquedaDoble)
                                            cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria);
                                        total90 += cantidad90;
                                    }
                                    l.Importe90 = cantidad90;
                                }

                                foreach (var l in listaMas90)
                                {
                                    bool busquedaDoble = l.IDHistRec != l.Numero;
                                    decimal cantidadMas90 = 0;
                                    if (l.Moneda == "D")
                                    {
                                        if (tomarTcEmision)
                                        {
                                            cantidadMas90 = l.Total * l.TipoCambio;
                                            cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true);
                                            if (busquedaDoble)
                                                cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria);
                                            totalMas90 += cantidadMas90;
                                        }
                                        else
                                        {
                                            cantidadMas90 = l.Total * tipoCambio;
                                            cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio);
                                            if (busquedaDoble)
                                                cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio);
                                            totalMas90 += cantidadMas90;
                                        }
                                    }
                                    else
                                    {
                                        cantidadMas90 = l.Total;
                                        cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true);
                                        if (busquedaDoble)
                                            cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria);
                                        totalMas90 += cantidadMas90;
                                    }
                                    l.ImporteMas90 = cantidadMas90;
                                }

                                registroCartera.Total30 = total30;
                                registroCartera.Total60 = total60;
                                registroCartera.Total90 = total90;
                                registroCartera.TotalMas90 = totalMas90;
                            }
                            else
                            {
                                decimal total30 = 0, total60 = 0, total90 = 0, totalMas90 = 0;
                                foreach (var l in lista30)
                                {
                                    bool busquedaDoble = l.IDHistRec != l.Numero;
                                    decimal cantidad30 = 0;
                                    if (l.Moneda == "D")
                                    {
                                        if (tomarTcEmision)
                                        {
                                            cantidad30 = (l.Importe - l.Descuento) * l.TipoCambio;
                                            cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false);
                                            if (busquedaDoble)
                                                cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria);
                                            total30 += cantidad30;
                                        }
                                        else
                                        {
                                            cantidad30 = (l.Importe - l.Descuento) * tipoCambio;
                                            cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio);
                                            if (busquedaDoble)
                                                cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio);
                                            total30 += cantidad30;
                                        }
                                    }
                                    else
                                    {
                                        cantidad30 = (l.Importe - l.Descuento);
                                        cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false);
                                        if (busquedaDoble)
                                            cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria);
                                        total30 += cantidad30;
                                    }
                                    l.Importe30 = cantidad30;
                                }

                                foreach (var l in lista60)
                                {
                                    bool busquedaDoble = l.IDHistRec != l.Numero;
                                    decimal cantidad60 = 0;
                                    if (l.Moneda == "D")
                                    {
                                        if (tomarTcEmision)
                                        {
                                            cantidad60 = (l.Importe - l.Descuento) * l.TipoCambio;
                                            cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false);
                                            if (busquedaDoble)
                                                cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria);
                                            total60 += cantidad60;
                                        }
                                        else
                                        {
                                            cantidad60 = (l.Importe - l.Descuento) * tipoCambio;
                                            cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio);
                                            if (busquedaDoble)
                                                cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio);
                                            total60 += cantidad60;
                                        }
                                    }
                                    else
                                    {
                                        cantidad60 = (l.Importe - l.Descuento);
                                        cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false);
                                        if (busquedaDoble)
                                            cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria);
                                        total60 += cantidad60;
                                    }
                                    l.Importe60 = cantidad60;
                                }

                                foreach (var l in lista90)
                                {
                                    bool busquedaDoble = l.IDHistRec != l.Numero;
                                    decimal cantidad90 = 0;
                                    if (l.Moneda == "D")
                                    {
                                        if (tomarTcEmision)
                                        {
                                            cantidad90 = (l.Importe - l.Descuento) * l.TipoCambio;
                                            cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false);
                                            if (busquedaDoble)
                                                cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria);
                                            total90 += cantidad90;
                                        }
                                        else
                                        {
                                            cantidad90 = (l.Importe - l.Descuento) * tipoCambio;
                                            cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio);
                                            if (busquedaDoble)
                                                cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio);
                                            total90 += cantidad90;
                                        }
                                    }
                                    else
                                    {
                                        cantidad90 = (l.Importe - l.Descuento);
                                        cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false);
                                        if (busquedaDoble)
                                            cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria);
                                        total90 += cantidad90;
                                    }
                                    l.Importe90 = cantidad90;
                                }

                                foreach (var l in listaMas90)
                                {
                                    bool busquedaDoble = l.IDHistRec != l.Numero;
                                    decimal cantidadMas90 = 0;
                                    if (l.Moneda == "D")
                                    {
                                        if (tomarTcEmision)
                                        {
                                            cantidadMas90 = (l.Importe - l.Descuento) * l.TipoCambio;
                                            cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false);
                                            if (busquedaDoble)
                                                cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria);
                                            totalMas90 += cantidadMas90;
                                        }
                                        else
                                        {
                                            cantidadMas90 = (l.Importe - l.Descuento) * tipoCambio;
                                            cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio);
                                            if (busquedaDoble)
                                                cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio);
                                            totalMas90 += cantidadMas90;
                                        }
                                    }
                                    else
                                    {
                                        cantidadMas90 = (l.Importe - l.Descuento);
                                        cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false);
                                        if (busquedaDoble)
                                            cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria);
                                        totalMas90 += cantidadMas90;
                                    }
                                    l.ImporteMas90 = cantidadMas90;
                                }

                                registroCartera.Total30 = total30;
                                registroCartera.Total60 = total60;
                                registroCartera.Total90 = total90;
                                registroCartera.TotalMas90 = totalMas90;
                            }

                            registroCartera.Recibos30 = lista30;
                            registroCartera.Recibos60 = lista60;
                            registroCartera.Recibos90 = lista90;
                            registroCartera.RecibosMas90 = listaMas90;

                            if (registroCartera.TotalCartera > 0)
                                listaCartera.Add(registroCartera);

                        }

                        OnCambioProgreso(80);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        string nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idInmobiliaria);
                        string nombreConjunto = SaariDB.getNombreConjuntoByID(idConjunto);
                        listaCartera = listaCartera.OrderBy(l => l.RazonSocialCliente).ToList();

                        EncabezadoEntity encabezado = new EncabezadoEntity()
                        {
                            Inmobiliaria = nombreInmobiliaria,
                            FechaFin = fechaCorte.ToString(@"dd \de MMMM \del yyyy").ToLower(),
                            Usuario = usuario,
                            Conjunto = nombreConjunto,
                            Mes30 = getNombreMes(fechaCorte.Month),
                            Mes60 = getNombreMes(fechaCorte.AddDays(-30).Month),
                            Mes90 = getNombreMes(fechaCorte.AddDays(-60).Month),
                            MesMas90 = getNombreMes(fechaCorte.AddDays(-90).Month),
                        };
                        List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                        listaEncabezado.Add(encabezado);


                        List<SaldoEntity> listaSaldos = new List<SaldoEntity>();
                        SaldoEntity saldo = new SaldoEntity()
                        {
                            Cartera = listaCartera.Sum(s => s.TotalCartera),
                            S30 = listaCartera.Sum(s => s.Total30),
                            S60 = listaCartera.Sum(s => s.Total60),
                            S90 = listaCartera.Sum(s => s.Total90),
                            SMas90 = listaCartera.Sum(s => s.TotalMas90)
                        };
                        listaSaldos.Add(saldo);

                        OnCambioProgreso(90);
                        if (CancelacionPendiente)
                            return "Proceso cancelado por el usuario";

                        if (File.Exists(rutaFormato))
                        {
                            Report report = new Report();
                            report.Load(rutaFormato);
                            report.RegisterData(listaEncabezado, "Encabezado");
                            report.RegisterData(listaCartera, "Recibo", 3);
                            report.RegisterData(listaSaldos, "Saldo");

                            DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                            bandaRecibos.DataSource = report.GetDataSource("Recibo");

                            DataBand bandaFacturas = report.FindObject("Data2") as DataBand;
                            bandaFacturas.DataSource = report.GetDataSource("Recibo.Recibos");

                            if (!esDetallado)
                                bandaFacturas.Visible = false;

                            return exportar(report, esPdf, "CarteraVencida306090");
                        }
                        else
                            return "No se encontro el formato " + rutaFormato + Environment.NewLine;
                    }
                    else
                        return "No se encontraron recibos con las condiciones dadas";
                }
                else
                    return "Error al obtener los recibos";

            }
            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte: " + Environment.NewLine + ex.Message;
            }
        }

        private string generarPorContratos()
        {

            try
            {
                if (File.Exists(rutaFormato))
                {
                    List<ReciboEntity> listaRecibos = null;
                    OnCambioProgreso(10);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    if (idConjunto == "Todos")
                        listaRecibos = SaariDB.getListaRecibos(idInmobiliaria, fechaCorte);
                    else
                        listaRecibos = SaariDB.getListaRecibos(idInmobiliaria, idConjunto, fechaCorte);

                    OnCambioProgreso(20);
                    if (CancelacionPendiente)
                        return "Proceso cancelado por el usuario";

                    if (listaRecibos != null)
                    {
                        if (listaRecibos.Count > 0)
                        {
                            List<string> listaIDsContratos = listaRecibos.Where(r => !r.IDContrato.ToLower().Contains("fac")).Select(r => r.IDContrato).Distinct().ToList();

                            OnCambioProgreso(25);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";

                            int porcentaje = 25;
                            decimal factor = 50 / listaIDsContratos.Count;
                            factor = factor >= 1 ? factor : 1;

                            List<Cartera306090Entity> listaCartera = new List<Cartera306090Entity>();
                            foreach (string idContrato in listaIDsContratos)
                            {
                                if (porcentaje <= 75)
                                    porcentaje += Convert.ToInt32(factor);
                                OnCambioProgreso(porcentaje);
                                if (CancelacionPendiente)
                                    return "Proceso cancelado por el usuario";

                                Cartera306090Entity registroCartera = new Cartera306090Entity();
                                registroCartera.IDContrato = idContrato;

                                List<ReciboEntity> lista30 = (from r in listaRecibos
                                                              where ((r.TipoDoc == "R" || r.TipoDoc == "X" || r.TipoDoc == "T" || r.TipoDoc == "Z" || r.TipoDoc == "W") &&
                                                              r.FechaEmision >= fechaCorte.Date.AddDays(-30) && r.FechaEmision <= fechaCorte.Date &&
                                                              r.IDContrato == idContrato && (r.Estatus == "1" || (r.Estatus == "2" && r.FechaPago > fechaCorte.Date) || (r.Estatus == "3" && r.FechaPago > fechaCorte.Date)) ||
                                                              (r.IDContrato == idContrato && r.TipoDoc == "T" && r.Estatus == "0" && r.VencimientoPago <= fechaCorte.Date))
                                                              select r).ToList();

                                List<ReciboEntity> lista60 = (from r in listaRecibos
                                                              where ((r.TipoDoc == "R" || r.TipoDoc == "X" || r.TipoDoc == "T" || r.TipoDoc == "Z" || r.TipoDoc == "W") &&
                                                              r.FechaEmision >= fechaCorte.Date.AddDays(-60) && r.FechaEmision <= fechaCorte.Date.AddDays(-31) &&
                                                              r.IDContrato == idContrato && (r.Estatus == "1" || (r.Estatus == "2" && r.FechaPago > fechaCorte.Date) || (r.Estatus == "3" && r.FechaPago > fechaCorte.Date)) ||
                                                              (r.IDContrato == idContrato && r.TipoDoc == "T" && r.Estatus == "0" && r.VencimientoPago <= fechaCorte.Date.AddDays(-31)))
                                                              select r).ToList();

                                List<ReciboEntity> lista90 = (from r in listaRecibos
                                                              where ((r.TipoDoc == "R" || r.TipoDoc == "X" || r.TipoDoc == "T" || r.TipoDoc == "Z" || r.TipoDoc == "W") &&
                                                              r.FechaEmision >= fechaCorte.Date.AddDays(-90) && r.FechaEmision <= fechaCorte.Date.AddDays(-61) &&
                                                              r.IDContrato == idContrato && (r.Estatus == "1" || (r.Estatus == "2" && r.FechaPago > fechaCorte.Date) || (r.Estatus == "3" && r.FechaPago > fechaCorte.Date)) ||
                                                              (r.IDContrato == idContrato && r.TipoDoc == "T" && r.Estatus == "0" && r.VencimientoPago <= fechaCorte.Date.AddDays(-61)))
                                                              select r).ToList();

                                List<ReciboEntity> listaMas90 = (from r in listaRecibos
                                                                 where ((r.TipoDoc == "R" || r.TipoDoc == "X" || r.TipoDoc == "T" || r.TipoDoc == "Z" || r.TipoDoc == "W") &&
                                                                 r.FechaEmision <= fechaCorte.Date.AddDays(-91) &&
                                                                 r.IDContrato == idContrato && (r.Estatus == "1" || (r.Estatus == "2" && r.FechaPago > fechaCorte.Date) || (r.Estatus == "3" && r.FechaPago > fechaCorte.Date)) ||
                                                                 (r.IDContrato == idContrato && r.TipoDoc == "T" && r.Estatus == "0" && r.VencimientoPago <= fechaCorte.Date.AddDays(-61)))
                                                                 select r).ToList();

                                ContratoEntity contrato = SaariDB.getContrato(idContrato);
                                if (contrato != null)
                                {
                                    registroCartera.RazonSocialCliente = contrato.ClienteNombre;
                                    registroCartera.NombreComercialCliente = contrato.ClienteNombreComercial;
                                    registroCartera.MonedaContrato = contrato.Moneda;
                                    string cuentaContable = Polizas.getCuentaByIDEntidadAndTipo(idContrato, "CC");
                                    if (!string.IsNullOrWhiteSpace(cuentaContable) && cuentaContable.Contains("|"))
                                    {
                                        registroCartera.Cuenta = cuentaContable.Split('|')[0];
                                        if (registroCartera.Cuenta.Contains("-"))
                                        {
                                            string[] secciones = registroCartera.Cuenta.Split('-');
                                            registroCartera.Cuenta = secciones[secciones.Length - 1];
                                        }
                                    }


                                    if (incluirIVA)
                                    {
                                        #region Si se incluye el IVA
                                        decimal total30 = 0, total60 = 0, total90 = 0, totalMas90 = 0, totalPesos30 = 0, totalPesos60 = 0, totalPesos90 = 0, totalPesosMas90 = 0;
                                        if (tomarTcEmision)
                                        {
                                            #region Si se toma el tipo de cambio de emision con IVA
                                            #region 30 dias
                                            foreach (var l in lista30)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad30 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad30 = l.Total;
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad30 = l.Total * l.TipoCambio;
                                                    else
                                                        cantidad30 = l.Total / l.TipoCambio;
                                                }
                                                cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, contrato.Moneda, l.Moneda);
                                                if (busquedaDoble)
                                                    cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, contrato.Moneda, l.Moneda);
                                                total30 += cantidad30;
                                                l.Importe30 = cantidad30;
                                                if (contrato.Moneda != "P")
                                                    totalPesos30 += cantidad30 * l.TipoCambio;
                                                else
                                                    totalPesos30 += cantidad30;
                                            }
                                            #endregion

                                            #region 60 dias
                                            foreach (var l in lista60)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad60 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad60 = l.Total;
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad60 = l.Total * l.TipoCambio;
                                                    else
                                                        cantidad60 = l.Total / l.TipoCambio;
                                                }
                                                cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, contrato.Moneda, l.Moneda);
                                                if (busquedaDoble)
                                                    cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, contrato.Moneda, l.Moneda);
                                                total60 += cantidad60;
                                                l.Importe60 = cantidad60;
                                                if (contrato.Moneda != "P")
                                                    totalPesos60 += cantidad60 * l.TipoCambio;
                                                else
                                                    totalPesos60 += cantidad60;
                                            }
                                            #endregion

                                            #region 90 dias
                                            foreach (var l in lista90)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad90 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad90 = l.Total;
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad90 = l.Total * l.TipoCambio;
                                                    else
                                                        cantidad90 = l.Total / l.TipoCambio;
                                                }
                                                cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, contrato.Moneda, l.Moneda);
                                                if (busquedaDoble)
                                                    cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, contrato.Moneda, l.Moneda);
                                                total90 += cantidad90;
                                                l.Importe90 = cantidad90;
                                                if (contrato.Moneda != "P")
                                                    totalPesos90 += cantidad90 * l.TipoCambio;
                                                else
                                                    totalPesos90 += cantidad90;
                                            }
                                            #endregion

                                            #region Mas de 90 dias
                                            foreach (var l in listaMas90)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidadMas90 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidadMas90 = l.Total;
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidadMas90 = l.Total * l.TipoCambio;
                                                    else
                                                        cantidadMas90 = l.Total / l.TipoCambio;
                                                }
                                                cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, contrato.Moneda, l.Moneda);
                                                if (busquedaDoble)
                                                    cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, contrato.Moneda, l.Moneda);
                                                totalMas90 += cantidadMas90;
                                                l.ImporteMas90 = cantidadMas90;
                                                if (contrato.Moneda != "P")
                                                    totalPesosMas90 += cantidadMas90 * l.TipoCambio;
                                                else
                                                    totalPesosMas90 += cantidadMas90;
                                            }
                                            #endregion
                                            #endregion
                                        }
                                        else
                                        {
                                            #region Si no se toma el tipo de cambio de emision con IVA
                                            #region 30 dias
                                            foreach (var l in lista30)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad30 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad30 = l.Total;
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                    {
                                                        //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                        //cantidad30 = l.Total * 1.TipoCambio; 
                                                        cantidad30 = l.Total * tipoCambio;
                                                    }
                                                    else if (l.MonedaPago == "P")
                                                    {
                                                        cantidad30 = l.Total / tipoCambio;
                                                    }

                                                }
                                                //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                //cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio, contrato.Moneda, l.Moneda);
                                                cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                if (busquedaDoble)
                                                {
                                                    //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                    //cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio, contrato.Moneda, l.Moneda);
                                                    cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                }
                                                total30 += cantidad30;
                                                l.Importe30 = cantidad30;
                                                if (contrato.Moneda != "P")
                                                {
                                                    totalPesos30 += cantidad30 * tipoCambio;
                                                }
                                                else
                                                    totalPesos30 += cantidad30;
                                            }
                                            #endregion

                                            #region 60 dias
                                            foreach (var l in lista60)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad60 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad60 = l.Total;
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad60 = l.Total * l.TipoCambio;
                                                    else
                                                        cantidad60 = l.Total / l.TipoCambio;
                                                }
                                                //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                //cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio, contrato.Moneda, l.Moneda);
                                                cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                if (busquedaDoble)
                                                {
                                                    //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                    //cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio, contrato.Moneda, l.Moneda);
                                                    cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                }
                                                total60 += cantidad60;
                                                l.Importe60 = cantidad60;
                                                if (contrato.Moneda != "P")
                                                    totalPesos60 += cantidad60 * tipoCambio;
                                                else
                                                    totalPesos60 += cantidad60;
                                            }
                                            #endregion

                                            #region 90 dias
                                            foreach (var l in lista90)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad90 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad90 = l.Total;
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad90 = l.Total * l.TipoCambio;
                                                    else
                                                        cantidad90 = l.Total / l.TipoCambio;
                                                }
                                                //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                //cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio, contrato.Moneda, l.Moneda);
                                                cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                if (busquedaDoble)
                                                {
                                                    //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                    //cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio, contrato.Moneda, l.Moneda);
                                                    cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                }
                                                total90 += cantidad90;
                                                l.Importe90 = cantidad90;
                                                if (contrato.Moneda != "P")
                                                    totalPesos90 += cantidad90 * tipoCambio;
                                                else
                                                    totalPesos90 += cantidad90;
                                            }
                                            #endregion

                                            #region Mas de 90 dias
                                            totalMas90 = 0.0M;
                                            totalPesosMas90 = 0.0M;
                                            #region Mas de 90 dias
                                            totalMas90 = 0.0M;
                                            totalPesosMas90 = 0.0M;
                                            foreach (var l in listaMas90)
                                            {

                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidadMas90 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidadMas90 = l.Total;
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidadMas90 = l.Total * l.TipoCambio;
                                                    else
                                                        cantidadMas90 = l.Total / l.TipoCambio;
                                                }
                                                //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                //cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio, contrato.Moneda, l.Moneda);
                                                cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, true, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                if (busquedaDoble)
                                                {
                                                    //Modify by Ing. Rodrigo Uzcanga 17/06/15
                                                    //cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio, contrato.Moneda, l.Moneda);
                                                    cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, true, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                }
                                                totalMas90 += cantidadMas90;
                                                l.ImporteMas90 = cantidadMas90;
                                                if (contrato.Moneda != "P")
                                                    totalPesosMas90 += cantidadMas90 * tipoCambio;
                                                else
                                                    totalPesosMas90 += cantidadMas90;
                                            }
                                            #endregion
                                            #endregion
                                        }

                                        registroCartera.Total30 = total30;
                                        registroCartera.Total60 = total60;
                                        registroCartera.Total90 = total90;
                                        registroCartera.TotalMas90 = totalMas90;
                                        registroCartera.TotalPesos30 = totalPesos30;
                                        registroCartera.TotalPesos60 = totalPesos60;
                                        registroCartera.TotalPesos90 = totalPesos90;
                                        registroCartera.TotalPesosMas90 = totalPesosMas90;
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Si no se incluye el IVA
                                        decimal total30 = 0, total60 = 0, total90 = 0, totalMas90 = 0, totalPesos30 = 0, totalPesos60 = 0, totalPesos90 = 0, totalPesosMas90 = 0;
                                        if (tomarTcEmision)
                                        {
                                            #region Si se toma el tipo de cambio de emision
                                            #region 30 dias
                                            foreach (var l in lista30)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad30 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad30 = (l.Importe - l.Descuento);
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad30 = (l.Importe - l.Descuento) * l.TipoCambio;
                                                    else
                                                        cantidad30 = (l.Importe - l.Descuento) / l.TipoCambio;
                                                }
                                                cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, contrato.Moneda, l.Moneda);
                                                if (busquedaDoble)
                                                    cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, contrato.Moneda, l.Moneda);
                                                total30 += cantidad30;
                                                l.Importe30 = cantidad30;
                                                if (contrato.Moneda != "P")
                                                    totalPesos30 += cantidad30 * l.TipoCambio;
                                                else
                                                    totalPesos30 += cantidad30;
                                            }
                                            #endregion

                                            #region 60 dias
                                            foreach (var l in lista60)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad60 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad60 = (l.Importe - l.Descuento);
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad60 = (l.Importe - l.Descuento) * l.TipoCambio;
                                                    else
                                                        cantidad60 = (l.Importe - l.Descuento) / l.TipoCambio;
                                                }
                                                cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, contrato.Moneda, l.Moneda);
                                                if (busquedaDoble)
                                                    cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, contrato.Moneda, l.Moneda);
                                                total60 += cantidad60;
                                                l.Importe60 = cantidad60;
                                                if (contrato.Moneda != "P")
                                                    totalPesos60 += cantidad60 * l.TipoCambio;
                                                else
                                                    totalPesos60 += cantidad60;
                                            }
                                            #endregion

                                            #region 90 dias
                                            foreach (var l in lista90)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad90 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad90 = (l.Importe - l.Descuento);
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad90 = (l.Importe - l.Descuento) * l.TipoCambio;
                                                    else
                                                        cantidad90 = (l.Importe - l.Descuento) / l.TipoCambio;
                                                }
                                                cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, contrato.Moneda, l.Moneda);
                                                if (busquedaDoble)
                                                    cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, contrato.Moneda, l.Moneda);
                                                total90 += cantidad90;
                                                l.Importe90 = cantidad90;
                                                if (contrato.Moneda != "P")
                                                    totalPesos90 += cantidad90 * l.TipoCambio;
                                                else
                                                    totalPesos90 += cantidad90;
                                            }
                                            #endregion

                                            #region Mas de 90 dias
                                            foreach (var l in listaMas90)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidadMas90 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidadMas90 = (l.Importe - l.Descuento);
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidadMas90 = (l.Importe - l.Descuento) * l.TipoCambio;
                                                    else
                                                        cantidadMas90 = (l.Importe - l.Descuento) / l.TipoCambio;
                                                }
                                                cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, contrato.Moneda, l.Moneda);
                                                if (busquedaDoble)
                                                    cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, contrato.Moneda, l.Moneda);
                                                totalMas90 += cantidadMas90;
                                                l.ImporteMas90 = cantidadMas90;
                                                if (contrato.Moneda != "P")
                                                    totalPesosMas90 += cantidadMas90 * l.TipoCambio;
                                                else
                                                    totalPesosMas90 += cantidadMas90;
                                            }
                                            #endregion
                                            #endregion
                                        }
                                        else
                                        {
                                            #region Si no se toma el tipo de cambio de emision
                                            //SIN IVA
                                            #region 30 dias
                                            foreach (var l in lista30)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad30 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad30 = (l.Importe - l.Descuento);
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        //cantidad30 = (l.Importe - l.Descuento) * l.TipoCambio;
                                                        cantidad30 = (l.Importe - l.Descuento) * tipoCambio;
                                                    else
                                                        //cantidad30 = (l.Importe - l.Descuento) / l.TipoCambio;
                                                        cantidad30 = (l.Importe - l.Descuento) / tipoCambio;
                                                }
                                                //cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio, contrato.Moneda, l.Moneda);
                                                cantidad30 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                if (busquedaDoble)
                                                    cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                //cantidad30 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                total30 += cantidad30;
                                                l.Importe30 = cantidad30;
                                                if (contrato.Moneda != "P")
                                                    totalPesos30 += cantidad30 * tipoCambio;
                                                else
                                                    totalPesos30 += cantidad30;
                                            }
                                            #endregion

                                            #region 60 dias
                                            foreach (var l in lista60)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad60 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad60 = (l.Importe - l.Descuento);
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        //cantidad60 = (l.Importe - l.Descuento) * l.TipoCambio;
                                                        cantidad60 = (l.Importe - l.Descuento) * tipoCambio;
                                                    else
                                                        //cantidad60 = (l.Importe - l.Descuento) / l.TipoCambio;
                                                        cantidad60 = (l.Importe - l.Descuento) / tipoCambio;
                                                }
                                                cantidad60 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                if (busquedaDoble)
                                                    cantidad60 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                total60 += cantidad60;
                                                l.Importe60 = cantidad60;
                                                if (contrato.Moneda != "P")
                                                    totalPesos60 += cantidad60 * tipoCambio;
                                                else
                                                    totalPesos60 += cantidad60;
                                            }
                                            #endregion

                                            #region 90 dias
                                            foreach (var l in lista90)
                                            {
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidad90 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidad90 = (l.Importe - l.Descuento);
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidad90 = (l.Importe - l.Descuento) * tipoCambio;
                                                    else
                                                        cantidad90 = (l.Importe - l.Descuento) / tipoCambio;
                                                }
                                                cantidad90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                if (busquedaDoble)
                                                    cantidad90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                total90 += cantidad90;
                                                l.Importe90 = cantidad90;
                                                if (contrato.Moneda != "P")
                                                    totalPesos90 += cantidad90 * tipoCambio;
                                                else
                                                    totalPesos90 += cantidad90;
                                            }
                                            #endregion

                                            #region Mas de 90 dias
                                            //SIN IVA Y T.C. DE CORTE
                                            totalMas90 = 0.0M;
                                            totalPesosMas90 = 0.0M;
                                            foreach (var l in listaMas90)
                                            {
                                                if (l.IDCliente == "SPD040")
                                                    l.IDCliente = "SPD040";
                                                bool busquedaDoble = l.IDHistRec != l.Numero;
                                                decimal cantidadMas90 = 0;
                                                if (contrato.Moneda == l.Moneda)
                                                    cantidadMas90 = (l.Importe - l.Descuento);
                                                else
                                                {
                                                    if (contrato.Moneda == "P")
                                                        cantidadMas90 = (l.Importe - l.Descuento) * tipoCambio;
                                                    else
                                                        cantidadMas90 = (l.Importe - l.Descuento) / tipoCambio;
                                                }
                                                cantidadMas90 -= getTotalNotasCredito(l.IDHistRec, fechaCorte, false, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                if (busquedaDoble)
                                                    cantidadMas90 -= getTotalNotasCredito(l.Numero, fechaCorte, false, idInmobiliaria, tipoCambio, contrato.Moneda, l.MonedaPago);
                                                totalMas90 += cantidadMas90;
                                                l.ImporteMas90 = cantidadMas90;
                                                if (contrato.Moneda != "P")
                                                    totalPesosMas90 += cantidadMas90 * tipoCambio;
                                                else
                                                    totalPesosMas90 += cantidadMas90;
                                            }
                                            #endregion
                                            #endregion
                                        }

                                        registroCartera.Total30 = total30;
                                        registroCartera.Total60 = total60;
                                        registroCartera.Total90 = total90;
                                        registroCartera.TotalMas90 = totalMas90;
                                        registroCartera.TotalPesos30 = totalPesos30;
                                        registroCartera.TotalPesos60 = totalPesos60;
                                        registroCartera.TotalPesos90 = totalPesos90;
                                        registroCartera.TotalPesosMas90 = totalPesosMas90;
                                        #endregion
                                    }
                                }

                                registroCartera.Recibos30 = lista30;
                                registroCartera.Recibos60 = lista60;
                                registroCartera.Recibos90 = lista90;
                                registroCartera.RecibosMas90 = listaMas90;

                                listaCartera.Add(registroCartera);
                            }


                            decimal totalPesos30Convertidos = listaCartera.Where(c => c.MonedaContrato != "P").Sum(c => c.TotalPesos30);
                            decimal totalPesos60Convertidos = listaCartera.Where(c => c.MonedaContrato != "P").Sum(c => c.TotalPesos60);
                            decimal totalPesos90Convertidos = listaCartera.Where(c => c.MonedaContrato != "P").Sum(c => c.TotalPesos90);
                            decimal totalPesosMas90Convertidos = listaCartera.Where(c => c.MonedaContrato != "P").Sum(c => c.TotalPesosMas90);
                            decimal totalPesosConvertidos = listaCartera.Where(c => c.MonedaContrato != "P").Sum(c => c.TotalPesos);

                            OnCambioProgreso(80);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";

                            #region Prepara Reporte
                            string nombreInmobiliaria = SaariDB.getNombreInmobiliariaByID(idInmobiliaria);
                            string nombreConjunto = SaariDB.getNombreConjuntoByID(idConjunto);
                            listaCartera = listaCartera.OrderBy(l => l.RazonSocialCliente).ToList();

                            EncabezadoEntity encabezado = new EncabezadoEntity()
                            {
                                Inmobiliaria = nombreInmobiliaria,
                                FechaFin = fechaCorte.ToString(@"dd \de MMMM \del yyyy").ToLower(),
                                Usuario = usuario,
                                Conjunto = nombreConjunto,
                                Mes30 = getNombreMes(fechaCorte.Month),
                                Mes60 = getNombreMes(fechaCorte.AddDays(-30).Month),
                                Mes90 = getNombreMes(fechaCorte.AddDays(-60).Month),
                                MesMas90 = getNombreMes(fechaCorte.AddDays(-90).Month),
                                TipoCambio = tipoCambio.ToString("N2")
                            };
                            List<EncabezadoEntity> listaEncabezado = new List<EncabezadoEntity>();
                            listaEncabezado.Add(encabezado);

                            OnCambioProgreso(90);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";

                            Report report = new Report();
                            report.Load(rutaFormato);
                            report.RegisterData(listaEncabezado, "Encabezado");
                            report.RegisterData(listaCartera, "Recibo", 3);

                            DataBand bandaRecibos = report.FindObject("Data1") as DataBand;
                            bandaRecibos.DataSource = report.GetDataSource("Recibo");

                            DataBand bandaFacturas = report.FindObject("Data2") as DataBand;
                            bandaFacturas.DataSource = report.GetDataSource("Recibo.Recibos");

                            TextObject textoTotalPesos30 = report.FindObject("textoPesos30") as TextObject;
                            textoTotalPesos30.Text = totalPesos30Convertidos.ToString("N2");

                            TextObject textoTotalPesos60 = report.FindObject("textoPesos60") as TextObject;
                            textoTotalPesos60.Text = totalPesos60Convertidos.ToString("N2");

                            TextObject textoTotalPesos90 = report.FindObject("textoPesos90") as TextObject;
                            textoTotalPesos90.Text = totalPesos90Convertidos.ToString("N2");

                            TextObject textoTotalPesosMas90 = report.FindObject("textoPesosMas90") as TextObject;
                            textoTotalPesosMas90.Text = totalPesosMas90Convertidos.ToString("N2");

                            TextObject textoTotalPesos = report.FindObject("textoPesos") as TextObject;
                            textoTotalPesos.Text = totalPesosConvertidos.ToString("N2");

                            TextObject textoTipoCambio = report.FindObject("memoTipoCambio") as TextObject;
                            textoTipoCambio.Visible = !tomarTcEmision;

                            if (!esDetallado)
                                bandaFacturas.Visible = false;
                            return exportar(report, esPdf, "CarteraVencida306090Contratos");

                            #endregion
                        }
                        else
                            return "No se encontraron recibos con las condiciones dadas";
                    }
                    else
                        return "Error al obtener los recibos";
                }
                else
                    return "No se encontro el formato " + rutaFormato + Environment.NewLine;
            }
            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte: " + Environment.NewLine + ex.Message;
            }

        }

        private string getNombreMes(int numMes)
        {
            switch (numMes)
            {
                case 1: return "Enero";
                case 2: return "Febrero";
                case 3: return "Marzo";
                case 4: return "Abril";
                case 5: return "Mayo";
                case 6: return "Junio";
                case 7: return "Julio";
                case 8: return "Agosto";
                case 9: return "Septiembre";
                case 10: return "Octubre";
                case 11: return "Noviembre";
                case 12: return "Diciembre";
                default: return string.Empty;
            }
        }

        private decimal getTotalNotasCredito(int idHistRec, DateTime fechaCorte, bool incluirIva)
        {
            return SaariDB.getSumaDeNotaCredOPagoParcial(idHistRec, fechaCorte, incluirIva);
        }

        private decimal getTotalNotasCredito(int idHistRec, DateTime fechaCorte, bool incluirIva, string monedaContrato, string monedaRecibo)
        {
            //return SaariDB.getSumaDeNotaCredOPagoParcial(idHistRec, fechaCorte, incluirIva, monedaContrato, monedaRecibo);
            return SaariDB.getSumaDeNotaCredOPagoParcial(idHistRec, fechaCorte, monedaContrato, incluirIva);
        }

        private decimal getTotalNotasCredito(int numRec, DateTime fechaCorte, bool incluirIva, string idArrendadora)
        {
            return SaariDB.getSumaDeNotaCredOPagoParcial(numRec, fechaCorte, incluirIva, idArrendadora);
        }

        private decimal getTotalNotasCredito(int numRec, DateTime fechaCorte, bool incluirIva, string idArrendadora, string monedaContrato, string monedaRecibo)
        {
            //return SaariDB.getSumaDeNotaCredOPagoParcial(numRec, fechaCorte, incluirIva, idArrendadora);
            return SaariDB.getSumaDeNotaCredOPagoParcial(numRec, fechaCorte, monedaContrato, incluirIva);
        }

        public decimal obtenerTipoCambio(DateTime fechaCorte)
        {
            return SaariDB.getTipoCambio(fechaCorte);
        }

        private decimal getTotalNotasCredito(int idHistRec, DateTime fechaCorte, bool incluirIva, decimal tipoCambio)
        {
            return SaariDB.getSumaDeNotaCredOPagoParcial(idHistRec, fechaCorte, incluirIva, tipoCambio);
        }

        private decimal getTotalNotasCredito(int idHistRec, DateTime fechaCorte, bool incluirIva, decimal tipoCambio, string monedaContrato, string monedaRecibo)
        {
            return SaariDB.getSumaDeNotaCredOPagoParcial(idHistRec, fechaCorte, incluirIva, tipoCambio, monedaContrato, monedaRecibo);
        }

        private decimal getTotalNotasCredito(int numRec, DateTime fechaCorte, bool incluirIva, string idArrendadora, decimal tipoCambio)
        {
            return SaariDB.getSumaDeNotaCredOPagoParcial(numRec, fechaCorte, incluirIva, idArrendadora, tipoCambio);
        }

        private decimal getTotalNotasCredito(int numRec, DateTime fechaCorte, bool incluirIva, string idArrendadora, decimal tipoCambio, string monedaContrato, string monedaRecibo)
        {
            return SaariDB.getSumaDeNotaCredOPagoParcial(numRec, fechaCorte, incluirIva, idArrendadora, tipoCambio, monedaContrato, monedaRecibo);
        }
    }
}
