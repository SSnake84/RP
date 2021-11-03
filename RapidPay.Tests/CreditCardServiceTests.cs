using Moq;
using RapidPay.Data.Repo;
using RapidPay.Services;
using Xunit;

namespace RapidPay.Tests
{
    public class CreditCardServiceTests
    {

        #region IsValidCardNumber
        [Fact]
        public void IsValidCardNumber_null_ShouldbeFalse()
        {
            ICreditCardService svc = new CreditCardService();
            var result = svc.IsValidCardNumber(null);

            Assert.False(result);
        }

        [Fact]
        public void IsValidCardNumber_no15length_ShouldbeFalse()
        {
            ICreditCardService svc = new Services.CreditCardService();
            var result = svc.IsValidCardNumber("12345");

            Assert.False(result);
        }

        [Fact]
        public void IsValidCardNumber_15length_ShouldbeTrue()
        {
            ICreditCardService svc = new Services.CreditCardService();
            var result = svc.IsValidCardNumber("123456789012345");

            Assert.True(result);
        }
        #endregion IsValidCardNumber

        #region GetCreditCardBalance
        [Fact]
        public void GetCreditCardBalance_null_ShouldThrowWrongCardNumberException()
        {
            ICreditCardService svc = new Services.CreditCardService();

            Assert.ThrowsAsync<ManagedException>(async () => await svc.GetCreditCardBalanceAsync(null));
        }
        [Fact]
        public async void GetCreditCardBalance_nonExistingNumber_ShouldThrowWrongCardNumberException()
        {
            try
            {
                ICreditCardService svc = new Services.CreditCardService();
                await svc.GetCreditCardBalanceAsync("123456789012345");
            }
            catch (ManagedException ex)
            {
                Assert.Equal(Messages.WRONG_CARD_NUMBER, ex.Message);
            }
            catch 
            {
                // ASsert Fail
                Assert.True(false, "Message");
            }
        }

        [Fact]
        public async void GetCreditCardBalance_100_ShouldReturn100()
        {
            var cardnumber = "123456789012345";
            var expected = 100;

            var repo = new Mock<ICreditCardsRepo>();
            repo.Setup(m => m.DoesExistCreditCardAsync(cardnumber)).ReturnsAsync(true);
            repo.Setup(m => m.GetCreditCardBalanceAsync(cardnumber)).ReturnsAsync(expected);
            ICreditCardService svc = new Services.CreditCardService(repo.Object);
            var actual = await svc.GetCreditCardBalanceAsync(cardnumber);

            Assert.Equal(expected, actual);
        }
        #endregion GetCreditCardBalance

        #region Pay
        [Fact]
        public async void PayAsync_0_ShouldThrowWrongAmountException()
        {
            string cardnumber = "123456789012345";
            var amount = 0;
            try
            {
                ICreditCardService svc = new Services.CreditCardService();
                var actual = await svc.PayAsync(cardnumber, amount);
            }
            catch (ManagedException ex) { Assert.Equal(Messages.WRONG_AMOUNT, ex.Message); }
            catch { Assert.True(false); }
        }
        [Fact]
        public async void PayAsync_null_ShouldThrowWrongCardNumberException()
        {
            string cardnumber = null;
            var amount= 100;
            try
            {
                ICreditCardService svc = new Services.CreditCardService();
                var actual = await svc.PayAsync(cardnumber, amount);
            }
            catch (ManagedException ex) { Assert.Equal(Messages.WRONG_CARD_NUMBER, ex.Message); }
            catch { Assert.True(false); }
        }
        [Fact]
        public async void PayAsync_nonExistingCardNumber_ShouldThrowWrongCardNumberException()
        {
            string cardnumber = "123456789012345";
            var amount = 100;

            var repo = new Mock<ICreditCardsRepo>();
            repo.Setup(m => m.DoesExistCreditCardAsync(cardnumber)).ReturnsAsync(false);
            try
            {
                ICreditCardService svc = new Services.CreditCardService(repo.Object);
                var actual = await svc.PayAsync(cardnumber, amount);
            }
            catch (ManagedException ex) { Assert.Equal(Messages.WRONG_CARD_NUMBER, ex.Message); }
            catch { Assert.True(false); }
        }
        [Fact]
        public async void PayAsync_100_ShouldReturnTrue()
        {
            var cardnumber = "123456789012345";
            var amount = 100;

            var repo = new Mock<ICreditCardsRepo>();
            repo.Setup(m => m.DoesExistCreditCardAsync(cardnumber)).ReturnsAsync(true);
            repo.Setup(m => m.UpdateBalanceAsync(cardnumber, amount)).ReturnsAsync(true);
            ICreditCardService svc = new Services.CreditCardService(repo.Object);
            var actual = await svc.PayAsync(cardnumber, amount);
            Assert.Equal(100, actual);
        }
        #endregion GetCreditCardBalance
    }
}
