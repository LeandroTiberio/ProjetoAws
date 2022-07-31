using Microsoft.AspNetCore.Http;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Lib.Models;

namespace ProjetoAWS.Application.Services
{
    public interface IUsuarioApplication 
    {
        Task<int> AdicionarUsuario(UsuarioDTO usuarioDTO);  
        Task CadastrarImagem(int id, IFormFile imagem);
        Task<List<Usuario>> BuscarTodos();
        Task<Usuario> BuscarUsuarioPorID(int id);
        Task<int> LoginEmail(string email, string senha);
        Task LoginImagem(int id, IFormFile imagem);
        Task AtualizarEmailUsuarioPorId(int id, string email);
        Task DeletarPorId(int id);
    }
}




       
        

        

    



        
        