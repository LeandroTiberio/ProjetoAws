using Microsoft.AspNetCore.Http;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Lib.Models;

namespace ProjetoAWS.Application.Services
{
    public interface IUsuarioApplication 
    {
        public Task<int> AdicionarUsuarioAsync(UsuarioDTO dto); 
        public Task<bool> AlterarSenhaAsync(int id, string senha);
        public Task<Usuario> ConferirSenha(Usuario usuario, string senha);
        public Task<bool> DeletarPorIdAsync(int id);  
        public Task<bool> CadastroDeImagem(int id, IFormFile imagem); 

        public Task<bool> ValidarImagem(string nomeArquivoS3); 

        public Task<string> SalvarNoS3(IFormFile imagem);

        public Task<bool> LoginPorEmail(string email, string senha); 

        public Task<bool> CompararRostoAsync(int id, IFormFile fotoLogin);

        public Task<List<Usuario>> BuscarTodosAsync(); 

        public Task<Usuario> BuscarPorIdAsync(int id);

        

    }
}



        