using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using BillsPaymentSystem.Models.Enums;

namespace BillsPaymentSystem.App
{
    public class DbInitializer
    {
        public static void Seed(BillsPaymentSystemContext context)
        {
            SeedUsers(context);
            SeedCreditCards(context);
            SeedBankAccounts(context);
            SeedPaymentMethods(context);
        }

        private static void SeedPaymentMethods(BillsPaymentSystemContext context)
        {
            List<CreditCard> creditCards = context.CreditCards.ToList();
            List<BankAccount> bankAccounts = context.BankAccounts.ToList();
            List<User> users = context.Users.ToList();
            int bankAccountsCounter = 0;
            int creditCardsCounter = 0;

            for (int i = 0; i < 10; i++)
            {
                PaymentMethod payment = new PaymentMethod()
                {
                    Type = (PaymentType)(i % 2),
                    User = users[i % users.Count]
                };

                if (i % 4 == 0)
                {
                    payment.CreditCardId = creditCards[creditCardsCounter].CreditCardId;
                    creditCardsCounter++;
                    payment.BankAccountId = bankAccounts[bankAccountsCounter].BankAccountId;
                    bankAccountsCounter++;
                }
                else if (i % 2 == 0)
                {
                    payment.CreditCardId = creditCards[creditCardsCounter].CreditCardId;
                    creditCardsCounter++;
                }
                else if (i == 5)
                {
                    payment.CreditCardId = null;
                    payment.BankAccountId = null;
                }
                else
                {
                    payment.BankAccountId = bankAccounts[bankAccountsCounter].BankAccountId;
                    bankAccountsCounter++;
                }

                if (IsValid(payment))
                {
                    context.PaymentMethods.Add(payment);
                }
            }

            context.SaveChanges();
        }

        private static void SeedBankAccounts(BillsPaymentSystemContext context)
        {
            for (int i = 0; i < 10; i++)
            {
                BankAccount bankAccount = new BankAccount()
                {
                    BankName = "Bank" + i,
                    Balance = new Random().Next(-1000, 20000),
                    SWIFT = "Swift" + i
                };

                if (i % 6 == 0)
                {
                    bankAccount.BankName = null;
                }
                else if (i % 7 == 0)
                {
                    bankAccount.SWIFT = null;
                }

                if (IsValid(bankAccount))
                {
                    context.BankAccounts.Add(bankAccount);
                }
            }

            context.SaveChanges();
        }

        private static void SeedCreditCards(BillsPaymentSystemContext context)
        {
            List<CreditCard> creditCards = new List<CreditCard>();
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                CreditCard creditCard = new CreditCard()
                {
                    Limit = random.Next(-1000, 20000),
                    MoneyOwed = random.Next(-1000, 10000),
                    ExpirationDate = DateTime.Now.AddDays(random.Next(-10, 60))
                };

                if (IsValid(creditCard))
                {
                    creditCards.Add(creditCard);
                }
            }

            context.AddRange(creditCards);
            context.SaveChanges();
        }

        private static void SeedUsers(BillsPaymentSystemContext context)
        {
            string[] firstNames = new string[]
            {
                "Atanas",
                "Kiro",
                "Gosho",
                "Pesho",
                "Toni",
                "Misho",
                "",
                null
            };

            string[] lastNames = new string[]
            {
                "Atanasov",
                "Kirov",
                "Goshov",
                "Peshov",
                "Toniv",
                "",
                "NAskov",
                null
            };

            string[] emails = new string[]
            {
                "atanas@abv.bg",
                "kiro@abv.bg",
                "gosho@abv.bg",
                "pesho@abv.bg",
                "toni@abv.bg",
                "misho@abv.bg",
                "abv.bg",
                null
            };

            for (int i = 0; i < firstNames.Length; i++)
            {
                User user = new User()
                {
                    FirstName = firstNames[i],
                    LastName = lastNames[i],
                    Email = emails[i],
                    Password = "secret" + i,
                };

                if (IsValid(user))
                {
                    context.Users.Add(user);
                }
            }

            context.SaveChanges();
        }

        private static bool IsValid(object value)
        {
            ValidationContext validationContext = new ValidationContext(value);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(value, validationContext, validationResults, true);
            return isValid;
        }
    }
}
