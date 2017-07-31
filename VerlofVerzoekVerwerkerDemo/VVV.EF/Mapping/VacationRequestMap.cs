using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VVV.Models;

namespace VVV.EF.Mapping
{
    public class VacationRequestMap : EntityTypeConfiguration<VacationRequest>
    {
        public VacationRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.VacationID);

            // Properties
            this.Property(t => t.Reason)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ReasonRejection)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("VacationRequest");
            this.Property(t => t.VacationID).HasColumnName("VacationID");
            this.Property(t => t.ManagerID).HasColumnName("ManagerID");
            this.Property(t => t.SecondManagerID).HasColumnName("SecondManagerID");
            this.Property(t => t.AssessedByID).HasColumnName("AssessedByID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.DateSubmission).HasColumnName("DateSubmission");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.ReasonRejection).HasColumnName("ReasonRejection");
            this.Property(t => t.BeginDate).HasColumnName("BeginDate");
            this.Property(t => t.BeginOfDay).HasColumnName("BeginOfDay");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.EndOfDay).HasColumnName("EndOfDay");
            this.Property(t => t.TotalMinutes).HasColumnName("TotalMinutes");
            this.Property(t => t.IsTotalDays).HasColumnName("IsTotalDays");
            this.Property(t => t.IsCommunicated).HasColumnName("IsCommunicated");
            this.Property(t => t.HasDeadlines).HasColumnName("HasDeadlines");
            this.Property(t => t.OldBeginDate).HasColumnName("OldBeginDate");
            this.Property(t => t.OldEndDate).HasColumnName("OldEndDate");
            this.Property(t => t.OldTotalMinutes).HasColumnName("OldTotalMinutes");
            this.Property(t => t.IsOldTotalDays).HasColumnName("IsOldTotalDays");
            this.Property(t => t.IsRejected).HasColumnName("IsRejected");
            this.Property(t => t.IsApproved).HasColumnName("IsApproved");
            this.Property(t => t.IsInTreatment).HasColumnName("IsInTreatment");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.DateChanged).HasColumnName("DateChanged");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.ChangeUser).HasColumnName("ChangeUser");

            // Relationships
            this.HasRequired(t => t.ApplicationUser)
                .WithMany(t => t.VacationRequests)
                .HasForeignKey(d => d.UserID);

        }
    }
}
