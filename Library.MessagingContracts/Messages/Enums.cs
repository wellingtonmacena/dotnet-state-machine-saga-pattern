namespace Library.MessagingContracts.Messages
{
    
        public enum PaymentStatus
    {
            Pending,
            Completed,
            Failed,
            Cancelled,
        Refunded
    }

        public enum PaymentMethod
        {
            CreditCard,
            DebitCard,
            Pix,
            Boleto,
            Other
        }
    
}
