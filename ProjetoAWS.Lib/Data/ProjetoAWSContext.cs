using Microsoft.EntityFrameworkCore;
using ProjetoAWS.Lib.Models;

namespace ProjetoAWS.Lib.Data
{
    public class ProjetoAWSContext : DbContext
    {
        public ProjetoAWSContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().ToTable("awsusuarios");
            modelBuilder.Entity<Usuario>().HasKey(key => key.Id); //Indica a propriedade da chave primaria, no caso Id
        } 
        public DbSet<Usuario> Usuarios { get; set; }

    }
}