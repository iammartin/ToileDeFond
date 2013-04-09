using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ToileDeFond.Utilities
{
    public static class Paging
    {
        public static PagerCollection GetPagesToDisplay<T>(this PagedCollection<T> collection,
                                                           int numberOfElementsPerPage,
                                                           int numberOfPagesToDisplay, int currentPageIndex,
                                                           string pageUrl = null, string queryStringPageName = null)
        {
            return GetPagesToDisplay(collection.TotalCount, numberOfElementsPerPage,
                                     numberOfPagesToDisplay, currentPageIndex, pageUrl, queryStringPageName);
        }

        public static PagerCollection GetPagesToDisplay(int numberOfItems, int itemsPerPage, int numberOfPagesToDisplay,
                                                        int currentPageIndex, string pageUrl = null,
                                                        string queryStringPageName = null)
        {
            return GetPagesToDisplay((int) Math.Ceiling(numberOfItems/(double) itemsPerPage), numberOfPagesToDisplay,
                                     currentPageIndex, pageUrl, queryStringPageName);
        }

        public static PagerCollection GetPagesToDisplay(int numberOfPages, int numberOfPagesToDisplay,
                                                        int currentPageIndex, string pageUrl = null,
                                                        string queryStringPageName = null)
        {
            if (numberOfPagesToDisplay < 3)
                throw new ArgumentException("numberOfPagesToDisplay cannot be less than 3");

            if (numberOfPagesToDisplay%2 != 1)
                throw new ArgumentException("numberOfPagesToDisplay%2 != 1");

            var pageList = new List<PagerPage>();

            var restOfPages = (int) Math.Floor((double) numberOfPagesToDisplay/2);
            var hasPrefix = currentPageIndex > (restOfPages + 1) && numberOfPages > numberOfPagesToDisplay;
            var hasSuffix = numberOfPages - currentPageIndex > restOfPages && numberOfPages > numberOfPagesToDisplay;

            if (hasPrefix && hasSuffix)
            {
                for (var i = currentPageIndex - restOfPages; i <= currentPageIndex + restOfPages; i++)
                {
                    pageList.Add(new PagerPage(i, i == currentPageIndex, pageUrl, queryStringPageName));
                }
            }
            else if (hasPrefix)
            {
                var numbers = new List<int>();

                for (var i = numberOfPages; i > numberOfPages - numberOfPagesToDisplay; i--)
                {
                    numbers.Add(i);
                }

                numbers.Reverse();

                for (var i = 0; i < numbers.Count; i++)
                {
                    pageList.Add(new PagerPage(numbers[i], i == currentPageIndex, pageUrl, queryStringPageName));
                }
            }
            else
            {
                var end = numberOfPages > numberOfPagesToDisplay ? numberOfPagesToDisplay : numberOfPages;
                for (var i = 1; i <= end; i++)
                {
                    pageList.Add(new PagerPage(i, i == currentPageIndex, pageUrl, queryStringPageName));
                }
            }

            return new PagerCollection(pageList, currentPageIndex, hasPrefix, hasSuffix, numberOfPages);
        }

        public class PagerCollection
        {
            private readonly List<PagerPage> _pages;
            private readonly int _pageIndex;
            private readonly bool _hasPrefix;
            private readonly bool _hasSuffix;
            private readonly int _numberOfPages;

            public PagerCollection(List<PagerPage> pages, int pageIndex, bool hasPrefix, bool hasSuffix,
                                   int numberOfPages)
            {
                _pages = pages;
                _pageIndex = pageIndex;
                _hasPrefix = hasPrefix;
                _hasSuffix = hasSuffix;
                _numberOfPages = numberOfPages;
            }

            public int PageIndex
            {
                get { return _pageIndex; }
            }

            public int NumberOfPages
            {
                get { return _numberOfPages; }
            }

            public bool HasSuffix
            {
                get { return _hasSuffix; }
            }

            public bool HasPrefix
            {
                get { return _hasPrefix; }
            }

            public List<PagerPage> Pages
            {
                get { return _pages; }
            }
        }

        public class PagerPage
        {
            private readonly string _queryStringPageName;
            private readonly int _index;
            private readonly bool _isCurrent;
            private readonly string _pageURL;
            private readonly string _url;

            public PagerPage(int index, bool isCurrent, string pageUrl = null, string queryStringPageName = null)
            {
                _queryStringPageName = queryStringPageName;
                _index = index;
                _isCurrent = isCurrent;
                _pageURL = pageUrl;
                _url = GetValidUrl(pageUrl, queryStringPageName, index);
            }

            public string URL
            {
                get { return _url; }
            }

            public string PageURL
            {
                get { return _pageURL; }
            }

            public int Index
            {
                get { return _index; }
            }

            public string QueryStringPageName
            {
                get { return _queryStringPageName; }
            }

            private static string GetValidUrl(string pageUrl, string queryStringParamName, int pageIndex)
            {
                return pageUrl == null
                           ? null
                           : string.Concat(pageUrl, pageUrl.Contains("?") ? "&" : "?", queryStringParamName, "=",
                                           pageIndex);
            }
        }

        public static MvcHtmlString Pager<T>(this AjaxHelper helper, PagedCollection<T> pagedCollection, int numberOfPagesToDisplay)
        {
            var stringBuilder = new StringBuilder("<p class=\"pager\">");

            if (pagedCollection.PageIndex > 1)
                stringBuilder.AppendLine("<a href=\"#\" class=\"pagernavigator previous\"> « </a>");

            var pagerPages = GetPagesToDisplay(pagedCollection, pagedCollection.PageSize, numberOfPagesToDisplay, pagedCollection.PageIndex);

            if (pagerPages.HasPrefix)
                stringBuilder.Append("...");

            for (var i = 0; i < pagerPages.Pages.Count; i++)
            {
                var pagerPage = pagerPages.Pages[i];

                stringBuilder.AppendFormat(pagerPage.Index == pagerPages.PageIndex ?
                        "<span data-pageindex=\"{0}\" class=\"pagerpage current\">{0}</a>" :
                        "<a data-pageindex=\"{0}\" href=\"#\" class=\"pagerpage\">{0}</a>", pagerPage.Index);

                if (i != pagerPages.Pages.Count - 1)
                {
                    stringBuilder.AppendFormat(" | ");
                }
            }

            if (pagerPages.HasSuffix)
                stringBuilder.Append("...");

            if (pagedCollection.PageIndex < pagerPages.NumberOfPages)
                stringBuilder.AppendLine("<a href=\"#\" class=\"pagernavigator next\"> »</a>");

            return new MvcHtmlString(stringBuilder.ToString());
        }

        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static int PageCount<TSource>(this IEnumerable<TSource> source, int pageSize)
        {
            return source.Count().PageCount(pageSize);
        }

        public static int PageCount(this int total, int pageSize)
        {
            return (int)Math.Ceiling(total / (decimal)pageSize);
        }
    }
}
