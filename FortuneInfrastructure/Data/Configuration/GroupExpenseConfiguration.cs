using FortuneDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FortuneInfrastructure.Data.Configuration
{
    public class GroupExpenseConfiguration : IEntityTypeConfiguration<GroupExpense>
    {
        public void Configure(EntityTypeBuilder<GroupExpense> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            builder.Property(e => e.Description).IsRequired().HasMaxLength(300);

            builder.HasOne(e => e.PaidByUser)
                .WithMany()
                .HasForeignKey(e => e.PaidByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Splits)
                .WithOne(s => s.GroupExpense)
                .HasForeignKey(s => s.GroupExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
