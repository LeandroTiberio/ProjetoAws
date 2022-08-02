using Microsoft.AspNetCore.Http;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Lib.Models;

namespace ProjetoAWS.Application.Services
{
    public interface IUsuarioApplication 
    {
        Task<int> AdicionarUsuario(UsuarioDTO usuarioDTO);  
        
        Task<List<Usuario>> BuscarTodos();
        Task<Usuario> BuscarUsuarioPorID(int id);
        Task AtualizarEmailUsuarioPorId(int id, string email);
        Task DeletarPorId(int id);
    }
}




       
        

        

    



        
        