using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using Playtesters.API.Services;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record ValidateTesterAccessRequest(string AccessKey);
public record ValidateTesterAccessResponse(string Name);

public class ValidateTesterAccessValidator 
    : AbstractValidator<ValidateTesterAccessRequest>
{
    public ValidateTesterAccessValidator()
    {
        RuleFor(t => t.AccessKey)
            .NotEmpty()
            .Must(key => Guid.TryParse(key, out _))
            .WithMessage("Access Key must be a valid GUID.");
    }
}

public class ValidateTesterAccessUseCase(
    AppDbContext dbContext,
    ValidateTesterAccessValidator validator,
    IClientIpProvider clientIpProvider)
{
    public async Task<Result<ValidateTesterAccessResponse>> ExecuteAsync(
        ValidateTesterAccessRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var tester = await dbContext
            .Set<Tester>()
            .FirstOrDefaultAsync(t => t.AccessKey == request.AccessKey);

        if (tester is null)
            return Result.Invalid("Invalid credentials.");

        var history = new AccessValidationHistory
        {
            TesterId = tester.Id,
            IpAddress = clientIpProvider.GetClientIp()
        };
        dbContext.Add(history);
        await dbContext.SaveChangesAsync();

        var response = new ValidateTesterAccessResponse(tester.Name);
        return Result.Success(response);
    }
}
