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
    public class UsuarioApplication : IUsuarioApplication
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IAmazonS3 _amazonS3;
        private static readonly List<string> _extensoesImagem =
        new List<string>() { "image/jpeg", "image/png", "image/jpg"};
        private readonly AmazonRekognitionClient _rekognitionClient;
        public UsuarioApplication(IUsuarioRepositorio repositorio, IAmazonS3 amazonS3, AmazonRekognitionClient rekognitionClient)
        {
            _repositorio = repositorio;
            _amazonS3 = amazonS3;
            _rekognitionClient = rekognitionClient;
        }

        public async Task<int> AdicionarUsuario(UsuarioDTO usuarioDTO)
        {
            var usuario = new Usuario(usuarioDTO.Id, usuarioDTO.Nome, usuarioDTO.Cpf, usuarioDTO.Email, usuarioDTO.Senha,
                                    usuarioDTO.DataNascimento, usuarioDTO.UrlImagemCadastro, usuarioDTO.DataCriacao);
            await _repositorio.Adicionar(usuario);
            return usuario.Id;
        }
        public async Task CadastrarImagem(int id, IFormFile imagem)
        {
            var nomeArquivo = await SalvarNoS3(imagem);
            var imagemValida = await ValidarImagem(nomeArquivo);
            if (imagemValida)
            {
                await _repositorio.AtualizarUrlImagemCadastro(id, nomeArquivo);
            }
            else
            {
                var response = await _amazonS3.DeleteObjectAsync("imagens-aulas", nomeArquivo);
                throw new ErroDeValidacaoException("Imagem inválida!");
            }
        }
        private async Task<string> SalvarNoS3(IFormFile imagem)
        {
            if (!_extensoesImagem.Contains(imagem.ContentType))
            {
                throw new ErroDeValidacaoException("Tipo de foto inválido!");
            }
            using (var streamDaImagem = new MemoryStream())
            {
                await imagem.CopyToAsync(streamDaImagem);
                var request = new PutObjectRequest();
                request.Key = "reconhecimento" + imagem.FileName;
                request.BucketName = "imagem-aulas";
                request.InputStream = streamDaImagem;

                var resposta = await _amazonS3.PutObjectAsync(request);
                return request.Key;
            }
        }
        private async Task<bool> ValidarImagem(string nomeArquivo)
        {
            var request = new DetectFacesRequest();
            var imagem = new Image();

            var s3Object = new Amazon.Rekognition.Model.S3Object()
            {
                Bucket = "imagem-aulas",
                Name = nomeArquivo

            };

            imagem.S3Object = s3Object;
            request.Image = imagem;
            request.Attributes = new List<string>() { "ALL" };

            var response = await _rekognitionClient.DetectFacesAsync(request);

            if (response.FaceDetails.Count == 1 && response.FaceDetails.First().Eyeglasses.Value == false)
            {
                return true;
            }
            return false;
        }
        public async Task<List<Usuario>> BuscarTodos()
        {
            return await _repositorio.BuscarTodos();
        }
        public async Task<Usuario> BuscarUsuarioPorID(int id)
        {
            return await _repositorio.BuscarPorId(id);
        }
        public async Task<int> LoginEmail(string email, string senha)
        {
            var usuario = await _repositorio.BuscarPorEmail(email);
            var validacao = await VerificarSenha(usuario, senha);
            if (validacao)
            {
                return usuario.Id;
            }
            throw new ErroDeValidacaoException("Senha invalida!");
        }
        private async Task<bool> VerificarSenha(Usuario usuario, string senha)
        {
            return usuario.Senha == senha;
        }
        public async Task LoginImagem(int id, IFormFile imagem)
        {
            var usuario = await _repositorio.BuscarPorId(id);
            var verificacao = await VerificarImagem(usuario.UrlImagemCadastro, imagem);
            if (verificacao == false)
            {
                throw new ErroDeValidacaoException("Face não compativel com cadastro!");
            }
        }
        private async Task<bool> VerificarImagem(string nomeArquivoS3, IFormFile fotoLogin)
        {
            using (var memStream = new MemoryStream())
            {
                var request = new CompareFacesRequest();

                var requestSource = new Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Bucket = "imagem-aulas",
                        Name = nomeArquivoS3
                    }
                };

                await fotoLogin.CopyToAsync(memStream);
                var requestTarget = new Image()
                {
                    Bytes = memStream
                };

                request.SourceImage = requestSource;
                request.TargetImage = requestTarget;

                var response = await _rekognitionClient.CompareFacesAsync(request);
                if (response.FaceMatches.Count == 1 && response.FaceMatches.First().Similarity >= 90)
                {
                    return true;
                }
                return false;
            }
        }
        public async Task AtualizarEmailUsuarioPorId(int id, string email)
        {
            await _repositorio.AtualizarEmail(id, email);
        }
        public async Task DeletarPorId(int id)
        {
            await _repositorio.DeletarItemDesejado(id);
        }
    }
    
}
