using FaunaDB.Types;

namespace Common
{
    public class Category
    {
        [FaunaField("name")]
        public string Name;
        [FaunaField("rating")]
        public int Rating;
    }
}
