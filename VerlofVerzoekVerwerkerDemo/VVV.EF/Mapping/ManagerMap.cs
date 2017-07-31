using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VVV.Models;

namespace VVV.EF.Mapping
{
    public class ManagerMap : EntityTypeConfiguration<Manager>
    {
        public ManagerMap()
        {
            // Primary Key
            this.HasKey(t => t.ManagerID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Manager");
            this.Property(t => t.ManagerID).HasColumnName("ManagerID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.IsAvailable).HasColumnName("IsAvailable");
            this.Property(t => t.From).HasColumnName("From");
            this.Property(t => t.Till).HasColumnName("Till");

            // Relationships
            this.HasRequired(t => t.ApplicationUser)
                .WithMany(t => t.Managers)
                .HasForeignKey(d => d.UserID);

        }
    }
}
