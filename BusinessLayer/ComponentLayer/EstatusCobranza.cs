using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorReportes.BusinessLayer.EntitiesReportes;
using GestorReportes.BusinessLayer.DataAccessLayer;
using FastReport;
using GestorReportes.BusinessLayer.Abstracts;
using System.ComponentModel;
using System.Windows.Forms;

namespace GestorReportes.BusinessLayer.ComponentLayer
{
    public class EstatusCobranza : SaariReport
    {   //JL 16/08/2018
        private string user;
        private string root;
        private DateTime fechaInicio;
        private DateTime fechaFin;
        private int opcion;
        private int opcionOrden;
        ConjuntoEntity Conjunt;
        InmobiliariaEntity Inmo;
        private BackgroundWorker Wkr = null;
        private bool esPdf = true;


        public EstatusCobranza()
        {

        }

        public EstatusCobranza(InmobiliariaEntity Inmob, ConjuntoEntity Conjunt, string user, string root, DateTime fechaInicio, DateTime fechaFin, int opcion, int opcionOrden,bool esPdf, BackgroundWorker worker)
        {
            this.Inmo = Inmob;
            this.user = user;
            this.root = root;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.opcion = opcion;
            this.opcionOrden = opcionOrden;
            this.esPdf = esPdf;
            this.Wkr = worker;
            this.Conjunt = Conjunt;         
        }

        public string generarReporte()
        {
            string error = string.Empty;
            
            try
            {
                var list = new List<EstadoCobranzaEntity>();
                list = SaariDB.GetEstatusCobranza(Inmo, Conjunt, fechaInicio, fechaFin,opcion, opcionOrden);

                if (list.Count == 0)
                {
                    error = "No existen movimientos";
                    return error;
                }

                OnCambioProgreso(10);
                List<EncabezadoEntity> encabezado = new List<EncabezadoEntity>();
                EncabezadoEntity objEnc = new EncabezadoEntity();

                objEnc.Inmobiliaria = Inmo.NombreComercial;
                objEnc.Usuario = user;
                //opciones rbutton.
                if (opcion == 1)
                {
                    objEnc.Conjunto = "Todos Los Conjuntos";
                }
                else if (opcion == 2)
                {
                     objEnc.Conjunto = Conjunt.Nombre;
                }
                else
                {
                    objEnc.Conjunto = "Todos Los Conjuntos";
                }
                
                objEnc.FechaInicio = fechaInicio.ToString("dd/MM/yyyy");
                objEnc.FechaFin = fechaFin.ToString("dd/MM/yyyy");
                encabezado.Add(objEnc);

                OnCambioProgreso(50);

                Report Report = new Report();
                Report.Load(root);
                Report.RegisterData(encabezado, "Encabezado");
                Report.RegisterData(list, "Detalle");
                DataBand bandDetalle = Report.FindObject("Data1") as DataBand;
                bandDetalle.DataSource = Report.GetDataSource("Detalle");
                OnCambioProgreso(90);
                if (Wkr.CancellationPending)
                    return "Proceso cancelado por el Usuario";
                error = exportar(Report, esPdf, "EstatusCobranza");
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }

    }
}
