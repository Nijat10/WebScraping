namespace SearchTrade.WebAPI.Helpers
{

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int AllDataCount { get; private set; }
        public int PageSize { get; private set; }
        public int TotalRows { get; private set; }

        const int PageLinkCount = 10;

        public PaginatedList(List<T> items, int allCount, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AllDataCount= allCount;
            PageSize = PageSize;
            TotalRows = count;

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public int FirstPageToShow
        {
            get
            {
                return ((PageIndex - 1) / PageLinkCount) * PageLinkCount + 1;
            }
        }

        public int LastPageToShow
        {
            get
            {
                int n = FirstPageToShow + PageLinkCount - 1;
                if (n > TotalPages)
                    n = TotalPages;
                return n;
            }
        }
    }

}
