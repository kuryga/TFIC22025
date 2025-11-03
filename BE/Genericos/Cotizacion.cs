using System;
using System.Collections.Generic;

namespace BE
{
    // -------- Ítems de la cotización --------
    public class MaterialCotizacion
    {
        public int IdMaterialCotizacion { get; set; }
        public int IdCotizacion { get; set; }
        public Material Material { get; set; }
        public decimal Cantidad { get; set; } // unidades

        public decimal CalcularCostoMaterial()
        {
            var precio = (Material != null) ? Material.PrecioUnidad : 0;
            return precio * Cantidad;
        }
    }

    public class MaquinariaCotizacion
    {
        public int IdMaquinariaCotizacion { get; set; }
        public int IdCotizacion { get; set; }
        public Maquinaria Maquinaria { get; set; }
        public decimal HorasUso { get; set; } // horas estimadas

        public decimal CalcularCostoMaquinaria()
        {
            var costoHora = (Maquinaria != null) ? Maquinaria.CostoPorHora : 0m;
            return costoHora * HorasUso;
        }
    }

    public class ServicioCotizacion
    {
        public int IdServicioCotizacion { get; set; }
        public int IdCotizacion { get; set; }
        public ServicioAdicional Servicio { get; set; }

        public decimal CalcularCostoServicio()
        {
            return (Servicio != null) ? Servicio.Precio : 0m;
        }
    }

    public class Cotizacion
    {
        public int IdCotizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public TipoEdificacion TipoEdificacion { get; set; }
        public Moneda Moneda { get; set; }
        public List<MaterialCotizacion> ListaMateriales { get; set; }
        public List<MaquinariaCotizacion> ListaMaquinaria { get; set; }
        public List<ServicioCotizacion> ListaServicios { get; set; }

        public Cotizacion()
        {
            ListaMateriales = new List<MaterialCotizacion>();
            ListaMaquinaria = new List<MaquinariaCotizacion>();
            ListaServicios = new List<ServicioCotizacion>();
            FechaCreacion = DateTime.UtcNow;
        }

        public string TipoDescripcion
        {
            get { return (TipoEdificacion != null && TipoEdificacion.Descripcion != null) ? TipoEdificacion.Descripcion : string.Empty; }
        }

        public string MonedaNombre
        {
            get { return (Moneda != null && Moneda.NombreMoneda != null) ? Moneda.NombreMoneda : string.Empty; }
        }

        public decimal CalcularCostoMateriales()
        {
            decimal total = 0m;
            if (ListaMateriales != null)
                for (int i = 0; i < ListaMateriales.Count; i++)
                    total += ListaMateriales[i].CalcularCostoMaterial();
            return total;
        }

        public decimal CalcularCostoMaquinaria()
        {
            decimal total = 0m;
            if (ListaMaquinaria != null)
                for (int i = 0; i < ListaMaquinaria.Count; i++)
                    total += ListaMaquinaria[i].CalcularCostoMaquinaria();
            return total;
        }

        public decimal CalcularCostoServicios()
        {
            decimal total = 0m;
            if (ListaServicios != null)
                for (int i = 0; i < ListaServicios.Count; i++)
                    total += ListaServicios[i].CalcularCostoServicio();
            return total;
        }

        public decimal CalcularCostoTotal()
        {
            return CalcularCostoMateriales() + CalcularCostoMaquinaria() + CalcularCostoServicios();
        }

        public decimal CalcularTiempoEstimadoHoras()
        {
            decimal horas = 0m;
            if (ListaMaquinaria != null)
                for (int i = 0; i < ListaMaquinaria.Count; i++)
                    horas += ListaMaquinaria[i].HorasUso;
            return horas;
        }
    }
}
