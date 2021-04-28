using System;
using System.Collections.Generic;
using FaunaDB.Client;
using FaunaDB.Types;
using System.Threading.Tasks;
using static FaunaDB.Query.Language;
using Common;


namespace UpdaterApp
{
    class Program
    {
        static  void Main(string[] args)
        {
            Run().Wait();
            Console.WriteLine("Hello World!");
        }

        private static async Task Run()
        {
            await FaunaDbInitializer.InitializeDatabase();

            var client = FaunaDbInitializer.GetClient("fnAEH0EIp-ACBw0RIi3jh_v4riRQyQK6MLZ_GvN-");
            Value result = await client.Query(Paginate(Match(Index(FaunaDbInitializer.INDEX_NAME))));
            IResult<Value[]> data = result.At("data").To<Value[]>();
            data.Match(
                Success: value => Task.Run(() => ProcessData(value, client)).Wait(),
                Failure: reason => Console.WriteLine($"Something went wrong: {reason}")
            );
        }

        private static async Task ProcessData(Value[] values, FaunaClient client)
        {
            var done = new TaskCompletionSource<object>();
            string reference = values.Length > 0 ? (values[0] as RefV).Id : "0";
            Category category = null;
            try
            {
                Value categoryValue = await client.Query(Get(Ref(Collection(FaunaDbInitializer.COLLECTION_NAME), reference)));

                category = Decoder.Decode<Category>(categoryValue.At("data"));
            }
            catch(Exception ex)
            {

            }
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine("Enter new category name");
                string categoryName = Console.ReadLine();
                Console.WriteLine("Enter new category rating");
                int categoryRating = Convert.ToInt32(Console.ReadLine());
                category.Name = categoryName;
                category.Rating = categoryRating;

                await client.Query(
                        Update(
                                Ref(Collection(FaunaDbInitializer.COLLECTION_NAME), reference),
                                Obj("data", FaunaDB.Types.Encoder.Encode(category))
                            )
                    );
                Console.WriteLine("Record has been updated");
                Console.WriteLine();
            }

        }
    }
}
