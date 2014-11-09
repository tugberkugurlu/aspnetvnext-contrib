using Microsoft.AspNet.Http;
using Microsoft.AspNet.HttpFeature;
using System;

namespace AspNet.Http.Common
{
    public static class HttpContextExtensions
    {
        public static string GetClientIPAddress(this HttpContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }

            IHttpConnectionFeature connection = context.GetFeature<IHttpConnectionFeature>();

            return connection != null
                ? connection.RemoteIpAddress.ToString()
                : null;
        }
    }
}