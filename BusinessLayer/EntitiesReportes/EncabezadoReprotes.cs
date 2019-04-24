using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.BusinessLayer.EntitiesReportes
{
    public enum EncabezadoReprotes
    {}

    public enum EncabezadoReprotesAnalisDeudores1
    {

        No_Local,
        M2, Costo_x_M2,
        Razón_Social,
        Giro,
        Nombre_Comercial,
        Importe_Renta_Sin_IVA

    }
    public enum EncabezadoReprotesAnalisDeudores2
    {
        No_Identificados,
        Total_Facturado_Sin_IVA_Del_Periodo,
        Total_Facturado_Con_IVA_Del_Periodo,
        Saldo_Inmediato_Mes_Anterior,
        Saldo_Total_Mes_Corriente_Mas_Saldo_Inmediato_Mes_Anterior,
        Total_Abono_Mes_Corriente,
        Mes_Ultimo_Abono,
        Saldo_Total,
        Saldo_Total_Con_Intereses,
        Saldo_a_Favor,
        Nota_de_Credito,
        Meses_de_Adeudo,
        Fecha_Inicio_de_Contrato,
        Fecha_Terminación_de_Contrato,
        Deposito_en_Garantía,
        Periodo_de_Gracia,
        Fecha_Inicio_de_Facturación
    }

    public enum EncabezadoCFDIsConsecutivos1
    {
        Fecha,
        Serie,
        Folio,
        Cliente,
        Alias,
        Ubicacion,
        Inmueble,
        Periodo,
        Moneda,
        TC,
        Renta,
        
    }
    public enum EncabezadoCFDIsConsecutivos2
    {
       //No_Clasificados,
      //Rentas_Anticipadas,
        Subtotal,
        IVA,
        Ret_IVA,
        Ret_ISR,
        Total        
    }

    /// <summary>
    ///  Listado para el encabezado del reporte de SaldoVenta
    /// </summary>
    public enum EncabezadoSaldoVentas
    {
        NUM,
        MANZANA,
        LOTE,
        TIPO,
        CLIENTE,
        SUPERFICIE,
        PRECIO_M2,
        TOTAL,
        ESTATUS,
        LOTES_VENDIDOS,
        ENGANCHE_1,
        ENGANCHE_,
        TOTAL_,
        TOTAL_A_PAGAR,
        TOTAL_ABONADO,
        IMPORTE_POR_PAGAR,
        PRC_,
        SUPERFICIE_POR_VENDER
    }

    public enum EncabezadoListadoDeContratos1
    {
        Cliente,
        Inmueble,
        Identificador_Inmueble,
        Conjunto,
        Clasificación,
        Fecha_de_Inicio,
        Fecha_de_Vencimeinto,
        Período,
        Vencimiento_Prorroga,
        Porcenjate_Aumento,
        Fecha_Aumento,
        Moneda,
        Importe_Original,
        Importe_Actual,
        Depósito,
        Actividad,
        Estatus_Contrato/*,
        Tipo_de_Pago,
        Pago_Suspendido,
        Telefonos,
        Datos_del_Cliente_Fax,
        Correo_Electronico*/
    }

}
