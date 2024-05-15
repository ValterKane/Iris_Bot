using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Iris_Bot.Models;

public partial class IrisDbContext : DbContext
{
    public IrisDbContext()
    {
    }

    public IrisDbContext(DbContextOptions<IrisDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<ActionPlan> ActionPlans { get; set; }

    public virtual DbSet<ActionType> ActionTypes { get; set; }

    public virtual DbSet<ChekIn> ChekIns { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Dayofweak> Dayofweaks { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=localhost;Port=3306;User=root;Password=12761276Kain!;Database=iris_db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.IdAction).HasName("PRIMARY");

            entity.ToTable("actions");

            entity.HasIndex(e => e.Type, "type_fk_idx");

            entity.Property(e => e.IdAction).HasColumnName("idAction");
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.AgeAllow)
                .HasMaxLength(15)
                .HasColumnName("Age_Allow");
            entity.Property(e => e.AvarageRating)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Avarage_rating");
            entity.Property(e => e.Chair).HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Actions)
                .HasForeignKey(d => d.Type)
                .HasConstraintName("type_fk");
        });

        modelBuilder.Entity<ActionPlan>(entity =>
        {
            entity.HasKey(e => new { e.IdAction, e.IdPlan }).HasName("PRIMARY");

            entity.ToTable("action_plan");

            entity.HasIndex(e => e.IdPlan, "plan_fk_idx");

            entity.Property(e => e.IdAction).HasColumnName("idAction");
            entity.Property(e => e.IdPlan).HasColumnName("idPlan");

            entity.HasOne(d => d.IdActionNavigation).WithMany(p => p.ActionPlans)
                .HasForeignKey(d => d.IdAction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("action_fk");

            entity.HasOne(d => d.IdPlanNavigation).WithMany(p => p.ActionPlans)
                .HasForeignKey(d => d.IdPlan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plan_fk");
        });

        modelBuilder.Entity<ActionType>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("PRIMARY");

            entity.ToTable("action_type");

            entity.Property(e => e.IdType).HasColumnName("idType");
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<ChekIn>(entity =>
        {
            entity.HasKey(e => new { e.ChekInId, e.IdAction, e.PhoneNumber }).HasName("PRIMARY");

            entity.ToTable("chek_in");

            entity.HasIndex(e => e.IdAction, "act_fk_idx");

            entity.HasIndex(e => e.PhoneNumber, "cl_fk_idx");

            entity.Property(e => e.ChekInId).HasColumnName("chek_in_id");
            entity.Property(e => e.IdAction).HasColumnName("idAction");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Date).HasColumnType("date");

            entity.HasOne(d => d.IdActionNavigation).WithMany(p => p.ChekIns)
                .HasForeignKey(d => d.IdAction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("act_fk");

            entity.HasOne(d => d.PhoneNumberNavigation).WithMany(p => p.ChekIns)
                .HasForeignKey(d => d.PhoneNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cl_fk");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.PhoneNumber).HasName("PRIMARY");

            entity.ToTable("clients");

            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Comment).HasMaxLength(255);
            entity.Property(e => e.DoB).HasColumnType("date");
            entity.Property(e => e.FirstName).HasMaxLength(45);
            entity.Property(e => e.LastName).HasMaxLength(45);
            entity.Property(e => e.Password).HasMaxLength(300);
            entity.Property(e => e.SecondName).HasMaxLength(45);
            entity.Property(e => e.Status).HasMaxLength(45);
            entity.Property(e => e.TelegramId)
                .HasMaxLength(45)
                .HasColumnName("TelegramID");
        });

        modelBuilder.Entity<Dayofweak>(entity =>
        {
            entity.HasKey(e => e.IdDay).HasName("PRIMARY");

            entity.ToTable("dayofweak");

            entity.Property(e => e.IdDay).HasColumnName("idDay");
            entity.Property(e => e.LongDay).HasMaxLength(45);
            entity.Property(e => e.ShortDay).HasMaxLength(45);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => new { e.PhoneNumber, e.ActionId }).HasName("PRIMARY");

            entity.ToTable("feedback");

            entity.HasIndex(e => e.ActionId, "action_fk_1_idx");

            entity.HasIndex(e => e.PhoneNumber, "client_fk_idx");

            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.ActionId).HasColumnName("Action_id");
            entity.Property(e => e.ActionRating)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Action_Rating");
            entity.Property(e => e.DateOfRquest).HasColumnType("datetime");
            entity.Property(e => e.TextRequest).HasColumnType("text");

            entity.HasOne(d => d.Action).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("action_fk_1");

            entity.HasOne(d => d.PhoneNumberNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.PhoneNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("client_fk_1");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.IdPlan).HasName("PRIMARY");

            entity.ToTable("plans");

            entity.Property(e => e.IdPlan).HasColumnName("idPlan");
            entity.Property(e => e.PlanName)
                .HasMaxLength(45)
                .HasColumnName("Plan_Name");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => new { e.IdAction, e.IdDay }).HasName("PRIMARY");

            entity.ToTable("schedule");

            entity.HasIndex(e => e.IdDay, "sch_day_fk_idx");

            entity.Property(e => e.IdAction).HasColumnName("idAction");
            entity.Property(e => e.IdDay).HasColumnName("idDay");
            entity.Property(e => e.Time).HasMaxLength(10);

            entity.HasOne(d => d.IdActionNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.IdAction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sch_act_fk");

            entity.HasOne(d => d.IdDayNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.IdDay)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sch_day_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
