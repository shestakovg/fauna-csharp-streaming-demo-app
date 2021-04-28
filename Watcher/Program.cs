using System;
using FaunaDB.Client;
using FaunaDB.Types;
using System.Threading.Tasks;
using static FaunaDB.Query.Language;
using Common;
using System.Collections.Generic;

namespace Watcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = FaunaDbInitializer.GetClient();
            Watch(client).Wait();
        }

        static async Task<string> GetReference(FaunaClient client)
        {
            Value result = await client.Query(Paginate(Match(Index(FaunaDbInitializer.INDEX_NAME))));
            IResult<Value[]> data = result.At("data").To<Value[]>();
            return (data.Value[0] as RefV).Id;
        }

        static async Task Watch(FaunaClient client)
        {
            var done = new TaskCompletionSource<object>();
            string reference = Task.Run(() => GetReference(client)).Result;
            var docRef = Get(Ref(Collection(FaunaDbInitializer.COLLECTION_NAME), reference));

            // create a data provider
            var provider = await client.Stream(docRef);
            // a collection for storage of incoming events from the provider
            List<Value> events = new List<Value>();

            // creating a subscriber
            // it takes 3 lambdas that describe the following:
            // - next event processing
            // - error processing
            // - completion processing
            var monitor = new StreamingEventMonitor(
                value =>
                {
                    events.Add(value);
                    Console.WriteLine(value);
                    Console.WriteLine();
                    if (events.Count == 20)
                    {
                        provider.Complete();
                    }
                    else
                    {
                        provider.RequestData();
                    }
                },
                ex => { done.SetException(ex); },
                () => { done.SetResult(null); }
            );

            // subscribe to data provider
            monitor.Subscribe(provider);

            await done.Task;

            // clear the subscription
            monitor.Unsubscribe();
        }
    }
}
