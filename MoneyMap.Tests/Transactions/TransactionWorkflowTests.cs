using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MoneyMap.Api.DTOs.Categories;
using MoneyMap.Api.DTOs.Reports;
using MoneyMap.Api.DTOs.Transactions;
using MoneyMap.Api.Models;
using MoneyMap.Tests.Infrastructure;

namespace MoneyMap.Tests.Transactions;

public sealed class TransactionWorkflowTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task Categories_Transactions_And_Monthly_Summary_Work_EndToEnd()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();
        await TestAuthHelper.AuthenticateAsync(client, "owner@example.com");

        var salaryCategory = await CreateCategoryAsync(client, "Salary", CategoryType.Income);
        var groceriesCategory = await CreateCategoryAsync(client, "Groceries", CategoryType.Expense);

        var incomeResponse = await client.PostAsJsonAsync("/api/transactions", new
        {
            categoryId = salaryCategory.Id,
            amount = 4000.00m,
            description = "March salary",
            occurredOn = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        var expenseResponse = await client.PostAsJsonAsync("/api/transactions", new
        {
            categoryId = groceriesCategory.Id,
            amount = 250.75m,
            description = "Weekly groceries",
            occurredOn = new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Utc)
        });

        Assert.Equal(HttpStatusCode.Created, incomeResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, expenseResponse.StatusCode);

        var transactionsResponse = await client.GetAsync("/api/transactions?year=2026&month=3");
        transactionsResponse.EnsureSuccessStatusCode();

        var transactions = await transactionsResponse.Content.ReadFromJsonAsync<List<TransactionResponseDto>>(JsonOptions);
        Assert.NotNull(transactions);
        Assert.Equal(2, transactions!.Count);

        var summaryResponse = await client.GetAsync("/api/reports/monthly-summary?year=2026&month=3");
        summaryResponse.EnsureSuccessStatusCode();

        var summary = await summaryResponse.Content.ReadFromJsonAsync<MonthlySummaryDto>(JsonOptions);
        Assert.NotNull(summary);
        Assert.Equal(4000.00m, summary!.TotalIncome);
        Assert.Equal(250.75m, summary.TotalExpenses);
        Assert.Equal(3749.25m, summary.Net);
        Assert.Equal(2, summary.TransactionCount);
        Assert.Equal(2, summary.Breakdown.Count);
    }

    [Fact]
    public async Task Users_Only_See_And_Modify_Their_Own_Transactions()
    {
        await factory.ResetDatabaseAsync();

        using var ownerClient = factory.CreateClient();
        await TestAuthHelper.AuthenticateAsync(ownerClient, "owner@example.com");
        var ownerCategory = await CreateCategoryAsync(ownerClient, "Travel", CategoryType.Expense);

        var createResponse = await ownerClient.PostAsJsonAsync("/api/transactions", new
        {
            categoryId = ownerCategory.Id,
            amount = 120.00m,
            description = "Taxi",
            occurredOn = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc)
        });

        var ownerTransaction = await createResponse.Content.ReadFromJsonAsync<TransactionResponseDto>(JsonOptions);
        Assert.NotNull(ownerTransaction);

        using var secondUserClient = factory.CreateClient();
        await TestAuthHelper.AuthenticateAsync(secondUserClient, "second@example.com");
        var secondCategory = await CreateCategoryAsync(secondUserClient, "Food", CategoryType.Expense);

        var updateResponse = await secondUserClient.PutAsJsonAsync($"/api/transactions/{ownerTransaction!.Id}", new
        {
            categoryId = secondCategory.Id,
            amount = 50.00m,
            description = "Attempted update",
            occurredOn = new DateTime(2026, 3, 11, 0, 0, 0, DateTimeKind.Utc)
        });

        var deleteResponse = await secondUserClient.DeleteAsync($"/api/transactions/{ownerTransaction.Id}");
        var listResponse = await secondUserClient.GetAsync("/api/transactions?year=2026&month=3");
        var secondUserTransactions = await listResponse.Content.ReadFromJsonAsync<List<TransactionResponseDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        Assert.NotNull(secondUserTransactions);
        Assert.Empty(secondUserTransactions!);
    }

    private static async Task<CategoryResponseDto> CreateCategoryAsync(HttpClient client, string name, CategoryType type)
    {
        var response = await client.PostAsJsonAsync("/api/categories", new
        {
            name,
            type
        });

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryResponseDto>(JsonOptions)
            ?? throw new InvalidOperationException("Category response was empty.");
    }
}
