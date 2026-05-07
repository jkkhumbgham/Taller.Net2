using Microsoft.EntityFrameworkCore;
using CursosAcabados.Datos.Modelos;

namespace CursosAcabados.Datos.Contexto;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<Usuario> Users { get; set; }
    public DbSet<Inscripcion> Enrollments { get; set; }
    public DbSet<ProgresoLeccion> LessonProgress { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().ToTable("users");
        modelBuilder.Entity<Inscripcion>().ToTable("enrollments");
        modelBuilder.Entity<ProgresoLeccion>().ToTable("lesson_progress");

        modelBuilder.Entity<Inscripcion>()
            .HasOne(i => i.Usuario)
            .WithMany(u => u.Inscripciones)
            .HasForeignKey(i => i.UserId);
    }
}
