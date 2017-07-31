using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
// using System.Web.Http;
using System.Web.Mvc;

namespace VVV.UI
{
    public class DIConfig
    {
        public static void RegisterIoc()
        {
            var builder = new ContainerBuilder();

            // Register MVC controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Register dependencies in filter attributes
            builder.RegisterFilterProvider();

            // Register dependencies in custom views
            builder.RegisterSource(new ViewRegistrationSource());

            // Default type registrations
            // builder.RegisterType<Security.WebSecurityContext>().As<ISecurityContext>();

            builder.RegisterModule(new DependencyResolution.InfrastructureModule());
            builder.RegisterModule(new DependencyResolution.BusinessServicesModule());

            // Build the container.
            var container = builder.Build();

            // Create the dependency resolver.
            var resolver = new AutofacDependencyResolver(container);

            // Configure ASP.NET MVC with the dependency resolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
