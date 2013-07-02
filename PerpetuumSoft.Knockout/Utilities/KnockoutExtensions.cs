using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
    public static class KnockoutExtensions
    {
        public static KnockoutContext<TModel> CreateKnockoutContext<TModel>(this HtmlHelper<TModel> helper, string _ViewModelName = "viewModel", string _JQSelector = null)
        {
            return new KnockoutContext<TModel>(helper.ViewContext, _ViewModelName, _JQSelector);
        }
    }
}
