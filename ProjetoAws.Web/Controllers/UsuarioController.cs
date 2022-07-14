using Microsoft.AspNetCore.Mvc;
using ProjetoAws.Web.Controllers.DTOs;
using ProjetoAWS.Lib.Models;

using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using ProjetoAWS.Lib.Exceptions;

namespace ProjetoAws.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase 
    {
        private readonly IUsuarioRepositorio _repositorio;
        public static List<Usuario> ListaUsuarios { get; set; } = new List<Usuario>();
        public UsuarioController(IUsuarioRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet("Todos")]
        public async Task<IActionResult> BuscarTodosAsync()
        {
            return Ok(await _repositorio.BuscarTodosAsync());
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
                var usuario = new Usuario(usuarioDTO.Id, usuarioDTO.Nome, usuarioDTO.Cpf, usuarioDTO.Email, usuarioDTO.Senha,
                                    usuarioDTO.DataNascimento, usuarioDTO.UrlImagemCadastro, usuarioDTO.DataCriacao);
                await _repositorio.AdicionarAsync(usuario);
                return Ok(usuario);
            }
            catch (ErroDeValidacaoException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Alterar(int id, string senha)
        {
            await _repositorio.AlterarSenhaAsync(id, senha);
            return Ok("Senha alteradada!");
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletarPorId(int id)
        {
            await _repositorio.DeletarAsync(id);
            return Ok("Usuario removido");
        }


    }


}

