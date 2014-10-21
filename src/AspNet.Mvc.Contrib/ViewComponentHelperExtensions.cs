using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;

namespace AspNet.Mvc.Contrib 
{
    public static class ViewComponentHelperExtensions
    {
        public static HtmlString Invoke<TViewComponent>(this IViewComponentHelper helper, params object[] args)
        {
            return helper.Invoke(typeof(TViewComponent), args);
        }
        
        public static void RenderInvoke<TViewComponent>(this IViewComponentHelper helper, params object[] args)
        {
            helper.RenderInvoke(typeof(TViewComponent), args);
        }
        
        public static Task<HtmlString> InvokeAsync<TViewComponent>(this IViewComponentHelper helper, params object[] args)
        {
            return helper.InvokeAsync(typeof(TViewComponent), args);
        }
        
        public static Task RenderInvokeAsync<TViewComponent>(this IViewComponentHelper helper, params object[] args)
        {
            return helper.RenderInvokeAsync(typeof(TViewComponent), args);
        }
    }
}