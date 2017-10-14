using System;
namespace DecentralizedBank.Models
{
    public class AccountBalanceResponse
    {
        public string Account
        {
            get;
            set;
        }

        public decimal Balance
        {
            get;
            set;
        }
    }
}
