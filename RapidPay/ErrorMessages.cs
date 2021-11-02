
namespace RapidPay
{
    public class Messages
    {
        public const string WRONG_DATA = "Some of the data was wrong"; // ideally no one should use this SO generic message. but it is useful to move on.
        public const string WRONG_CARD_NUMBER = "Wrong Card number";
        public const string WRONG_CREDENTIALS = "Username or password is incorrect";
        public const string CREDIT_CARD_NUMBER_ALREADY_EXISTS = "Credit Card number already exists";
        public const string PAYMENT_FAILED = "There was an error trying to pay";

        public const string CREDIT_CARD_CREATED = "Credit Card has been created succesfully";
    }
}
