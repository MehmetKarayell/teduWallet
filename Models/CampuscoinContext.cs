using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace teduWallet.Models;

public partial class CampuscoinContext : DbContext
{
  public CampuscoinContext()
  {
  }

  public CampuscoinContext(DbContextOptions<CampuscoinContext> options)
      : base(options)
  {
  }

  public virtual DbSet<Activity> Activities { get; set; }

  public virtual DbSet<Admin> Admins { get; set; }

  public virtual DbSet<Apply> Applies { get; set; }

  public virtual DbSet<Complete> Completes { get; set; }

  public virtual DbSet<Log> Logs { get; set; }

  public virtual DbSet<Reward> Rewards { get; set; }

  public virtual DbSet<Student> Students { get; set; }

  public virtual DbSet<VwActivityDetail> VwActivityDetails { get; set; }

  public virtual DbSet<VwStudentCompletion> VwStudentCompletions { get; set; }

  public virtual DbSet<VwStudentWallet> VwStudentWallets { get; set; }

  public virtual DbSet<VwTop3StudentsThisWeek> VwTop3StudentsThisWeeks { get; set; }

  public virtual DbSet<Wallet> Wallets { get; set; }

  public virtual DbSet<WalletSpendsReward> WalletSpendsRewards { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Activity>(entity =>
    {
      entity.HasKey(e => e.ActivityId).HasName("PK__ACTIVITY__45F4A791C3E88714");

      entity.ToTable("ACTIVITY");

      entity.Property(e => e.ActivityId).ValueGeneratedNever();
      entity.Property(e => e.Description)
              .HasMaxLength(255)
              .IsUnicode(false);
      entity.Property(e => e.PriorityLevel).HasColumnType("decimal(18, 0)");
      entity.Property(e => e.RewardTokenAmount).HasColumnType("decimal(18, 0)");
      entity.Property(e => e.Status)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Title)
              .HasMaxLength(30)
              .IsUnicode(false);

      entity.HasOne(d => d.Admin).WithMany(p => p.Activities)
              .HasForeignKey(d => d.AdminId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__ACTIVITY__AdminI__4AB81AF0");
    });

    modelBuilder.Entity<Admin>(entity =>
    {
      entity.ToTable("ADMIN");

      entity.Property(e => e.Email)
              .HasMaxLength(50)
              .IsUnicode(false)
              .HasColumnName("email");
      entity.Property(e => e.Name)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Password)
              .HasMaxLength(255)
              .IsUnicode(false);
      entity.Property(e => e.Position)
              .HasMaxLength(20)
              .IsUnicode(false);
      entity.Property(e => e.Surname)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Username)
              .HasMaxLength(20)
              .IsUnicode(false);
    });

    modelBuilder.Entity<Apply>(entity =>
    {
      entity.HasKey(e => new { e.StudentId, e.ActivityId }).HasName("PK_Apply");

      entity.ToTable("APPLY", tb => tb.HasTrigger("logApplyActivity"));

      entity.Property(e => e.Status)
              .HasMaxLength(20)
              .IsUnicode(false);

      entity.HasOne(d => d.Activity).WithMany(p => p.Applies)
              .HasForeignKey(d => d.ActivityId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__APPLY__ActivityI__5535A963");

      entity.HasOne(d => d.Student).WithMany(p => p.Applies)
              .HasForeignKey(d => d.StudentId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__APPLY__StudentId__5441852A");
    });

    modelBuilder.Entity<Complete>(entity =>
    {
      entity.HasKey(e => new { e.StudentId, e.ActivityId }).HasName("PK_Completes");

      entity.ToTable("COMPLETES", tb =>
              {
                tb.HasTrigger("logCompletesActivity");
                tb.HasTrigger("trg_UpdateWalletOnCompletion");
              });

      entity.Property(e => e.AwardedAmount).HasColumnType("decimal(18, 0)");

      entity.HasOne(d => d.Activity).WithMany(p => p.Completes)
              .HasForeignKey(d => d.ActivityId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__COMPLETES__Activ__52593CB8");

      entity.HasOne(d => d.Student).WithMany(p => p.Completes)
              .HasForeignKey(d => d.StudentId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__COMPLETES__Stude__5165187F");
    });

    modelBuilder.Entity<Log>(entity =>
    {
      entity.HasKey(e => e.LogId).HasName("PK__LOG__5E54864883D30812");

      entity.ToTable("LOG");

      entity.Property(e => e.RewardId).IsRequired(false);

      entity.HasIndex(e => e.Timestamp, "idx_Log_Timestamp").IsDescending();

      entity.Property(e => e.ActionType)
              .HasMaxLength(50)
              .IsUnicode(false);

      entity.HasOne(d => d.Activity).WithMany(p => p.Logs)
              .HasForeignKey(d => d.ActivityId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__LOGGED__Activity__4E88ABD4");

      entity.HasOne(d => d.Reward).WithMany(p => p.Logs)
              .HasForeignKey(d => d.RewardId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__LOGGED__RewardId__4F7CD00D");

      entity.HasOne(d => d.Student).WithMany(p => p.Logs)
              .HasForeignKey(d => d.StudentId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__LOGGED__StudentI__4D94879B");
    });

    modelBuilder.Entity<Reward>(entity =>
    {
      entity.HasKey(e => e.RewardId).HasName("PK__REWARD__825015B9721A8556");

      entity.ToTable("REWARD");

      entity.HasIndex(e => e.UniqueCode, "UQ__REWARD__BB96DE6F7359824A").IsUnique();

      entity.Property(e => e.RewardId).ValueGeneratedNever();
      entity.Property(e => e.Cost).HasColumnType("decimal(18, 0)");
      entity.Property(e => e.RewardName)
              .HasMaxLength(100)
              .IsUnicode(false);
      entity.Property(e => e.RewardType)
              .HasMaxLength(50)
              .IsUnicode(false);
      entity.Property(e => e.Status)
              .HasMaxLength(20)
              .IsUnicode(false);
      entity.Property(e => e.UniqueCode)
              .HasMaxLength(50)
              .IsUnicode(false);
      entity.Property(e => e.Vendor)
              .HasMaxLength(100)
              .IsUnicode(false);
    });

    modelBuilder.Entity<Student>(entity =>
    {
      entity.ToTable("STUDENT");

      entity.HasIndex(e => e.Email, "idx_student_email");

      entity.HasIndex(e => e.Username, "idx_student_username");

      entity.Property(e => e.Coins).HasColumnType("decimal(18, 0)");
      entity.Property(e => e.Departmant)
              .HasMaxLength(20)
              .IsUnicode(false);
      entity.Property(e => e.Email)
              .HasMaxLength(50)
              .IsUnicode(false)
              .HasColumnName("email");
      entity.Property(e => e.Name)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Password)
              .HasMaxLength(255)
              .IsUnicode(false);
      entity.Property(e => e.Surname)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Username)
              .HasMaxLength(20)
              .IsUnicode(false);
    });

    modelBuilder.Entity<VwActivityDetail>(entity =>
    {
      entity
              .HasNoKey()
              .ToView("vw_activity_details");

      entity.Property(e => e.AdminName)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.AdminSurname)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Description)
              .HasMaxLength(255)
              .IsUnicode(false);
      entity.Property(e => e.RewardTokenAmount).HasColumnType("decimal(18, 0)");
      entity.Property(e => e.Status)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Title)
              .HasMaxLength(30)
              .IsUnicode(false);
    });

    modelBuilder.Entity<VwStudentCompletion>(entity =>
    {
      entity
              .HasNoKey()
              .ToView("vw_student_completions");

      entity.Property(e => e.AwardedAmount).HasColumnType("decimal(18, 0)");
    });

    modelBuilder.Entity<VwStudentWallet>(entity =>
    {
      entity
              .HasNoKey()
              .ToView("vw_student_wallet");

      entity.Property(e => e.Balance).HasColumnType("decimal(18, 0)");
      entity.Property(e => e.Name)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Surname)
              .HasMaxLength(30)
              .IsUnicode(false);
      entity.Property(e => e.Username)
              .HasMaxLength(20)
              .IsUnicode(false);
    });

    modelBuilder.Entity<VwTop3StudentsThisWeek>(entity =>
    {
      entity
              .HasNoKey()
              .ToView("vw_Top3StudentsThisWeek");

      entity.Property(e => e.FullName)
              .HasMaxLength(61)
              .IsUnicode(false);
      entity.Property(e => e.WeeklyCoinsEarned).HasColumnType("decimal(38, 0)");
    });

    modelBuilder.Entity<Wallet>(entity =>
    {
      entity.HasKey(e => e.WalletId).HasName("PK__WALLET__84D4F90E7B359D95");

      entity.ToTable("WALLET");

      entity.HasIndex(e => e.StudentId, "UQ__WALLET__32C52B987D515317").IsUnique();

      entity.Property(e => e.WalletId).ValueGeneratedNever();
      entity.Property(e => e.Balance).HasColumnType("decimal(18, 0)");

      entity.HasOne(d => d.Student).WithOne(p => p.Wallet)
              .HasForeignKey<Wallet>(d => d.StudentId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__WALLET__StudentI__403A8C7D");
    });

    modelBuilder.Entity<WalletSpendsReward>(entity =>
    {
      entity.HasKey(e => e.TransactionId).HasName("PK__WALLET_S__55433A6BC74D7CB8");

      entity.ToTable("WALLET_SPENDS_REWARD", tb => tb.HasTrigger("logSpends"));

      entity.Property(e => e.TransactionId).ValueGeneratedNever();

      entity.HasOne(d => d.Reward).WithMany(p => p.WalletSpendsRewards)
              .HasForeignKey(d => d.RewardId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__WALLET_SP__Rewar__46E78A0C");

      entity.HasOne(d => d.Student).WithMany(p => p.WalletSpendsRewards)
              .HasForeignKey(d => d.StudentId)
              .HasConstraintName("FK__WALLET_SP__Stude__17036CC0");

      entity.HasOne(d => d.Wallet).WithMany(p => p.WalletSpendsRewards)
              .HasForeignKey(d => d.WalletId)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("FK__WALLET_SP__Walle__45F365D3");
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
