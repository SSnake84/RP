using RapidPay.Data.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RapidPay.Data.Repo
{
    public interface ICreditCardsRepo
    {
        Task<bool> AddCreditCardAsync(CreditCard card);
        Task<decimal> GetCreditCardBalanceAsync(string cardNumber);
        Task<bool> PayAsync(string cardNumber, decimal amount);

        Task<bool> DoesExistCreditCardAsync(string cardNumber);
    }

    public class CreditCardsRepo : ICreditCardsRepo
    {
        public async Task<bool> AddCreditCardAsync(CreditCard card)
        {
            using (var context = new RapidPayContext())
            {
                await context.AddAsync(card);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<decimal> GetCreditCardBalanceAsync(string cardNumber)
        {
            using (var context = new RapidPayContext())
            {
                return await context.CreditCards.Where(c => c.CardNumber == cardNumber).Select(c => c.Balance).SingleOrDefaultAsync();
            }
        }

        public async Task<bool> PayAsync(string cardNumber, decimal amount)
        {
            using (var context = new RapidPayContext())
            {
                var card = await context.CreditCards.Where(c => c.CardNumber == cardNumber).SingleOrDefaultAsync();
                card.Balance += amount;
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DoesExistCreditCardAsync(string cardNumber)
        {
            using (var context = new RapidPayContext())
            {
                return await context.CreditCards.AnyAsync(c => c.CardNumber == cardNumber);
            }
        }
    }
}
