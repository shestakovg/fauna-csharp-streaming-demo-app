using System;
using FaunaDB.Client;
using FaunaDB.Types;
using System.Threading.Tasks;
using static FaunaDB.Query.Language;
using Common;


namespace Editor
{
    class Program
    {
        static  void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            //init database with default values
            await FaunaDbInitializer.InitializeDatabase();

            //get reference from collection
            var client = FaunaDbInitializer.GetClient();
            Value result = await client.Query(Paginate(Match(Index(FaunaDbInitializer.INDEX_NAME))));
            IResult<Value[]> data = result.At("data").To<Value[]>();
            data.Match(
                Success: value => Task.Run(() => ProcessData(value, client)).Wait(),
                Failure: reason => Console.WriteLine($"Something went wrong: {reason}")
            );
        }

        //Update data in database
        private static async Task ProcessData(Value[] values, FaunaClient client)
        {
            string reference = (values[0] as RefV).Id;
            Value categoryValue = await client.Query(Get(Ref(Collection(FaunaDbInitializer.COLLECTION_NAME), reference)));

            Category category = Decoder.Decode<Category>(categoryValue.At("data"));

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine("Enter new category name");
                category.Name = Console.ReadLine();
                Console.WriteLine("Enter new category rating");
                category.Rating = Convert.ToInt32(Console.ReadLine());

                await client.Query(
                        Update(
                                Ref(Collection(FaunaDbInitializer.COLLECTION_NAME), reference),
                                Obj("data", Encoder.Encode(category))
                            )
                    );
                Console.WriteLine("Record has been updated");
                Console.WriteLine();
            }

        }
    }
}
