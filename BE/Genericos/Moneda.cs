namespace BE
{
    public class Moneda
    {
        public int IdMoneda { get; set; }
        public string NombreMoneda { get; set; } = string.Empty; // Unique
        public string Simbolo { get; set; }
        public decimal ValorCambio { get; set; }
        public bool Deshabilitado { get; set; }
    }
}
