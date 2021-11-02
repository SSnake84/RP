using RapidPay.Data.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using RapidPay.Data.Repo;

namespace RapidPay.Services
{
    public interface ICreditCardService 
    {
        Task<bool> AddCreditCardAsync(CreditCard card);
        Task<decimal> GetCreditCardBalanceAsync(string cardNumber);
        Task<bool> PayAsync(string cardNumber, decimal amount);
        bool IsValidCardNumber(string cardNumber);
    }

    public class CreditCardService : ICreditCardService
    {
        ICreditCardsRepo _CreditCardRepo = null;

        #region Constructors
        public CreditCardService()
        {
            _CreditCardRepo = new CreditCardsRepo();
        }

        public CreditCardService(ICreditCardsRepo ccRepo)
        {
            _CreditCardRepo = ccRepo;
        }
        #endregion Constructors

        public bool IsValidCardNumber(string cardNumber) 
        {
            return cardNumber != null && cardNumber.Length == 15;
        }

        public async Task<decimal> GetCreditCardBalanceAsync(string cardNumber)
        {
            if (!IsValidCardNumber(cardNumber) || !(await _CreditCardRepo.DoesExistCreditCardAsync(cardNumber)))
                throw new Exception(Messages.WRONG_CARD_NUMBER);

            return await _CreditCardRepo.GetCreditCardBalanceAsync(cardNumber);
        }

        public async Task<bool> AddCreditCardAsync(CreditCard card)
        {
            // Validations

            return await _CreditCardRepo.AddCreditCardAsync(card);
        }

        public async Task<bool> PayAsync(string cardNumber, decimal amount)
        {
            if (!(await _CreditCardRepo.DoesExistCreditCardAsync(cardNumber)))
                throw new Exception(Messages.WRONG_CARD_NUMBER);

            return await _CreditCardRepo.PayAsync(cardNumber, amount);
        }
    }
}
