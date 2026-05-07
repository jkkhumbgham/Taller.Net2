using Microsoft.EntityFrameworkCore;
using ClasesMasTomadas.Datos.Modelos;

namespace ClasesMasTomadas.Datos.Contexto;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options) { }

    public DbSet<Curso> Courses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Curso>().ToTable("courses");
    }
}
