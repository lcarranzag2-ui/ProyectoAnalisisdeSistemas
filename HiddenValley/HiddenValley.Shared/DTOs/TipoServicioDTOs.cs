namespace HiddenValley.Shared.DTOs
{
    public class TipoServicioReadDto
    {
        public int IdTipoServicio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class TipoServicioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
