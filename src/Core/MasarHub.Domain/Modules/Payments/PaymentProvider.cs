namespace MasarHub.Domain.Modules.Payments
{
    public enum PaymentProvider
    {
        Mock, // for test
        Stripe,
        Paymob,
        VodafoneCash,
    }
}
