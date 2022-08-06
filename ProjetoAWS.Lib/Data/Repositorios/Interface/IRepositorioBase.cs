using ProjetoAWS.Lib.Models;

namespace Curso.ProjetoAWS.Lib.Data.Repositorios.Interface
{
    public interface IRepositorioBase<T> where T : ModelBase
    {
        Task Adicionar(T item);
        Task<T> BuscarPorId(Guid id);
        Task<List<T>> BuscarTodos();
        Task DeletarItemDesejado(Guid id);
    }
}

        




      