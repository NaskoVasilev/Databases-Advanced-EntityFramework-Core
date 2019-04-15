using System.ComponentModel.DataAnnotations;

namespace BillsPaymentSystem.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        [Required]
        [MinLength(3), MaxLength(50)]
        public string BankName { get; set; }

        [Required]
        [MinLength(3), MaxLength(20)]
        public string SWIFT { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public void Deposit(decimal ammount)
        {
            if (ammount > 0)
            {
                this.Balance += ammount;
            }
        }

        public void Withdraw(decimal ammount)
        {
            if(this.Balance >= ammount)
            {
                this.Balance -= ammount;
            }
        }
    }
}
