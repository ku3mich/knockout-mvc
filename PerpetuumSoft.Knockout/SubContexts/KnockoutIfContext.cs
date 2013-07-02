using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
    public class KnockoutIfContext<TModel> : KnockoutCommonRegionContext<TModel>
    {
        public KnockoutIfContext(ViewContext viewContext, string expression, string ViewModelName = "viewModel", string JQSelector = null)
            : base(viewContext, expression, ViewModelName, JQSelector)
        {
        }

        protected override string Keyword
        {
            get
            {
                return "if";
            }
        }
    }
}