using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VVV.Models;

namespace VVV.EF.Mapping
{
    public class ApplicationUserMap : EntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserID);

            // Properties
            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ApplicationUser");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.Manager).HasColumnName("Manager");
            this.Property(t => t.SecondManager).HasColumnName("SecondManager");
            this.Property(t => t.IsMedewerker).HasColumnName("IsMedewerker");
            this.Property(t => t.IsManager).HasColumnName("IsManager");
            this.Property(t => t.IsHRManager).HasColumnName("IsHRManager");
            this.Property(t => t.IsBlocked).HasColumnName("IsBlocked");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.DateChanged).HasColumnName("DateChanged");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.ChangeUser).HasColumnName("ChangeUser");

            // Relationships
            this.HasRequired(t => t.Department1)
                .WithMany(t => t.ApplicationUsers)
                .HasForeignKey(d => d.Department);

        }
    }
}
