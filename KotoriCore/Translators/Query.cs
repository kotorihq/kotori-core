namespace KotoriCore.Translators
{
    public class Query
    {
        public string Select { get; set; }
        public string Filter { get; set; }
        public int? Top { get; set; }
        public int? Skip { get; set; }
        public string OrderBy { get; set; }
        public bool Count { get; set; }

        public Query()
        {
        }

        public Query(string select, string filter, int? top, int? skip, string orderBy, bool count = false)
        {
            Select = select;
            Filter = filter;
            Top = top;
            Skip = skip;
            OrderBy = orderBy;
            Count = count;
        }
    }
}