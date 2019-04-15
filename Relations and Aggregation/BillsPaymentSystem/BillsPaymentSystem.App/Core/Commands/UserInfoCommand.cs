using System.Collections.Generic;
using System.Text;
using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BillsPaymentSystem.App.Core.Commands
{
    public class UserInfoCommand : Command
    {
        public UserInfoCommand(BillsPaymentSystemContext context) : base(context)
        {
        }

        public override string Execute(string[] data)
        {
            int userId = int.Parse(data[0]);

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

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{user.FirstName} {user.LastName}");
            List<CreditCard> creditCards = new List<CreditCard>();
            List<BankAccount> bankAccounts = new List<BankAccount>();

            foreach (var paymentMethod in user.PaymentMethods)
            {
                if (paymentMethod.CreditCard != null)
                {
                    creditCards.Add(paymentMethod.CreditCard);
                }
                else
                {
                    bankAccounts.Add(paymentMethod.BankAccount);
                }
            }

            sb.AppendLine("Bank Accounts:");
            foreach (var bankAccount in bankAccounts)
            {
                sb.AppendLine($"-- ID: {bankAccount.BankAccountId}");
                sb.AppendLine($"--- Balance: {bankAccount.Balance}");
                sb.AppendLine($"--- Bank: {bankAccount.BankName}");
                sb.AppendLine($"--- SWIFT: {bankAccount.SWIFT}");
            }

            sb.AppendLine("Credit cards:");
            foreach (var creditCard in creditCards)
            {
                sb.AppendLine($"-- ID: {creditCard.CreditCardId}");
                sb.AppendLine($"--- LImit: {creditCard.Limit:F2}");
                sb.AppendLine($"--- MoneyOwed: {creditCard.MoneyOwed:F2}");
                sb.AppendLine($"--- Limit Left: {creditCard.LimitLeft:F2}");
                sb.AppendLine($"--- Expiration Date: {creditCard.ExpirationDate.Year}/{creditCard.ExpirationDate.Month:D2}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
