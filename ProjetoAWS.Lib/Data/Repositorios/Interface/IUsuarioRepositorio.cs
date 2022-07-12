using ProjetoAWS.Lib.Models;

namespace Curso.ProjetoAWS.Lib.Data.Repositorios.Interface
{
    public interface IUsuarioRepositorio : IRepositorioBase<Usuario>
    {
        public Task AlterarSenhaAsync(int id, string senha);
    }
}