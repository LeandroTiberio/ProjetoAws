using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Lib.Exceptions;
using ProjetoAWS.Lib.Models;

namespace ProjetoAWS.Application.Services
{
    
    public class UsuarioApplication
    {
        
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IAmazonS3 _amazonS3;
        private readonly AmazonRekognitionClient _rekognitionClient;
        private static readonly List<string> _extensoesImagem = new List<string>(){"image/jpeg", "image/jpg", "image/png"};
        public static List<Usuario> ListaUsuarios { get; set; } = new List<Usuario>();
        public UsuarioApplication(IUsuarioRepositorio repositorio, IAmazonS3 amazonS3, AmazonRekognitionClient rekognitionClient)
        {
            _repositorio = repositorio;
            _amazonS3 = amazonS3;
            _rekognitionClient = rekognitionClient;
        }

        public async Task<List<Usuario>> BuscarTodosAsync()
        {   
            return await _repositorio.BuscarTodosAsync();
        }

        public async Task<Usuario> BuscarUsuarioId(int id)
        {
            return (await _repositorio.BuscarPorIdAsync(id));
        }

        public async Task<Usuario> AdicionarUsuario(UsuarioDTO usuarioDTO)
        {
            try
            {
                var usuario = new Usuario(usuarioDTO.Id, usuarioDTO.Nome, usuarioDTO.Cpf, usuarioDTO.Email, usuarioDTO.Senha,
                                    usuarioDTO.DataNascimento, usuarioDTO.UrlImagemCadastro, usuarioDTO.DataCriacao);
                await _repositorio.AdicionarUsuarioAsync(usuario);
                return (usuario);
            }
            catch (ErroDeValidacaoException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task CadastroDeImagem(int id, IFormFile imagem)
        {
            try
            {
                //chamada do metodo na application
                var imagemCadastro = await _repositorio.CadastroDeImagem(int id, IFormFile imagem);
                //return ok com o resultado
                return Ok("Cadastro de imagem com sucesso");
            }
            catch (ErroDeValidacaoException ex)
            {
                return BadRequest(ex.Message);
            }
        }   
        
        private async Task<Usuario> ValidarImagem(string nomeArquivoS3)
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

       
        public async Task<Usuario> AlterarSenha(int id, string senha)
        {
            await _repositorio.AlterarSenhaAsync(id, senha);
            return Ok("Senha alteradada!");
        }


        public async Task<Usuario> DeletarPorId(int id)
        {
            await _repositorio.DeletarAsync(id);
            return Ok("Usuario removido");
        }
            

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