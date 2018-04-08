namespace KotoriCore.Translators
{
    // TODO
    public class ComplexQuery : Query
    {
        public string AdditionalFilter { get; set; }
        
        public ComplexQuery()
        {
        }
        
        public ComplexQuery(string select, string filter, int? top, int? skip, string orderBy, string additionalFilter, bool count = false)
        {
            Select = select;
            Filter = filter;
            Top = top;
            Skip = skip;
            OrderBy = orderBy;
            AdditionalFilter = additionalFilter;
            Count = count;
        }

        public ComplexQuery(Query query)
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
        }
    }
}