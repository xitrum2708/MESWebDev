using MESWebDev.Models;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.IQC;
using MESWebDev.Models.OQC;
using MESWebDev.Models.PE;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Models.REPAIR;
using MESWebDev.Models.SMT;
using MESWebDev.Models.SPO;
using MESWebDev.Models.TELSTAR;
using MESWebDev.Models.UVASSY;
using MESWebDev.Models.WHS;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuTranslation> MenuTranslations { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<ExecutionLog> ExecutionLogs { get; set; }
        public DbSet<UVAssyProduction> UVAssyProductions { get; set; }
        public DbSet<UVAssyProductResult> UVAssyProductResults { get; set; }
        public DbSet<UVAssyOutputDetail> UVAssyOutputDetails { get; set; }
        public DbSet<UVAssyErrorDetail> UVAssyErrorDetails { get; set; }
        public DbSet<UVAssyAllOutputResult> UVAssyAllOutputResults { get; set; }

        //============= IQC
        public DbSet<UV_IQC_Report> UV_IQC_Reports { get; set; }

        public DbSet<UV_IQC_ReportItem> UV_IQC_ReportItems { get; set; }
        public DbSet<UV_IQC_InspectionGroup> UV_IQC_InspectionGroups { get; set; }
        public DbSet<UV_IQC_ReportImage> UV_IQC_ReportImages { get; set; }
        public DbSet<UV_IQC_ReportFile> UV_IQC_ReportFiles { get; set; }
        public DbSet<UV_IQC_ItemName> UV_IQC_ItemNames { get; set; }
        public DbSet<UV_IQC_ErrorsItemMaster> UV_IQC_ErrorsItemMasters { get; set; }
        public DbSet<MonthlyRejectRateModel> MonthlyRejectRateModels { get; set; }
        public DbSet<TopSupplierErrorReportModel> TopSupplierErrorReportModels { get; set; }
        public DbSet<UV_IQC_RESULT> UV_IQC_RESULTs { get; set; }
        public DbSet<WHS_RECEIVING_TOTAL> WHS_RECEIVING_TOTALs { get; set; }
        public DbSet<WHS_IQC_CONTROL> WHS_IQC_CONTROLs { get; set; }

        //=======================
        public DbSet<UV_PRO_LINE_Model> UV_PRO_LINEs { get; set; }

        public DbSet<TELSTAR_ASSY_Model> TELSTAR_ASSYs { get; set; }
        public DbSet<TELSTAR_DQC> TELSTAR_DQCs { get; set; }
        public DbSet<TONLY_DQC> TONLY_DQCs { get; set; }
        public DbSet<EASTECH_SMT_OUTPUT_Model> EASTECH_SMT_OUTPUTs { get; set; }
        public DbSet<UV_SPO_MASTER_ALL_Model> UV_SPO_MASTER_ALLs { get; set; }
        public DbSet<tbl_EstechSerialGeneral_Model> tbl_EstechSerialGenerals { get; set; }

        //=============REPAIR====================
        public DbSet<UV_REPAIRRESULT> UV_REPAIRRESULTs { get; set; }

        public DbSet<EASTECH_OQC> EASTECH_OQCs { get; set; }
        public DbSet<A_OperatorProcessData> A_OperatorProcessDatas { get; set; }
        public DbSet<UVSMT_MODEL_MATRIX_MASTER> UVSMT_MODEL_MATRIX_MASTERs { get; set; }
        public DbSet<UV_IQC_WHS_SORTING> UV_IQC_WHS_SORTINGs { get; set; }
        //=======================================
        public DbSet<UV_LOTCONTROL_MASTER> UV_LOTCONTROL_MASTERs { get; set; }
        public DbSet<UV_LOTGENERALSUMMARY_MASTER> UV_LOTGENERALSUMMARY_MASTERs { get; set; }

        //--------------+ Production Plan +-------------------
        public DbSet<ProdPlanModel> PP_ProdPlan_tbl { get; set; }
        public DbSet<ProdPlanParaModel> PP_Para_tbl { get; set; }
        public DbSet<CalendarModel> PP_Calendar_tbl { get; set; }

        //--------------+ PE - manpower +-------------------
        public DbSet<ManpowerModel> PE_Manpower_tbl { get; set; }

        public DbSet<UploadFileMaster> Master_UploadFile_mst { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UV_LOTCONTROL_MASTER>(entity =>
            {
                entity.ToTable("UV_LOTCONTROL_MASTER");
            });
            modelBuilder.Entity<UV_LOTGENERALSUMMARY_MASTER>(entity =>
            {
                entity.ToTable("UV_LOTGENERALSUMMARY_MASTER");
            });
            modelBuilder.Entity<UV_LOTCONTROL_MASTER>()
                .HasKey(l => l.LotControlID);
            modelBuilder.Entity<UV_LOTCONTROL_MASTER>()
                .HasIndex(l => l.LotNo)
                .IsUnique();
            modelBuilder.Entity<UV_LOTGENERALSUMMARY_MASTER>()
                .HasOne(s => s.UV_LOTCONTROL_MASTER)
                .WithMany(l => l.UV_LOTGENERALSUMMARY_MASTER)
                .HasForeignKey(s => s.LotNo)
                .HasPrincipalKey(l => l.LotNo);

            modelBuilder.Entity<UV_LOTCONTROL_MASTER>()
                .HasOne(l => l.SPO_MASTER)
                .WithMany()
                .HasForeignKey(l => l.LotNo)
                .HasPrincipalKey(s => s.LotNo);

            modelBuilder.Entity<UV_IQC_WHS_SORTING>(entity =>
            {
                entity.ToTable("UV_IQC_WHS_SORTING");
            });
            modelBuilder.Entity<WHS_RECEIVING_TOTAL>()
                .Property(x => x.ID)
                .HasDefaultValueSql("(CONVERT( ,getdate(),(112))+'-')+right('00000'+CONVERT( ,NEXT VALUE FOR [seq_id],0),(5))");

            modelBuilder.Entity<UV_IQC_WHS_SORTING>(entity =>
            {
                entity.ToTable("UV_IQC_WHS_SORTING");
            });
            modelBuilder.Entity<UVSMT_MODEL_MATRIX_MASTER>(entity =>
            {
                entity.ToTable("UVSMT_MODEL_MATRIX_MASTER");
            });
            modelBuilder.Entity<A_OperatorProcessData>(entity =>
            {
                entity.ToTable("A_OperatorProcessData");
            });
            modelBuilder.Entity<EASTECH_OQC>(entity =>
            {
                entity.ToTable("EASTECH_OQC");
            });
            modelBuilder.Entity<TELSTAR_DQC>(entity =>
            {
                entity.ToTable("TELSTAR_DQC");
            });
            modelBuilder.Entity<TONLY_DQC>(entity =>
            {
                entity.ToTable("TONLY_DQC");
            });
            modelBuilder.Entity<UV_PRO_LINE_Model>(entity =>
            {
                entity.ToTable("UV_PRO_LINE");
            });
            modelBuilder.Entity<TELSTAR_ASSY_Model>(entity =>
            {
                entity.ToTable("TELSTAR_ASSY");
            });
            modelBuilder.Entity<EASTECH_SMT_OUTPUT_Model>(entity =>
            {
                entity.ToTable("EASTECH_SMT_OUTPUT");
            });
            modelBuilder.Entity<UV_SPO_MASTER_ALL_Model>(entity =>
            {
                entity.ToTable("UV_SPO_MASTER_ALL");
            });

            modelBuilder.Entity<tbl_EstechSerialGeneral_Model>(entity =>
            {
                entity.ToTable("tbl_EstechSerialGeneral");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("UVMES_Users");
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("UVMES_Roles");
            });
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UVMES_UserRoles");
            });
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("UVMES_Menus");
            });
            modelBuilder.Entity<MenuTranslation>(entity =>
            {
                entity.ToTable("UVMES_MenuTranslation");
            });
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("UVMES_Permissions");
            });
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("UVMES_RolePermissions");
            });
            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("UVMES_Languages");
            });
            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("UVMES_Translations");
            });
            modelBuilder.Entity<ExecutionLog>(entity =>
            {
                entity.ToTable("UVMES_ExecutionLog");
            });

            // Cấu hình khóa chính cho bảng trung gian UserRoles
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Cấu hình khóa chính cho bảng trung gian RolePermissions
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Cấu hình khóa chính cho bảng trung gian MenuTranslations
            modelBuilder.Entity<MenuTranslation>()
                .HasKey(mt => new { mt.MenuId, mt.LanguageId });

            // Các mối quan hệ (nếu cần thêm ràng buộc)
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<MenuTranslation>()
                .HasOne(mt => mt.Menu)
                .WithMany(m => m.MenuTranslations)
                .HasForeignKey(mt => mt.MenuId);

            modelBuilder.Entity<MenuTranslation>()
                .HasOne(mt => mt.Language)
                .WithMany()
                .HasForeignKey(mt => mt.LanguageId);

            modelBuilder.Entity<UVAssyProductResult>()
                .HasNoKey()
                .ToView(null);

            modelBuilder.Entity<UVAssyProduction>()
                .HasNoKey()
                .ToView(null); // No underlying table/view since data comes from a stored procedure

            modelBuilder.Entity<UVAssyErrorDetail>()
                .HasNoKey()
                .ToView(null);

            modelBuilder.Entity<UVAssyOutputDetail>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<UVAssyAllOutputResult>()
                .HasNoKey()
                .ToView(null);
            //modelBuilder.Entity<UV_IQC_RESULTModel>()
            //    .HasOne(r => r.WHS_IQC_CONTROLModel)
            //    .WithOne(c => c.UV_IQC_RESULTModel)
            //    .HasForeignKey<WHS_IQC_CONTROLModel>(c => c.ID) // WHS_IQC_CONTROL là bên phụ
            //    .OnDelete(DeleteBehavior.Restrict);

            // === IQC ==========
            modelBuilder.Entity<UV_IQC_Report>()
               .HasMany(r => r.ReportItems)
               .WithOne(ri => ri.Reports)
               .HasForeignKey(ri => ri.ReportID);

            modelBuilder.Entity<UV_IQC_ErrorsItemMaster>()
                .HasMany(r => r.ReportItems)
                .WithOne(ri => ri.ErrorsItemMasters)
                .HasForeignKey(ri => ri.ErrorCodeID);

            modelBuilder.Entity<UV_IQC_Report>()
                .HasOne(r => r.InspectionGroups)
                .WithMany(g => g.Reports)
                .HasForeignKey(r => r.InspectionGroupID);

            modelBuilder.Entity<UV_IQC_ReportImage>()
                .Property(b => b.UploadedDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<UV_IQC_ReportFile>()
                .Property(p => p.UploadedDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<UV_IQC_ItemName>()
               .Property(i => i.CreatedDate)
               .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<UV_IQC_ItemName>()
                .Property(i => i.UpdatedDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<UV_IQC_ErrorsItemMaster>()
                .Property(e => e.CreatedDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<UV_IQC_ErrorsItemMaster>()
                .Property(e => e.UpdatedDate)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<MonthlyRejectRateModel>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<TopSupplierErrorReportModel>()
                .HasNoKey()
                .ToView(null);

            modelBuilder.Entity<MonthlyRejectRateModel>()
                .Property(m => m.JAN)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
                .Property(m => m.FEB)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
               .Property(m => m.MAR)
               .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.APR)
              .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.MAY)
              .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.JUN)
              .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.JUL)
              .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.AUG)
              .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.SEP)
              .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.OCT)
              .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.NOV)
              .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<MonthlyRejectRateModel>()
              .Property(m => m.DEC)
              .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TopSupplierErrorReportModel>()
                .Property(m => m.RATE_ACCEPT)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<UV_IQC_ReportItem>()
                .Property(m => m.NG_Rate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<UV_REPAIRRESULT>(entity =>
            {
                entity.Property(e => e.DailyOutput).HasDefaultValue(0);
                entity.Property(e => e.DDRDailyUpdate).HasDefaultValue(0);
            });
        }
    }
}