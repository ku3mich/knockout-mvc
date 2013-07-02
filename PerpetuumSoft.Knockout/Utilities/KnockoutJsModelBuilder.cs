using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using PerpetuumSoft.Knockout.Utilities;

namespace PerpetuumSoft.Knockout
{
    public static class KnockoutJsModelBuilder
    {
        //public static string AddComputedToModel<TModel>(string modelName)
        //{
        //    return AddComputedToModel(typeof(TModel), modelName);
        //}

        //public static string AddComputedToModel(Type modelType, string modelName)
        //{
        //    if (modelType.GetConstructor(new Type[] { }) == null)
        //        return "";

        //    var sb = new StringBuilder();
        //    var model = Activator.CreateInstance(modelType);

        //    foreach (var method in modelType.GetMethods().Where(method => typeof(Expression).IsAssignableFrom(method.ReturnType)))
        //    {
        //        sb.Append(modelName);
        //        sb.Append('.');
        //        sb.Append(method.Name);
        //        sb.Append(" = ");
        //        sb.Append("ko.computed(function() { try { return ");
        //        sb.Append(GetGetExpression(modelType, model, method));
        //        sb.Append(string.Format("}} catch(e) {{ return null; }}  ;}}, {0});", modelName));
        //        sb.AppendLine();
        //    }

        //    return sb.ToString();
        //}

        public static string CreateMappingData<TModel>()
        {
            var sb = new StringBuilder();

            sb.Append("{\n");

            List<Type> memberTypes = new List<Type>();
            List<string> members = new List<string>();

            var mType = typeof(TModel);

            Action<Type> modelFunc = null;

            modelFunc = modelType =>
                {
                    foreach (var prop in modelType.GetProperties())
                    {
                        var propType = prop.PropertyType;
                        if (memberTypes.Contains(propType))
                            continue;

                        memberTypes.Add(propType);

                        if (propType.IsEnum)
                        {
                            var vals =
                                Enum.GetValues(propType)
                                    .Cast<ValueType>()
                                    .Select(s => new
                                    {
                                        val = (int)s,
                                        disp = s.GetEnumMemberDisplay().Replace("\"", "\\\"").Replace("\'", "\\\'")
                                    })
                                    .ToList();

                            members.Add(
                                string.Format("EnumDisplay{0}: function (val) {{ var dt = {{{1}}}; return dt[val]; }}\n",
                                propType.FullName.Replace(".", ""), string.Join(", ", vals.Select(s => string.Format("{0}:'{1}'", s.val, s.disp))))
                                );

                        }

                        // todo : add sub models and list
                        // todo : uncomment to .net 4 (.net 4.5)
                        //foreach (var g in propTypeGenericTypeArguments)
                            //modelFunc(g);

                        if (propType.IsClass)
                            modelFunc(propType);
                    }
                };

            modelFunc(mType);

            sb.Append(string.Join(", ", members));
            sb.Append("}\n");

            return sb.ToString();

            //return "{}";


            //Type modelType = typeof(TModel);
            //var sb = new StringBuilder();
            //sb.AppendLine("{");

            //bool first = true;
            //foreach (var property in modelType.GetProperties())
            //{
            //    var propType = property.PropertyType;

            //    if (typeof(IEnumerable).IsAssignableFrom(propType) && !typeof(string).IsAssignableFrom(propType) && propType.IsGenericType)
            //    {
            //        Type itemType = propType.GetGenericArguments()[0];

            //        var computed = AddComputedToModel(itemType, "data");

            //        if (string.IsNullOrWhiteSpace(computed))
            //            continue;

            //        if (first)
            //            first = false;
            //        else
            //            sb.Append(',');

            //        sb.Append("'");
            //        sb.Append(property.Name);
            //        sb.AppendLine("': { create: function(options) {");
            //        sb.AppendLine("var data = ko.mapping.fromJS(options.data);");

            //        sb.Append(computed);

            //        sb.AppendLine("return data;");
            //        sb.AppendLine("}}");
            //    }
            //}

            //sb.Append("}");

            //return first ? "{}" : sb.ToString();
        }

        //private static string GetGetExpression(Type modelType, object model, MethodInfo method)
        //{
        //    var expression = method.Invoke(model, null) as Expression;
        //    var data = KnockoutExpressionData.CreateConstructorData();
        //    data.Aliases[modelType.FullName] = "this";
        //    return KnockoutExpressionConverter.Convert(expression, data);
        //}
    }
}
