using HiddenValley.Domain.Entities;

namespace HiddenValley.Data.Repositories;

public interface ICabanaRepositorio
{
    int Agregar(Cabana cabana);
    IEnumerable<Cabana> ObtenerTodas();
    int ContarRegistros();
}
