using Microsoft.EntityFrameworkCore;
using Notas.Datos.Modelos;

namespace Notas.Datos.Contexto;

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
