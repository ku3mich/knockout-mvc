using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
    public class KnockoutHtml<TModel> : KnockoutSubContext<TModel>
    {
        private readonly ViewContext viewContext;

        public KnockoutHtml(ViewContext viewContext, KnockoutContext<TModel> context, string[] instancesNames = null, Dictionary<string, string> aliases = null)
            : base(context, instancesNames, aliases)
        {
            this.viewContext = viewContext;
        }

        //protected Dictionary<string, object> ValidationAttrs<V>(Expression<Func<TModel, V>> field)
        //{
        //    var meta = ModelMetadata.FromLambdaExpression(field, new ViewDataDictionary<TModel>());

        //    var validationRules = ModelValidatorProviders.Providers
        //        .GetValidators(meta, viewContext)
        //        .SelectMany(v => v.GetClientValidationRules());

        //    var results = new Dictionary<string, object>();
        //    UnobtrusiveValidationAttributesGenerator.GetValidationAttributes(validationRules, results);
        //    var idname = ExpressionHelper.GetExpressionText(field);
        //    results.Add("name", idname);
        //    results.Add("id", idname);

        //    return results;
        //}

        protected KnockoutTagBuilder<TModel> ValidationAttrs<V>(Expression<Func<TModel, V>> field, KnockoutTagBuilder<TModel> tagBuilder)
        {
            var meta = ModelMetadata.FromLambdaExpression(field, new ViewDataDictionary<TModel>());

            var validationRules = ModelValidatorProviders.Providers
                .GetValidators(meta, viewContext)
                .SelectMany(v => v.GetClientValidationRules());

            var results = new Dictionary<string, object>();
            UnobtrusiveValidationAttributesGenerator.GetValidationAttributes(validationRules, results);
            var idname = ExpressionHelper.GetExpressionText(field);
            tagBuilder.ApplyAttributes(results);

            StringBuilder sb = new StringBuilder();
            foreach (var ctx in Context.ContextStack)
            {
                sb.Append(ctx.Expression);
                if (ctx is KnockoutForeachContext<TModel>)
                    sb.Append("['+").Append(ctx.GetIndex()).Append("+']");
            }

            var prf = sb.ToString();
            if (!string.IsNullOrEmpty(prf))
                prf += ".";
            // ContextStack

            tagBuilder.Attr("name", prf+idname);
            tagBuilder.Attr("id", (prf + idname).Replace('.', '_'));

            return tagBuilder;
        }
        private KnockoutTagBuilder<TModel> Input<T>(Expression<Func<TModel, T>> text, string type, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "input", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            if (!string.IsNullOrWhiteSpace(type))
                tagBuilder.ApplyAttributes(new { type });

            if (text != null)
            {
                tagBuilder.Value(text);
                ValidationAttrs(text, tagBuilder);
            }
            tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> TextBox<T>(Expression<Func<TModel, T>> text, object htmlAttributes = null)
        {
            var i = Input(text, null, htmlAttributes);
            i.ApplyAttributes(new { type = "text" });
            return i;
        }

        public KnockoutTagBuilder<TModel> Password(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            return Input(text, "password", htmlAttributes);
        }

        public KnockoutTagBuilder<TModel> Hidden(object htmlAttributes = null)
        {
            return Input<object>(null, "hidden", htmlAttributes);
        }
        
        public KnockoutTagBuilder<TModel> RadioButton<V>(Expression<Func<TModel, V>> @checked, object val = null, object htmlAttributes = null)
        {
            var tagBuilder = Input<V>(null, "radio", htmlAttributes);
            if (val != null)
                tagBuilder.Attr("value", val is ValueType ? ((int)val).ToString() : val.ToString());
            
            tagBuilder.Checked(@checked);
            ValidationAttrs(@checked, tagBuilder);

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> CheckBox<V>(Expression<Func<TModel, V>> @checked, object htmlAttributes = null)
        {
            var tagBuilder = Input<V>(null, "checkbox", htmlAttributes);
            tagBuilder.Checked(@checked);
            ValidationAttrs(@checked,  tagBuilder);

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> TextArea(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "textarea", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Value(text);
            ValidationAttrs(text, tagBuilder);

            return tagBuilder;
        }

        private string DisplayNameFromMemberExpression(MemberExpression mem)
        {
            var da = mem.Member.GetCustomAttributes(false)
                      .Where(a => a is DisplayAttribute)
                      .Select(m => ((DisplayAttribute)m).Name)
                      .FirstOrDefault();
            return da ?? mem.Member.Name;
        }

        public KnockoutTagBuilder<TModel> Label(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            UnaryExpression u = text.Body as UnaryExpression;
            string t;

            if (u != null)
            {
                var p = u.Operand as MemberExpression;
                if (p == null)
                {
                    var c = u.Operand as ConstantExpression;
                    if (c == null)
                    {
                        throw new ArgumentException("only constant or property expressions supptorted");
                    }

                    var valtp = c.Value.GetType();

                    object[] val =
                        valtp.IsEnum ?
                            valtp.GetMember(c.Value.ToString())[0].GetCustomAttributes(false) :
                            valtp.GetCustomAttributes(false);

                    var da = val.Where(a => a is DisplayAttribute)
                                  .Select(m => ((DisplayAttribute)m).Name)
                                  .FirstOrDefault();
                    t = da ?? c.Value.ToString();
                }
                else
                {
                    t = DisplayNameFromMemberExpression(p);
                }
            }
            else
            {
                var m = text.Body as MemberExpression;
                if (m == null)
                    throw new ArgumentException("only unary and member expresions supptorted");
                t = DisplayNameFromMemberExpression(m);
            }

            tagBuilder.Text(string.Format("'{0}'", t.Replace("'", "\\'")));
            return tagBuilder;
        }


        public KnockoutTagBuilder<TModel> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes = null, Expression<Func<TItem, object>> optionsText = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            if (options != null)
                tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
            if (optionsText != null)
            {
                var data = new KnockoutExpressionData { InstanceNames = new[] { "item" } };
                tagBuilder.OptionsText("function(item) { return " + KnockoutExpressionConverter.Convert(optionsText, data) + "; }");
            }


            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> EnumDropDownList<TItem>(Expression<Func<TModel, TItem>> enumMember, object htmlAttributes = null)
        {
            var kv = GetEnumVals<TItem>();
            
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            StringBuilder sb = new StringBuilder();
            foreach (var k in kv) 
                sb.AppendFormat("<option value=\"{0}\">{1}</option>", k.Key, HttpUtility.HtmlEncode(k.Value) );

            tagBuilder.InnerHtml = sb.ToString();
            tagBuilder.NumericValue(enumMember);

            return tagBuilder;
        }

        private static KeyValuePair<int, string>[] GetEnumVals<T>()
        {
            return typeof(T)
               .GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public)
               .Select(f => new
                   KeyValuePair<int, string>(
                        (int)f.GetValue(null),
                        f.IsDefined(typeof(DisplayAttribute), false)
                            ? ((DisplayAttribute)(f.GetCustomAttributes(typeof(DisplayAttribute), false)[0])).Name : f.GetValue(null).ToString()))
               .ToArray();
        }

        public KnockoutTagBuilder<TModel> ListBox<TItem>(Expression<Func<TModel, IList<TItem>>> options, string optionsText, string optionsValue)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(new { multiple = "multiple" });

            if (options != null)
                tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));

            if (!string.IsNullOrEmpty(optionsText))
                tagBuilder.Custom("optionsText", optionsText);

            if (!string.IsNullOrEmpty(optionsValue))
                tagBuilder.Custom("optionsValue", optionsValue);

            return tagBuilder;
        }


        public KnockoutTagBuilder<TModel> ListBox<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(new { multiple = "multiple" });
            tagBuilder.ApplyAttributes(htmlAttributes);
            if (options != null)
                tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));

            return tagBuilder;
        }
        public KnockoutTagBuilder<TModel> ListBox<TItem>(Expression<Func<TModel, IList<TItem>>> options)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(new { multiple = "multiple" });

            if (options != null)
                tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> DropDownList<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes, Expression<Func<TModel, TItem, object>> optionsText)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            if (options != null)
                tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
            if (optionsText != null)
            {
                var data = CreateData();
                var keys = data.Aliases.Keys.ToList();
                if (!string.IsNullOrEmpty(Context.GetInstanceName()))
                    foreach (var key in keys)
                    {
                        data.Aliases[Context.GetInstanceName() + "." + key] = data.Aliases[key];
                        data.Aliases.Remove(key);
                    }
                data.InstanceNames = new[] { Context.GetInstanceName(), "item" };
                tagBuilder.OptionsText("function(item) { return " + KnockoutExpressionConverter.Convert(optionsText, data) + "; }");
            }
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> ListBox<TItem>(Expression<Func<TModel, IList<TItem>>> options, object htmlAttributes, Expression<Func<TModel, TItem, object>> optionsText)
        {
            var tagBuilder = DropDownList(options, htmlAttributes, optionsText);
            tagBuilder.ApplyAttributes(new { multiple = "multiple" });
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Span(Expression<Func<TModel, object>> text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Text(text);
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> DisplayEnum<TEnum>(Expression<Func<TModel, TEnum>> text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            var strmem = KnockoutExpressionConverter.Convert(text);
            var mem = text.Body as MemberExpression;

            if (mem == null || !mem.Type.IsEnum)
                throw new Exception("enum member expression expected");

            tagBuilder.Custom("text", string.Format("$root.__ko_mapping__.{0}({1}())", "EnumDisplay" + mem.Type.FullName.Replace(".", ""), strmem), false);
            
            return tagBuilder;
        }


        public KnockoutTagBuilder<TModel> EnumSpan<E>(Expression<Func<TModel, E>> text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);

            var expr = KnockoutExpressionConverter.Convert(text);

            //tagBuilder.Text(text);
            
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> A(Expression<Func<TModel, object>> text, object htmlAttributes = null, bool emptyRandomHref = true)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "a", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Text(text);

            if (emptyRandomHref)
                tagBuilder.Attr("href", DateTime.Now.ToString("#yyyymmddHHss"));

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Tag(string tag, Expression<Func<TModel, object>> text = null, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, tag, InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            if (text != null)
                tagBuilder.Text(text);

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> JQIcon(string jqiconclass)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(new { @class = jqiconclass + " ui-icon" });


            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> JQIconBox(string jqiconclass)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "div", InstanceNames, Aliases);
            
            tagBuilder.ApplyAttributes(new { @class = "ui-widget ui-button ui-corner-all ui-state-default", style="padding-right: 1px; padding-bottom: 1px" });

            tagBuilder.InnerHtml = JQIcon(jqiconclass).ToHtmlString();

            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> Span(string text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(text));
            return tagBuilder;
        }

        public KnockoutTagBuilder<TModel> SpanInline(string text, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Text(text);
            return tagBuilder;
        }

        //public KnockoutTagBuilder<TModel> ButtonConfirm(string caption, string url, object htmlAttributes = null)
        //{
        //    var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "button", InstanceNames, Aliases);
        //    tagBuilder.ApplyAttributes(htmlAttributes);
        //    tagBuilder.ClickConfirm(url);
        //    tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(caption));
        //    return tagBuilder;
        //}

        public KnockoutTagBuilder<TModel> Button(string caption, string JS, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "button", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.Click(JS);
            tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(caption));
            return tagBuilder;
        }

        //public KnockoutTagBuilder<TModel> ButtonConfirm(string caption, string JS, string confirmQuest, object htmlAttributes = null)
        //{
        //    var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "button", InstanceNames, Aliases);
        //    tagBuilder.ApplyAttributes(htmlAttributes);
        //    tagBuilder.ClickConfirm(JS, confirmQuest);
        //    tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(caption));
        //    return tagBuilder;
        //}

        public KnockoutTagBuilder<TModel> HyperlinkButton(string caption, string url, object htmlAttributes = null)
        {
            var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "a", InstanceNames, Aliases);
            tagBuilder.ApplyAttributes(htmlAttributes);
            tagBuilder.ApplyAttributes(new { href = "#" });
            tagBuilder.Click(url);
            tagBuilder.SetInnerHtml(HttpUtility.HtmlEncode(caption));
            return tagBuilder;
        }

        public KnockoutFormContext<TModel> Form(string actionName, string controllerName, object routeValues = null, object htmlAttributes = null)
        {
            var formContext = new KnockoutFormContext<TModel>(
              viewContext,
              Context, InstanceNames, Aliases,
              actionName, controllerName, routeValues, htmlAttributes);
            formContext.WriteStart(viewContext.Writer);
            return formContext;
        }
    }
}