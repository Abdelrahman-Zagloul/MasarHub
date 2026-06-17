using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Exams
{
    public sealed class Question : BaseEntity
    {
        private readonly List<Option> _options = new();
        public sealed record OptionInput(string Text, bool IsCorrect);
        public sealed record OptionUpdateInput(Guid OptionId, string? Text, bool? IsCorrect);

        public Guid ExamId { get; private set; }
        public string QuestionText { get; private set; } = null!;
        public decimal QuestionMark { get; private set; }
        public QuestionType QuestionType { get; private set; }
        public IReadOnlyCollection<Option> Options => _options.AsReadOnly();

        private Question() { }

        private Question(Guid examId, string questionText, decimal questionMark, QuestionType questionType)
        {
            ExamId = examId;
            QuestionText = questionText;
            QuestionMark = questionMark;
            QuestionType = questionType;
        }

        public static DomainResult<Question> Create(
            Guid examId,
            string questionText,
            decimal questionMark,
            QuestionType questionType,
            IEnumerable<OptionInput> options)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(examId, nameof(examId)),
                Guard.AgainstNullOrWhiteSpace(questionText, nameof(questionText)),
                Guard.AgainstNegativeOrZero(questionMark, nameof(questionMark)),
                Guard.AgainstEnumOutOfRange(questionType, nameof(questionType)),
                Guard.AgainstNull(options, nameof(options))
            );

            if (error is not null)
                return error;

            var question = new Question(examId, questionText, questionMark, questionType);
            var setOptionsResult = question.InitializeOptions(options);

            return setOptionsResult.IsFailure
                ? setOptionsResult.Error
                : question;
        }
        public DomainResult UpdateQuestionText(string questionText)
        {
            var error = Guard.AgainstNullOrWhiteSpace(questionText, nameof(questionText));
            if (error != DomainError.None)
                return error;

            QuestionText = questionText;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateQuestionMark(decimal questionMark)
        {
            var error = Guard.AgainstNegativeOrZero(questionMark, nameof(questionMark));
            if (error != DomainError.None)
                return error;

            QuestionMark = questionMark;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateOptions(IEnumerable<OptionUpdateInput> optionInputs)
        {
            var inputs = optionInputs.ToList();
            if (inputs.Count == 0)
                return ExamErrors.QuestionMustHaveOptions;

            var optionsMap = _options.ToDictionary(o => o.Id);
            foreach (var input in inputs)
            {
                if (!optionsMap.TryGetValue(input.OptionId, out var option))
                    return ExamErrors.OptionNotFound(input.OptionId);

                if (input.Text != null)
                {
                    var updatedResult = option.UpdateOptionText(input.Text);
                    if (updatedResult.IsFailure)
                        return updatedResult;
                }

                if (input.IsCorrect.HasValue)
                    option.SetIsCorrect(input.IsCorrect.Value);
            }

            var result = EnsureValidQuestion();
            if (result.IsFailure)
                return result;

            MarkAsUpdated();
            return DomainResult.Success();
        }
        private DomainResult InitializeOptions(IEnumerable<OptionInput> options)
        {
            var inputs = options.ToList();
            if (inputs.Count == 0)
                return ExamErrors.QuestionMustHaveOptions;

            foreach (var input in inputs)
            {
                var optionResult = Option.Create(Id, input.Text, input.IsCorrect);
                if (optionResult.IsFailure)
                    return optionResult.Error;

                _options.Add(optionResult.Value);
            }

            return EnsureValidQuestion();
        }
        private DomainResult EnsureValidQuestion()
        {
            var optionsCount = _options.Count;
            var correctCount = _options.Count(o => o.IsCorrect);

            var uniqueOptions = new HashSet<string>(_options.Select(o => o.Text.Trim()), StringComparer.OrdinalIgnoreCase);
            if (uniqueOptions.Count != optionsCount)
                return ExamErrors.DuplicateOptionText;


            return QuestionType switch
            {
                QuestionType.TrueFalse when optionsCount != 2 => ExamErrors.TrueFalseMustHaveTwoOptions,
                QuestionType.TrueFalse when correctCount != 1 => ExamErrors.TrueFalseMustHaveOneCorrect,

                QuestionType.SingleChoice when optionsCount is < 3 or > 6 => ExamErrors.SingleChoiceMustHaveBetween3And6,
                QuestionType.SingleChoice when correctCount != 1 => ExamErrors.SingleChoiceMustHaveOneCorrect,

                QuestionType.MultipleChoice when optionsCount is < 3 or > 10 => ExamErrors.MultipleChoiceMustHaveBetween3And10,
                QuestionType.MultipleChoice when correctCount < 2 => ExamErrors.MultipleChoiceMustHaveAtLeastTwoCorrect,

                _ => DomainResult.Success()
            };
        }
    }
}
