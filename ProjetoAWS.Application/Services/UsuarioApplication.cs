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
        private readonly IServicesDaAws _servicesDaAws;
        
        public UsuarioApplication(IUsuarioRepositorio repositorio, ServicesDaAws servicesDaAws )
        {
            _repositorio = repositorio;
            _servicesDaAws servicesDaAws;
            
        }

        public async Task<int> AdicionarUsuario(UsuarioDTO usuarioDTO)
        {
            var usuario = new Usuario(usuarioDTO.Id, usuarioDTO.Nome, usuarioDTO.Cpf, usuarioDTO.Email, usuarioDTO.Senha,
                                    usuarioDTO.DataNascimento, usuarioDTO.UrlImagemCadastro, usuarioDTO.DataCriacao);
            await _repositorio.Adicionar(usuario);
            return usuario.Id;
        }
       
        public async Task<List<Usuario>> BuscarTodos()
        {
            return await _repositorio.BuscarTodos();
        }
        public async Task<Usuario> BuscarUsuarioPorID(int id)
        {
            return await _repositorio.BuscarPorId(id);
        }
        public async Task CadastrarImagem(int id, IFormFile imagem)
        {
            var nomeArquivo = await _servicesDaAws.SalvarNoS3(imagem);
            var imagemValida = await _servicesDaAws.ValidarImagem(nomeArquivo);
            if (imagemValida)
            {
                await _repositorio.AtualizarUrlImagemCadastro(id, nomeArquivo);
            }
            else
            {
                var response = await _servicesDaAws.DeleteObjectAsync("imagens-aulas", nomeArquivo);
                throw new ErroDeValidacaoException("Imagem inválida!");
            }
        }
        public async Task<int> LoginEmail(string email, string senha)
        {
            var usuario = await _servicesDaAws.BuscarPorEmail(email);
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
         public async Task<bool> LoginImagem(int id, IFormFile image)
        {
            var buscarUsuarioId = await _repositorio.BuscarPorId(id);//Buscar usuário no bando por Id.
            var buscarUsuarioImagem = await _servicesDaAws.BuscarUsuarioPorImagem(buscarUsuarioId.UrlImagemCadastro, image);
            if(buscarUsuarioImagem)
            {
                return true;
            }
            throw new Exception ("A imagem do usuário não corresponde com o cadastro.");
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
