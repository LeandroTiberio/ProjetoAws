using ProjetoAWS.Lib.Models;

namespace Curso.ProjetoAWS.Lib.Data.Repositorios.Interface
{
    public interface IRepositorioBase<T> where T : ModelBase
    {
        Task<List<T>> BuscarTodosAsync();
        Task<T> BuscarPorIdAsync(int id);
        Task AdicionarAsync(T item);
        Task DeletarAsync(int id);
         
    }
}