using Microsoft.AspNetCore.Mvc;
using ProjetoAWS.Lib.Models;
using ProjetoAWS.Lib.Exceptions;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Application.Services;

namespace ProjetoAws.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase 
    {
        private readonly IUsuarioApplication _application;
        
        private static readonly List<string> _extensoesImagem = new List<string>(){"image/jpeg", "image/jpg", "image/png"};
        public static List<Usuario> ListaUsuarios { get; set; } = new List<Usuario>();
        public UsuarioController(IUsuarioApplication application )
        {
            _application = application;
        }

        [HttpGet("Todos")]
        public async Task<List<Usuario>> BuscarTodosAsync()
        {
            return await _application.BuscarTodosAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarUsuarioId(int id)
        {
            return Ok(await _application.BuscarPorIdAsync(id));
        }

        [HttpPost]
        public async Task<int> AdicionarUsuarioAsync(UsuarioDTO usuarioDTO)
        {
            try
            {
                await _application.AdicionarUsuarioAsync(usuarioDTO);
                throw new Exception ("Usuario Adicionado");
            }
            catch (ErroDeValidacaoException)
            {
                throw new Exception ("Usuario Não Adicionado");
            }
        }

        [HttpPost("Cadastro Imagem")]
        public async Task CadastroDeImagem(int id, IFormFile imagem)
        {
            try
            {
                //chamada do metodo na application
                await _application.CadastroDeImagem(id, imagem);
                throw new Exception ("Cadastro de imagem com sucesso");
            }
            catch (ErroDeValidacaoException)
            {
                throw new Exception ("Erro ao cadastra imagem tente novamente"); 
            }
        }    
        
        public async Task ValidarImagem(string nomeArquivoS3)
        {
            try
            {
                await _application.ValidarImagem(nomeArquivoS3);
                throw new Exception ("Imagem salva com sucesse");
            }
            catch(ErroDeValidacaoException)
            {
                throw new Exception ("Imagem não valida");
            }
        }

        public async Task SalvarNoS3(IFormFile imagem)
        {
            try
            {
                await _application.SalvarNoS3(imagem);
                throw new Exception ("Foto salva com sucesso");
            }
            catch(ErroDeValidacaoException)
            {
                throw new Exception ("Foto foi possível salva foto tente novamente");
            }
            
        }
     
        [HttpPut("Alterar")]
        public async Task<IActionResult> AlterarSenha(int id, string senha)
        {
            await _application.AlterarSenhaAsync(id, senha);
            return Ok("Senha alteradada!");
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletarPorId(int id)
        {
            await _application.DeletarPorIdAsync(id);
            return Ok("Usuario removido");
        }
        

        [HttpPost("Login email")]        

        public async Task<IActionResult> LoginPorEmail(string email, string senha)
        {
            try
            {
               return Ok (await _application.LoginPorEmail(email, senha)); 
               
            }
            catch(ErroDeValidacaoException ex)
            {
                return BadRequest (ex.Message);
            }
        }
     
        public async Task CoferirSenha(Usuario usuario, string senha)
        {
            try
            {
                await _application.ConferirSenha(usuario, senha);
            }
            catch(ErroDeValidacaoException)
            {
                throw new Exception ("Usuario não confere com a senha rever dados");
            }
        }



        [HttpPost("comparar rosto")]
        
        public async Task CompararRostoAsync(int id, IFormFile fotoLogin)
        {
            try
            {
                await _application.CompararRostoAsync(id, fotoLogin);
                
            }
            catch(ErroDeValidacaoException)
            {
                throw new Exception ("Foto não confere com banco de dados");
            }
        }

 
    }
}

