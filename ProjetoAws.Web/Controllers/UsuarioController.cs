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
        private static readonly List<string> _extensoesImagem = new List<string>(){"image/jpeg", "image/jpg", "image/png"};
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
                //atualizar foto cadastro no repositorio usuario
                await _repositorio.AtualizarImagemAsync(id, nomeArquivo);
              
                return Ok("imagem confirmada para cadastro");
            }
            else
            {
                await _amazonS3.DeleteObjectAsync("imagem-aulas", nomeArquivo);
                return BadRequest("Imagem Invalido para cadastro");
            }
        }

       private async Task<bool> ValidarImagem(string nomeArquivoS3)
        {
            using (var memoryStream = new MemoryStream())
            {
                var entrada = new DetectFacesRequest();
                var imagem = new Image();

                var s3Object = new Amazon.Rekognition.Model.S3Object()
                {
                    Bucket = "imagem-aulas",
                    Name = nomeArquivoS3
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
                    return false;
                }

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
                request.BucketName = "imagem-aulas";
                request.InputStream = streamDaImagem;
                
                await _amazonS3.PutObjectAsync(request);
                return request.Key;
            }
        }

       

        [HttpPut("Alterar")]
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

        public async Task<IActionResult> LoginPorEmail(string email, string senha)
        {
            var usuario = await _repositorio.BuscarUsuarioPorEmail(email);
            var confirmacao = await ConferirSenha(usuario , senha);

            if(confirmacao)
            {
                return Ok(usuario.Id);
            }
            else
            {
                return BadRequest("Senha não cadastro ou invalida");
            }
        } 
    
           
        private async Task<bool> ConferirSenha(Usuario usuario, string senha)
        {
            if (usuario.Senha == senha)
            {
                return true;
            }
            return false;
        }



        [HttpPost("comparar rosto")]
        
        public async Task<IActionResult> CompararRostoAsync(int id, IFormFile fotoLogin)
        { 
            using (var memoryStream = new MemoryStream())
            {

                var request = new CompareFacesRequest();
                var usuario = await _repositorio.BuscarPorIdAsync(id);
                var requestsourceImagem = new Image()
                
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Bucket = "imagem-aulas",
                        Name = usuario.GetUrlImagemCadastro()
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
                return Ok(resposta);
            }
        }
    }
}

