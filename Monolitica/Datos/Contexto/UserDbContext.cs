using Microsoft.EntityFrameworkCore;
using Monolitica.Datos.Modelos;

namespace Monolitica.Datos.Contexto;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<Usuario> Users { get; set; }
    public DbSet<Inscripcion> Enrollments { get; set; }
    public DbSet<ProgresoLeccion> LessonProgress { get; set; }
    public DbSet<IntentoCuestionario> QuizAttempts { get; set; }
    public DbSet<IntentoRespuesta> QuestionAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().ToTable("users");
        modelBuilder.Entity<Inscripcion>().ToTable("enrollments");
        modelBuilder.Entity<ProgresoLeccion>().ToTable("lesson_progress");
        modelBuilder.Entity<IntentoCuestionario>().ToTable("quiz_attempts");
        modelBuilder.Entity<IntentoRespuesta>().ToTable("question_attempts");

        modelBuilder.Entity<IntentoRespuesta>()
            .HasOne(ir => ir.IntentoCuestionario)
            .WithMany(ic => ic.IntentosRespuestas)
            .HasForeignKey(ir => ir.QuizAttemptId);
    }
}
