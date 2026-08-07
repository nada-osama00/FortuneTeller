using FortuneDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FortuneInfrastructure.Data.Configuration
{
    public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
    {
        public void Configure(EntityTypeBuilder<GroupMember> builder)
        {
            builder.HasKey(m => m.Id);

            // A user can only belong to a given group once.
            builder.HasIndex(m => new { m.GroupId, m.UserId }).IsUnique();

            builder.HasOne(m => m.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
