
using Microsoft.EntityFrameworkCore;
using proyectofinal_septimo.Models;

using System.Collections.Generic;



namespace proyectofinal_septimo.Data 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

   

        //public DbSet<DiarioEntradas> DiarioEntradas { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        //public DbSet<CategoriasTareas> CategoriasTareas { get; set; }
        public DbSet<EstatusTareas> EstatusTareas { get; set; }
        public DbSet<FrecuenciaHabitos> FrecuenciaHabitos { get; set; }
        public DbSet<PrioridadTareas> PrioridadTareas { get; set; }
        public DbSet<Habitos> Habitos { get; set; }
        public DbSet<Tareas> Tareas { get; set; }
        public DbSet<BitacoraHabitos> BitacoraHabitos { get; set; }
        public DbSet<Notas> Notas { get; set; }

        public DbSet<PomodoroSesion> PomodoroSesiones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Esto le enseña a C# a convertir automáticamente:
            // true <--> 1
            // false <--> 0

            modelBuilder.Entity<Notas>()
                .Property(n => n.Estatus)
                .HasConversion<int>(); // Convierte bool a int

            modelBuilder.Entity<Notas>()
                .Property(n => n.EsArchivada)
                .HasConversion<int>(); // Convierte bool a int
        }

        //public DbSet<RecordatoriosTareas> RecordatoriosTareas { get; set; }
        //public DbSet<Subtareas> Subtareas { get; set; }
    }
}