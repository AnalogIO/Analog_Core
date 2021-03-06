﻿namespace CoffeeCard.MobilePay.ResponseMessage
{
    public sealed class RefundPaymentResponse : IMobilePayApiResponse
    {
        public string TransactionId { get; set; }
        public string OriginalTransactionId { get; set; }
        public double Remainder { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(TransactionId)}: {TransactionId}, {nameof(OriginalTransactionId)}: {OriginalTransactionId}, {nameof(Remainder)}: {Remainder}";
        }
    }
}