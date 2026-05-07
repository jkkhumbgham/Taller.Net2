using Microsoft.EntityFrameworkCore;
using Cuestionarios.Datos.Modelos;

namespace Cuestionarios.Datos.Contexto;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options) { }

    public DbSet<Cuestionario> Quizzes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cuestionario>().ToTable("quizzes");
    }
}
