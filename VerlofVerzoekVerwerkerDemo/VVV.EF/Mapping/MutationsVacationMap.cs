using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VVV.Models;

namespace VVV.EF.Mapping
{
    public class MutationsVacationMap : EntityTypeConfiguration<MutationsVacation>
    {
        public MutationsVacationMap()
        {
            // Primary Key
            this.HasKey(t => t.MutationID);

            // Properties
            // Table & Column Mappings
            this.ToTable("MutationsVacation");
            this.Property(t => t.MutationID).HasColumnName("MutationID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.VacationID).HasColumnName("VacationID");
            this.Property(t => t.VacationModification).HasColumnName("VacationModification");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.DateChanged).HasColumnName("DateChanged");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.ChangeUser).HasColumnName("ChangeUser");

            // Relationships
            this.HasRequired(t => t.ApplicationUser)
                .WithMany(t => t.MutationsVacations)
                .HasForeignKey(d => d.UserID);
            this.HasRequired(t => t.ApplicationUser1)
                .WithMany(t => t.MutationsVacations1)
                .HasForeignKey(d => d.CreateUser);
            this.HasOptional(t => t.VacationRequest)
                .WithMany(t => t.MutationsVacations)
                .HasForeignKey(d => d.VacationID);

        }
    }
}
