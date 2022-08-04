using Microsoft.AspNetCore.Mvc;
using ProjetoAWS.Lib.Models;
using ProjetoAWS.Lib.Exceptions;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Application.Services;
using Amazon.S3;
using Amazon.Rekognition;
using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;

namespace ProjetoAws.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase 
    {
        private readonly IUsuarioApplication _application;
        private readonly IAmazonS3 _amazonS3;
        private static readonly List<string> _extensoesImagem =
        new List<string>() { "image/jpeg", "image/png", "image/jpg"};
        private readonly AmazonRekognitionClient _rekognitionClient;
        public UsuarioController(IUsuarioApplication application, IAmazonS3 amazonS3, AmazonRekognitionClient rekognitionClient )
        {
            _application = application;
            _amazonS3 = amazonS3;
            _rekognitionClient = rekognitionClient;
        }
        //UUDI - SQL / GUID - VISUAL STUDIO
        [HttpPost()]
        public async Task<IActionResult> AdicionarUsuario(UsuarioDTO usuarioDTO)
        {
            var resposta = await _application.AdicionarUsuario(usuarioDTO);
            return Ok(resposta);
        }
        [HttpPost("imagem")]
        public async Task<IActionResult> CadastrarImagem(int id, IFormFile imagem)
        {
            await _application.CadastrarImagem(id, imagem);
            return Ok();
        }
        [HttpGet("todos")]
        public async Task<IActionResult> BuscarTodos()
        {
            var resposta = await _application.BuscarTodos();
            return Ok(resposta);
        }
        [HttpGet("Id")]
        public async Task<IActionResult> BuscarUsuarioPorID(int id)
        {
            var resposta = await _application.BuscarUsuarioPorID(id);
            return Ok(resposta);
        }
        [HttpGet("LoginEmail")]
        public async Task<IActionResult> LoginEmail(string email, string senha)
        {
            var resposta = await _application.LoginEmail(email, senha);
            return Ok(resposta);
        }
        [HttpPost("LoginImagem")]
        public async Task<IActionResult> LoginImagem(int id, IFormFile imagem)
        {
            await _application.LoginImagem(id, imagem);
            return Ok();
        }
        [HttpPut("Email")]
        public async Task<IActionResult> AtualizarEmailUsuarioPorId(int id, string email)
        {
            await _application.AtualizarEmailUsuarioPorId(id, email);
            return Ok();
        }
        [HttpDelete()]
        public async Task<IActionResult> DeletarUsuarioPorId(int id)
        {
            await _application.DeletarPorId(id);
            return Ok();
        }
    }
}




        

