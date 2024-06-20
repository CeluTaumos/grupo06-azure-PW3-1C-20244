using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Azurecito.Data.Entidades;

public partial class AzurecitoFotosContext : DbContext
{
    public AzurecitoFotosContext()
    {
    }

    public AzurecitoFotosContext(DbContextOptions<AzurecitoFotosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Foto> Fotos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-QUOBQAV;Database=AzurecitoFotos;Trusted_Connection=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Foto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fotos__3214EC0729BC537C");

            entity.Property(e => e.FotoUrl).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.Fotos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Fotos__UserId__398D8EEE");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC076CFC6FC0");

            entity.Property(e => e.NombreUsuario).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
