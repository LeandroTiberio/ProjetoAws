using Microsoft.AspNetCore.Http;
using ProjetoAWS.Lib.Models;

namespace Curso.ProjetoAWS.Lib.Data.Repositorios.Interface
{
    public interface IUsuarioRepositorio : IRepositorioBase<Usuario>
    {
        Task AtualizarEmail(int id, string emailAtualizado);
        Task AtualizarUrlImagemCadastro(int id, string urlAtualizada);
        Task<Usuario> BuscarPorEmail(string email);
    }
}
       
       
       
      
       
       
       
       
         
    
