using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VVV.Models;

namespace VVV.EF.Mapping
{
    public class HolidayMap : EntityTypeConfiguration<Holiday>
    {
        public HolidayMap()
        {
            // Primary Key
            this.HasKey(t => t.HolidayID);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Holiday");
            this.Property(t => t.HolidayID).HasColumnName("HolidayID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Date).HasColumnName("Date");
        }
    }
}
