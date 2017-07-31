using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using VVV.Models;

namespace VVV.EF.Mapping
{
    public class MessageMap : EntityTypeConfiguration<Message>
    {
        public MessageMap()
        {
            // Primary Key
            this.HasKey(t => t.MessageID);

            // Properties
            this.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MessageText)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Message");
            this.Property(t => t.MessageID).HasColumnName("MessageID");
            this.Property(t => t.SenderID).HasColumnName("SenderID");
            this.Property(t => t.ReceiverID).HasColumnName("ReceiverID");
            this.Property(t => t.VacationRequestID).HasColumnName("VacationRequestID");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.MessageText).HasColumnName("MessageText");
            this.Property(t => t.IsRead).HasColumnName("IsRead");
            this.Property(t => t.DateSend).HasColumnName("DateSend");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");

            // Relationships
            this.HasRequired(t => t.ApplicationUser)
                .WithMany(t => t.Messages)
                .HasForeignKey(d => d.SenderID);
            this.HasRequired(t => t.VacationRequest)
                .WithMany(t => t.Messages)
                .HasForeignKey(d => d.VacationRequestID);

        }
    }
}
