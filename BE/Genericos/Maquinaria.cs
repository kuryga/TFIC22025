namespace BE
{
    public class Maquinaria
    {
        public int IdMaquinaria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal CostoPorHora { get; set; }
        public bool Deshabilitado { get; set; }
    }
}
