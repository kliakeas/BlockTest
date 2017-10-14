using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace DecentralizedBank
{
    public static class ResponseHelper
    {
        public static HttpResponseMessage Create(HttpStatusCode statusCode = HttpStatusCode.OK, object content = null)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new ObjectContent(typeof(object), content, System.Web.Http.GlobalConfiguration.Configuration.Formatters.JsonFormatter)
            };
        }
    }
}