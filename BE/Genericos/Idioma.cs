namespace BE
{
    public class Idioma
    {
        public int IdIdioma { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoISO { get; set; } = string.Empty; // Unique
    }
}
