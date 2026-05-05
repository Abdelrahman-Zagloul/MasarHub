using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MasarHub.Infrastructure.ExternalServices
{
    public sealed class TwilloService : ISmsService
    {
        private readonly TwilioSettings _twilio;

        public TwilloService(IOptions<TwilioSettings> twilio)
        {
            _twilio = twilio.Value;
        }

        public async Task<Result> SendSmsAsync(string phoneNumber, string body)
        {
            try
            {
                TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);

                var result = await MessageResource.CreateAsync(

                    body: body,
                    from: new PhoneNumber(_twilio.TwilioPhoneNumber),
                    to: phoneNumber
                );

                if (result.ErrorCode != null)
                    return Result.Failure(Error.BadRequest("sms.send.failed", result.ErrorMessage));

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.BadRequest("sms.send.failed", ex.Message));
            }
        }

    }
}
