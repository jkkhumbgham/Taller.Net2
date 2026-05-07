using Microsoft.EntityFrameworkCore;
using ClasesMasTomadas.Datos.Modelos;

namespace ClasesMasTomadas.Datos.Contexto;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<Inscripcion> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Inscripcion>().ToTable("enrollments");
    }
}
