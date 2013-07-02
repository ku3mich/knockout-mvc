using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace PerpetuumSoft.Knockout
{
    public class KnockoutTagBuilder<TModel> : KnockoutBinding<TModel>
    {
        private readonly TagBuilder tagBuilder;

        public KnockoutTagBuilder(KnockoutContext<TModel> context, string tagName, string[] instanceNames, Dictionary<string, string> aliases)
            : base(context, instanceNames, aliases)
        {
            tagBuilder = new TagBuilder(tagName);
            TagRenderMode = TagRenderMode.Normal;
        }

        public KnockoutTagBuilder<TModel> ApplyAttributes(object htmlAttributes)
        {
            ApplyAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            return this;
        }

        public KnockoutTagBuilder<TModel> ApplyAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes != null)
                foreach (var htmlAttribute in htmlAttributes)
                    tagBuilder.Attributes[htmlAttribute.Key] = htmlAttribute.Value.ToString();
            return this;
        }

        public string InnerHtml
        {
            get
            {
                return tagBuilder.InnerHtml;
            }
            set
            {
                tagBuilder.InnerHtml = value;
            }
        }

        public KnockoutTagBuilder<TModel> SetInnerHtml(string innerHtml)
        {
            InnerHtml = innerHtml;
            return this;
        }

        public TagRenderMode TagRenderMode { get; set; }

        public override string ToHtmlString()
        {
            tagBuilder.Attributes["data-bind"] = BindingAttributeContent();
            return tagBuilder.ToString(TagRenderMode);
        }
    }
}