using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BillsPaymentSystem.App.Core.Commands
{
    public class PayBillCommand : Command
    {
        public PayBillCommand(BillsPaymentSystemContext context) : base(context)
        {
        }

        public override string Execute(string[] data)
        {
            int userId = int.Parse(data[0]);
            decimal ammount = decimal.Parse(data[1]);

            User user = Context.Users
               .Include(u => u.PaymentMethods)
                   .ThenInclude(p => p.CreditCard)
                .Include(u => u.PaymentMethods)
                   .ThenInclude(p => p.BankAccount)
               .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return "There is no such user in the database!";
            }

            List<BankAccount> bankAccounts = user.PaymentMethods
                .Where(p => p.BankAccount != null)
                .Select(p => p.BankAccount)
                .OrderBy(p => p.BankAccountId)
                .ToList();

            List<CreditCard> creditCards = user.PaymentMethods
                .Where(p => p.CreditCard != null)
                .Select(p => p.CreditCard)
                .OrderBy(p => p.CreditCardId)
                .ToList();

            decimal allAvailableMoney = bankAccounts.Sum(b => b.Balance) + creditCards.Sum(c => c.LimitLeft);
            if (allAvailableMoney < ammount)
            {
                return "You  have not enough money!";
            }

            string successMessage = $"{ammount} money was withdrawed successfully!";

            foreach (var bankAccount in bankAccounts)
            {
                if(bankAccount.Balance < ammount)
                {
                    ammount -= bankAccount.Balance;
                    bankAccount.Withdraw(bankAccount.Balance);
                }
                else
                {
                    bankAccount.Withdraw(ammount);
                    ammount = 0;
                    break;
                }
            }

            if(ammount <= 0)
            {
                Context.SaveChanges();
                return successMessage;
            }

            foreach (var creditCard in creditCards)
            {
                if (creditCard.LimitLeft < ammount)
                {
                    ammount -= creditCard.LimitLeft;
                    creditCard.Withdraw(creditCard.LimitLeft);
                }
                else
                {
                    creditCard.Withdraw(ammount);
                    break;
                }
            }

            Context.SaveChanges();
            return successMessage;
        }
    }
}
