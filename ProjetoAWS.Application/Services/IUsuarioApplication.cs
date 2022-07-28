using Microsoft.AspNetCore.Http;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Lib.Models;

namespace ProjetoAWS.Application.Services
{
    public interface IUsuarioApplication
    {
        public Task<int> AdicionarUsuarioAsync(UsuarioDTO dto); 
        public Task<bool> DeletarPorIdAsync(int id);  
        public Task<bool> CadastroDeImagem(int id, IFormFile imagem); 

        public Task<Usuario> ValidarImagem(string nomeArquivoS3); 

        public Task<string> SalvarNoS3(IFormFile imagem);

        public Task<bool> LoginPorEmail(string email, string senha); 

        public Task<bool> ConferirSenha(Usuario usuario, string senha);

        public Task<bool> CompararRostoAsync(int id, IFormFile fotoLogin);

        public Task<bool> AlterarSenhaAsync(int id, string senha);

        public Task<List<Usuario>> BuscarTodosAsync(); 

        public Task<Usuario> BuscarPorIdAsync(int id);

        public Task<Usuario> BuscarUsuarioPorEmail (string email);

        public Task<bool> AtualizarImagemAsync(int id, string UrlImagemCadastro);
        

    }
}



        