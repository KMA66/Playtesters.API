using Microsoft.EntityFrameworkCore;
using Playtesters.API.Data;
using Playtesters.API.Entities;
using SimpleResults;

namespace Playtesters.API.UseCases.Testers;

public record RevokeAllKeysResponse(int RevokedCount);

public class RevokeAllKeysUseCase(AppDbContext dbContext)
{
    public async Task<Result<RevokeAllKeysResponse>> ExecuteAsync()
    {
        int affectedRows = await dbContext.Set<Tester>()
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.AccessKey, t => null));

        return Result.Success(new RevokeAllKeysResponse(affectedRows));
    }
}
