using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTI_AssetManager_Project.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ACTI_AssetManager_Project.Application.Data
{
    public class AM_DBContext : DbContext
    {

        public AM_DBContext(DbContextOptions<AM_DBContext> options)
            : base(options) { }

        // Representa la tabla SYSUsuarios
        public DbSet<Usuario> Usuarios { get; set; }
        // Representa la tabla AMRecurso
        public DbSet<Recurso> Recursos { get; set; }
        // Representa la tabla AMSYSTipoRecurso
        public DbSet<TipoRecurso> TiposRecurso { get; set; }
        // Representa la tabla AMSYSEstadoRecurso
        public DbSet<EstadoRecurso> EstadosRecurso { get; set; }
        // Representa la tabla AMLicencia
        public DbSet<Licencia> Licencias { get; set; }

        public DbSet<Asignacion> Asignacion { get; set; }

        public DbSet<Proyecto> Proyectos { get; set; }

        public DbSet<CategoriaRecurso> CategoriaRecursos { get; set; }

        public DbSet<AsignacionDetalle> AsignacionDetalles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {

                entity.ToTable("SYSUsuarios", "ACTIVERSE.SYSTEM");


                entity.HasKey(e => e.IdUsuario);


                entity.Property(e => e.IdUsuario)
                      .HasColumnName("idUsuario");

                entity.Property(e => e.NombreCompleto)
                      .HasColumnName("NombreCompleto")
                      .HasMaxLength(200);

                entity.Property(e => e.PasswordHash)
                      .HasColumnName("PasswordHash");

                entity.Property(e => e.LimitePassword)
                      .HasColumnName("LimitePassword");

                entity.Property(e => e.SuperUsuario)
                      .HasColumnName("SuperUsuario");

                entity.Property(e => e.IdTipoUsuario)
                      .HasColumnName("idTipoUsuario");

                entity.Property(e => e.Eliminado)
                      .HasColumnName("ELIMINADO");

                entity.Property(e => e.FechaHoraCambio)
                      .HasColumnName("FECHAHORACAMBIO");

                entity.Property(e => e.IdUsuarioCambio)
                      .HasColumnName("IdUSUARIOCAMBIO");
            });

            modelBuilder.Entity<TipoRecurso>(entity =>
            {
                entity.ToTable("AMTipoRecurso", "ACTIVERSE.SYSTEM");
                entity.HasKey(e => e.IdTipoRecurso);

                entity.Property(e => e.IdTipoRecurso).HasColumnName("idTipoRecurso");
                entity.Property(e => e.NombreTipoRecurso).HasColumnName("NombreTipoRecurso").HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasColumnName("Descripcion").HasMaxLength(250);
                entity.Property(e => e.Eliminado).HasColumnName("ELIMINADO");
                entity.Property(e => e.FechaHoraCambio).HasColumnName("FECHAHORACAMBIO");
                entity.Property(e => e.IdCategoria).HasColumnName("IdCategoria");

                entity.HasOne(d => d.Categoria)
               .WithMany(p => p.TiposRecursos)
               .HasForeignKey(d => d.IdCategoria)
               .OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<EstadoRecurso>(entity =>
            {
                entity.ToTable("AMEstadoRecurso", "ACTIVERSE.SYSTEM");
                entity.HasKey(e => e.IdEstado);

                entity.Property(e => e.IdEstado).HasColumnName("idEstado");
                entity.Property(e => e.NombreEstado).HasColumnName("NombreEstado").HasMaxLength(50);
            });

            modelBuilder.Entity<Recurso>(entity =>
            {
                entity.ToTable("AMRecurso", "ACTIVERSE.SYSTEM");
                entity.HasKey(e => e.IdRecurso);

                entity.Property(e => e.IdRecurso).HasColumnName("IdRecurso");
                entity.Property(e => e.CodigoInterno).HasColumnName("CodigoInterno").HasMaxLength(50);
                entity.Property(e => e.IdTipoRecurso).HasColumnName("IdTipoRecurso");
                entity.Property(e => e.IdEstado).HasColumnName("IdEstado");
                entity.Property(e => e.FechaAdquisicion).HasColumnName("FechaAdquisicion");
                entity.Property(e => e.Vigencia).HasColumnName("Vigencia");
                entity.Property(e => e.Eliminado).HasColumnName("ELIMINADO");
                entity.Property(e => e.FechaHoraCambio_Creacion_Recurso).HasColumnName("FECHAHORACAMBIO_CREACION_RECURSO");
                entity.Property(e => e.IdUsuarioCambio).HasColumnName("IdUsuarioCambio").HasMaxLength(15);
                entity.Property(e => e.IdUsuarioResponsable).HasColumnName("IdUsuarioResponsable").HasMaxLength(15);
                entity.Property(e => e.FechaHoraCambio_Asignacion_Recurso).HasColumnName("FECHAHORACAMBIO_ASIGNACION_RECURSO");
                entity.Property(e => e.CapacidadUso).HasColumnName("CapacidadUso");


                // Relaciones
                entity.HasOne(r => r.TipoRecurso)
                      .WithMany(t => t.Recursos)
                      .HasForeignKey(r => r.IdTipoRecurso)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.EstadoRecurso)
                      .WithMany(e => e.Recursos)
                      .HasForeignKey(r => r.IdEstado)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.UsuarioResponsable)
                      .WithMany()
                      .HasForeignKey(r => r.IdUsuarioResponsable)
                      .HasPrincipalKey(u => u.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Licencia>(entity =>
            {
                entity.ToTable("AMLicencia", "ACTIVERSE.SYSTEM");
                entity.HasKey(e => e.IdLicencia);
                entity.Property(e => e.IdLicencia).HasColumnName("idLicencia");
                entity.Property(e => e.IdRecurso).HasColumnName("idRecurso");
                entity.Property(e => e.CantidadTotal).HasColumnName("CantidadTotal");
                entity.Property(e => e.CantidadEnUso).HasColumnName("CantidadEnUso");
                entity.Property(e => e.FechaVencimiento).HasColumnName("FechaVencimiento");

                entity.HasOne(l => l.Recurso)
                      .WithOne()
                      .HasForeignKey<Licencia>(l => l.IdRecurso)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Asignacion>(entity =>
            {
                // Especificamos que idAsignacion es la llave primaria e identidad
                entity.ToTable("AMAsignacion", "ACTIVERSE.SYSTEM");
                entity.HasKey(e => e.IdAsignacion);
                entity.Property(e => e.IdProyecto).HasColumnName("IdProyecto");
                entity.Property(e => e.FechaInicio).HasColumnName("FechaInicio");
                entity.Property(e => e.FechaFin).HasColumnName("FechaFin");
                entity.Property(e => e.IdUsuarioRegistroAsignacion).HasColumnName("IdUsuarioRegistroAsignacion");
                entity.Property(e => e.FechaRegistroAsignacion).HasColumnName("FechaRegistroAsignacion");
                entity.Property(e => e.Eliminado).HasColumnName("ELIMINADO");


                entity.Ignore("Recurso");

                entity.HasOne(d => d.Proyecto)
                .WithMany() // O .WithMany(p => p.Asignaciones) si Proyecto tiene la lista
                .HasForeignKey(d => d.IdProyecto);

            });

            //Crear Asignacion Detalle

            modelBuilder.Entity<AsignacionDetalle>(entity =>
            {
                // Especificamos que idAsignacion es la llave primaria e identidad
                entity.ToTable("AMAsignacionDetalle", "ACTIVERSE.SYSTEM");
                entity.HasKey(e => e.IdAsignacionDetalle);
                entity.Property(e => e.IdAsignacion).HasColumnName("IdAsignacion");
                entity.Property(e => e.IdRecurso).HasColumnName("IdRecurso");
                entity.Property(e => e.IdUsuarioResponsable).HasColumnName("IdUsuarioResponsable");
                entity.Property(e => e.Activo).HasColumnName("Activo");


                entity.HasOne(d => d.Recurso) // <--- Ahora sí usamos la propiedad de la imagen
               .WithMany(r => r.AsignacionDetalles)
               .HasForeignKey(d => d.IdRecurso);


                entity.HasOne(d => d.Asignacion)      // El detalle tiene UNA Asignacion
               .WithMany(p => p.Detalles)      // El padre tiene MUCHOS Detalles
               .HasForeignKey(d => d.IdAsignacion);
               
               

            });



            modelBuilder.Entity<Proyecto>(entity =>
            {
                // Especificamos que idAsignacion es la llave primaria e identidad
                entity.ToTable("AMProyecto", "ACTIVERSE.SYSTEM");
                entity.HasKey(e =>   e.IdProyecto);
                entity.Property(e => e.NombreProyecto).HasColumnName("NombreProyecto");
                entity.Property(e => e.Descripcion).HasColumnName("Descripcion");
                entity.Property(e => e.ELIMINADO).HasColumnName("ELIMINADO");
                entity.Property(e => e.FECHAHORACAMBIO).HasColumnName("FECHAHORACAMBIO");



            });

            modelBuilder.Entity<CategoriaRecurso>(entity =>
            {
                // Especificamos que idAsignacion es la llave primaria e identidad
                entity.ToTable("AMCategoriaRecurso", "ACTIVERSE.SYSTEM");
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.Nombre).HasColumnName("Nombre");

            });



        }
    }
}
