using FaunaDB.Client;
using FaunaDB.Types;
using System.Threading.Tasks;
using static FaunaDB.Query.Language;

namespace Common
{
    public class FaunaDbInitializer
    {
        private static readonly string ENDPOINT = "https://db.fauna.com:443";
        public static readonly string COLLECTION_NAME = "Categories";
        public static readonly string INDEX_NAME = "all_Categories";

        public static string ROOT_KEY = "PUT YOUR ROOT KEY HERE";
        public static FaunaClient GetClient() =>
            new FaunaClient(endpoint: ENDPOINT, secret: ROOT_KEY);
        
        public static async Task InitializeDatabase()
        {
            //Initialize Fauna client
            var client = GetClient();
            //Checkig if collection already exists
            Value exist = await client.Query(Exists(Collection(COLLECTION_NAME)));

            //if not create collection, index and populate one sample record
            if (!(bool)exist)
            {
                //create collection
                await client.Query(CreateCollection(Obj("name", COLLECTION_NAME)));

                //create index for collection
                await client.Query(CreateIndex(
                    Obj(
                            "name", INDEX_NAME, "source", Collection(COLLECTION_NAME))
                        )
                    );

                //create one record in collection
                await client.Query(
                        Create(
                                Collection(COLLECTION_NAME),
                                Obj("data", FaunaDB.Types.Encoder.Encode(new Category() { Name = "First Category", Rating = 1 }))
                            )
                    );
            }

        }
    }
}
