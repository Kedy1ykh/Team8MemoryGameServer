using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MemoryGameServer.Models;

namespace MemoryGameServer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["waitingList"] = new List<Person>();
            HttpContext.Current.Application["matchList"] = new List<Match>();
            HttpContext.Current.Application["gameList"] = new List<Game>();
            HttpContext.Current.Application["userCounter"] = 0;
            HttpContext.Current.Application.UnLock();
        }
    }
}
