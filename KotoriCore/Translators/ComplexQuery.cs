namespace KotoriCore.Translators
{
    // TODO
    public class ComplexQuery
    {
        public string Select { get; set; }
        public string Filter { get; set; }
        public int? Top { get; set; }
        public int? Skip { get; set; }
        public string OrderBy { get; set; }
        public string AdditionalFilter { get; set; }
        public bool Count { get; set; }

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
    }
}