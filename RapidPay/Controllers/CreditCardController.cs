using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RapidPay.Data.Models;
using RapidPay.Handlers;
using RapidPay.Services;
using System.Threading.Tasks;

namespace RapidPay.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CreditCardController : ControllerBase
    {
        private readonly ILogger<CreditCardController> _logger;
        private IUserService _userService;
        private ICreditCardService _creditCardService;

        public CreditCardController(
            ILogger<CreditCardController> logger,
            IUserService userService,
            ICreditCardService creditCardRepo)
        {
            _creditCardService = creditCardRepo;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            var user = await _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = Messages.WRONG_CREDENTIALS });

            return Ok(user);
        }

        // GET api/CreditCard/ {15digits}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{cardNumber:regex([[0-9]]{{15}})}")]
        public async Task<IActionResult> GetBalance(string cardNumber)
        {
            try
            {
                decimal balance = await _creditCardService.GetCreditCardBalanceAsync(cardNumber);
                return Ok(balance);
            }
            catch (ManagedException ex) { return BadRequest(ex.Message); }
            catch 
            {
                return BadRequest(Messages.WRONG_CARD_NUMBER); 
            }
        }

        // POST api/CreditCard
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreditCard card)
        {
            try
            {
                if(await _creditCardService.AddCreditCardAsync(card))
                    return StatusCode(201, Messages.CREDIT_CARD_CREATED);
                else
                    return BadRequest(Messages.CREDIT_CARD_NUMBER_ALREADY_EXISTS);
            }
            catch (ManagedException ex) { return BadRequest(ex.Message); }
            catch 
            { 
                return BadRequest(Messages.WRONG_DATA); 
            }
        }

        // PUT api/CreditCard/ {15 digits}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{cardNumber:regex([[0-9]]{{15}})}")]
        public async Task<IActionResult> PayAsync([FromRoute] string cardNumber, [FromBody] decimal amount)
        {
            try
            {
                var ok = await _creditCardService.PayAsync(cardNumber, amount);
                if (ok)
                    return Ok();
                return StatusCode(500, Messages.PAYMENT_FAILED);
            }
            catch (ManagedException ex) { return StatusCode(500, ex.Message); }
            catch
            {
                return StatusCode(500, Messages.PAYMENT_FAILED); 
            }
        }
    }
}
