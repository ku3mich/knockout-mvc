using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
    public class KnockoutWithContext<TModel> : KnockoutCommonRegionContext<TModel>
    {
        public KnockoutWithContext(ViewContext viewContext, string expression, string ViewModelName = "viewModel", string JQSelector = null)
            : base(viewContext, expression, ViewModelName, JQSelector)
        {
        }

        protected override string Keyword
        {
            get
            {
                return "with";
            }
        }
    }
}