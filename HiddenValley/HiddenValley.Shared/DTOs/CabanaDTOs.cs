using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiddenValley.Shared.DTOs;

public class CambiarEstadoRequest
{
    public int IdCabana { get; set; }
    public string NuevoEstado { get; set; } = string.Empty;
    public int IdEmpleado { get; set; }
}

public class RegistrarCabanaRequest
{
    public int IdTipoCabana { get; set; }
    public int IdEstadoCabana { get; set; }
}

public class PagedResponse<T>
{
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; } = [];
}