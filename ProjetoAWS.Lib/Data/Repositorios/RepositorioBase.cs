using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using ProjetoAWS.Lib.Models;
using Microsoft.EntityFrameworkCore;
using ProjetoAWS.Lib.Data;

namespace Curso.ProjetoAWS.Data.Repositorios
{
    public class RepositorioBase<T> : IRepositorioBase<T> where T : ModelBase
    {
        
        protected readonly ProjetoAWSContext _context;
        protected readonly DbSet<T> _dbSet;
        public RepositorioBase(ProjetoAWSContext context, DbSet<T> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        public async Task Adicionar(T item)
        {
            await _dbSet.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task<T> BuscarPorId(int id)
        {
            //First = primeiro que tenha mesmo id
            return await _dbSet.AsNoTracking().FirstAsync(x => x.Id == id);
        }

        public async Task<List<T>> BuscarTodos()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task DeletarItemDesejado(int id)
        {
            //Find = achar pelo id
            var itemARemover = await _dbSet.FindAsync(id);
            _dbSet.Remove(itemARemover);
            await _context.SaveChangesAsync();
        }
        

    }
}



       
       