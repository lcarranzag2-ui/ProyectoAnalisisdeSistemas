namespace HiddenValley.Shared.DTOs;

    public record ClienteCreateDTO(int IdPersona);

    public record ClienteDetalleDTO(
        int IdCliente, 
        string NombreCompleto, 
        string DPI, 
        string Telefono, 
        string Gmail
    );

    public record HistorialReservaDTO(
        int Id, 
        DateTime FechaEntrada, 
        DateTime FechaSalida, 
        string Estado, 
        decimal Total
    );
