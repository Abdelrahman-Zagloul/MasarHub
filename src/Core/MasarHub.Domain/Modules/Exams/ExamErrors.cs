using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Exams
{
    public static class ExamErrors
    {
        public static readonly DomainError InvalidPassingScore = new("Exam.InvalidPassingScore", "PassingScore");
        public static readonly DomainError InvalidQuestionExamRelation = new("Exam.InvalidQuestionExamRelation", "ExamId");
        public static readonly DomainError MissingQuestions = new("Exam.MissingQuestions", "Questions");
        public static readonly DomainError CannotUnpublishAfterAttempts = new("Exam.CannotUnpublishAfterAttempts");
        public static readonly DomainError CannotModifyPublishedExam = new("Exam.CannotModifyPublishedExam");
        public static readonly DomainError NoAnswersSubmitted = new("Exam.NoAnswersSubmitted", "Answers");
        public static readonly DomainError QuestionMustHaveOptions = new("Exam.QuestionMustHaveOptions", "Options");
        public static readonly DomainError TrueFalseMaxOptions = new("Exam.TrueFalseMaxOptions", "Options");
        public static readonly DomainError MultipleCorrectOptionsNotAllowed = new("Exam.MultipleCorrectOptionsNotAllowed", "Options");
        public static readonly DomainError InvalidQuestionType = new("Exam.InvalidQuestionType", "QuestionType");
        public static readonly DomainError TrueFalseMustHaveTwoOptions = new("Exam.TrueFalseMustHaveTwoOptions", "Options");
        public static readonly DomainError TrueFalseMustHaveOneCorrect = new("Exam.TrueFalseMustHaveOneCorrect", "Options");
        public static readonly DomainError SingleChoiceMustHaveOneCorrect = new("Exam.SingleChoiceMustHaveOneCorrect", "Options");
        public static readonly DomainError MultipleChoiceMustHaveAtLeastTwoCorrect = new("Exam.MultipleChoiceMustHaveAtLeastTwoCorrect", "Options");
    }
}
