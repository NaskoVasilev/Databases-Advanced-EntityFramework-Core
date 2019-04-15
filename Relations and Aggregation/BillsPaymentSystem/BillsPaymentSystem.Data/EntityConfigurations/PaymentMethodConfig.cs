using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillsPaymentSystem.Data.EntityConfigurations
{
    public class PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasOne(p => p.User)
                .WithMany(u => u.PaymentMethods)
                .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.BankAccount)
                .WithOne(b => b.PaymentMethod);

            builder.HasOne(p => p.CreditCard)
                .WithOne(b => b.PaymentMethod);
        }
    }
}
