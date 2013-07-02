using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace PerpetuumSoft.Knockout
{
    public interface IKnockoutContext
    {
        string GetInstanceName();
        string GetIndex();
        string Expression { get; }
    }

    public class KnockoutContext<TModel> : IKnockoutContext
    {
        public readonly string ViewModelName;
        public readonly string JQSelector;

        private TModel model;

        public TModel Model
        {
            get
            {
                return model;
            }
        }

        public List<IKnockoutContext> ContextStack { get; protected set; }

        public KnockoutContext(ViewContext viewContext, string _ViewModelName = "viewModel", string _JQSelector = null)
        {
            ViewModelName = _ViewModelName;
            JQSelector = _JQSelector;
            this.viewContext = viewContext;
            ContextStack = new List<IKnockoutContext>();
            Expression = "";
        }

        private readonly ViewContext viewContext;

        private bool isInitialized;

        private string GetInitializeData(TModel _model, bool needBinding)
        {
            if (isInitialized)
                return "";

            isInitialized = true;

            model = _model;

            var sb = new StringBuilder();

            var json = JsonConvert.SerializeObject(_model, new JsonConverter[] { new JsonDateConverter() });

            sb.AppendLine(@"<script type=""text/javascript""> ");
            sb.AppendLine(string.Format("var {0}Js = {1};", ViewModelName, json));

            var mappingData = KnockoutJsModelBuilder.CreateMappingData<TModel>();

            sb.AppendLine(string.Format("var {0}MappingData = {1};", ViewModelName, mappingData));
            sb.AppendLine(string.Format("var {0} = ko.mapping.fromJS({0}Js, {0}MappingData); ", ViewModelName));

            //sb.Append(KnockoutJsModelBuilder.AddComputedToModel<TModel>(ViewModelName));

            if (needBinding)
                sb.AppendLine(string.Format("ko.applyBindings({0}{1});", ViewModelName, JQSelector == null ? "" : string.Format(", $('{0}')[0]", JQSelector)));

            sb.AppendLine(@"</script>");
            return sb.ToString();
        }

        public HtmlString Initialize(TModel _model)
        {
            return new HtmlString(GetInitializeData(_model, false));
        }

        public HtmlString Apply(TModel _model)
        {
            if (isInitialized)
            {
                var sb = new StringBuilder();
                sb.AppendLine(@"<script type=""text/javascript"">");
                sb.AppendLine(string.Format("ko.applyBindings({0}{1});", ViewModelName, JQSelector == null ? "" : string.Format(", $('{0}')[0]", JQSelector)));
                sb.AppendLine(@"</script>");
                return new HtmlString(sb.ToString());
            }
            return new HtmlString(GetInitializeData(_model, true));
        }

        public HtmlString Apply()
        {
            if (!isInitialized)
                throw new Exception("model not initialized");

            var sb = new StringBuilder();
            sb.AppendLine(@"<script type=""text/javascript"">");
            sb.AppendLine(string.Format("ko.applyBindings({0}{1});", ViewModelName, JQSelector == null ? "" : string.Format(", $('{0}')[0]", JQSelector)));
            sb.AppendLine(@"</script>");
            return new HtmlString(sb.ToString());
        }

        public HtmlString Validation()
        {
            return new HtmlString(string.Format(
                @"
<script type=""text/javascript"">
{1}.validate = function() {{
    if (!(this.validator.validate())) {{
        var m = {{ WasError: true, StatusMessages: ['ERROR'] }};
        for (var msg in this.validator.options.messages) {{
            if ($('#'+msg.replace('.','_')).hasClass('input-validation-error')) {{
                for (var det in this.validator.options.messages[msg]) {{
                    m.StatusMessages.push(this.validator.options.messages[msg][det]);
                }}
            }}
        }}

        ko_updateRes($('{0}'), m);
        return false;
    }}
    else
    {{
        $('.result-cnt', $('{0}')).hide('fast');
    }}
    return true;
}};

{1}.parseValidation = function (){{
    $.validator.unobtrusive.parse($('{0}'));
    {1}.validator = $('{0}').data('unobtrusiveValidation');
}};

{1}.parseValidation();
</script>
", JQSelector, ViewModelName));
        }

        public static string jsfunc(string js)
        {
            return string.Format("function () {{ {0};}}", js);
        }

        public HtmlString PostOnServerRes(string url, string succJS = null, string errJS = null)
        {
            return new HtmlString(
                string.Format("postOnServerRes({0}, \"{1}\", \"{2}\", {3}, {4});", ViewModelName, JQSelector, url, succJS == null ? "undefined" : jsfunc(succJS), errJS == null ? "undefined" : jsfunc(errJS)));
        }

        public HtmlString ExecuteOnServerRes(string url, string succJS = null, string errJS = null)
        {
            return new HtmlString(
                string.Format("executeOnServerRes(\"{0}\", \"{1}\", {2}, {3});",
                    JQSelector,
                    url,
                    succJS == null ? "undefined" : jsfunc(succJS),
                    errJS == null ? "undefined" : jsfunc(errJS)));
        }

        public HtmlString ExecuteOnServer(string url)
        {
            return new HtmlString(
                string.Format("executeOnServer({0}, \"{1}\");", ViewModelName, url));
        }

        public HtmlString BlindExecuteOnServer(string url)
        {
            return new HtmlString(
                string.Format("blindExecuteOnServer({0}, \"{1}\");", ViewModelName, url));
        }


        public HtmlString LazyApply(string url)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"<script type=""text/javascript""> ");
            sb.Append("$(document).ready(function() {");

            sb.AppendLine(string.Format("$.ajax({{ url: '{0}', type: 'POST', success: function (data) {{", url));

            string mappingData = KnockoutJsModelBuilder.CreateMappingData<TModel>();
            if (mappingData == "{}")
                sb.AppendLine(string.Format("window.{0} = ko.mapping.fromJS(data); ", ViewModelName));
            else
            {
                sb.AppendLine(string.Format("var {0}MappingData = {1};", ViewModelName, mappingData));
                sb.AppendLine(string.Format("window.{0} = ko.mapping.fromJS(data, {0}MappingData); ", ViewModelName));
            }

            //sb.Append(KnockoutJsModelBuilder.AddComputedToModel<TModel>(ViewModelName));

            sb.AppendLine(string.Format("ko.applyBindings({0}{1});", ViewModelName, JQSelector == null ? "" : string.Format(", $('{0}')[0]", JQSelector)));

            sb.AppendLine("}, error: function (error) { alert('There was an error posting the data to the server: ' + error.responseText); } });");

            sb.Append("});");
            sb.AppendLine(@"</script>");

            return new HtmlString(sb.ToString());
        }

        private int ActiveSubcontextCount
        {
            get
            {
                return ContextStack.Count - 1 - ContextStack.IndexOf(this);
            }
        }

        public KnockoutForeachContext<TItem> Foreach<TItem>(Expression<Func<TModel, IList<TItem>>> binding)
        {
            var expression = KnockoutExpressionConverter.Convert(binding, CreateData());
            var regionContext = new KnockoutForeachContext<TItem>(viewContext, expression, ViewModelName, JQSelector);
            regionContext.WriteStart(viewContext.Writer);
            regionContext.ContextStack = ContextStack;
            ContextStack.Add(regionContext);
            return regionContext;
        }

        public KnockoutWithContext<TItem> With<TItem>(Expression<Func<TModel, TItem>> binding)
        {
            var expression = KnockoutExpressionConverter.Convert(binding, CreateData());
            var regionContext = new KnockoutWithContext<TItem>(viewContext, expression, ViewModelName, JQSelector);
            regionContext.WriteStart(viewContext.Writer);
            regionContext.ContextStack = ContextStack;
            ContextStack.Add(regionContext);
            return regionContext;
        }

        public KnockoutIfContext<TModel> If(Expression<Func<TModel, bool>> binding)
        {
            var regionContext = new KnockoutIfContext<TModel>(viewContext, KnockoutExpressionConverter.Convert(binding), ViewModelName, JQSelector)
                {
                    InStack = false
                };

            regionContext.WriteStart(viewContext.Writer);
            return regionContext;
        }

        public string GetInstanceName()
        {
            switch (ActiveSubcontextCount)
            {
                case 0:
                    return "";
                case 1:
                    return "$parent";
                default:
                    return "$parents[" + (ActiveSubcontextCount - 1) + "]";
            }
        }

        private string GetContextPrefix()
        {
            var sb = new StringBuilder();
            int count = ActiveSubcontextCount;
            for (int i = 0; i < count; i++)
                sb.Append("$parentContext.");
            return sb.ToString();
        }

        public string GetIndex()
        {
            return GetContextPrefix() + "$index()";
        }

        public virtual string Expression { get; protected set; }

        public virtual KnockoutExpressionData CreateData()
        {
            return new KnockoutExpressionData { InstanceNames = new[] { GetInstanceName() } };
        }

        public virtual KnockoutBinding<TModel> Bind
        {
            get
            {
                return new KnockoutBinding<TModel>(this, CreateData().InstanceNames, CreateData().Aliases);
            }
        }

        public virtual KnockoutHtml<TModel> Html
        {
            get
            {
                return new KnockoutHtml<TModel>(viewContext, this, CreateData().InstanceNames, CreateData().Aliases);
            }
        }

        //TODO: rewrite
        public MvcHtmlString ServerAction(string actionName, string controllerName, object routeValues = null)
        {
            var url = Url().Action(actionName, controllerName, routeValues);
            string exec = string.Format(@"executeOnServer({0}, '{1}')", ViewModelName, url);
            exec = exec.Replace("%28%29", "()");
            if (exec.Contains("%24index()"))
            {
                int count = exec.Length / 17 + 1;
                string[] from = new string[count], to = new string[count];
                from[0] = "%24index()";
                to[0] = "$index()";
                for (int i = 1; i < count; i++)
                {
                    from[i] = "%24parentContext." + from[i - 1];
                    to[i] = "$parentContext." + to[i - 1];
                }
                for (int i = count - 1; i >= 0; i--)
                    exec = exec.Replace(from[i], "'+" + to[i] + "+'");
            }
            return new MvcHtmlString(exec);
        }

        protected static UrlHelper Url()
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var requestContext = new RequestContext(httpContext, new RouteData());
            return new UrlHelper(requestContext);
        }

        public MvcHtmlString Subscribe(Expression<Func<TModel, object>> member, string JS)
        {
            var mem = KnockoutExpressionConverter.Convert(member).Replace("()", "");
            var fullmember = string.Format("{0}.{1}", ViewModelName, mem);

            //var sb = new StringBuilder();
            //sb.Append(string.Format("if ({0}.push) {{ ", fullmember));
            //sb.Append(string.Format(""))

            return new MvcHtmlString(string.Format("{0}.subscribe(function(value) {{ {1}; }});", fullmember, JS));
        }
    }
}
