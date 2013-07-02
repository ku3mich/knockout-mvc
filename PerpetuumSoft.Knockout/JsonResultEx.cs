using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace PerpetuumSoft.Knockout
{
    public class JsonResultEx : JsonResult
    {
        public JsonResultEx()
        {
        }

        public JsonResultEx(object model)
        {
            Data = model;
        }

        public JsonResultEx(object m, JsonRequestBehavior requestBehavior)
        {
            Data = m;
            JsonRequestBehavior = requestBehavior;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("MvcResources.JsonRequest_GetNotAllowed");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                var ser = DependencyResolver.Current.GetService<JsonSerializer>();

                StringBuilder sb = new StringBuilder();
                ser.Serialize(new StringWriter(sb),  Data);
                response.Write(sb.ToString());
            }
        }
    }
}
