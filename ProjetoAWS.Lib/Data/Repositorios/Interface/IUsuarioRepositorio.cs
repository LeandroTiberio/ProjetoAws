using ProjetoAWS.Lib.Models;

namespace Curso.ProjetoAWS.Lib.Data.Repositorios.Interface
{
    public interface IUsuarioRepositorio : IRepositorioBase<Usuario>
    {
        public Task AlterarSenhaAsync(int id, string senha);

        public Task<Usuario> BuscarUsuarioPorEmail (string email);

        public Task AtualizarImagemAsync(int id, string UrlImagemCadastro);
        public Task<Usuario> ConferirSenha(string usuario, string senha);

        public Task ComparandoRostoAsync( int id, string fotoLogin);
    }
}