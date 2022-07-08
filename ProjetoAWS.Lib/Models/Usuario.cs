namespace ProjetoAWS.Lib.Models
{
    public class Usuario : ModelBase
    {
        public string Nome { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get;private set; }
        public string Senha { get; private set; }
        public double DataNascimento { get; private set; }
        public string UrlImagemCadastro { get; private set; }
        public string DataCriacao { get; private set; }

        public Usuario (string nome, string cpj, string email, string senha, double dataNascimento, 
                        string urlImagemCadastro, string dataCriacao)
        {
            SetNome(nome);
            SetCpj(cpj);
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
        public string GetCpj()
        {
            return Cpj;
        }
        public void SetCpj(string cpj)
        {
            ValidarCpf(cpf);
            Cpj = Cpf;
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
        public string GetDataNascimento()
        {
            return DataNascimento;
        }
        public void SetDataNascimento(string dataNascimento)
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
        public string GetDataCriacao()
        {
            return DataCriacao;
        }
        public void SetDataCriacao(string dataCriacao)
        {
            DataCriacao = dataCriacao;
        }
        public void ValidarDataNascimento(string dataNascimento)
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
        public void ValidarSenha(string senha)
        {
            if (senha > 8)
            {
                throw ErroDeValidacaoException ("Senha deve ter ao menos 8 caracter");
            }
            Senha = senha;
        }
        



        



    }
}