using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
    public class KnockoutForeachContext<TModel> : KnockoutCommonRegionContext<TModel>
    {
        public KnockoutForeachContext(ViewContext viewContext, string expression, string ViewModelName = "viewModel", string JQSelector = null) :
            base(viewContext, expression, ViewModelName, JQSelector)
        {
        }

        protected override string Keyword
        {
            get
            {
                return "foreach";
            }
        }
    }
}
