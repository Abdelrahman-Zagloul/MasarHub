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

        public static ExamAnswerOption Create(Guid examAnswerId, Guid optionId)
            => new(examAnswerId, optionId);
    }
}