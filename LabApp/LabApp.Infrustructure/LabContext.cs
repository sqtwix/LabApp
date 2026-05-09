using LabApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Infrustructure;

public partial class LabContext : DbContext
{
    public LabContext()
    {
    }

    public LabContext(DbContextOptions<LabContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Biomaterial> Biomaterials { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<GetDeprtmentsWithStaffCount> GetDeprtmentsWithStaffCounts { get; set; }

    public virtual DbSet<GetPatientsWithAnalyze> GetPatientsWithAnalyzes { get; set; }

    public virtual DbSet<GetPensionPatient> GetPensionPatients { get; set; }

    public virtual DbSet<Indicator> Indicators { get; set; }

    public virtual DbSet<InsuranceCompany> InsuranceCompanies { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientsAudit> PatientsAudits { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Reagent> Reagents { get; set; }

    public virtual DbSet<Research> Researches { get; set; }

    public virtual DbSet<ResearchType> ResearchTypes { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<ResultCarrierType> ResultCarrierTypes { get; set; }

    public virtual DbSet<ResultsAudit> ResultsAudits { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceIndicator> ServiceIndicators { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<StaffSchedule> StaffSchedules { get; set; }

    public virtual DbSet<PatientInsurance> PatientInsurances { get; set; }

    public virtual DbSet<StaffDepartment> StaffDepartments { get; set; }

    public virtual DbSet<StaffSpecialization> StaffSpecializations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=medical_lab;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("appointments_pk");

            entity.ToTable("appointments");

            entity.Property(e => e.AppointmentId)
                .ValueGeneratedNever()
                .HasColumnName("appointment_id");
            entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date");
            entity.Property(e => e.AppointmentTime).HasColumnName("appointment_time");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("appointments_patients_fk");

            entity.HasOne(d => d.Staff).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("appointments_staffs_fk");
        });

        modelBuilder.Entity<Biomaterial>(entity =>
        {
            entity.HasKey(e => e.BiomaterialId).HasName("biomaterials_pk");

            entity.ToTable("biomaterials");

            entity.Property(e => e.BiomaterialId)
                .ValueGeneratedNever()
                .HasColumnName("biomaterial_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("cities_pk");

            entity.ToTable("cities");

            entity.Property(e => e.CityId)
                .ValueGeneratedNever()
                .HasColumnName("city_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("departments_pk");

            entity.ToTable("departments");

            entity.Property(e => e.DepartmentId)
                .ValueGeneratedNever()
                .HasColumnName("department_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("equipment_pk");

            entity.ToTable("equipment");

            entity.Property(e => e.EquipmentId)
                .ValueGeneratedNever()
                .HasColumnName("equipment_id");
            entity.Property(e => e.ExplotationDate).HasColumnName("explotation_date");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.ServiceLife).HasColumnName("service_life");

            entity.HasOne(d => d.Room).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("equipment_rooms_fk");
        });

        modelBuilder.Entity<GetDeprtmentsWithStaffCount>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("get_deprtments_with_staff_count");

            entity.Property(e => e.IdОтделения).HasColumnName("ID отделения");
            entity.Property(e => e.КолВоСотурдниковВОтделении).HasColumnName("Кол-во сотурдников в отделении");
            entity.Property(e => e.НазваниеОтделения)
                .HasMaxLength(100)
                .HasColumnName("Название отделения");
        });

        modelBuilder.Entity<GetPatientsWithAnalyze>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("get_patients_with_analyzes");

            entity.Property(e => e.IdПациента).HasColumnName("ID пациента");
            entity.Property(e => e.КолВоАнализов).HasColumnName("Кол-во анализов");
            entity.Property(e => e.ФиоПациента).HasColumnName("ФИО пациента");
            entity.Property(e => e.ЧастотаОбращений).HasColumnName("Частота обращений");
        });

        modelBuilder.Entity<GetPensionPatient>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("get_pension_patients");

            entity.Property(e => e.IdПациента).HasColumnName("ID пациента");
            entity.Property(e => e.Пол).HasMaxLength(1);
            entity.Property(e => e.Фио).HasColumnName("ФИО");
        });

        modelBuilder.Entity<Indicator>(entity =>
        {
            entity.HasKey(e => e.IndicatorId).HasName("indicators_pk");

            entity.ToTable("indicators");

            entity.Property(e => e.IndicatorId)
                .ValueGeneratedNever()
                .HasColumnName("indicator_id");
            entity.Property(e => e.BiomaterialId).HasColumnName("biomaterial_id");
            entity.Property(e => e.Cost)
                .HasPrecision(10, 2)
                .HasColumnName("cost");
            entity.Property(e => e.ExecutionTime)
                .HasMaxLength(50)
                .HasColumnName("execution_time");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");

            entity.HasOne(d => d.Biomaterial).WithMany(p => p.Indicators)
                .HasForeignKey(d => d.BiomaterialId)
                .HasConstraintName("indicators_biomaterials_fk");
        });

        modelBuilder.Entity<InsuranceCompany>(entity =>
        {
            entity.HasKey(e => e.InsuranceId).HasName("insurance_companies_pk");

            entity.ToTable("insurance_companies");

            entity.Property(e => e.InsuranceId)
                .ValueGeneratedNever()
                .HasColumnName("insurance_id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("patients_pk");

            entity.ToTable("patients");

            entity.Property(e => e.PatientId)
                .ValueGeneratedNever()
                .HasColumnName("patient_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middle_name");
            entity.Property(e => e.Passport)
                .HasColumnType("character varying")
                .HasColumnName("passport");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");

            entity.HasOne(d => d.City).WithMany(p => p.Patients)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("patients_cities_fk");
        });

            modelBuilder.Entity<PatientsAudit>(entity =>
            {
                entity.HasKey(e => e.AuditId).HasName("patients_audit_pkey");

                entity.ToTable("patients_audit");

                entity.Property(e => e.AuditId).HasColumnName("audit_id");
                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");
                entity.Property(e => e.BirthDate).HasColumnName("birth_date");
                entity.Property(e => e.ChangedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("changed_at");
                entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
                entity.Property(e => e.CityId).HasColumnName("city_id");
                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_name");
                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .HasColumnName("gender");
                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_name");
                entity.Property(e => e.MiddleName)
                    .HasMaxLength(100)
                    .HasColumnName("middle_name");
                entity.Property(e => e.OperationType).HasColumnName("operation_type");
                entity.Property(e => e.Passport)
                    .HasMaxLength(20)
                    .HasColumnName("passport");
                entity.Property(e => e.PatientId).HasColumnName("patient_id");
                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");
            });

            modelBuilder.Entity<PaymentType>(entity =>
            {
                entity.HasKey(e => e.PaymentTypeId).HasName("payment_types_pk");

                entity.ToTable("payment_types");

                entity.Property(e => e.PaymentTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("payment_type_id");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<PatientInsurance>(entity =>
            {
                entity.HasKey(e => new { e.PatientId, e.InsuranceId }).HasName("patient_insurance_pk");
                entity.ToTable("patient_insurance");

                entity.Property(e => e.PatientId).HasColumnName("patient_id");
                entity.Property(e => e.InsuranceId).HasColumnName("insurance_id");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientInsurances)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("patient_insurance_patients_fk");

                entity.HasOne(d => d.Insurance)
                    .WithMany(i => i.PatientInsurances)
                    .HasForeignKey(d => d.InsuranceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("patient_insurance_insurance_companies_fk");
            });



            modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("positions_pk");

            entity.ToTable("positions");

            entity.Property(e => e.PositionId)
                .ValueGeneratedNever()
                .HasColumnName("position_id");
            entity.Property(e => e.Duties).HasColumnName("duties");
            entity.Property(e => e.EducationLevel)
                .HasMaxLength(100)
                .HasColumnName("education_level");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Salary)
                .HasPrecision(10, 2)
                .HasColumnName("salary");
        });

            modelBuilder.Entity<Reagent>(entity =>
            {
                entity.HasKey(e => e.ReagentId).HasName("reagents_pk");

                entity.ToTable("reagents");

                entity.Property(e => e.ReagentId)
                    .ValueGeneratedNever()
                    .HasColumnName("reagent_id");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.HasMany(d => d.ResearchesNavigation).WithMany(p => p.Reagents)
                    .UsingEntity<Dictionary<string, object>>(
                        "ResearchesReagent",
                        r => r.HasOne<Research>().WithMany()
                            .HasForeignKey("ResearchId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("researches_reagents_researches_fk"),
                        l => l.HasOne<Reagent>().WithMany()
                            .HasForeignKey("ReagentId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("researches_reagents_reagents_fk"),
                        j =>
                        {
                            j.HasKey("ReagentId", "ResearchId").HasName("researches_reagents_pk");
                            j.ToTable("researches_reagents");
                            j.IndexerProperty<int>("ReagentId").HasColumnName("reagent_id");
                            j.IndexerProperty<int>("ResearchId").HasColumnName("research_id");
                        });
            });

            modelBuilder.Entity<Research>(entity =>
            {
                entity.HasKey(e => e.ResearchId).HasName("researches_pk");

                entity.ToTable("researches");

                entity.Property(e => e.ResearchId)
                    .ValueGeneratedNever()
                    .HasColumnName("research_id");
                entity.Property(e => e.Cost)
                    .HasPrecision(10, 2)
                    .HasColumnName("cost");
                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasColumnName("name");
                entity.Property(e => e.ReagentId).HasColumnName("reagent_id");
                entity.Property(e => e.ResearchTypeId).HasColumnName("research_type_id");

                entity.HasOne(d => d.Reagent).WithMany(p => p.Researches)
                    .HasForeignKey(d => d.ReagentId)
                    .HasConstraintName("researches_reagents_fk");

                entity.HasOne(d => d.ResearchType).WithMany(p => p.Researches)
                    .HasForeignKey(d => d.ResearchTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("researches_research_types_fk");

                entity.HasMany(d => d.Equipment).WithMany(p => p.Researches)
                    .UsingEntity<Dictionary<string, object>>(
                        "ResearchEquipment",
                        r => r.HasOne<Equipment>().WithMany()
                            .HasForeignKey("EquipmentId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("research_equipment_equipment_fk"),
                        l => l.HasOne<Research>().WithMany()
                            .HasForeignKey("ResearchId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("research_equipment_researches_fk"),
                        j =>
                        {
                            j.HasKey("ResearchId", "EquipmentId").HasName("research_equipment_pk");
                            j.ToTable("research_equipment");
                            j.IndexerProperty<int>("ResearchId").HasColumnName("research_id");
                            j.IndexerProperty<int>("EquipmentId").HasColumnName("equipment_id");
                        });
            });

            modelBuilder.Entity<ResearchType>(entity =>
            {
                entity.HasKey(e => e.ResearchTypeId).HasName("research_types_pk");

                entity.ToTable("research_types");

                entity.Property(e => e.ResearchTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("research_type_id");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => new { e.ReferralId, e.ResearchId }).HasName("results_pk");

                entity.ToTable("results");

                entity.Property(e => e.ReferralId).HasColumnName("referral_id");
                entity.Property(e => e.ResearchId).HasColumnName("research_id");
                entity.Property(e => e.CarrierTypeId).HasColumnName("carrier_type_id");
                entity.Property(e => e.CompletionDate).HasColumnName("completion_date");
                entity.Property(e => e.Description).HasColumnName("description");

                entity.HasOne(d => d.CarrierType).WithMany(p => p.Results)
                    .HasForeignKey(d => d.CarrierTypeId)
                    .HasConstraintName("results_result_carrier_types_fk");

                entity.HasOne(d => d.Referral).WithMany(p => p.Results)
                    .HasForeignKey(d => d.ReferralId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("results_appointments_fk");

                entity.HasOne(d => d.Research).WithMany(p => p.Results)
                    .HasForeignKey(d => d.ResearchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("results_researches_fk");
            });

            modelBuilder.Entity<ResultCarrierType>(entity =>
            {
                entity.HasKey(e => e.CarrierTypeId).HasName("result_carrier_types_pk");

                entity.ToTable("result_carrier_types");

                entity.Property(e => e.CarrierTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("carrier_type_id");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ResultsAudit>(entity =>
            {
                entity.HasKey(e => e.AuditId).HasName("results_audit_pkey");

                entity.ToTable("results_audit");

                entity.Property(e => e.AuditId).HasColumnName("audit_id");
                entity.Property(e => e.ChangedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("changed_at");
                entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
                entity.Property(e => e.OldCarrierTypeId).HasColumnName("old_carrier_type_id");
                entity.Property(e => e.OldCompletionDate).HasColumnName("old_completion_date");
                entity.Property(e => e.OldDescription).HasColumnName("old_description");
                entity.Property(e => e.OperationType).HasColumnName("operation_type");
                entity.Property(e => e.ReferralId).HasColumnName("referral_id");
                entity.Property(e => e.ResearchId).HasColumnName("research_id");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.RoomId).HasName("rooms_pk");

                entity.ToTable("rooms");

                entity.Property(e => e.RoomId)
                    .ValueGeneratedNever()
                    .HasColumnName("room_id");
                entity.Property(e => e.DepartmentId).HasColumnName("department_id");
                entity.Property(e => e.RoomNumber)
                    .HasMaxLength(20)
                    .HasColumnName("room_number");

                entity.HasOne(d => d.Department).WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("rooms_departments_fk");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => new { e.StaffId, e.ResearchId, e.AppointmentId }).HasName("services_pk");

                entity.ToTable("services");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");
                entity.Property(e => e.ResearchId).HasColumnName("research_id");
                entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
                entity.Property(e => e.DepartmentId).HasColumnName("department_id");
                entity.Property(e => e.PaymentTypeId).HasColumnName("payment_type_id");
                entity.Property(e => e.ServiceDate).HasColumnName("service_date");

                entity.HasOne(d => d.Appointment).WithMany(p => p.Services)
                    .HasForeignKey(d => d.AppointmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("services_appointments_fk");

                entity.HasOne(d => d.Department).WithMany(p => p.Services)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("services_departments_fk");

                entity.HasOne(d => d.PaymentType).WithMany(p => p.Services)
                    .HasForeignKey(d => d.PaymentTypeId)
                    .HasConstraintName("services_payment_types_fk");

                entity.HasOne(d => d.Research).WithMany(p => p.Services)
                    .HasForeignKey(d => d.ResearchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("services_researches_fk");

                entity.HasOne(d => d.Staff).WithMany(p => p.Services)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("services_staffs_fk");
            });

            modelBuilder.Entity<ServiceIndicator>(entity =>
            {
                entity.HasKey(e => new { e.ServiceId, e.IndicatorId }).HasName("service_indicator_pk");

                entity.ToTable("service_indicator");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");
                entity.Property(e => e.IndicatorId).HasColumnName("indicator_id");
                entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
                entity.Property(e => e.ResearchId).HasColumnName("research_id");
                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.HasOne(d => d.Indicator).WithMany(p => p.ServiceIndicators)
                    .HasForeignKey(d => d.IndicatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("service_indicator_indicators_fk");

                entity.HasOne(d => d.Service).WithMany(p => p.ServiceIndicators)
                    .HasForeignKey(d => new { d.StaffId, d.ResearchId, d.AppointmentId })
                    .HasConstraintName("fk_service_indicator_services");
            });

            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.HasKey(e => e.SpecializationId).HasName("specializations_pk");

                entity.ToTable("specializations");

                entity.Property(e => e.SpecializationId)
                    .ValueGeneratedNever()
                    .HasColumnName("specialization_id");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId).HasName("staffs_pk");

                entity.ToTable("staffs");

                entity.Property(e => e.StaffId)
                    .ValueGeneratedNever()
                    .HasColumnName("staff_id");
                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");
                entity.Property(e => e.BirthDate).HasColumnName("birth_date");
                entity.Property(e => e.CityId).HasColumnName("city_id");
                entity.Property(e => e.Education)
                    .HasMaxLength(100)
                    .HasColumnName("education");
                entity.Property(e => e.Login).HasMaxLength(50);
                entity.HasIndex(e => e.Login).IsUnique().HasFilter("login IS NOT NULL");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.RoleName).HasColumnName("role_name").HasMaxLength(50);
                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_name");
                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .HasColumnName("gender");
                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_name");
                entity.Property(e => e.MiddleName)
                    .HasMaxLength(100)
                    .HasColumnName("middle_name");
                entity.Property(e => e.Passport)
                    .HasMaxLength(50)
                    .HasColumnName("passport");
                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");
                entity.Property(e => e.PositionId).HasColumnName("position_id");

                entity.HasOne(d => d.City).WithMany(p => p.Staff)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("staffs_cities_fk");

                entity.HasOne(d => d.Position).WithMany(p => p.Staff)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("staffs_positions_fk");

                entity.HasMany(d => d.Departments).WithMany(p => p.Staff)
                    .UsingEntity<Dictionary<string, object>>(
                        "StaffDepartment",
                        r => r.HasOne<Department>().WithMany()
                            .HasForeignKey("DepartmentId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("staff_department_departments_fk"),
                        l => l.HasOne<Staff>().WithMany()
                            .HasForeignKey("StaffId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("staff_department_staffs_fk"),
                        j =>
                        {
                            j.HasKey("StaffId", "DepartmentId").HasName("staff_department_pk");
                            j.ToTable("staff_department");
                            j.IndexerProperty<int>("StaffId").HasColumnName("staff_id");
                            j.IndexerProperty<int>("DepartmentId").HasColumnName("department_id");
                        });

                entity.HasMany(d => d.Specializations).WithMany(p => p.Staff)
                    .UsingEntity<Dictionary<string, object>>(
                        "StaffSpecialization",
                        r => r.HasOne<Specialization>().WithMany()
                            .HasForeignKey("SpecializationId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("staff_specialization_specializations_fk"),
                        l => l.HasOne<Staff>().WithMany()
                            .HasForeignKey("StaffId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("staff_specialization_staffs_fk"),
                        j =>
                        {
                            j.HasKey("StaffId", "SpecializationId").HasName("staff_specialization_pk");
                            j.ToTable("staff_specialization");
                            j.IndexerProperty<int>("StaffId").HasColumnName("staff_id");
                            j.IndexerProperty<int>("SpecializationId").HasColumnName("specialization_id");
                        });
            });

            modelBuilder.Entity<StaffSchedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId).HasName("staff_schedule_pk");

                entity.ToTable("staff_schedule");

                entity.Property(e => e.ScheduleId)
                    .ValueGeneratedNever()
                    .HasColumnName("schedule_id");
                entity.Property(e => e.DayOfWeek)
                    .HasMaxLength(20)
                    .HasColumnName("day_of_week");
                entity.Property(e => e.EndTime).HasColumnName("end_time");
                entity.Property(e => e.RoomId).HasColumnName("room_id");
                entity.Property(e => e.StaffId).HasColumnName("staff_id");
                entity.Property(e => e.StartTime).HasColumnName("start_time");

                entity.HasOne(d => d.Room).WithMany(p => p.StaffSchedules)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("staff_schedule_rooms_fk");

                entity.HasOne(d => d.Staff).WithMany(p => p.StaffSchedules)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("staff_schedule_staffs_fk");
            });

        modelBuilder.Entity<StaffDepartment>(entity =>
        {
            entity.HasKey(e => new { e.StaffId, e.DepartmentId }).HasName("staff_department_pk");
            entity.ToTable("staff_department");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");

            entity.HasOne(d => d.Staff)
                .WithMany(p => p.StaffDepartments)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("staff_department_staffs_fk");

            entity.HasOne(d => d.Department)
                .WithMany(p => p.StaffDepartments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("staff_department_departments_fk");
        });

        modelBuilder.Entity<StaffSpecialization>(entity =>
        {
            entity.HasKey(e => new { e.StaffId, e.SpecializationId }).HasName("staff_specialization_pk");
            entity.ToTable("staff_specialization");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");

            entity.HasOne(d => d.Staff)
                .WithMany(p => p.StaffSpecializations)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("staff_specialization_staffs_fk");

            entity.HasOne(d => d.Specialization)
                .WithMany(p => p.StaffSpecializations)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("staff_specialization_specializations_fk");
        });

        OnModelCreatingPartial(modelBuilder);
        }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
