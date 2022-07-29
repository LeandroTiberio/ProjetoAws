using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using Microsoft.AspNetCore.Http;
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

        public async Task<Usuario> AdicionarUsuarioAsync(UsuarioDTO usuarioDTO)
        {
            try
            {
                var usuario = new Usuario(usuarioDTO.Id, usuarioDTO.Nome, usuarioDTO.Cpf, usuarioDTO.Email, usuarioDTO.Senha,
                                    usuarioDTO.DataNascimento, usuarioDTO.UrlImagemCadastro, usuarioDTO.DataCriacao);
                await _repositorio.AdicionarAsync (usuario);
                return (usuario);
                throw new Exception("Usuario adicionado");
            }
            catch (ErroDeValidacaoException)
            {
                throw new Exception ("Usuario não Adicionado");
            }
        }

        public async Task<bool> CadastroDeImagem(int id, IFormFile imagem)
        {
            var nomeArquivo = await SalvarNoS3(imagem);
            var imagemValida = await ValidarImagem(nomeArquivo);
            if (imagemValida)
            {
                await _repositorio.CadastroDeImagem(id, imagem);
                return true;
            }
            else
            {
                await _amazonS3.DeleteObjectAsync("imagem-aulas", nomeArquivo);
                return false;
            }
        }   
        
        public async Task<bool> ValidarImagem(string nomeArquivoS3)
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


        public async Task<string> SalvarNoS3(IFormFile imagem)
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
            throw new Exception ("Senha alteradada!");
        }


        public async Task<Usuario> DeletarPorId(int id)
        {
            await _repositorio.DeletarAsync(id);
            throw new Exception ("Usuario removido");
        }
            

        public async Task<int> LoginPorEmail(string email, string senha)
        {
            var usuario = await _repositorio.BuscarUsuarioPorEmail(email);
            var confirmacao = await ConferirSenha(usuario , senha);

            if(confirmacao)
            {
                return usuario.Id;
            }
            else
            {
                throw new ErroDeValidacaoException ("Senha ou email invalida");
            }
        } 
    
           
        private async Task<bool> ConferirSenha(Usuario usuario, string senha)
        {
            if (usuario.Senha == senha)
            {
                return true;
            }
            return false;
            throw new ErroDeValidacaoException ("Usuario não confere com a senha rever dados");
        }

        
        public async Task<bool> CompararRostoAsync(int id, IFormFile fotoLogin)
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

                if(resposta.FaceMatches.Count == 1 && resposta.FaceMatches.First().Similarity == 90)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
        }

        

    }
}