using Library.MessagingContracts.Messages;

namespace WebApi.Payments.Models
{
    public class Payment : Entity
    {
        public Guid OrderId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public PaymentMethod Method { get; set; } = PaymentMethod.Other;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Você pode adicionar um código de transação externo se quiser integrar com gateway
        public string? TransactionCode { get; set; }
    }
}
