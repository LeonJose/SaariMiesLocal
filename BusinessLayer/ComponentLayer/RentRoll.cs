using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using System.IO;
using GestorReportes.BusinessLayer.Abstracts;
using GestorReportes.BusinessLayer.Interfaces;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class RentRoll : SaariReport, IReport, IBackgroundReport
    {
        private string idInmobiliaria = string.Empty, nombreInmobiliaria = string.Empty, idConjunto = string.Empty, nombreConjunto = string.Empty, rutaFormato = string.Empty;
        private DateTime inicio = new DateTime(), fin = DateTime.Now.Date;
        private bool esPdf = true;
        public RentRoll(string idInmobiliaria, string nombreInmobiliaria, string idConjunto, string nombreConjunto, DateTime inicio, DateTime fin, bool esPdf, string rutaFormato)
        {
            this.idInmobiliaria = idInmobiliaria;
            this.nombreInmobiliaria = nombreInmobiliaria;
            this.idConjunto = idConjunto;
            this.nombreConjunto = nombreConjunto;
            this.inicio = inicio.Date;
            this.fin = fin.Date;
            this.esPdf = esPdf;
            this.rutaFormato = rutaFormato;
        }

        public string generar()
        {
            try
            {
                string resultValidar = validarParametros(idInmobiliaria, nombreInmobiliaria, idConjunto, nombreConjunto, inicio, fin, rutaFormato);
                OnCambioProgreso(10);
                if (CancelacionPendiente)
                    return "Proceso cancelado por el usuario";
                if (string.IsNullOrEmpty(resultValidar))
                {
                    List<RentRollEntity> listaRegistros = SaariDB.getListaRentRoll(idInmobiliaria, idConjunto, inicio, fin);
                    if (listaRegistros != null)
                    {
                        if (listaRegistros.Count > 0)
                        {
                            OnCambioProgreso(40);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";
                            EncabezadoEntity encabezado = new EncabezadoEntity();
                            encabezado.Inmobiliaria = nombreInmobiliaria;
                            encabezado.Conjunto = nombreConjunto;
                            encabezado.FechaInicio = inicio.ToString("MMMM yyyy").ToUpper();
                            List<EncabezadoEntity> listaEncabezados = new List<EncabezadoEntity>();
                            listaEncabezados.Add(encabezado);

                            OnCambioProgreso(50);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";

                            List<string> listaContratos = listaRegistros.Select(c => c.IDContrato).Distinct().ToList();
                            List<RentRollEntity> listaFinal = new List<RentRollEntity>();
                            TotalesRentRollEntity totales = new TotalesRentRollEntity();
                            totales.TotalEmitidoDP = 0;
                            totales.TotalTotalDP = 0;
                            int contadorContPesos = 0;
                            int contadorContDolares = 0;
                            decimal sumaPromediosPesos = 0;
                            decimal sumaPromediosDolares = 0;
                            

                            foreach (string idContrato in listaContratos)
                            {
                                RentRollEntity rent = new RentRollEntity();
                                List<RentRollEntity> registrosPorContrato = (from r in listaRegistros
                                                                             where r.IDContrato == idContrato
                                                                             select r).ToList();

                                rent.NombreInmueble = registrosPorContrato.First().NombreInmueble;
                                rent.IdInmueble = registrosPorContrato.First().IdInmueble;
                                rent.NombreCliente = registrosPorContrato.First().NombreCliente;
                                if (rent.NombreCliente == "GARCIA REYES MARTIN ANGEL")
                                    System.Diagnostics.Debugger.Break();
                                rent.NombreClienteComercial = registrosPorContrato.First().NombreClienteComercial;
                                rent.MetrosCuadradosConstruccion = registrosPorContrato.First().MetrosCuadradosConstruccion;
                                rent.MetrosCuadradosDisponibles = registrosPorContrato.First().MetrosCuadradosDisponibles;
                                rent.ImporteRentaEmitido = registrosPorContrato.Sum(i => i.ImporteRentaEmitido);
                                if (rent.MetrosCuadradosConstruccion > 0 && rent.ImporteRentaEmitido > 0)
                                    rent.PrecioMetroCuadrado = rent.ImporteRentaEmitido / rent.MetrosCuadradosConstruccion;
                                else
                                    rent.PrecioMetroCuadrado = 0;
                                rent.ImporteMantenimientoEmitido = registrosPorContrato.Sum(m => m.ImporteMantenimientoEmitido);
                                //Add JL | 17/10/2018
                                if (rent.ImporteMantenimientoEmitido !=0 && rent.MetrosCuadradosConstruccion !=0)
                                {
                                    rent.PrecioMantoEntreMts = rent.ImporteMantenimientoEmitido / rent.MetrosCuadradosConstruccion;
                                    
                                }
                                rent.RentasAnticipadasEmitidas = registrosPorContrato.Sum(r => r.RentasAnticipadasEmitidas);
                                rent.OtrosEmitidos = registrosPorContrato.Sum(o => o.OtrosEmitidos);
                                rent.ImporteRentaCobrado = registrosPorContrato.Sum(c => c.ImporteRentaCobrado);
                                rent.ImporteMantenimientoCobrado = registrosPorContrato.Sum(m => m.ImporteMantenimientoCobrado);
                                rent.ImportePublicidadEmitido = registrosPorContrato.Sum(p => p.ImportePublicidadEmitido);
                                //Add JL | 17/10/2018
                                if (rent.ImportePublicidadEmitido != 0 && rent.MetrosCuadradosConstruccion !=0)
                                {
                                    rent.PrecioPubEntreMts = rent.ImportePublicidadEmitido / rent.MetrosCuadradosConstruccion;
                                   
                                }
                                rent.RentaVariableEmitida = registrosPorContrato.Sum(v => v.RentaVariableEmitida);
                                rent.RentaVariableCobrado = registrosPorContrato.Sum(v => v.RentaVariableCobrado);
                                rent.ImporteServiciosEmitido = registrosPorContrato.Sum(s => s.ImporteServiciosEmitido);
                                rent.DescuentoEmitido = registrosPorContrato.Sum(s => s.DescuentoEmitido);
                                rent.OtrosCobrado = registrosPorContrato.Sum(o => o.OtrosCobrado);
                                rent.ClasificacionUbicacion = registrosPorContrato.First().ClasificacionUbicacion;
                                rent.Moneda = registrosPorContrato.First().Moneda;
                                rent.TipoDeCambioEmitido = registrosPorContrato.First().TipoDeCambioEmitido;
                                rent.MonedaID = registrosPorContrato.First().MonedaID;
                                //rent.ImpuestoIVA = registrosPorContrato.First().ImpuestoIVA;
                                //Add JL | 04/04/2019
                                rent.ImpuestoIVA = registrosPorContrato.Sum(i => i.ImpuestoIVA);


                                List<RentRollEntity> listaPesos = (from r in registrosPorContrato where r.MonedaID == "P" select r).ToList();
                                List<RentRollEntity> listaDolares = (from r in registrosPorContrato where r.MonedaID == "D" select r).ToList();
                                decimal totalImporte = rent.TotalEmitido;
                                //Add JL | 17/10/2018
                                rent.IngresoAntImpuesto = rent.TotalEmitido - rent.ImpuestoIVA;
                                
                                decimal sumaPesos = listaPesos.Sum(lp => lp.TotalEmitido);
                                decimal sumaDolares = listaDolares.Sum(ld => (ld.TotalEmitido * ld.TipoDeCambioEmitido));
                                
                                totales.TotalEmitidoDP =  (listaPesos.Sum(lp => lp.TotalEmitido) + listaDolares.Sum(ld => (ld.TotalEmitido * ld.TipoDeCambioEmitido)));
                                totales.TotalDescuentosDP += ((listaPesos.Sum(lp => lp.DescuentoEmitido) + listaDolares.Sum(ld => (ld.DescuentoEmitido * ld.TipoDeCambioEmitido))) * (-1));                                
                                totales.TotalTotalDP += (totales.TotalEmitidoDP - totales.TotalDescuentosDP);                                                                                              
                                totales.TotalContratosDP += (listaPesos.Sum(lp => lp.ImporteRentaEmitido) + listaDolares.Sum(ld => (ld.ImporteRentaEmitido * ld.TipoDeCambioEmitido)));
                                
                                decimal promedioPesos =0;
                                decimal promedioDolares=0;
                                if (listaPesos.Count > 0)
                                {
                                    promedioPesos = (listaPesos.Sum(lp => (lp.ImporteRentaEmitido > 0 && lp.MetrosCuadradosConstruccion > 0) ? (lp.ImporteRentaEmitido / lp.MetrosCuadradosConstruccion) : 0));// / listaPesos.Count;
                                    contadorContPesos++;
                                    sumaPromediosPesos += promedioPesos;
                                }
                                if (listaDolares.Count > 0)
                                {
                                    promedioDolares = (listaDolares.Sum(ld => (ld.ImporteRentaEmitido > 0 && ld.MetrosCuadradosConstruccion > 0) ? ((ld.ImporteRentaEmitido / ld.MetrosCuadradosConstruccion)) * ld.TipoDeCambioEmitido : 0));// / listaDolares.Count;
                                    contadorContDolares++;
                                    sumaPromediosDolares += promedioDolares;
                                }
                                //totales.TotalPrecioPromedioDP += (promedioPesos + promedioDolares) ;
                                                                                               
                                totales.TotalRentaVariableDP += (listaPesos.Sum(lp => lp.RentaVariableEmitida) + listaDolares.Sum(ld => (ld.RentaVariableEmitida * ld.TipoDeCambioEmitido)));
                                totales.TotalMantenimientoDP += (listaPesos.Sum(lp => lp.ImporteMantenimientoEmitido) + listaDolares.Sum(ld => (ld.ImporteMantenimientoEmitido * ld.TipoDeCambioEmitido)));
                                totales.TotalPublicidadDP += (listaPesos.Sum(lp => lp.ImportePublicidadEmitido) + listaDolares.Sum(ld => (ld.ImportePublicidadEmitido * ld.TipoDeCambioEmitido)));
                                totales.TotalServiciosDP += (listaPesos.Sum(lp => lp.ImporteServiciosEmitido) + listaDolares.Sum(ld => (ld.ImporteServiciosEmitido * ld.TipoDeCambioEmitido)));
                                totales.TotalOtrosDP += (listaPesos.Sum(lp => lp.OtrosEmitidos) + listaDolares.Sum(ld => (ld.OtrosEmitidos * ld.TipoDeCambioEmitido)));
                                //totales.TotalAreaDisponible +=(listaPesos.Sum(lp => lp.MetrosCuadradosDisponibles) + listaDolares.Sum(ld => ld.MetrosCuadradosDisponibles)) ;
                                //totales.TotalAraConstruida += (listaPesos.Sum(lp => lp.MetrosCuadradosConstruccion) + listaDolares.Sum(ld => ld.MetrosCuadradosConstruccion)) ;
                                totales.TotalAreaDisponible += rent.MetrosCuadradosDisponibles;
                                totales.TotalAraConstruida += rent.MetrosCuadradosConstruccion;
                                listaFinal.Add(rent);
                            }
                            totales.TotalPrecioPromedioDP = (sumaPromediosPesos + sumaPromediosDolares) / (contadorContPesos + contadorContDolares);
                            //totales.TotalPrecioPromedioDP = totales.TotalPrecioPromedioDP / listaContratos.Count;
                            OnCambioProgreso(70);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";
                            listaFinal = listaFinal.OrderBy(l => l.NombreInmueble).ToList();
                            //TotalesRentRollEntity totales = new TotalesRentRollEntity();
                            totales.TotalFacturado = listaFinal.Sum(l => l.TotalEmitido);
                            totales.TotalCobrado = listaFinal.Sum(l => l.TotalCobrado);
                            totales.TotalDescuentos = listaFinal.Sum(d => d.DescuentoEmitido);
                            totales.TotalDescuentos = totales.TotalDescuentos * (-1);
                            
                            totales.GranTotal = totales.TotalFacturado - totales.TotalDescuentos;
                            
                            //List<RentRollEntity> listaPesos = (from r in listaFinal where r.MonedaID == "P" select r).ToList();
                            //List<RentRollEntity> listaDolares = (from r in listaFinal where r.MonedaID == "D" select r).ToList();                            
                            //List<RentRollEntity> listaPesos2 = (from r in listaRegistros where r.MonedaID == "P" select r).ToList();
                            //List<RentRollEntity> listaDolares2 = (from r in listaRegistros where r.MonedaID == "D" select r).ToList();           

                            //totales.TotalPrecioPromedioDP = (listaPesos2.Sum(lp => lp.PrecioMetroCuadrado) + listaDolares2.Sum(ld => (ld.PrecioMetroCuadrado * ld.TipoDeCambioEmitido))) / (listaPesos2.Count + listaDolares2.Count);
                            //totales.TotalContratosDP = listaPesos2.Sum(lp=>lp.ImporteRentaEmitido) + listaDolares2.Sum(ld=>(ld.ImporteRentaEmitido * ld.TipoDeCambioEmitido)) ;
                            //totales.TotalRentaVariableDP = listaPesos2.Sum(lp=>lp.RentaVariableEmitida) + listaDolares2.Sum(ld=>(ld.RentaVariableEmitida * ld.TipoDeCambioEmitido)) ;
                            //totales.TotalMantenimientoDP = listaPesos2.Sum(lp=>lp.ImporteMantenimientoEmitido) + listaDolares2.Sum(ld=>(ld.ImporteMantenimientoEmitido * ld.TipoDeCambioEmitido)) ;
                            //totales.TotalPublicidadDP = listaPesos.Sum(lp=>lp.ImportePublicidadEmitido) + listaDolares2.Sum(ld=>(ld.ImportePublicidadEmitido * ld.TipoDeCambioEmitido)) ;
                            //totales.TotalServiciosDP = listaPesos.Sum(lp=>lp.ImporteServiciosEmitido) + listaDolares2.Sum(ld=>(ld.ImporteServiciosEmitido * ld.TipoDeCambioEmitido)) ;
                            //totales.TotalOtrosDP = listaPesos.Sum(lp=>lp.OtrosEmitidos) + listaDolares2.Sum(ld=>(ld.OtrosEmitidos * ld.TipoDeCambioEmitido));
                            //totales.TotalDescuentosDP = (listaPesos.Sum(lp=>lp.DescuentoEmitido) + listaDolares.Sum(ld=>(ld.DescuentoEmitido * ld.TipoDeCambioEmitido))) * (-1) ;
                            //decimal sumaPesos = listaPesos.Sum(lp => lp.TotalEmitido);
                            //decimal sumaDolares = listaDolares.Sum(ld => (ld.TotalEmitido * ld.TipoDeCambioEmitido));
                            //totales.TotalEmitidoDP = listaPesos.Sum(lp => lp.TotalEmitido) + listaDolares.Sum(ld => (ld.TotalEmitido * ld.TipoDeCambioEmitido));
                            //totales.TotalTotalDP = totales.TotalEmitidoDP - totales.TotalDescuentosDP;
                            //totales.TotalAreaDisponible = listaFinal.Sum(l => l.MetrosCuadradosDisponibles);
                            //totales.TotalAraConstruida = listaFinal.Sum(l => l.MetrosCuadradosConstruccion);

                            totales.Bancos = new List<BancosRentRoll>();
                            List<string> listaBancos = listaRegistros.Select(b => b.CuentaBancaria).Distinct().ToList();
                            OnCambioProgreso(80);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";
                            foreach (string cta in listaBancos)
                            {
                                List<RentRollEntity> registrosPorBanco = (from r in listaRegistros
                                                                          where r.CuentaBancaria == cta
                                                                          select r).ToList();
                                totales.Bancos.Add(new BancosRentRoll() { NombreBanco = cta, Total = registrosPorBanco.Sum(r => r.TotalCobrado) });
                            }
                            OnCambioProgreso(90);
                            if (CancelacionPendiente)
                                return "Proceso cancelado por el usuario";
                            totales.TotalCobrando = totales.Bancos.Sum(l => l.Total);
                            List<TotalesRentRollEntity> listaTotales = new List<TotalesRentRollEntity>();
                            listaTotales.Add(totales);
                            Report report = new Report();
                            report.Load(rutaFormato);
                            report.RegisterData(listaEncabezados, "Encabezado");
                            report.RegisterData(listaFinal, "Registro");
                            DataBand bandaRegistros = report.FindObject("Data1") as DataBand;
                            bandaRegistros.DataSource = report.GetDataSource("Registro");
                            report.RegisterData(listaTotales, "Total");
                            report.RegisterData(totales.Bancos, "Banco");
                            DataBand bandaBancos = report.FindObject("Data2") as DataBand;
                            bandaBancos.DataSource = report.GetDataSource("Banco");
                            

                            return exportar(report, esPdf, "RentRoll");
                        }
                        else
                            return "No se encontraron registro en la base de datos, de acuerdo a los parametros establecidos";
                    }
                    else
                        return "Error al obtener los registros en la base de datos";
                }
                else
                    return resultValidar;
            }
            catch (Exception ex)
            {
                return "Error inesperado al generar el reporte: " + ex.Message;
            }
        }

        private string validarParametros(string idInmobiliaria, string nombreInmobiliaria, string idConjunto, string nombreConjunto, DateTime inicio, DateTime fin, string rutaFormato)
        {
            try
            {
                string errores = string.Empty;
                if (string.IsNullOrEmpty(idInmobiliaria))
                    errores += "No se encontró el identificador de la inmobiliaria" + Environment.NewLine;
                if (string.IsNullOrEmpty(nombreInmobiliaria))
                    errores += "No se encontró el nombre de la inmobiliaria" + Environment.NewLine;
                if (string.IsNullOrEmpty(idConjunto))
                    errores += "No se encontró el identificador del conjunto" + Environment.NewLine;
                if (string.IsNullOrEmpty(nombreConjunto))
                    errores += "No se encontró el nombre del conjunto" + Environment.NewLine;
                if (inicio >= fin)
                    errores += "La fecha de inicio debe ser menor a la fecha de fin" + Environment.NewLine;
                if (!File.Exists(rutaFormato))
                    errores += "No se encontró el formato " + rutaFormato + Environment.NewLine;
                return errores;
            }
            catch (Exception ex)
            {
                return "Error inesperado de validación: " + ex.Message;
            }
        }
    }
}
