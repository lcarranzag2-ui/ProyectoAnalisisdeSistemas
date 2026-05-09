namespace HiddenValley.Shared.DTOs
{
public class TipoCabanaDTO
{
    public int IdTipoCabana { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}

public class TipoCabanaCreateDTO
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}
}