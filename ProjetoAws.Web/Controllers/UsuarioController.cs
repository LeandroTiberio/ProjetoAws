using Microsoft.AspNetCore.Mvc;
using ProjetoAws.Web.Properties.DTOs;
using ProjetoAWS.Lib.Models;




namespace ProjetoAws.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase 
    {
        private readonly IsuarioRepositorio _repositorio;
        public static List<Usuario> ListaUsuarios { get; set; } = new List<Usuario>();
        public UsuarioController(IUsuarioRepositorio _repositorio)
        {
            _repositorio = _repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodos()
        {
            return Ok(await _repositorio.BuscarTodos());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioId(int id)
        {
            return Ok(await _repositorio.BuscarPorIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar(UsuarioDTO usuarioDTO)
        {
            try
            {
                var usuario = new Usuario(usuarioDTO.Nome, usuarioDTO.Cpf, usuarioDTO.Email, usuarioDTO.Senha,
                                    usuarioDTO.DataNascimento, usuarioDTO.UrlImagemCadastro, usuarioDTO.DataCriacao);
                await _repositorio.AdicionarAsync(usuario);
                return Ok(usuario);
            }
            catch (ErroDeValidacaoException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletarPorId(int id)
        {
            _repositorio.DeletarAsync(id);
            return Ok("Usuario removido");
        }


    }


}

