namespace KotoriCore.Translators
{
    public class ComplexQuery : Query
    {
        public string Instance { get; private set; }

        public ComplexQuery()
        {
        }

        public ComplexQuery(string select, string filter, int? top, int? skip, string orderBy, string instance, bool count = false)
        {
            Select = select;
            Filter = filter;
            Top = top;
            Skip = skip;
            OrderBy = orderBy;
            Instance = instance;
            Count = count;
        }

        public ComplexQuery(Query query, string instance)
        {
            if (query != null)
            {
                Select = query.Select;
                Filter = query.Filter;
                Top = query.Top;
                Skip = query.Skip;
                OrderBy = query.OrderBy;
                Count = query.Count;
            }

            Instance = instance;
        }
    }
}