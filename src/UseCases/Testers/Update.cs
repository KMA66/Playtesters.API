using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record UpdateTesterRequest(
    string AccessKey = null, 
    string Name = null,
    double? TotalHoursPlayed = null
);
public record UpdateTesterResponse(string Name, string AccessKey);

public class UpdateTesterValidator 
    : AbstractValidator<UpdateTesterRequest>
{
    public UpdateTesterValidator()
    {
        RuleFor(t => t.AccessKey)
            .Must(key => string.IsNullOrEmpty(key) || Guid.TryParse(key, out _))
            .WithMessage("AccessKey must be either null, empty (to revoke) or a valid GUID.");

        RuleFor(t => t.Name)
            .NotEmpty()
            .MinimumLength(3)
            .When(t => t.Name is not null);

        RuleFor(t => t.TotalHoursPlayed)
            .GreaterThanOrEqualTo(0)
            .When(t => t.TotalHoursPlayed is not null);
    }
}

public class UpdateTesterUseCase(
    AppDbContext dbContext,
    UpdateTesterValidator validator)
{
    public async Task<Result<UpdateTesterResponse>> ExecuteAsync(string name, UpdateTesterRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var tester = await dbContext
            .Set<Tester>()
            .FirstOrDefaultAsync(t => t.Name == name);

        if (tester is null)
            return Result.NotFound();

        if (request.AccessKey is not null)
            tester.AccessKey = string.IsNullOrEmpty(request.AccessKey) ? null : request.AccessKey;

        if (request.Name is not null)
            tester.Name = request.Name;

        if (request.TotalHoursPlayed is not null)
            tester.TotalHoursPlayed = request.TotalHoursPlayed.Value;

        await dbContext.SaveChangesAsync();

        var response = new UpdateTesterResponse(tester.Name, tester.AccessKey);
        return Result.Success(response);
    }
}
