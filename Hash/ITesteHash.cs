namespace Curso.ProjetoAws.Hash
{
    public interface ITesteHash
    {
        public Task<string> CriptografarSenha(string senha);
        public Task<bool> VerificarSenha(string senhaDigitada, string senhaCadastrada);
    }
}