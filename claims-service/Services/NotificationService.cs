using YCompanyClaimsApi.Data;
using YCompanyClaimsApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
namespace YCompanyClaimsApi.Services
{

public interface INotificationService
{
    Task NotifyAdjustorAsync(int adjustorId, int claimId);
}

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyAdjustorAsync(int adjustorId, int claimId)
    {
        _logger.LogInformation($"Notifying adjustor {adjustorId} about new assessment for claim {claimId}");
        // Implement actual notification logic here (e.g., email, push notification)
        return Task.CompletedTask;
    }
}
}