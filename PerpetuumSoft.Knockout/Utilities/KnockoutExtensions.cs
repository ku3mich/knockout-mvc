using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
    public static class KnockoutExtensions
    {
        public static KnockoutContext<TModel> CreateKnockoutContext<TModel>(this HtmlHelper<TModel> helper, string _ViewModelName, string _JQSelector)
        {
            return new KnockoutContext<TModel>(helper.ViewContext, _ViewModelName, _JQSelector);
        }

        public static KnockoutContext<TModel> CreateKnockoutContext<TModel>(this HtmlHelper<TModel> helper, string containerId)
        {
            return new KnockoutContext<TModel>(helper.ViewContext, containerId+"Vm", "#"+containerId);
        }
    }
}
