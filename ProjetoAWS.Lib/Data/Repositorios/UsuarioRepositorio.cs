using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using ProjetoAWS.Lib.Models;

using Microsoft.EntityFrameworkCore;
using ProjetoAWS.Lib.Data;

namespace Curso.ProjetoAWS.Data.Repositorios
{
    public class UsuarioRepositorio : RepositorioBase<Usuario>, IUsuarioRepositorio
    {
        private readonly ProjetoAWSContext _context;

        public UsuarioRepositorio(ProjetoAWSContext context) : base(context, context.Usuarios)
        {
            _context = context;
            
        }
        public async Task AlterarSenhaAsync(int id, string senha)
        {
            var item = await _context.Usuarios.FirstAsync(x => x.Id == id);
            item.SetSenha(senha);
            await _context.SaveChangesAsync();
        }
        public async Task<Usuario> BuscarUsuarioPorEmail(string email)
        {
            return await _context.Usuarios.AsNoTracking().FirstAsync(x => x.Email == email);
        }

        public async Task<Usuario> ConferirSenha(string usuario, string senha)
        {
            return await _context.Usuarios.AsNoTracking().FirstAsync(x => x.Senha == senha);
        }
        public async Task AtualizarImagemAsync(int id, string UrlImagemCadastro)
        {
            var usuario = await _context.Usuarios.FirstAsync(x=> x.Id == id);
            usuario.SetUrlImagemCadastro(UrlImagemCadastro);
            await _context.SaveChangesAsync();
        }

        public async Task ComparandoRostoAsync( int id, string fotoLogin)
        {
            var usuario = await _context.Usuarios.FirstAsync(x => x.Id == id);
            usuario.SetUrlImagemCadastro(fotoLogin);
            await _context.SaveChangesAsync();
        }
        

        
    }       
}