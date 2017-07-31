using FluentValidation.Mvc;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
// using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace VVV.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Tell application to use our custom Ioc (autofac)
            DIConfig.RegisterIoc();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Configures Fluent Validation.
            FluentValidationModelValidatorProvider.Configure();

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("nl-NL");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("nl-NL");

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("nl-NL");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("nl-NL");
            // Depending on the setting we profile.
            if (Properties.Settings.Default.EnableProfiler)
            {
                // Non SQL Server based installs can use other formatters like: new StackExchange.Profiling.SqlFormatters.InlineFormatter()
                MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
                MiniProfiler.Settings.Storage = new HttpRuntimeCacheStorage(TimeSpan.FromDays(1));

                // Start the profiler.
                MiniProfilerEF6.Initialize();
            }
        }


        protected void Application_BeginRequest()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("nl-NL");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("nl-NL");

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("nl-NL");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("nl-NL");
            // Depending on the setting we profile.
            if (Properties.Settings.Default.EnableProfiler)
            {
                if (Request.IsLocal) { MiniProfiler.Start(); }
            }
        }

        protected void Application_EndRequest()
        {
            // Depending on the setting we profile.
            if (Properties.Settings.Default.EnableProfiler)
            {
                MiniProfiler.Stop();
            }
        }
    }
}
