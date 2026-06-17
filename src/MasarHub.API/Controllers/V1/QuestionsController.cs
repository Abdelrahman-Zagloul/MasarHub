using Asp.Versioning;
using MasarHub.API.Controllers.Shared;
using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Models;
using MasarHub.Application.Features.Questions.Commands.CreateQuestion;
using MasarHub.Application.Features.Questions.Commands.DeleteQuestion;
using MasarHub.Application.Features.Questions.Commands.UpdateQuestion;
using MasarHub.Application.Features.Questions.Queries.GetQuestionById;
using MasarHub.Domain.Modules.Exams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Controllers.V1
{
    [ApiVersion(1.0)]
    [Tags("Questions")]
    [Route("api/v{version:apiVersion}/exams/{examId:guid}/questions")]
    public sealed class QuestionsController : ApiControllerBase
    {
        private readonly ISender _sender;

        public QuestionsController(ILocalizationService localizationService, ISender sender)
            : base(localizationService)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Create a question")]
        [EndpointDescription("Creates a new question with options for an exam. Instructor only.")]
        public async Task<IActionResult> Create(Guid examId, CreateQuestionRequest request)
        {
            var options = request.Options.Select(o => new Question.OptionInput(o.Text, o.IsCorrect)).ToList();
            var command = new CreateQuestionCommand(examId, GetUserId(), request.QuestionText, request.QuestionMark, request.QuestionType, options);
            var result = await _sender.Send(command);

            return result.IsFailure
                ? await HandleError(result)
                : CreatedAtAction(nameof(GetQuestionById), new { examId = examId, id = result.Value.Id }, result.Value);
        }

        [HttpPut("{questionId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Update a question")]
        [EndpointDescription("Updates the text, mark, and/or options of a question. Instructor only.")]
        public async Task<IActionResult> Update(Guid examId, Guid questionId, UpdateQuestionRequest request)
        {
            var options = request.Options?.Select(o => new Question.OptionUpdateInput(o.OptionId, o.Text, o.IsCorrect)).ToList();
            var command = new UpdateQuestionCommand(examId, questionId, GetUserId(), request.QuestionText, request.QuestionMark, options);
            var result = await _sender.Send(command);

            return await ToNoContentResultAsync(result);
        }

        [HttpDelete("{questionId:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Delete a question")]
        [EndpointDescription("Deletes a question from an exam. Instructor only.")]
        public async Task<IActionResult> Delete(Guid examId, Guid questionId)
        {
            var command = new DeleteQuestionCommand(examId, questionId, GetUserId());
            var result = await _sender.Send(command);

            return await ToNoContentResultAsync(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = Roles.Instructor)]
        [EndpointSummary("Get question by ID")]
        [EndpointDescription("Retrieves a specific question with its options by ID. Instructor only.")]
        public async Task<IActionResult> GetQuestionById(Guid examId, Guid id)
        {
            var result = await _sender.Send(new GetQuestionByIdQuery(examId, id, GetUserId()));
            return await ToOkResultAsync(result);
        }
    }
}
