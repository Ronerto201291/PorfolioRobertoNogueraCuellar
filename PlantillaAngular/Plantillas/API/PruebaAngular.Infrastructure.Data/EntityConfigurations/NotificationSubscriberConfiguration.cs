using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaAngular.Domain.AggregateModels;

namespace PruebaAngular.Infrastructure.Data.EntityConfigurations
{
    public class NotificationSubscriberConfiguration : IEntityTypeConfiguration<NotificationSubscriber>
    {
        public void Configure(EntityTypeBuilder<NotificationSubscriber> builder)
        {
            builder.ToTable("notification_subscribers", "public");
            builder.HasKey(s => s.SubscriberId);
            builder.Property(s => s.SubscriberId)
                .HasColumnName("id");
            builder.Property(s => s.Email)
                .HasColumnName("email")
                .HasMaxLength(320)
                .IsRequired();
        }
    }
}
