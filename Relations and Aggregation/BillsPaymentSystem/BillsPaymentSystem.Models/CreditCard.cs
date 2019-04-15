using System;
using System.ComponentModel.DataAnnotations;
using BillsPaymentSystem.Models.Attributes;

namespace BillsPaymentSystem.Models
{
    public class CreditCard
    {
        public int CreditCardId { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Limit { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal MoneyOwed { get; set; }

        public decimal LimitLeft => this.Limit - this.MoneyOwed;

        [ExpirationDate]
        public DateTime ExpirationDate { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public void Deposit(decimal ammount)
        {
            if (ammount > 0)
            {
                this.MoneyOwed -= ammount;
            }
        }

        public void Withdraw(decimal ammount)
        {
            if (this.LimitLeft - ammount >= 0)
            {
                this.MoneyOwed += ammount;
            }
        }
    }
}
