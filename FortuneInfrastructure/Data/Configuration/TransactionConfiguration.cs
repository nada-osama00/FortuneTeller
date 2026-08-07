using FortuneDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FortuneInfrastructure.Data.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Description).HasMaxLength(500);

            builder.HasIndex(t => new { t.AccountId, t.Date });
        }
    }
}
