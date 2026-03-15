using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ms_transferencias.Infrastructure.Entities;

namespace ms_transferencias.Infrastructure.Persistence.Configurations;

public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.ToTable("transfers");
        builder.HasKey(t => t.ExternalOperationId);
        builder.Property(t => t.ExternalOperationId).HasColumnName("external_operation_id").ValueGeneratedOnAdd(); ;
        builder.Property(t => t.CustomerId).HasColumnName("customer_id");
        builder.Property(t => t.ServiceProviderId).HasColumnName("service_provider_id");
        builder.Property(t => t.PaymentMethodId).HasColumnName("payment_method_id");
        builder.Property(t => t.Amount).HasColumnName("amount");
        builder.Property(t => t.Status).HasColumnName("status").HasDefaultValue("evaluating");
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").ValueGeneratedOnAdd(); ;
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");
    }
}