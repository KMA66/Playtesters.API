using DotEnv.Core;
using Microsoft.Data.Sqlite;
using Playtesters.API.Data;

namespace Playtesters.API.Tests.Common;

public partial class TestBase
{
    private CustomWebApplicationFactory _webApplicationFactory;
    protected CustomWebApplicationFactory ApplicationFactory => _webApplicationFactory;

    /// <summary>
    /// Initializes the web application before starting tests for a class.
    /// </summary>
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
        => _webApplicationFactory = new CustomWebApplicationFactory();

    /// <summary>
    /// Frees resources upon completion of all tests in a class.
    /// </summary>
    [OneTimeTearDown]
    public void RunAfterAnyTests()
        => _webApplicationFactory.Dispose();

    /// <summary>
    /// Creates the database before starting a test.
    /// </summary>
    /// <remarks>The database is only created if it does not exist.</remarks>
    [SetUp]
    public void Init()
    {
        using var serviceScope = _webApplicationFactory.Services.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
        context.Database.EnsureCreated();
    }

    /// <summary>
    /// Deletes the data from each table after the test is completed.
    /// </summary>
    [TearDown]
    public async Task CleanUp()
    {
        var envReader = new EnvReader();
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = envReader["SQLITE_DATA_SOURCE"]
        };
        using var connection = new SqliteConnection(builder.ConnectionString);
        await connection.OpenAsync();
        var sql =
        """
        DELETE FROM AccessValidationHistory;
        DELETE FROM IpGeoCache;
        DELETE FROM Tester;
        DELETE FROM sqlite_sequence WHERE name IN ('Tester','AccessValidationHistory','IpGeoCache');
        """;

        using var transaction = connection.BeginTransaction();
        using var command = new SqliteCommand(sql, connection, transaction);
        await command.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
    }

    protected HttpClient CreateHttpClientWithApiKey()
    {
        var httpClient = _webApplicationFactory.CreateClient();
        httpClient
            .DefaultRequestHeaders
            .Add("X-Api-Key", "Test123");
        return httpClient;
    }
}
