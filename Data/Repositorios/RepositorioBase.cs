namespace Curso.ProjetoAWS.Data.Repositorios
{
    public class RepositorioBase<T> : IRepositorioBase<T> where T : ModelBase
    {
        private readonly ProjetoAWSContext _context;
        private readonly DbSet<T> _dbset;
        public RepositorioBase(ProjetoAWSContext dbContext, DbSet<T> dbset)
        {
            _context = dbContext;
            _dbset = dbset;
        }
        public async Task<List<T>> BuscarTodosAsync()
        {
            return await _dbset.AsNoTracking().ToListAsync();
        }
        public async Task<T> BuscarPorIdAsync(int id)
        {
            return await _dbset.AsNoTracking().FirstAsync(x => x.Id == id);
        }
        public async Task AdicionarAsync(T item)
        {
            await _dbset.AddAsync(item);
            await _context.SaveChangesAsync();
        }
       
         public async Task DeletarAsync(int id)
        {
            var item = await _dbset.AsNoTracking().FirstAsync(x => x.Id == id);
            _dbset.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}