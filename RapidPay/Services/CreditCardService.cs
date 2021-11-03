using RapidPay.Data.Models;
using System.Threading.Tasks;
using System;
using RapidPay.Data.Repo;

namespace RapidPay.Services
{
    public interface ICreditCardService 
    {
        Task<bool> AddCreditCardAsync(CreditCard card);
        Task<decimal> GetCreditCardBalanceAsync(string cardNumber);
        Task<decimal> PayAsync(string cardNumber, decimal amount);
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
            if (!IsValidCardNumber(cardNumber) || !await _CreditCardRepo.DoesExistCreditCardAsync(cardNumber))
                throw new ManagedException(Messages.WRONG_CARD_NUMBER);

            return await _CreditCardRepo.GetCreditCardBalanceAsync(cardNumber);
        }

        public async Task<bool> AddCreditCardAsync(CreditCard card)
        {
            if (!IsValidCardNumber(card.CardNumber))
                throw new ManagedException(Messages.WRONG_CARD_NUMBER);

            if(card.ExpirationMonth <1 || card.ExpirationMonth > 12)
                throw new ManagedException(Messages.WRONG_EXPIRATION_DATE);

            var today = DateTime.Today;

            if (card.ExpirationYear < today.Year 
                || (card.ExpirationYear == today.Year && card.ExpirationMonth < today.Month ))
                throw new ManagedException(Messages.CREDIT_CARD_EXPIRED);

            if(String.IsNullOrEmpty(card.CardName))
                throw new ManagedException(Messages.WRONG_CARD_NAME);

            if(card.VerificationCode < 0 || card.VerificationCode > 9999)
                throw new ManagedException(Messages.WRONG_CVC);

            return await _CreditCardRepo.AddCreditCardAsync(card);
        }

        public async Task<decimal> PayAsync(string cardNumber, decimal amount)
        {
            if (amount == 0)
                throw new ManagedException(Messages.WRONG_AMOUNT);

            var balance = await GetCreditCardBalanceAsync(cardNumber);
            balance += amount;
            await _CreditCardRepo.UpdateBalanceAsync(cardNumber, balance);
            return balance;
        }
    }
}
