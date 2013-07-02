using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace PerpetuumSoft.Knockout
{
    public class KnockoutBinding<TModel> : KnockoutSubContext<TModel>, IHtmlString
    {
        public KnockoutBinding(KnockoutContext<TModel> context, string[] instanceNames = null, Dictionary<string, string> aliases = null)
            : base(context, instanceNames, aliases)
        {
        }

        // *** Controlling text and appearance ***

        // Visible
        public KnockoutBinding<TModel> Visible(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "visible", Expression = binding });
            return this;
        }

        // Text
        public KnockoutBinding<TModel> Text(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "text", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Text(string binding)
        {
            Items.Add(new KnockoutBindingStringItem("text", binding, false));
            return this;
        }

        // Html
        public KnockoutBinding<TModel> Html(Expression<Func<TModel, string>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, string> { Name = "html", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Html(Expression<Func<TModel, Expression<Func<string>>>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, Expression<Func<string>>> { Name = "html", Expression = binding });
            return this;
        }

        // *** Working with form fields ***
        // Value
        public KnockoutBinding<TModel> Value<T>(Expression<Func<TModel, T>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, T> { Name = "value", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> NumericValue<T>(Expression<Func<TModel, T>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, T> { Name = "numericValue", Expression = binding });
            return this;
        }

        // Disable
        public KnockoutBinding<TModel> Disable(Expression<Func<TModel, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, bool> { Name = "disable", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Disable(Expression<Func<TModel, Expression<Func<bool>>>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, Expression<Func<bool>>> { Name = "disable", Expression = binding });
            return this;
        }

        // Enable
        public KnockoutBinding<TModel> Enable(Expression<Func<TModel, bool>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, bool> { Name = "enable", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Enable(Expression<Func<TModel, Expression<Func<bool>>>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, Expression<Func<bool>>> { Name = "enable", Expression = binding });
            return this;
        }

        // Checked
        public KnockoutBinding<TModel> Checked<V>(Expression<Func<TModel, V>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, V> { Name = "checked", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> If(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "if", Expression = binding });
            return this;
        }

        // Options
        public KnockoutBinding<TModel> Options(Expression<Func<TModel, IEnumerable>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, IEnumerable> { Name = "options", Expression = binding });
            return this;
        }


        public KnockoutBinding<TModel> OptionsBind<T>(
            Expression<Func<TModel, IEnumerable<T>>> options, 
            Expression<Func<T, object>> text, 
            Expression<Func<T, object>> value)
        {
            
            Items.Add(new KnockoutBindingItem<TModel, IEnumerable<T>> { Name = "options", Expression = options });

            var t= KnockoutExpressionConverter.Convert(text);
            var v= KnockoutExpressionConverter.Convert(value);
            
            Items.Add(new KnockoutBindingStringItem("optionsBind", string.Format("text: {0}, value: {1}", t, v), true));
            
            return this;
        }

        public KnockoutBinding<TModel> Options(Expression<Func<TModel, Expression<Func<IEnumerable>>>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, Expression<Func<IEnumerable>>> { Name = "options", Expression = binding });
            return this;
        }

        // SelectedOptions
        public KnockoutBinding<TModel> SelectedOptions(Expression<Func<TModel, IEnumerable>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, IEnumerable> { Name = "selectedOptions", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> SelectedOptions(Expression<Func<TModel, Expression<Func<IEnumerable>>>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, Expression<Func<IEnumerable>>> { Name = "selectedOptions", Expression = binding });
            return this;
        }

        //public KnockoutBinding<TModel> Binding(string bindingName, Expression<Func<TModel, Expression<Func<IEnumerable>>>> binding)
        //{
        //    Items.Add(new KnockoutBindingItem<TModel, Expression<Func<IEnumerable>>> { Name = bindingName, Expression = binding });
        //    return this;
        //}

        // OptionsCaption
        public KnockoutBinding<TModel> OptionsCaption(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "optionsCaption", Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> OptionsCaption(string text)
        {
            Items.Add(new KnockoutBindingStringItem("optionsCaption", text));
            return this;
        }

        public KnockoutBinding<TModel> OptionsText(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "optionsText", Expression = binding });
            return this;
        }

        // OptionsText
        public KnockoutBinding<TModel> OptionsText(string text) // TODO: rewrite to expressions with functions
        {
            Items.Add(new KnockoutBindingStringItem("optionsText", text, false));
            return this;
        }

        public KnockoutBinding<TModel> OptionsValue(string text) // TODO: rewrite to expressions with functions
        {
            Items.Add(new KnockoutBindingStringItem("optionsValue", text, false));
            return this;
        }
        
        // UniqueName
        public KnockoutBinding<TModel> UniqueName()
        {
            Items.Add(new KnockoutBindingStringItem("uniqueName", "true", false));
            return this;
        }

        public KnockoutBinding<TModel> UniqueId()
        {
            Items.Add(new KnockoutBindingStringItem("uniqueId", "true", false));
            return this;
        }

        // ValueUpdate
        public KnockoutBinding<TModel> ValueUpdate(KnockoutValueUpdateKind kind)
        {
            Items.Add(new KnockoutBindingStringItem("valueUpdate", Enum.GetName(typeof(KnockoutValueUpdateKind), kind).ToLower()));
            return this;
        }

        // HasFucus
        public KnockoutBinding<TModel> HasFocus(Expression<Func<TModel, object>> binding)
        {
            Items.Add(new KnockoutBindingItem<TModel, object> { Name = "hasfocus", Expression = binding });
            return this;
        }

        // *** Complex ***
        public KnockoutBinding<TModel> Css(string name, Expression<Func<TModel, object>> binding)
        {
            ComplexItem("css").Add(new KnockoutBindingItem<TModel, object> { Name = name, Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Style(string name, Expression<Func<TModel, object>> binding)
        {
            ComplexItem("style").Add(new KnockoutBindingItem<TModel, object> { Name = name, Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Attr(string name, Expression<Func<TModel, object>> binding)
        {
            ComplexItem("attr").Add(new KnockoutBindingItem<TModel, object> { Name = name, Expression = binding });
            return this;
        }

        public KnockoutBinding<TModel> Attr(string name, string value, bool needQuotes = true)
        {
            ComplexItem("attr").Add(new KnockoutBindingStringItem { Name = name, Value = value, NeedQuotes = needQuotes });
            return this;
        }
        // *** Events ***
        protected virtual KnockoutBinding<TModel> Event(string eventName, string actionName, string controllerName, object routeValues)
        {
            var sb = new StringBuilder();
            sb.Append("function() {");
            sb.Append(Context.ServerAction(actionName, controllerName, routeValues));
            sb.Append(";}");
            Items.Add(new KnockoutBindingStringItem(eventName, sb.ToString(), false));
            return this;
        }

        protected virtual KnockoutBinding<TModel> Event(string eventName, string actionName, string controllerName, string confirmQuestion, object routeValues)
        {
            var sb = new StringBuilder();
            sb.Append("function() { ko_confirmDialog('" + confirmQuestion.Replace("'", "\\'" ) + "', function () {");
            sb.Append(Context.ServerAction(actionName, controllerName, routeValues));
            sb.Append(";} ); }");
            Items.Add(new KnockoutBindingStringItem(eventName, sb.ToString(), false));
            return this;
        }

        public virtual KnockoutBinding<TModel> JSEvent(string eventName, string JS, bool stopPropagation = true)
        {
            var sb = new StringBuilder();
            sb.Append("function(model, ev) {");
            sb.Append(JS);
            sb.Append(";");
            if (stopPropagation)
                sb.Append("ko_stopPropagation(ev);");
            sb.Append("}");
            Items.Add(new KnockoutBindingStringItem(eventName, sb.ToString(), false));
            return this;
        }

        protected virtual KnockoutBinding<TModel> JSEvent(string eventName, string confirmQuestion, string JS)
        {
            var sb = new StringBuilder();
            sb.Append("function() { ko_confirmDialog('" + confirmQuestion.Replace("'", "\\'" ) + "', function () {");
            sb.Append(JS);
            sb.Append(";} ); }");
            Items.Add(new KnockoutBindingStringItem(eventName, sb.ToString(), false));
            return this;
        }
        public KnockoutBinding<TModel> Click(string actionName, string controllerName, object routeValues = null)
        {
            return Event("click", actionName, controllerName, routeValues);
        }

        public KnockoutBinding<TModel> ClickConfirm(string actionName, string controllerName, string confirmQuestion, object routeValues = null)
        {
            return Event("click", actionName, controllerName, confirmQuestion, routeValues);
        }

        public KnockoutBinding<TModel> ClickConfirm(string JS, string confirmQuestion)
        {
            return JSEvent("click", confirmQuestion, JS);
        }
        public KnockoutBinding<TModel> Submit(string actionName, string controllerName, object routeValues = null)
        {
            return Event("submit", actionName, controllerName, routeValues);
        }

        public KnockoutBinding<TModel> Click(string JS)
        {
            return JSEvent("click", JS);
        }

        public KnockoutBinding<TModel> JSOtherEvent(string evt, string js)
        {
            var sb = new StringBuilder();
            sb.Append("{ " + evt + ": function() {");
            sb.Append(js);
            sb.Append(";} }");
            Items.Add(new KnockoutBindingStringItem("event", sb.ToString(), false));
            return this;
        }

        public KnockoutBinding<TModel> JSOtherEventFunc(string evt, string jsfunc)
        {
            var sb = new StringBuilder();
            sb.Append("{ " + evt + ": " + jsfunc + "}");
            Items.Add(new KnockoutBindingStringItem("event", sb.ToString(), false));
            return this;
        }

        public KnockoutBinding<TModel> Change(string JS)
        {
            return JSOtherEvent("change", JS);
        }

        public KnockoutBinding<TModel> Submit(string JS)
        {
            return JSEvent("submit", JS);
        }

        // *** Custom ***    
        public KnockoutBinding<TModel> Custom(string name, string value, bool needQuotes = true)
        {
            Items.Add(new KnockoutBindingStringItem(name, value, needQuotes));
            return this;
        }

        public KnockoutBinding<TModel> Custom<V>(string name, Expression<Func<TModel,V>> value, bool needQuotes = true)
        {
            Items.Add(new KnockoutBindingItem<TModel,V> { Name = name, Expression = value });
            return this;
        }

        public KnockoutBinding<TModel> Autocomplete(string autocompleteUrl, string valueModelFieldName, string labelModelFieldName)
        {
            string func =
                string.Format("|function (event,ui){{ {0}['{1}'](ui.item.value);{0}['{2}'](ui.item.label); return false; }}|",
                              Context.ViewModelName, valueModelFieldName, labelModelFieldName);
            string funcCheck =
                string.Format("|function (event,ui){{ if (!ui.item) {{ {0}['{1}']('');{0}['{2}'](''); }} }}|",
                  Context.ViewModelName, valueModelFieldName, labelModelFieldName);
            var opts = new
            {
                source = autocompleteUrl,
                select = func,
                focus = func,
                change = funcCheck
            };
            return JQueryUI("autocomplete", opts);
        }

        public KnockoutBinding<TModel> JQueryUI(string _widget, object _opts = null)
        {
            var val = new { widget = _widget, options = _opts };
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //string value = js.Serialize(val)

            string value = JsonConvert.SerializeObject(val).Replace("\"", "'").Replace("'|", "").Replace("|'", "").Replace("\\u0027", "'").Replace("}}", "} }");
            Items.Add(new KnockoutBindingStringItem("jqueryui", value, false));
            return this;
        }

        public KnockoutBinding<TModel> JQueryUI2(string _widget, object _opts = null)
        {
            var val = new { widget = _widget, options = _opts };
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //string value = js.Serialize(val)

            string value = JsonConvert.SerializeObject(val).Replace("\"", "'").Replace("'|", "").Replace("|'", "").Replace("\\u0027", "'").Replace("}}", "} }");
            Items.Add(new KnockoutBindingStringItem("jqueryui2", value, false));
            return this;
        }

        // *** Common ***

        private readonly List<KnockoutBindingItem> items = new List<KnockoutBindingItem>();

        private readonly Dictionary<string, KnockoutBingindComplexItem> complexItems = new Dictionary<string, KnockoutBingindComplexItem>();

        public List<KnockoutBindingItem> Items
        {
            get
            {
                return items;
            }
        }

        private KnockoutBingindComplexItem ComplexItem(string name)
        {
            if (!complexItems.ContainsKey(name))
            {
                complexItems[name] = new KnockoutBingindComplexItem { Name = name };
                items.Add(complexItems[name]);
            }
            return complexItems[name];
        }

        public virtual string ToHtmlString()
        {
            var sb = new StringBuilder();
            sb.Append(@"data-bind=""");
            sb.Append(BindingAttributeContent());
            sb.Append(@"""");
            return sb.ToString();
        }

        public string BindingAttributeContent()
        {
            var sb = new StringBuilder();
            bool first = true;
            foreach (var item in Items.Where(item => item.IsValid()))
            {
                if (first)
                    first = false;
                else
                    sb.Append(',');
                sb.Append(item.GetKnockoutExpression(CreateData()));
            }
            return sb.ToString();
        }
    }
}
