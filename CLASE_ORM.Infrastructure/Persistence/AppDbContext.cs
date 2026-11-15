using CLASE_ORM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CLASE_ORM.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {

        // AppDbContext - AplicationDbContext

        public AppDbContext(DbContextOptions<AppDbContext> options): base (options)
        {

        }

        // add-migration, update-database, remove-migration, drop-database.

        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Rol)
                .HasDefaultValue("Usuario");

            // SQL Server no permite dos cascadas de borrado hacia la misma tabla:
            // si se borra un Alumno se borran sus Inscripciones, pero si se borra
            // una Materia con inscripciones, la operacion se rechaza (Restrict).
            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Materia)
                .WithMany(m => m.Inscripciones)
                .HasForeignKey(i => i.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
