using System.Text;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProjetoAWS.Application.DTOs;
using ProjetoAWS.Lib.Exceptions;
using ProjetoAWS.Lib.Models;
using ProjetoAWS.ServicesAWS;
using ServicesAWS;

namespace ProjetoAWS.Application.Services
{
    public class UsuarioApplication : IUsuarioApplication
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IServicesDaAws _servicesDaAws;
        public static List<Usuario> ListaUsuarios { get; set; } = new List<Usuario>();
        public readonly List<string> _imageFormats = new List<string>() { "image/jpeg", "image/png", "image/jpn"};
        
        public UsuarioApplication(IUsuarioRepositorio repositorio, IServicesDaAws servicesDaAws )
        {
            _repositorio = repositorio;
            _servicesDaAws = servicesDaAws;
            
        }

        public async Task<JsonIdHash> AdicionarUsuario(UsuarioDTO usuarioDTO)
        {
            var senhaHash = await MudarSenhaEmHash(usuarioDTO.Senha);
            var usuario = new Usuario(usuarioDTO.Nome, usuarioDTO.Cpf, usuarioDTO.Email, usuarioDTO.Senha,
                                    usuarioDTO.DataNascimento, usuarioDTO.UrlImagemCadastro, usuarioDTO.DataCriacao);
            await _repositorio.Adicionar(usuario);
            //return usuario.Id;
            var retorna= new JsonIdHash()
            {
                Id = usuario.Id.ToString()
            };
            return (retorna);
        }
       
        public async Task<List<Usuario>> BuscarTodos()
        {
            return await _repositorio.BuscarTodos();
        }
        public async Task<Usuario> BuscarUsuarioPorID(Guid id)
        {
            return await _repositorio.BuscarPorId(id);
        }
        public async Task CadastrarImagem(Guid id, IFormFile imagem)
        {
            var nomeArquivo = await _servicesDaAws.SalvarNoS3(imagem);
            var imagemValida = await _servicesDaAws.ValidarImagem(nomeArquivo);
            if (imagemValida)
            {
                await _repositorio.AtualizarUrlImagemCadastro(id, nomeArquivo);
            }
            else
            {
                await _servicesDaAws.DeletarImagemNoS3 ("imagens-aulas", nomeArquivo);
                throw new Exception("Imagem inválida!");
            }
        }
        public async Task<JsonIdHash> LoginEmail(string email, string senha)
        {
            var usuario = await _repositorio.BuscarPorEmail(email);
            var validacao = await VerificarSenha(usuario, senha);
            if (validacao)
            {
                throw new Exception("Senha invalida!");
            }
            
            var retorno = new JsonIdHash()
            {
                Id = usuario.Id.ToString()
            };
            return (retorno);
            
         
        }
        private async Task<bool> VerificarSenha(Usuario usuario, string senha)
        {
            var senhaHash = await MudarSenhaEmHash(senha);
            return senha == senhaHash;
        }
        public async Task<string> MudarSenhaEmHash(string senha)
        {
            byte[] password = Encoding.UTF8.GetBytes(senha);
            byte[] salt = Encoding.UTF8.GetBytes("UOrd7FcW33T5gy");
            var argon2 = new Argon2d(password);
            argon2.DegreeOfParallelism = 10;
            argon2.MemorySize = 8192;
            argon2.Iterations = 20;
            argon2.Salt = salt;
            var hash = await argon2.GetBytesAsync(64);
            return Convert.ToBase64String(hash);
        }
        public async Task<bool> LoginImagem(Guid id, IFormFile image)
        {
            var buscarUsuarioId = await _repositorio.BuscarPorId(id);
            var buscarUsuarioImagem = await _servicesDaAws.VerificarImagem(buscarUsuarioId.UrlImagemCadastro , image);
            if(buscarUsuarioImagem)
            {
                return true;
            }
            throw new Exception ("A imagem do usuário não corresponde com o cadastro.");
        }
        
        
        public async Task AtualizarEmailUsuarioPorId(Guid id, string email)
        {
            await _repositorio.AtualizarEmail(id, email);
        }
        public async Task DeletarPorId(Guid id)
        {
            await _repositorio.DeletarItemDesejado(id);
        }
    }
    
}
