using FortuneDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FortuneInfrastructure.Data.Configuration
{
    public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
    {
        public void Configure(EntityTypeBuilder<Settlement> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Amount).HasColumnType("decimal(18,2)");

            builder.HasOne(s => s.FromUser)
                .WithMany()
                .HasForeignKey(s => s.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.ToUser)
                .WithMany()
                .HasForeignKey(s => s.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
