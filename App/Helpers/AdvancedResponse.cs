using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    public class AdvancedResponse
    {
        public object PagedData { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PageInfo
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }

        public PageInfo() { }

        public PageInfo(int totalCount, int pageSize, int currentPage, int totalPages, bool hasNext, bool hasPrevious)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPages;
            HasNext = hasNext;
            HasPrevious = hasPrevious;
        }
    }
}
