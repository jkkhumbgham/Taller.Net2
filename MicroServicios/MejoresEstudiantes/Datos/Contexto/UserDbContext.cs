using Microsoft.EntityFrameworkCore;
using MejoresEstudiantes.Datos.Modelos;

namespace MejoresEstudiantes.Datos.Contexto;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<Usuario> Users { get; set; }
    public DbSet<IntentoCuestionario> QuizAttempts { get; set; }
    public DbSet<Inscripcion> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().ToTable("users");
        modelBuilder.Entity<IntentoCuestionario>().ToTable("quiz_attempts");
        modelBuilder.Entity<Inscripcion>().ToTable("enrollments");

        modelBuilder.Entity<IntentoCuestionario>()
            .HasOne(i => i.Usuario)
            .WithMany(u => u.IntentosCuestionarios)
            .HasForeignKey(i => i.UserId);
    }
}
