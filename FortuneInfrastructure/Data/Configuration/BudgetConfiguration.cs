using FortuneDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FortuneInfrastructure.Data.Configuration
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.MonthlyLimit).HasColumnType("decimal(18,2)");

            // One budget per category per month per user.
            builder.HasIndex(b => new { b.UserId, b.CategoryId, b.Month }).IsUnique();
        }
    }
}
