using System;
using ProjetoAWS.Lib.Exceptions;

namespace ProjetoAWS.Lib.Models
{
    public class Usuario : ModelBase
    {
        public string Nome { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get;private set; }
        public string Senha { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public string? UrlImagemCadastro { get; private set; }
        public DateTime DataCriacao { get; private set; }
        private  Usuario ( )
        {

        }

        public Usuario (int id, string nome, string cpf, string email, string senha, DateTime dataNascimento, 
                        string urlImagemCadastro, DateTime dataCriacao) : base (id)
        {
            SetNome(nome);
            SetCpf(cpf);
            SetEmail(email);
            SetSenha(senha);
            SetDataNascimento(dataNascimento);
            SetUrlImagemCadastro(urlImagemCadastro);
            SetDataCriacao(dataCriacao);
        }
        public string GetNome()
        {
            return Nome;
        }
        public void SetNome(string nome)
        {
            Nome = nome;
        }
        public string GetCpf()
        {
            return Cpf;
        }
        public void SetCpf(string cpf)
        {
            ValidarCpf(cpf);
            Cpf = Cpf;
        }
        public string GetEmail()
        {
            return Email;
        }
        public void SetEmail(string email)
        {
            ValidarEmail(email);
            Email = email;
        }
        public string GetSenha()
        {
            return Senha;
        }
        public void SetSenha(string senha)
        {
            ValidarSenha(senha);
            Senha = senha;
        }
        public DateTime GetDataNascimento()
        {
            return DataNascimento;
        }
        public void SetDataNascimento(DateTime dataNascimento)
        {
            ValidarDataNascimento(dataNascimento);
            DataNascimento = dataNascimento;
        }
        public string GetUrlImagemCadastro()
        {
            return UrlImagemCadastro;
        }
        public void SetUrlImagemCadastro(string urlImagemCadastro)
        {
            UrlImagemCadastro = urlImagemCadastro;
        }
        public DateTime GetDataCriacao()
        {
            return DataCriacao;
        }
        public void SetDataCriacao(DateTime dataCriacao)
        {
            DataCriacao = dataCriacao;
        }
        public void ValidarDataNascimento(DateTime dataNascimento)
        {
            if (dataNascimento < DateTime.Parse("01/01/2010"))
            {
                throw new ErroDeValidacaoException ("Ano de nascimento deve ser nemor que 2010");
            }
            DataNascimento = DataNascimento;
        }
        public void ValidarEmail(string email)
        {
            if (! email.Contains("@"))
            {
                throw new ErroDeValidacaoException ("Email invalido falta @");
            }
            Email = email;
        }
        public void ValidarCpf(string cpf)
        {
            if (cpf.Count() != 11)
            {
                throw new ErroDeValidacaoException ("Não inserir espaço e nem ponto em cpf");
            }
            Cpf = cpf;
        }
        public void ValidarSenha(string senha)
        {
            if (senha.Count() < 8)
            {
                throw new ErroDeValidacaoException ("Senha deve ter ao menos 8 caracter");
            }
            Senha = senha;
        }
    }
}