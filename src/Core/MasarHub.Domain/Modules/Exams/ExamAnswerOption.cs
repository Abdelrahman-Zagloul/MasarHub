using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Exams
{
    public sealed class ExamAnswerOption
    {
        public Guid ExamAnswerId { get; private set; }
        public Guid OptionId { get; private set; }

        private ExamAnswerOption() { }

        private ExamAnswerOption(Guid examAnswerId, Guid optionId)
        {
            ExamAnswerId = examAnswerId;
            OptionId = optionId;
        }

        public static Result<ExamAnswerOption> Create(Guid examAnswerId, Guid optionId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(examAnswerId, nameof(examAnswerId)),
                Guard.AgainstEmptyGuid(optionId, nameof(optionId))
            );

            if (error is not null)
                return error;

            return new ExamAnswerOption(examAnswerId, optionId);
        }
    }
}
