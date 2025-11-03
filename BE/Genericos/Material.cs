namespace BE
{
    public class Material
    {
        public int IdMaterial { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal PrecioUnidad { get; set; }
        public decimal UsoPorM2 { get; set; }
        public bool Deshabilitado { get; set; }
    }
}
