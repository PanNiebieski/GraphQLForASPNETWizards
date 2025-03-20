using _08GraphQLSubscriptions.Subs.Models;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace _08GraphQLSubscriptions.Subs;

public class BackgroundServiceDocumentTaskPublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITopicEventSender _eventSender;
    private readonly Random _random = new Random();

    private readonly string[] _taskTypes = new[]
    {
        "Review",
        "Analyze",
        "Approve",
        "Reject",
        "Archive",
        "Share",
        "Update",
        "Translate"
    };

    public BackgroundServiceDocumentTaskPublisher(IServiceScopeFactory scopeFactory, ITopicEventSender eventSender)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a new scope for each operation
                using (var scope = _scopeFactory.CreateScope())
                {
                    // Get the dbContext from the scope
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Get document IDs from the database
                    var documentIds = await dbContext.Documents
                        .Select(d => d.Id)
                        .ToListAsync(stoppingToken);

                    if (documentIds.Any())
                    {
                        // Choose a random document ID
                        int randomDocumentId = documentIds[_random.Next(documentIds.Count)];

                        // Create a random task
                        string randomTask = _taskTypes[_random.Next(_taskTypes.Length)];

                        // Create a new document task
                        var documentTask = new DocumentTask(randomDocumentId, randomTask);

                        // Send the task to the topic
                        await _eventSender.SendAsync("DocumentTasked", documentTask, stoppingToken);

                        Console.WriteLine($"Published task: {randomTask} for document ID: {randomDocumentId}");
                    }
                    else
                    {
                        Console.WriteLine("No documents found in the database.");
                    }
                }

                // Wait for some time before publishing the next task
                // Random interval between 1 and 4 seconds
                int delaySeconds = _random.Next(1, 4);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DocumentTaskPublisher: {ex.Message}");

                // Wait before retrying
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}