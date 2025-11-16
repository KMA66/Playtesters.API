using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record UpdateTesterRequest(string ApiKey);
public record UpdateTesterResponse(string UserName, string ApiKey);

public class UpdateTesterValidator 
    : AbstractValidator<UpdateTesterRequest>
{
    public UpdateTesterValidator()
    {
        RuleFor(t => t.ApiKey)
            .Must(key => string.IsNullOrEmpty(key) || Guid.TryParse(key, out _))
            .WithMessage("API Key must be a valid GUID.");
    }
}

public class UpdateTesterUseCase(
    AppDbContext dbContext,
    UpdateTesterValidator validator)
{
    public async Task<Result<UpdateTesterResponse>> ExecuteAsync(string userName, UpdateTesterRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var tester = await dbContext
            .Set<Tester>()
            .FirstOrDefaultAsync(t => t.UserName == userName);

        if (tester is null)
            return Result.NotFound();

        tester.ApiKey = request.ApiKey;
        await dbContext.SaveChangesAsync();

        var response = new UpdateTesterResponse(tester.UserName, tester.ApiKey);
        return Result.Success(response);
    }
}
