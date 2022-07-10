using ProjetoAWS.Exception;

namespace ProjetoAWS.Lib.Models
{
    public class Usuario : ModelBase
    {
        public string Nome { get; private set; }
        public double Cpf { get; private set; }
        public string Email { get;private set; }
        public double Senha { get; private set; }
        public double DataNascimento { get; private set; }
        public string UrlImagemCadastro { get; private set; }
        public double DataCriacao { get; private set; }

        public Usuario (string nome, double cpf, string email, double senha, double dataNascimento, 
                        string urlImagemCadastro, double dataCriacao)
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
        public double GetCpf()
        {
            return Cpf;
        }
        public void SetCpf(double cpf)
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
        public double GetSenha()
        {
            return Senha;
        }
        public void SetSenha(double senha)
        {
            ValidarSenha(senha);
            Senha = senha;
        }
        public double GetDataNascimento()
        {
            return DataNascimento;
        }
        public void SetDataNascimento(double dataNascimento)
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
        public double GetDataCriacao()
        {
            return DataCriacao;
        }
        public void SetDataCriacao(double dataCriacao)
        {
            DataCriacao = dataCriacao;
        }
        public void ValidarDataNascimento(double dataNascimento)
        {
            if (DataNascimento < 2010)
            {
                throw ErroDeValidacaoException ("Ano de nascimento deve ser nemor que 2010");
            }
            DataNascimento = DataNascimento;
        }
        public void ValidarEmail(string email)
        {
            if (Email.Contains("@"))
            {
                throw ErroDeValidacaoException ("Email invalido falta @");
            }
            Email = email;
        }
        public void ValidarCpf(double cpf)
        {
            if (cpf == 11);
            {
                throw ErroDeValidacaoException ("Não inserir espaço e nem ponto em cpf");
            }
            Cpf = cpf;
        }
        public void ValidarSenha(double senha)
        {
            if (senha > 8)
            {
                throw ErroDeValidacaoException ("Senha deve ter ao menos 8 caracter");
            }
            Senha = senha;
        }
    }
}