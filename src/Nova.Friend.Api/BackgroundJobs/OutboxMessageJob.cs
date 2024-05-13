using Core.Arango;
using Core.Arango.Linq;
using Core.Arango.Protocol;
using DomainDrivenDesign.Abstractions;
using MediatR;
using Newtonsoft.Json;
using Nova.Friend.Application.Constants;
using Nova.Friend.Infrastructure.OutboxPattern;
using Quartz;

namespace Nova.Friend.Api.BackgroundJobs;

[DisallowConcurrentExecution]
public class OutboxMessageJob : IJob
{
    private readonly IPublisher _publisher;
    private readonly ILogger<OutboxMessageJob> _logger;
    private readonly IArangoContext _arango;
    private const string WorkerName = nameof(OutboxMessageJob);

    public OutboxMessageJob(
        IPublisher publisher,
        ILogger<OutboxMessageJob> logger,
        IArangoContext arango)
    {
        _publisher = publisher;
        _logger = logger;
        _arango = arango;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        List<OutboxMessage> outboxMessages;
        try
        {
            outboxMessages = await _arango.Query<OutboxMessage>(DatabaseOptions.DatabaseName)
                .Where(x => x.HandledAtUtc == null)
                .ToListAsync();
        }
        catch (Exception)
        {
            Console.WriteLine($"{WorkerName} was thrown exception");
            return;
        }

        Console.WriteLine($"Count outboxMessages: {outboxMessages.Count}");
        foreach (var message in outboxMessages)
        {
            ArangoHandle? arangoTransaction = null;
            try
            {
                arangoTransaction = await _arango.Transaction.BeginAsync(
                    DatabaseOptions.DatabaseName,
                    new ArangoTransaction()
                    {
                        Collections = new ArangoTransactionScope()
                        {
                            Write = new List<string>
                            {
                                DatabaseOptions.OutboxMessage, DatabaseOptions.RequestCollection, DatabaseOptions.UserCollection, DatabaseOptions.FriendEdge
                            },
                            Read = new List<string>
                            {
                                DatabaseOptions.OutboxMessage, DatabaseOptions.RequestCollection, DatabaseOptions.UserCollection, DatabaseOptions.FriendEdge
                            }
                        }
                    }, context.CancellationToken);

                IDomainEvent? domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                
                if (domainEvent is null)
                {
                    _logger.LogWarning(@"Should debug background job: {@job} with domain event: {@event} content: {@content}",
                        WorkerName,
                        message.Id,
                        domainEvent);
                    continue;
                }
                
                _logger.LogInformation(@"Outbox message {@BackgroundJobId} was received by {@Worker}, type: {@Type}, content: {@Content}",
                    message.Id,
                    WorkerName,
                    message.Type,
                    message.Content);

                await _publisher.Publish(domainEvent, context.CancellationToken);
                
                message.HandledAtUtc = DateTime.UtcNow;

                await _arango.Document.UpdateAsync(DatabaseOptions.DatabaseName, DatabaseOptions.OutboxMessage,
                    message, cancellationToken: context.CancellationToken);
                
                _logger.LogInformation("Outbox message {@BackgroundJobId} was handled by {@Worker}",
                    message.Id,
                    WorkerName);

                await _arango.Transaction.CommitAsync(arangoTransaction, context.CancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Outbox message has failed {@Id} with error message {@ErrorMessage}",
                    message.Id,
                    e.Message);

                await _arango.Transaction.AbortAsync(arangoTransaction, context.CancellationToken);
            }
        }
    }
}