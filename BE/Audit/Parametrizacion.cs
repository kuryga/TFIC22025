namespace BE.Params
{
    public class Parametrizacion
    {
        public string NombreEmpresa { get; set; } = string.Empty; // PK
        public string Cuit { get; set; } = string.Empty;
        public int IdIdioma { get; set; }
    }
}
