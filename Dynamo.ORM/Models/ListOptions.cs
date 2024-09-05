namespace Dynamo.ORM.Models
{
    public class ListOptions
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = int.MaxValue;

        public string TableName { get; set; } = null;
    }
}