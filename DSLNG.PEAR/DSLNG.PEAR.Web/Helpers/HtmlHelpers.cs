

using System.Web.Mvc;
namespace DSLNG.PEAR.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString LimitString(this HtmlHelper htmlHelper, string source, int length) {
            if (source.Length > length) {
                var result = source.Substring(0, length);
                result += "... <a class=\"see-more\" href=\"#\" data-toggle=\"modal\" data-target=\"#modalDialog\">See More</a>";
                result += "<span style=\"display:none;color:#fff\" class=\"full-string\">";
                result += source;
                result += "</span>";
                return MvcHtmlString.Create(result);
            }
            return MvcHtmlString.Create(source);
        }
    }
}