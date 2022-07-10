using ProjetoAWS.lib.Models;
using ProjetoAWS.lib.Data.Repositorios.Interface;
using Microsoft.EntityFrameworkCore;

namespace Curso.ProjetoAWS.Data.Repositorios
{
    public class UsuarioRepositorio : RepositorioBase<Usuario>, IUsuarioRepositorio
    {
        private readonly RedeHoteisContext _context;

        public HospedeRepositorio(ProjetoAWSContext context) : base(context, context.Usuarios)
        {
            _context = context;
        }
        public async Task AtualizarAsync(int IdUsuario)
        {
            var item = await _context.Usuarios.AsNoTracking().FirstAsync(x => x.Id == IdUsuario);
            await _context.SaveChangesAsync();
            
        }
    }
}