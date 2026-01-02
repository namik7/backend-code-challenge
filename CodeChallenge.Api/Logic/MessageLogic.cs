using CodeChallenge.Api.Models;
using CodeChallenge.Api.Repositories;

namespace CodeChallenge.Api.Logic;

public class MessageLogic : IMessageLogic
{
    private readonly IMessageRepository _repository;

    public MessageLogic(IMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> GetAllMessagesAsync(Guid organizationId)
    {
        if (organizationId == Guid.Empty)
        {
            return Validation("organizationId", "OrganizationId is required");
        }

        var messages = await _repository.GetAllByOrganizationAsync(organizationId);
        return new Created<IEnumerable<Message>>(messages);
    }

    public async Task<Result> GetMessageAsync(Guid organizationId, Guid id)
    {
        if (organizationId == Guid.Empty || id == Guid.Empty)
        {
            return Validation("id", "OrganizationId and MessageId are required");
        }

        var message = await _repository.GetByIdAsync(organizationId, id);
        if (message == null)
        {
            return new NotFound("Message not found");
        }

        return new Created<Message>(message);
    }

    public async Task<Result> CreateMessageAsync(Guid organizationId, CreateMessageRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (organizationId == Guid.Empty)
            errors["organizationId"] = new[] { "OrganizationId is required" };

        if (string.IsNullOrWhiteSpace(request.Title) ||
            request.Title.Length < 3 || request.Title.Length > 200)
            errors["title"] = new[] { "Title must be between 3 and 200 characters" };

        if (string.IsNullOrWhiteSpace(request.Content) ||
            request.Content.Length < 10 || request.Content.Length > 1000)
            errors["content"] = new[] { "Content must be between 10 and 1000 characters" };

        if (errors.Any())
            return new ValidationError(errors);

        var existing = await _repository.GetByTitleAsync(organizationId, request.Title);
        if (existing != null)
        {
            return new Conflict("Title must be unique per organization");
        }

        var message = new Message
        {
            OrganizationId = organizationId,
            Title = request.Title,
            Content = request.Content,
            IsActive = true
        };

        var created = await _repository.CreateAsync(message);
        return new Created<Message>(created);
    }

    public async Task<Result> UpdateMessageAsync(Guid organizationId, Guid id, UpdateMessageRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        //Validate identifiers
        if (organizationId == Guid.Empty)
            errors["organizationId"] = new[] { "OrganizationId is required" };

        if (id == Guid.Empty)
            errors["id"] = new[] { "MessageId is required" };

        //Validate Title
        if (string.IsNullOrWhiteSpace(request.Title) ||
            request.Title.Length < 3 || request.Title.Length > 200)
        {
            errors["title"] = new[] { "Title must be between 3 and 200 characters" };
        }

        //Validate Content
        if (string.IsNullOrWhiteSpace(request.Content) ||
            request.Content.Length < 10 || request.Content.Length > 1000)
        {
            errors["content"] = new[] { "Content must be between 10 and 1000 characters" };
        }

        if (errors.Any())
            return new ValidationError(errors);

        //Fetch existing message
        var message = await _repository.GetByIdAsync(organizationId, id);
        if (message == null)
            return new NotFound("Message not found");

        //only active messages can be updated
        if (!message.IsActive)
            return Validation("isActive", "Inactive messages cannot be updated");

        //title must be unique per organization
        var existingWithSameTitle =
            await _repository.GetByTitleAsync(organizationId, request.Title);

        if (existingWithSameTitle != null &&
            existingWithSameTitle.Id != id)
        {
            return new Conflict("Title must be unique per organization");
        }

        //Applying updates if all of the above passes
        message.Title = request.Title;
        message.Content = request.Content;
        message.IsActive = request.IsActive;
        message.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(message);
        return new Updated();
    }

    public async Task<Result> DeleteMessageAsync(Guid organizationId, Guid id)
    {
        var message = await _repository.GetByIdAsync(organizationId, id);
        if (message == null)
            return new NotFound("Message not found");

        if (!message.IsActive)
            return Validation("isActive", "Inactive messages cannot be deleted");

        await _repository.DeleteAsync(organizationId, id);
        return new Deleted();
    }

    private static ValidationError Validation(string key, string message)
        => new(new Dictionary<string, string[]> { { key, new[] { message } } });
}
