using System;
using System.Collections.Generic;

#nullable disable

namespace RapidPay.Data.Models
{
    public partial class CreditCard
    {
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public int UserId { get; set; }
        public short ExpirationMonth { get; set; }
        public short ExpirationYear { get; set; }
        public short VerficationCode { get; set; }
    }
}
