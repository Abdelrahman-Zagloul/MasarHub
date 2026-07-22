using MasarHub.Domain.Modules.Exams;

namespace MasarHub.Application.Features.Exams.Commands.StartExamAttempt
{
    public sealed record StartExamAttemptResponse
    (
        Guid AttemptId,
        DateTimeOffset StartedAt,
        string ExamTitle,
        int? DurationMinutes,
        decimal MaxScore,
        List<QuestionResponse> Questions
    );

    public sealed record QuestionResponse
    (
        Guid Id,
        string QuestionText,
        decimal QuestionMark,
        QuestionType QuestionType,
        List<ExamOptionResponse> Options
    );

    public sealed record ExamOptionResponse(Guid Id, string Text);
}
