using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MasarHub.Infrastructure.ExternalServices
{
    public sealed class TwilioService : ISmsService
    {
        private readonly TwilioSettings _twilioSettings;
        private readonly ILogger<TwilioService> _logger;
        public TwilioService(IOptions<TwilioSettings> twilioSettings, ILogger<TwilioService> logger)
        {
            _twilioSettings = twilioSettings.Value;
            _logger = logger;
        }

        public async Task<Result> SendSmsAsync(string phoneNumber, string body)
        {
            try
            {
                TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);

                var result = await MessageResource.CreateAsync(

                    body: body,
                    from: new PhoneNumber(_twilioSettings.TwilioPhoneNumber),
                    to: phoneNumber
                );

                if (result.ErrorCode != null)
                {
                    _logger.LogWarning("Failed to send SMS message: {ErrorMessage}", result.ErrorMessage);
                    return Result.Failure(Error.BadRequest("sms.send.failed"));
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS");
                return Result.Failure(Error.BadRequest("sms.send.failed"));
            }
        }

    }
}
