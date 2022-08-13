using Microsoft.AspNetCore.Http;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Lib.Models;

namespace ProjetoAWS.Application.Services
{
    public interface IUsuarioApplication 
    {
        Task<JsonIdHash> AdicionarUsuario(UsuarioDTO usuarioDTO);  
        
        Task<List<Usuario>> BuscarTodos();
        Task<Usuario> BuscarUsuarioPorID(Guid id);
        Task CadastrarImagem(Guid id, IFormFile image);
        Task<JsonIdHash> LoginEmail(string email, string senha);
        Task<bool> LoginImagem(Guid id, IFormFile image);
        Task <string> MudarSenhaEmHash(string senha);
        Task AtualizarEmailUsuarioPorId(Guid id, string email);
        Task DeletarPorId(Guid id);
        
    }
}




       
        

        

    



        
        