using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.TesterAccessHistory;

public record GetAllTestersAccessHistoryRequest(
    string Name,
    string IpAddress,
    int PageNumber = 1,
    int PageSize = 20
);

public class GetAllTestersAccessHistoryResponse
{
    public required string Name { get; init; }
    public required string CheckedAt { get; init; }
    public required string IpAddress { get; init; }
}

public class GetAllTestersAccessHistoryValidator 
    : AbstractValidator<GetAllTestersAccessHistoryRequest>
{
    public GetAllTestersAccessHistoryValidator()
    {
        RuleFor(t => t.PageNumber).GreaterThan(0);
        RuleFor(t => t.PageSize).InclusiveBetween(10, 100);
    }
}

public class GetAllTestersAccessHistoryUseCase(
    AppDbContext dbContext,
    GetAllTestersAccessHistoryValidator validator)
{
    public async Task<PagedResult<GetAllTestersAccessHistoryResponse>> ExecuteAsync(
        GetAllTestersAccessHistoryRequest request)
    {
        var validationResult = validator.Validate(request);
        if (validationResult.IsFailed())
            return validationResult.Invalid();

        var query = dbContext
            .Set<AccessValidationHistory>()
            .Include(h => h.Tester)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(h => h.Tester.Name == request.Name);

        if (!string.IsNullOrWhiteSpace(request.IpAddress))
            query = query.Where(h => h.IpAddress == request.IpAddress);

        int itemsToSkip = (request.PageNumber - 1) * request.PageSize;
        var testers = await query
            .OrderByDescending(h => h.CheckedAt)
            .Select(h => new GetAllTestersAccessHistoryResponse
            {
                Name = h.Tester.Name,
                CheckedAt = h.CheckedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                IpAddress = h.IpAddress,
            })
            .Skip(itemsToSkip)
            .Take(request.PageSize) 
            .ToListAsync();

        int totalRecords = await query.CountAsync();
        var pagedInfo = new PagedInfo(
            request.PageNumber, 
            request.PageSize,
            totalRecords
        );
        return Result.Success(testers, pagedInfo);
    }
}
