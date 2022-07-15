using Microsoft.AspNetCore.Mvc;
using ProjetoAws.Web.Controllers.DTOs;
using ProjetoAWS.Lib.Models;

using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using ProjetoAWS.Lib.Exceptions;
using Amazon.S3;
using Amazon.Rekognition;
using Amazon.S3.Model;
using Amazon.Rekognition.Model;

namespace ProjetoAws.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase 
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IAmazonS3 _amazonS3;
        private readonly AmazonRekognitionClient _rekognitionClient;
        private static readonly List<string> _extensoesImagem = new List<string>(){"imagejpeg", "imagepng"};
        public static List<Usuario> ListaUsuarios { get; set; } = new List<Usuario>();
        public UsuarioController(IUsuarioRepositorio repositorio, IAmazonS3 amazonS3, AmazonRekognitionClient rekognitionClient  )
        {
            _repositorio = repositorio;
            _amazonS3 = amazonS3;
            _rekognitionClient = rekognitionClient;
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
        [HttpPost("Cadastro Imagem")]
        public async Task<IActionResult> CadastroDeImagem(int id, IFormFile imagem)
        {
            var nomeArquivo = await SalvarNoS3(imagem);
            var imagemValida = await ValidarImagem(nomeArquivo);
            if(imagemValida)
            {   
                await _amazonS3.PutBucketAsync(nomeArquivo);
                return BadRequest();
            }
            else
            {
                await _amazonS3.DeleteObjectAsync("imagem-aulas", nomeArquivo);
                return BadRequest();
            }
        }

        private async Task<string> SalvarNoS3(IFormFile imagem)
        {
            if(!_extensoesImagem.Contains(imagem.ContentType))
            {
                throw  new Exception("Tipo Invalido de imagem");
            }
            using (var streamDaImagem = new MemoryStream())
            {  
                await imagem.CopyToAsync(streamDaImagem);  

                var request = new PutObjectRequest();
                request.Key = "reconhecimento" + imagem.FileName;
                request.BucketName = "imagem-Aulas";
                request.InputStream = streamDaImagem;
                
                var resposta = await _amazonS3.PutObjectAsync(request);
                return request.Key;
            }
        }

        private async Task<bool> ValidarImagem(string nomeArquivo)
        {
             var entrada = new DetectFacesRequest();
            var imagem = new Image();

            var s3Object = new Amazon.Rekognition.Model.S3Object()
            {
                Bucket = "imagem-Aulas",
                Name = nomeArquivo
            };

            imagem.S3Object = s3Object;
            entrada.Image = imagem;
            entrada.Attributes = new List<string>(){"ALL"};
            
            var resposta = await _rekognitionClient.DetectFacesAsync(entrada);
            
            if(resposta.FaceDetails.Count == 1 && resposta.FaceDetails.First().Eyeglasses.Value == false)
            {
                return true;
            }
            else
            {
                return true;
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

