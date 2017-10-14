using System;
namespace DecentralizedBank.Models
{
    public class TransferRequest
    {
        public string Sender
        {
            get;
            set;
        }

        public string Receiver
        {
            get;
            set;
        }

        public decimal Amount
        {
            get;
            set;
        }
    }
}
