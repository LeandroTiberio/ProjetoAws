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
        private static readonly List<string> _extensoesImagem = new List<string>(){"image/jpeg", "image/jpg", "image/jpg"};
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
            var usuario = await _repositorio.BuscarPorIdAsync(id);
            var imagemValida = await ValidarImagem(usuario.UrlImagemCadastro, imagem);
            
            if(imagemValida)
            {   
                return Ok("Id imagem confirmada");
            }
            else
            {
                return BadRequest("Imagem Invalido com cadastro");
            }
        }

       private async Task<bool> ValidarImagem(string nomeArquivoS3, IFormFile fotoLogin)
        {
            using (var memoryStream = new MemoryStream())
            {
                var request = new CompareFacesRequest();
                var requestSource = new Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Bucket = "registro",
                        Name = nomeArquivoS3
                    }
                };


                await fotoLogin.CopyToAsync(memoryStream);
                var requestTarget = new Image()
                {
                    Bytes = memoryStream
                };

                request.SourceImage = requestSource;
                request.TargetImage = requestTarget;

                var response = await _rekognitionClient.CompareFacesAsync(request);
                if (response.FaceMatches.Count == 1 && response.FaceMatches.First().Similarity >= 80)
                {
                    return true;
                }
                return false;
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
                request.BucketName = "Registro";
                request.InputStream = streamDaImagem;
                
                var resposta = await _amazonS3.PutObjectAsync(request);
                return request.Key;
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
        

        [HttpPost("Login email")]

        private async Task<IActionResult> LoginPorEmail(string email, string senha)
        {
            var usuario = await _repositorio.BuscarUsuarioPorEmail(email);
            var confirmacao = await ConferenciaSenha(usuario , senha);

            if(confirmacao)
            {
                return Ok(usuario.Id);
            }
            else
            {
                return BadRequest("Login e senha não são de cadastro");
            }
        } 
    
           
        private async Task<bool> ConferenciaSenha(Usuario usuario, string senha)
        {
            
            if (usuario.Senha == senha)
            {
                return true;
            }
            return false;
        }



        [HttpPost("comparar rosto")]
        
        public async Task<bool> CompararRostoAsync(string nomeArquivoS3, IFormFile fotoLogin)
        { 
            using (var memoryStream = new MemoryStream())
            {
                var request = new CompareFacesRequest();
                var requestsourceImagem = new Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Bucket = "imagem-Aulas",
                        Name = nomeArquivoS3
                    }
                };
                    
                await fotoLogin.CopyToAsync(memoryStream);

                var requesttargetImagem = new Image()
                {
                    Bytes = memoryStream
                };
                request.SourceImage = requestsourceImagem;  
                request.TargetImage = requesttargetImagem;

                var resposta = await _rekognitionClient.CompareFacesAsync(request);
                return true;
            }
        }
    }
}

