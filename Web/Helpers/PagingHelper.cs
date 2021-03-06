﻿using System.Text;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using Web.Domen.Viewmodels;

namespace Web.Helpers
{
    public static class PagingHelper
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            var result = new StringBuilder();
            for (var i = 0; i < pagingInfo.TotalPages; i++)
            {
                var tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i+1));
                tag.InnerHtml = (i+1).ToString();
                if (i == pagingInfo.CurrentPage-1)
                {
                    tag.AddCssClass("currentPage");
                }
                tag.AddCssClass("pageAlbom");
                result.Append(tag.ToString());
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}