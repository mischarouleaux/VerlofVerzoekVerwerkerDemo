using Autofac;
using System.Linq;
using VVV.Business.Identity;
using VVV.Business.Services;
using VVV.Interfaces.Services;

namespace VVV.DependencyResolution
{
    public class BusinessServicesModule : Module
    {
        /// <summary>
        /// Adds Business services registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        /// <remarks>Note that the ContainerBuilder parameter is unique to this module.</remarks>
        protected override void Load(ContainerBuilder builder)
        {
            // Scan for business services
            builder.RegisterAssemblyTypes(
                System.Reflection.Assembly.GetAssembly(typeof(ApplicationUserService)),
                System.Reflection.Assembly.GetAssembly(typeof(IApplicationUserService)))
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();


            builder.RegisterType<IdentityFactory>();

        }
    }
}

