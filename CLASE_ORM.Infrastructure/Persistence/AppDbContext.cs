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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Rol)
                .HasDefaultValue("Usuario");
        }

    }
}
