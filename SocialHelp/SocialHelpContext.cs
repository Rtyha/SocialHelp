using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Collections.Generic;

namespace SocialHelp.Models
{
    // Модель для таблицы Employees
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; }

        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Login { get; set; }

        [Required]
        [StringLength(256)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Employee";
    }

    // Модель для таблицы Children
    public class Child
    {
        [Key]
        public int ChildId { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }

        [Required]
        public bool HasDisability { get; set; } = false;

        [StringLength(100)]
        public string School { get; set; }

        [StringLength(150)]
        public string Notes { get; set; }
    }

    // Модель для таблицы Mothers
    public class Mother
    {
        [Key]
        public int MotherId { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(100)]
        public string WorkPlace { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(10)]
        [Index(IsUnique = true)]
        public string Passport { get; set; }

        [StringLength(16)]
        public string MedicalInsurance { get; set; }

        [StringLength(11)]
        [Index(IsUnique = true)]
        public string SNILS { get; set; }

        [StringLength(12)]
        [Index(IsUnique = true)]
        public string INN { get; set; }

        [StringLength(20)]
        public string EducationLevel { get; set; }

        [StringLength(100)]
        public string AttitudeToChildren { get; set; }
    }

    // Модель для таблицы Fathers
    public class Father
    {
        [Key]
        public int FatherId { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(100)]
        public string WorkPlace { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(10)]
        [Index(IsUnique = true)]
        public string Passport { get; set; }

        [StringLength(16)]
        public string MedicalInsurance { get; set; }

        [StringLength(11)]
        [Index(IsUnique = true)]
        public string SNILS { get; set; }

        [StringLength(12)]
        [Index(IsUnique = true)]
        public string INN { get; set; }

        [StringLength(20)]
        public string EducationLevel { get; set; }

        [StringLength(100)]
        public string AttitudeToChildren { get; set; }
    }

    // Модель для таблицы Families
    public class Family
    {
        [Key]
        public int FamilyId { get; set; }

        [Required]
        [StringLength(20)]
        public string FamilyName { get; set; }

        public int? FatherId { get; set; }
        [ForeignKey("FatherId")]
        public virtual Father Father { get; set; }

        public int? MotherId { get; set; }
        [ForeignKey("MotherId")]
        public virtual Mother Mother { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        // Навигационное свойство для связи с детьми
        public virtual ICollection<ChildInFamily> ChildrenInFamilies { get; set; } = new List<ChildInFamily>();
    }
    // Модель для таблицы ChildrenInFamilies
    public class ChildInFamily
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChildId { get; set; }
        [ForeignKey("ChildId")]
        public virtual Child Child { get; set; }

        [Required]
        public int FamilyId { get; set; }
        [ForeignKey("FamilyId")]
        public virtual Family Family { get; set; }
    }

    // Модель для таблицы SignalCards
    public class SignalCard
    {
        [Key]
        public int SignalCardId { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public DateTime? DetectionDate { get; set; }

        [Required]
        public int FamilyId { get; set; }
        [ForeignKey("FamilyId")]
        public virtual Family Family { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(50)]
        public string DetectedByEmployee { get; set; }

        public DateTime? ReviewDate { get; set; }

        [StringLength(50)]
        public string ReviewedByEmployee { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "InProgress";
    }

    // Модель для таблицы Commissions
    public class Commission
    {
        [Key]
        public int CommissionId { get; set; }

        [StringLength(50)]
        public string GuardianshipEmployee { get; set; }

        [StringLength(50)]
        public string PDNEmployee { get; set; }

        [StringLength(50)]
        public string KDNEmployee { get; set; }
    }

    // Модель для таблицы InspectionPlans
    public class InspectionPlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        public int SignalCardId { get; set; }
        [ForeignKey("SignalCardId")]
        public virtual SignalCard SignalCard { get; set; }

        [Required]
        public int CommissionId { get; set; }
        [ForeignKey("CommissionId")]
        public virtual Commission Commission { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Required]
        public int FamilyId { get; set; }
        [ForeignKey("FamilyId")]
        public virtual Family Family { get; set; }

        public DateTime? PlanDate { get; set; }

        [StringLength(200)]
        public string Tasks { get; set; }
    }
    // Модель для таблицы InspectionReports
    public class InspectionReport
    {
        [Key]
        public int ReportId { get; set; }

        [Required]
        public int PlanId { get; set; }
        [ForeignKey("PlanId")]
        public virtual InspectionPlan InspectionPlan { get; set; }

        public DateTime? InspectionDate { get; set; }

        [StringLength(200)]
        public string CitizenComplaints { get; set; }

        [StringLength(200)]
        public string PDNMaterials { get; set; }

        [StringLength(200)]
        public string KDNMaterials { get; set; }

        [StringLength(20)]
        public string FamilyName { get; set; }
    }

    // Модель для таблицы ActionLogs
    public class ActionLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; }

        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string Description { get; set; }
    }

    // Контекст базы данных
    public class SocialHelpContext : DbContext
    {
        public SocialHelpContext() : base("name=SocialHelpConnection")
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Mother> Mothers { get; set; }
        public DbSet<Father> Fathers { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<ChildInFamily> ChildrenInFamilies { get; set; }
        public DbSet<SignalCard> SignalCards { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        public DbSet<InspectionPlan> InspectionPlans { get; set; }
        public DbSet<InspectionReport> InspectionReports { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Настройка проверки Gender в таблице Children
            modelBuilder.Entity<Child>()
                .Property(c => c.Gender)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnAnnotation("CheckConstraint", "Gender IN ('М', 'Ж')");

            // Настройка уникальности полей с учетом NULL
            modelBuilder.Entity<Mother>()
                .Property(m => m.Passport)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
            modelBuilder.Entity<Mother>()
                .Property(m => m.SNILS)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
            modelBuilder.Entity<Mother>()
                .Property(m => m.INN)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));

            modelBuilder.Entity<Father>()
                .Property(f => f.Passport)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
            modelBuilder.Entity<Father>()
                .Property(f => f.SNILS)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));
            modelBuilder.Entity<Father>()
                .Property(f => f.INN)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute { IsUnique = true }));

            // Отключение каскадного удаления для связей
            modelBuilder.Entity<Family>()
                .HasOptional(f => f.Father)
                .WithMany()
                .HasForeignKey(f => f.FatherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Family>()
                .HasOptional(f => f.Mother)
                .WithMany()
                .HasForeignKey(f => f.MotherId)
                .WillCascadeOnDelete(false);

            // Явное указание имени таблицы для ChildInFamily
            modelBuilder.Entity<ChildInFamily>()
                .ToTable("ChildrenInFamilies");
        }
    }
}