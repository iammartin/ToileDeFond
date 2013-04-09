using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ToileDeFond.Utilities
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString BootstrapCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlLabelAttributes = null, object htmlCheckBoxAttributes = null)
        {
            var checkbox = htmlHelper.CheckBoxFor(expression, htmlCheckBoxAttributes);
            var label = htmlHelper.LabelFor(expression, htmlLabelAttributes);
            string text = Regex.Match(label.ToString(), "(?<=^|>)[^><]+?(?=<|$)").Value;

            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("checkbox");
            labelTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlLabelAttributes));
            labelTag.InnerHtml = checkbox + text;

            return new MvcHtmlString(labelTag.ToString());
        }

        public static MvcHtmlString BootstrapCheckBox<TModel>(this HtmlHelper<TModel> htmlHelper, string name, object htmlLabelAttributes = null, object htmlCheckBoxAttributes = null)
        {
            var checkbox = htmlHelper.CheckBox(name, htmlCheckBoxAttributes);
            var label = htmlHelper.Label(name, htmlLabelAttributes);
            string text = Regex.Match(label.ToString(), "(?<=^|>)[^><]+?(?=<|$)").Value;

            var labelTag = new TagBuilder("label");
            labelTag.AddCssClass("checkbox");
            labelTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlLabelAttributes));
            labelTag.InnerHtml = checkbox + text;

            return new MvcHtmlString(labelTag.ToString());
        }
    }
}
