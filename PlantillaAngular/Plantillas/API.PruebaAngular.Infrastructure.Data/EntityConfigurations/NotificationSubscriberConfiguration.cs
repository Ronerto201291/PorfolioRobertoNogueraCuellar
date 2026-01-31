using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaAngular.Domain.AggregateModels;

namespace PruebaAngular.Infrastructure.Data.EntityConfigurations
{
    public class NotificationSubscriberConfiguration : IEntityTypeConfiguration<NotificationSubscriber>
    {
        public void Configure(EntityTypeBuilder<NotificationSubscriber> builder)
        {
            builder.ToTable("NotificationSubscribers");
            builder.HasKey(s => s.SubscriberId);
            builder.Property(s => s.Email).IsRequired().HasMaxLength(256);
            builder.Property(s => s.CreatedAt).IsRequired();
            builder.HasIndex(s => s.Email).IsUnique();
        }
    }
}
