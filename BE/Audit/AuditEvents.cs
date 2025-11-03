using System;

namespace BE.Audit
{
    public static class AuditEvents
    {
        // ==== C5 informativas ====
        public const string IngresoSesion = "Autenticacion.IngresoSesion";
        public const string CierreSesion = "Autenticacion.CierreSesion";

        public const string ConsultaFamiliasPorUsuario = "Familias.ConsultaPorUsuario";
        public const string ConsultaPatentesPorUsuario = "Patentes.ConsultaPorUsuario";
        public const string ConsultaPatentesPorFamilia = "Patentes.ConsultaPorFamilia";
        public const string ConsultaUsuariosPorPatente = "Usuarios.ConsultaPatentes";
        public const string ConsultaFamilias = "Familias.ConsultaDeListado";
        public const string ConsultaPatentes = "Patentes.ConsultaDeListado";
       
        public const string ConsultaCotizaciones = "Cotizacion.ConsultaDeListado";
        public const string ConsultaCotizacionDetalle = "Cotizacion.ConsultaDetalle";
        public const string ExportacionCotizacion = "Cotizacion.Exportacion";

        public const string ConsultaMateriales = "Material.ConsultaDeListado";
        public const string ConsultaMaterialPorId = "Material.ConsultaPorId";

        public const string ConsultaMaquinarias = "Maquinaria.ConsultaDeListado";
        public const string ConsultaMaquinariaPorId = "Maquinaria.ConsultaPorId";

        public const string ConsultaServicios = "ServicioAdicional.ConsultaDeListado";
        public const string ConsultaServicioPorId = "ServicioAdicional.ConsultaPorId";

        public const string ConsultaMonedas = "Moneda.ConsultaDeListado";
        public const string ConsultaMonedaPorId = "Moneda.ConsultaPorId";

        public const string ConsultaTiposEdificacion = "TipoEdificacion.ConsultaDeListado";
        public const string ConsultaTipoEdificacionPorId = "TipoEdificacion.ConsultaPorId";

        public const string ConsultaUsuarios = "Usuario.ConsultaDeListado";
        public const string ConsultaUsuarioPorCorreo = "Usuario.ConsultaPorCorreo";

        public const string ConsultaBitacora = "Bitacora.Consulta";
        public const string ConsultaIdiomas = "Idioma.ConsultaDeListado";

        public const string ActivacionUsuarioPorCorreo = "Usuario.ActivacionPorCorreo";

        public const string ConsultaParametrizacion = "Parametrizacion.consulta";

        // ==== C4 (altas / acciones relevantes) ====
        public const string CambioContrasena = "Usuario.CambioContrasena";
        public const string CreacionUsuario = "Usuario.Creacion";

        public const string CreacionCotizacion = "Cotizacion.Creacion";
        public const string AgregarServiciosAdicionales = "Cotizacion.AgregarServiciosAdicionales";

        public const string CreacionMaquinaria = "Maquinaria.Creacion";
        public const string CreacionMaterial = "Material.Creacion";
        public const string AltaMoneda = "Moneda.Creacion";
        public const string CreacionTipoEdificacion = "TipoEdificacion.Creacion";
        public const string CreacionServicioAdicional = "ServicioAdicional.Creacion";

        public const string AsociacionPatenteUsuario = "Usuario.AsociarPatente";
        public const string ForzarCierreSesion = "Autenticacion.ForzarCierreSesion";

        // ==== C3 (modificaciones / cambios de negocio) ====
        public const string CambioTiempoEstimadoCotizacion = "Cotizacion.CambioTiempoEstimado";
        public const string ModificacionValorMoneda = "Moneda.ModificacionValor";
        public const string ModificacionMaterial = "Material.Modificacion";
        public const string ModificacionMaquinaria = "Maquinaria.Modificacion";
        public const string ModificacionCantidadMaterialCtz = "Cotizacion.ModificacionCantidadMaterial";
        public const string ModificacionMaquinariaAsignadaCtz = "Cotizacion.ModificacionMaquinariaAsignada";
        public const string ModificacionPersonalOTiempo = "Cotizacion.ModificacionPersonalOTiempo";
        public const string GeneracionInformeCotizacion = "Cotizacion.GeneracionInforme";
        public const string CreacionOModificacionFamilia = "Familia.CreacionOModificacion";
        public const string ModificacionUsuario = "Usuario.Modificacion";
        public const string RecuperacionContrasena = "Autenticacion.RecuperacionContrasena";
        public const string EliminacionCotizacion = "Cotizacion.Eliminacion";
        public const string ModificacionTipoEdificacion = "TipoEdificacion.Modificacion";
        public const string ModificacionCotizacionHeader = "Cotizacion.ModificacionHeader";
        public const string ModificacionServicioAdicional = "ServicioAdicional.Modificacion";
        public const string ModificarFamiliasUsuario = "Familias.ModificacionPorUsuario";
        public const string ModificarPatentesUsuario = "Patentes.ModificacionPorUsuario";
        public const string AsignarPatentesAFamilia = "Familia.AsignacionDePatente";
        public const string ModificacionFamilia = "Familia.Modificacion";
        public const string HabilitacionTipoEdificacion = "TipoEdificacion.Habilitar";
        public const string DeshabilitacionTipoEdificacion = "TipoEdificacion.Deshabilitar";
        public const string HabilitacionServicioAdicional = "ServicioAdicional.Habilitar";
        public const string DeshabilitacionServicioAdicional = "ServicioAdicional.Deshabilitar";
        public const string DeshabilitacionMoneda = "Moneda.Deshabilitar";
        public const string HabilitacionMoneda = "Moneda.Habilitar";
        public const string DeshabilitacionMaterial = "Material.Deshabilitar";
        public const string HabilitacionMaterial = "Material.Habilitar";
        public const string DeshabilitacionMaquinaria = "Maquinaria.Deshabilitar";
        public const string HabilitacionMaquinaria = "Maquinaria.Habilitar";


        // ==== C2 (acciones sensibles / seguridad operativa) ====
        public const string BajaManualUsuario = "Usuario.BajaManual";
        public const string BloquearUsuario = "Usuario.Bloqueo";
        public const string DesbloquearUsuario = "Usuario.Desbloqueo";
        public const string EliminacionPatenteUsuario = "Usuario.EliminacionPatente";
        public const string EliminacionMaterial = "Material.Eliminacion";
        public const string IntentosFallidosAcceso = "Autenticacion.IntentosFallidos";
        public const string RespaldoBase = "BaseDatos.Respaldo";
        public const string EliminacionPatentesCriticaUsuario = "Patentes.IntentoEliminacionCritica";
        public const string AltaFamilia = "Familia.Alta";
        public const string RecuperoContrasenaSolicitud = "Contrasena.Recupero";
        public const string ExportarReporteBitacora = "Bitacora.ExportarReporte";

        // ==== C1 (muy críticas) ====
        public const string FalloConexionBD = "BaseDatos.FalloConexion";
        public const string FalloVerificacionIntegridad = "DV.FalloVerificacion";
        public const string ReparacionIntegridadDatos = "DV.ReparacionIntegridad";
        public const string RespaldoBaseDatos = "BaseDatos.BackupCreado";
        public const string RestauracionBaseDatos = "BaseDatos.RestoreEjecutado";

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
                case ConsultaUsuarioPorCorreo:
                case ConsultaBitacora:
                case ConsultaIdiomas:
                case ExportacionCotizacion:
                case ActivacionUsuarioPorCorreo:
                case ConsultaParametrizacion:
                case ConsultaFamiliasPorUsuario:
                case ConsultaPatentesPorUsuario:
                case ConsultaPatentesPorFamilia:
                case ConsultaFamilias:
                case ConsultaUsuariosPorPatente:
                    return Criticidad.C5;
            }

            // C4
            switch (accion)
            {
                case CambioContrasena:
                case CreacionUsuario:
                case CreacionCotizacion:
                case AgregarServiciosAdicionales:
                case CreacionMaquinaria:
                case CreacionMaterial:
                case AltaMoneda:
                case CreacionTipoEdificacion:
                case CreacionServicioAdicional:
                case AsociacionPatenteUsuario:
                case ForzarCierreSesion:
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
                case ModificacionTipoEdificacion:
                case ModificacionCotizacionHeader:
                case ModificacionServicioAdicional:
                case ModificarFamiliasUsuario:
                case AsignarPatentesAFamilia:
                case ModificacionFamilia:
                case DeshabilitacionTipoEdificacion:
                case HabilitacionTipoEdificacion:
                case HabilitacionServicioAdicional:
                case DeshabilitacionServicioAdicional:
                case DeshabilitacionMaterial:
                case HabilitacionMaterial:
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
                case AltaFamilia:
                case RecuperoContrasenaSolicitud:
                case ExportarReporteBitacora:
                    return Criticidad.C2;
            }

            // C1
            switch (accion)
            {
                case FalloConexionBD:
                case FalloVerificacionIntegridad:
                case ReparacionIntegridadDatos:
                case RespaldoBaseDatos:
                case RestauracionBaseDatos:
                    return Criticidad.C1;
            }

            return Criticidad.C5;
        }
    }
}
