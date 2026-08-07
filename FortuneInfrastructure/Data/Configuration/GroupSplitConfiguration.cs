using FortuneDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FortuneInfrastructure.Data.Configuration
{
    public class GroupSplitConfiguration : IEntityTypeConfiguration<GroupSplit>
    {
        public void Configure(EntityTypeBuilder<GroupSplit> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.AmountOwed).HasColumnType("decimal(18,2)");

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
