using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.Logging;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace AspNet.Http.ETag
{
    public class ETagMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<string> _varyHeaders;
        private readonly ConcurrentDictionary<string, EntityTag> _eTagDictionary;
        private readonly ILogger _logger;
        
        public ETagMiddleware(RequestDelegate next, IEnumerable<string> varyHeaders, ILoggerFactory loggerFactory)
        {
            if(varyHeaders == null)
            {
                throw new ArgumentNullException("varyHeaders");
            }
            
            if(loggerFactory == null)
            {
                throw new ArgumentNullException("loggerFactory");
            }
            
            _next = next;
            _varyHeaders = varyHeaders;
            _eTagDictionary = new ConcurrentDictionary<string, EntityTag>();
            _logger = loggerFactory.Create(typeof(ETagMiddleware).Name);
        }
        
        public async Task Invoke(HttpContext httpContext)
        {
            string fullPath = string.Concat(
                "{0}/{1}?{2}",
                httpContext.Request.PathBase.ToString().TrimEnd('/'),
                httpContext.Request.Path.ToString().TrimStart('/'),
                httpContext.Request.QueryString);
            
            EntityTag entityTag = null;
            string resourceKey = GetResourceKey(fullPath, _varyHeaders, httpContext.Request);
            
            if(httpContext.Request.Method.ToUpper() == "GET")
            {
                IList<string> eTags = httpContext.Request.Headers.GetCommaSeparatedValues("If-None-Match");
                string modifiedSinceHeader = httpContext.Request.Headers["If-Modified-Since"];
                bool anyEtagsFromTheClientExist = eTags != null && eTags.Any();
                bool doWeHaveAnyEntityTagForTheRequest = _eTagDictionary.TryGetValue(resourceKey, out entityTag);
                
                if (anyEtagsFromTheClientExist)
                {
                    if (doWeHaveAnyEntityTagForTheRequest) 
                    {
                        if (eTags.Any(eTag => eTag == entityTag.Value))
                        {                            
                            httpContext.Response.StatusCode = 304;
                            httpContext.Response.Headers.SetCommaSeparatedValues(
                                "Cache-Control", 
                                "must-revalidate", 
                                "max-age=0", 
                                "private");
                            
                            return;
                        }
                    }
                }
            }
            
            await _next.Invoke(httpContext);
        }
        
        // privates
        
        private string GetResourceKey(string trimedRequestUri, IEnumerable<string> varyHeaders, HttpRequest request) 
        {
            var requestedVaryHeaderValuePairs = request.Headers
                .Where(x => varyHeaders.Any(h => h.Equals(x.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(x => string.Format("{0}:{1}", x.Key, string.Join(";", x.Value)));

            return string.Format(
                "{0}:{1}", 
                trimedRequestUri, 
                string.Join("_", requestedVaryHeaderValuePairs)).ToLowerInvariant();
        }
        
        private string GenerateETag() 
        {
            return string.Concat("\"", Guid.NewGuid().ToString("N"), "\"");
        }
    }
}