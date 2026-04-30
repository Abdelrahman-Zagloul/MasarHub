using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

namespace MasarHub.Domain.Modules.Exams
{
    public sealed class Question : BaseEntity
    {
        private readonly List<Option> _options = new();
        public sealed record OptionInput(string Text, bool IsCorrect); // for input validation and creation
        public Guid ExamId { get; private set; }
        public string QuestionText { get; private set; } = null!;
        public decimal QuestionMark { get; private set; }
        public QuestionType QuestionType { get; private set; }

        public IReadOnlyCollection<Option> Options => _options.AsReadOnly();

        private Question() { }

        private Question(Guid examId, string questionText, decimal questionMark, QuestionType questionType)
        {
            ExamId = Guard.AgainstEmptyGuid(examId, nameof(examId));
            QuestionText = Guard.AgainstNullOrWhiteSpace(questionText, nameof(questionText));
            QuestionMark = Guard.AgainstNegativeOrZero(questionMark, nameof(questionMark));
            QuestionType = Guard.AgainstEnumOutOfRange(questionType, nameof(questionType));
        }

        public static Question Create(
            Guid examId,
            string questionText,
            decimal questionMark,
            QuestionType questionType,
            IEnumerable<OptionInput> options)
        {
            var question = new Question(examId, questionText, questionMark, questionType);

            question.SetOptions(options);
            return question;
        }

        private void SetOptions(IEnumerable<OptionInput> options)
        {
            if (options is null || !options.Any())
                throw new DomainException(ErrorCodes.Exam.QuestionMustHaveOptions);

            _options.Clear();
            foreach (var opttion in options)
            {
                var option = Option.Create(Id, opttion.Text, opttion.IsCorrect);
                ValidateOptionRules(option);
                _options.Add(option);
            }

            EnsureValid();
        }

        private void ValidateOptionRules(Option option)
        {
            switch (QuestionType)
            {
                case QuestionType.TrueFalse:
                    if (_options.Count >= 2)
                        throw new DomainException(ErrorCodes.Exam.TrueFalseMaxOptions);

                    if (option.IsCorrect && HasCorrectOption())
                        throw new DomainException(ErrorCodes.Exam.MultipleCorrectOptionsNotAllowed);
                    break;

                case QuestionType.SingleChoice:
                    if (option.IsCorrect && HasCorrectOption())
                        throw new DomainException(ErrorCodes.Exam.MultipleCorrectOptionsNotAllowed);
                    break;

                case QuestionType.MultipleChoice:
                    break;

                default:
                    throw new DomainException(ErrorCodes.Exam.InvalidQuestionType);
            }
        }

        private void EnsureValid()
        {
            if (!_options.Any())
                throw new DomainException(ErrorCodes.Exam.QuestionMustHaveOptions);

            switch (QuestionType)
            {
                case QuestionType.TrueFalse:
                    if (_options.Count != 2)
                        throw new DomainException(ErrorCodes.Exam.TrueFalseMustHaveTwoOptions);

                    if (_options.Count(o => o.IsCorrect) != 1)
                        throw new DomainException(ErrorCodes.Exam.TrueFalseMustHaveOneCorrect);
                    break;

                case QuestionType.SingleChoice:
                    if (_options.Count(o => o.IsCorrect) != 1)
                        throw new DomainException(ErrorCodes.Exam.SingleChoiceMustHaveOneCorrect);
                    break;

                case QuestionType.MultipleChoice:
                    if (_options.Count(o => o.IsCorrect) < 2)
                        throw new DomainException(ErrorCodes.Exam.MultipleChoiceMustHaveAtLeastTwoCorrect);
                    break;
            }
        }

        private bool HasCorrectOption() => _options.Any(o => o.IsCorrect);
    }
}