using Microsoft.EntityFrameworkCore;
using CursosAcabados.Datos.Modelos;

namespace CursosAcabados.Datos.Contexto;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options) { }

    public DbSet<Curso> Courses { get; set; }
    public DbSet<Modulo> Modules { get; set; }
    public DbSet<Leccion> Lessons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Curso>().ToTable("courses");
        modelBuilder.Entity<Modulo>().ToTable("modules");
        modelBuilder.Entity<Leccion>().ToTable("lessons");

        modelBuilder.Entity<Modulo>()
            .HasOne(m => m.Curso)
            .WithMany(c => c.Modulos)
            .HasForeignKey(m => m.CourseId);

        modelBuilder.Entity<Leccion>()
            .HasOne(l => l.Modulo)
            .WithMany(m => m.Lecciones)
            .HasForeignKey(l => l.ModuleId);
    }
}
