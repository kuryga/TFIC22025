using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BE.Audit
{
    public static class AuditEvents
    {
        // ==== C5 (informativas / lecturas) ====
        public const string IngresoSesion = "Auth.Login";
        public const string CierreSesion = "Auth.Logout";
        public const string ConsultaCotizaciones = "Cotizacion.QueryList";
        public const string ConsultaCotizacionDetalle = "Cotizacion.Get";
        public const string ConsultaMateriales = "Material.QueryList";
        public const string ConsultaMaterialPorId = "Material.Get";
        public const string ConsultaMaquinarias = "Maquinaria.QueryList";
        public const string ConsultaMaquinariaPorId = "Maquinaria.Get";
        public const string ConsultaServicios = "Servicio.QueryList";
        public const string ConsultaServicioPorId = "Servicio.Get";
        public const string ConsultaMonedas = "Moneda.QueryList";
        public const string ConsultaMonedaPorId = "Moneda.Get";
        public const string ConsultaTiposEdificacion = "TipoEdificacion.QueryList";
        public const string ConsultaTipoEdificacionPorId = "TipoEdificacion.Get";
        public const string ConsultaUsuarios = "User.QueryList";
        public const string ConsultaUsuarioPorCorreo = "Usuario.Consulta.Correo";
        public const string ConsultaBitacora = "Bitacora.Query";
        public const string ConsultaIdiomas = "Idiomas.Query";
        public const string ExportacionCotizacion = "Cotizacion.Export";
        public const string ActivacionUsuarioPorCorreo = "User.ActivationEmail";

        // ==== C4 ====
        public const string CambioContrasena = "User.ChangePassword";
        public const string CreacionUsuario = "Usuario.Creacion";
        public const string CreacionCotizacion = "Cotizacion.Create";
        public const string CreacionMaquinaria = "Maquinaria.Create";
        public const string CreacionMaterial = "Material.Create";
        public const string CreacionTipoEdificacion = "TipoEdificacion.Create";
        public const string CreacionServicioAdicional = "ServicioAdicional.Create";
        public const string AsociacionPatenteUsuario = "User.AttachPatent";
        public const string AltaMoneda = "Moneda.Create";
        public const string AgregarServiciosAdicionales = "Cotizacion.AddServicios";
        public const string ForzarCierreSesion = "Auth.ForceLogout";

        // TODO: revisar si hace falta esto de idiomas, creo que no porque no tengo porque crear idiomas
        public const string CreacionIdioma = "Idioma.Creacion";
        public const string ModificacionIdioma = "Idioma.Modificacion";
        //


        // ==== C3 ====
        public const string CambioTiempoEstimadoCotizacion = "Cotizacion.ChangeTime";
        public const string ModificacionValorMoneda = "Moneda.UpdateValor";
        public const string ModificacionMaterial = "Material.Update";
        public const string ModificacionMaquinaria = "Maquinaria.Update";
        public const string ModificacionCantidadMaterialCtz = "Cotizacion.UpdateMaterialQty";
        public const string ModificacionMaquinariaAsignadaCtz = "Cotizacion.UpdateMaquinaria";
        public const string ModificacionPersonalOTiempo = "Cotizacion.UpdatePersonalTiempo";
        public const string GeneracionInformeCotizacion = "Cotizacion.Report";
        public const string CreacionOModificacionFamilia = "Familia.Upsert";
        public const string ModificacionUsuario = "User.Update";
        public const string RecuperacionContrasena = "Auth.RecoverPassword";
        public const string EliminacionCotizacion = "Cotizacion.Delete";
        public const string ModificacionTipoEdificacion = "TipoEdificacion.Update";
        public const string ModificacionCotizacionHeader = "Cotizacion.UpdateHeader";
        public const string ModificacionServicioAdicional = "ServicioAdicional.Update";

        // ==== C2 ====
        public const string BajaManualUsuario = "User.Disable";
        public const string BloquearUsuario = "User.Block";
        public const string DesbloquearUsuario = "User.Unblock";
        public const string EliminacionPatenteUsuario = "User.RemovePatent";
        public const string EliminacionMaterial = "Material.Delete";
        public const string IntentosFallidosAcceso = "Auth.FailedAttempts";
        public const string RespaldoBase = "DB.Backup";

        // ==== C1 (muy críticas) ====
        public const string FalloConexionBD = "DB.ConnectionFailure";
        public const string FalloVerificacionIntegridad = "DV.VerifyFailure";
        public const string ReparacionIntegridadDatos = "DV.Repair";

        // === Mapeo interno ===
        public static Criticidad GetCriticidad(string accion)
        {
            if (string.IsNullOrEmpty(accion)) return Criticidad.C5;

            // C5
            switch (accion)
            {
                case IngresoSesion:
                case CierreSesion:
                case ConsultaCotizaciones:
                case ConsultaCotizacionDetalle:
                case ConsultaMateriales:
                case ConsultaMaterialPorId:
                case ConsultaMaquinarias:
                case ConsultaMaquinariaPorId:
                case ConsultaServicios:
                case ConsultaServicioPorId:
                case ConsultaMonedas:
                case ConsultaMonedaPorId:
                case ConsultaTiposEdificacion:
                case ConsultaTipoEdificacionPorId:
                case ConsultaUsuarios:
                case ConsultaBitacora:
                case ExportacionCotizacion:
                case ActivacionUsuarioPorCorreo:
                    return Criticidad.C5;
            }

            // C4
            switch (accion)
            {
                // TODO: revisar si hace falta esto de idiomas, creo que no porque no tengo porque crear idiomas
                case CreacionIdioma:
                case ModificacionIdioma:
                //
                case CambioContrasena:
                case CreacionCotizacion:
                case AsociacionPatenteUsuario:
                case AltaMoneda:
                case AgregarServiciosAdicionales:
                case ForzarCierreSesion:
                case CreacionMaquinaria:
                case CreacionMaterial:
                case CreacionTipoEdificacion:
                case CreacionServicioAdicional:
                    return Criticidad.C4;
            }

            // C3
            switch (accion)
            {
                case CambioTiempoEstimadoCotizacion:
                case ModificacionValorMoneda:
                case ModificacionMaterial:
                case ModificacionMaquinaria:
                case ModificacionCantidadMaterialCtz:
                case ModificacionMaquinariaAsignadaCtz:
                case ModificacionPersonalOTiempo:
                case GeneracionInformeCotizacion:
                case CreacionOModificacionFamilia:
                case ModificacionUsuario:
                case RecuperacionContrasena:
                case EliminacionCotizacion:
                case ModificacionCotizacionHeader:
                case ModificacionTipoEdificacion:
                case ModificacionServicioAdicional:
                    return Criticidad.C3;
            }

            // C2
            switch (accion)
            {
                case BajaManualUsuario:
                case BloquearUsuario:
                case DesbloquearUsuario:
                case EliminacionPatenteUsuario:
                case EliminacionMaterial:
                case IntentosFallidosAcceso:
                case RespaldoBase:
                    return Criticidad.C2;
            }

            // C1
            switch (accion)
            {
                case FalloConexionBD:
                case FalloVerificacionIntegridad:
                case ReparacionIntegridadDatos:
                    return Criticidad.C1;
            }

            // por defecto
            return Criticidad.C5;
        }
    }
}
