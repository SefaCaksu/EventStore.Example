using System.Text.Json;
using EventStore.Client;
using Shared.Services.Abstractions;

namespace Shared.Services
{
    public class EventstoreService : IEventstoreService
    {
        public EventstoreService()
        {
        }

        public async Task AppendToStreamAsync(string streamName, IEnumerable<EventData> eventData)
        {
            await Client.AppendToStreamAsync(streamName: streamName, eventData: eventData, expectedState: StreamState.Any);
        }

        public EventData GenerateEventData(object @event)
        {
            return new EventData(eventId: Uuid.NewUuid(), type: @event.GetType().Name, data: JsonSerializer.SerializeToUtf8Bytes(@event));

        }

        public async Task SubscribeToStreamAsync(string streamName, Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared)
        {
            await Client.SubscribeToStreamAsync(
                streamName: streamName,
                start: FromStream.Start,
                eventAppeared: eventAppeared,
                subscriptionDropped: (streamSubscription, subscriptionDroppedReason, ex) =>
                {
                    Console.WriteLine("Disconnected");
                });
        }



        EventStoreClient Client
        {
            get
            {
                return new(GetEventStoreClientSettings());
            }
        }

        EventStoreClientSettings GetEventStoreClientSettings()
        {
            //TODO: appsettings taşı
            string connectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false";

            return EventStoreClientSettings.Create(connectionString);
        }

    }
}

