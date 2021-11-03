using System;
using System.Threading;
using System.Threading.Tasks;

namespace RapidPay.Services
{
    public interface IUniversalFeesExchangeService 
    {
        Task<decimal> GetFeeAsync();
    }

    public sealed class UniversalFeesExchangeService : IUniversalFeesExchangeService
    {
        private static readonly UniversalFeesExchangeService instance = new UniversalFeesExchangeService();

        private static decimal _fee = 1;
        private const decimal UPDATE_TIME_MINUTES = 60 ;
        private static Timer _timer = new Timer( e => UpdateFee(), null, TimeSpan.Zero, TimeSpan.FromMinutes((double)UPDATE_TIME_MINUTES));
        private static Random _random = new Random();

        static UniversalFeesExchangeService()
        {
            // to start with a different value than 1 during the first hour.
            UpdateFee();
        }

        private static void UpdateFee()
        {
            _fee *= (decimal)_random.NextDouble() * 2;
        }

        private UniversalFeesExchangeService() { }

        public static UniversalFeesExchangeService GetInstance()
        {
            return instance;
        }

        public async Task<decimal> GetFeeAsync()
        {
            return await Task.Run(()=> UniversalFeesExchangeService._fee);
        }
    }
}
