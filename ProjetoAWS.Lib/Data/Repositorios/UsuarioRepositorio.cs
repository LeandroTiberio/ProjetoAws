using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using ProjetoAWS.Lib.Models;

using Microsoft.EntityFrameworkCore;
using ProjetoAWS.Lib.Data;
using Microsoft.AspNetCore.Http;

namespace Curso.ProjetoAWS.Data.Repositorios
{
    public class UsuarioRepositorio : RepositorioBase<Usuario>, IUsuarioRepositorio
    {
        public UsuarioRepositorio(ProjetoAWSContext context) : base(context, context.Usuarios)
        {

        }
        public async Task AtualizarEmail(Guid id, string email)
        {
            await _context.Usuarios.FirstAsync(x => x.Email == email);
            await _context.SaveChangesAsync();
            
        }
        public async Task AtualizarUrlImagemCadastro(Guid id, string urlAtualizada)
        {
            await _context.Usuarios.FirstAsync(x => x.Id == id);
            await _context.SaveChangesAsync();
        }
        public async Task<Usuario> BuscarPorEmail(string email)
        {
            return await _dbSet.AsNoTracking().FirstAsync(x => x.Email == email);
        }
    }
} 
        
        

       
        
        
        
        
       

        
    