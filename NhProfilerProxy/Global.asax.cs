using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcMiniProfiler;

namespace NhProfilerProxy
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitProfilerSettings();
        }

        private static void InitProfilerSettings()
        {
            //Don't profile any resource files 
            //MiniProfiler.Settings.IgnoredPaths = new[] { "/mini-profiler-", "/css/", "/scripts/", "/images/", "/favicon.ico" };

            //Clean up the nhibernate stack trace
            MiniProfiler.Settings.ExcludeAssembly("mscorlib");
            MiniProfiler.Settings.ExcludeAssembly("NHibernate");
            MiniProfiler.Settings.ExcludeAssembly("System.Web.Extensions");
            MiniProfiler.Settings.ExcludeType("DbCommandProxy");

            MiniProfiler.Settings.SqlFormatter = new MvcMiniProfiler.SqlFormatters.InlineFormatter();
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}