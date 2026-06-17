using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Exams
{
    public static class ExamErrors
    {
        public static readonly DomainError InvalidPassingScore = new("exam.invalid_passing_score", "PassingScore");
        public static readonly DomainError InvalidQuestionExamRelation = new("exam.invalid_question_exam_relation", "ExamId");
        public static readonly DomainError MissingQuestions = new("exam.missing_questions", "Questions");
        public static readonly DomainError CannotUnpublishAfterAttempts = new("exam.cannot_unpublish_after_attempts");
        public static readonly DomainError CannotDeleteExamWithSubmissions = new("exam.cannot_delete_has_submission");
        public static readonly DomainError CannotModifyPublishedExam = new("exam.cannot_modify_published_exam");
        public static readonly DomainError AlreadyPublished = new("exam.already_published");
        public static readonly DomainError AlreadyUnpublished = new("exam.already_unpublished");
        public static readonly DomainError NoAnswersSubmitted = new("exam.no_answers_submitted", "Answers");

        public static readonly DomainError QuestionMustHaveOptions = new("exam.question_must_have_options", "Options");
        public static readonly DomainError InvalidQuestionType = new("exam.invalid_question_type", "QuestionType");

        public static readonly DomainError TrueFalseMaxOptions = new("exam.true_false_max_options", "Options");
        public static readonly DomainError TrueFalseMustHaveTwoOptions = new("exam.true_false_must_have_two_options", "Options");
        public static readonly DomainError TrueFalseMustHaveOneCorrect = new("exam.true_false_must_have_one_correct", "Options");

        public static readonly DomainError SingleChoiceMustHaveOneCorrect = new("exam.single_choice_must_have_one_correct", "Options");
        public static readonly DomainError SingleChoiceMustHaveBetween3And6 = new("exam.single_choice_must_have_between_3_and_6", "Options");

        public static readonly DomainError MultipleChoiceMustHaveAtLeastTwoCorrect = new("exam.multiple_choice_must_have_at_least_two_correct", "Options");
        public static readonly DomainError MultipleChoiceMustHaveBetween3And10 = new("exam.multiple_choice_must_have_between_3_and_10", "Options");
        public static DomainError OptionNotFound(Guid optionId) => new("exam.option_not_found", $"Option:{optionId}");
    }
}
