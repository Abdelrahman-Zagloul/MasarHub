using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Exams
{
    public sealed class Question : BaseEntity
    {
        private readonly List<Option> _options = new();

        public sealed record OptionInput(string Text, bool IsCorrect);

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

        public static Result<Question> Create(
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
            var setOptionsResult = question.SetOptions(options);

            return setOptionsResult.IsFailure
                ? setOptionsResult.Error
                : question;
        }

        private Result SetOptions(IEnumerable<OptionInput> options)
        {
            var optionInputs = options.ToList();
            if (!optionInputs.Any())
                return ExamErrors.QuestionMustHaveOptions;

            _options.Clear();
            foreach (var optionInput in optionInputs)
            {
                var optionResult = Option.Create(Id, optionInput.Text, optionInput.IsCorrect);
                if (optionResult.IsFailure)
                    return optionResult.Error;

                var ruleResult = ValidateOptionRules(optionResult.Value!);
                if (ruleResult.IsFailure)
                    return ruleResult;

                _options.Add(optionResult.Value!);
            }

            return EnsureValid();
        }

        private Result ValidateOptionRules(Option option)
        {
            switch (QuestionType)
            {
                case QuestionType.TrueFalse:
                    if (_options.Count >= 2)
                        return ExamErrors.TrueFalseMaxOptions;

                    if (option.IsCorrect && HasCorrectOption())
                        return ExamErrors.MultipleCorrectOptionsNotAllowed;

                    break;

                case QuestionType.SingleChoice:
                    if (option.IsCorrect && HasCorrectOption())
                        return ExamErrors.MultipleCorrectOptionsNotAllowed;

                    break;

                case QuestionType.MultipleChoice:
                    break;

                default:
                    return ExamErrors.InvalidQuestionType;
            }

            return Result.Success();
        }

        private Result EnsureValid()
        {
            if (!_options.Any())
                return ExamErrors.QuestionMustHaveOptions;

            switch (QuestionType)
            {
                case QuestionType.TrueFalse:
                    if (_options.Count != 2)
                        return ExamErrors.TrueFalseMustHaveTwoOptions;

                    if (_options.Count(o => o.IsCorrect) != 1)
                        return ExamErrors.TrueFalseMustHaveOneCorrect;

                    break;

                case QuestionType.SingleChoice:
                    if (_options.Count(o => o.IsCorrect) != 1)
                        return ExamErrors.SingleChoiceMustHaveOneCorrect;

                    break;

                case QuestionType.MultipleChoice:
                    if (_options.Count(o => o.IsCorrect) < 2)
                        return ExamErrors.MultipleChoiceMustHaveAtLeastTwoCorrect;

                    break;
            }

            return Result.Success();
        }

        private bool HasCorrectOption() => _options.Any(o => o.IsCorrect);
    }
}
