using System.IO;
using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
    public abstract class KnockoutCommonRegionContext<TModel> : KnockoutRegionContext<TModel>
    {
        public override string Expression { get; protected set; }

        protected KnockoutCommonRegionContext(ViewContext viewContext, string expression, string ViewModelName = "viewModel", string JQSelecttor = null)
            : base(viewContext, ViewModelName, JQSelecttor)
        {
            Expression = expression;
        }

        public override void WriteStart(TextWriter writer)
        {
            writer.WriteLine(string.Format(@"<!-- ko {0}: {1} -->", Keyword, Expression));
        }

        protected override void WriteEnd(TextWriter writer)
        {
            writer.WriteLine(@"<!-- /ko -->");
        }

        protected abstract string Keyword { get; }
    }
}