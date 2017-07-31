using Autofac;
using System.Linq;
using VVV.EF;
using VVV.EF.Repositories;
using VVV.Interfaces;
using VVV.Interfaces.Repositories;

namespace VVV.DependencyResolution
{
    public class InfrastructureModule : Module
    {
        /// <summary>
        /// Adds Entity Framework registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        /// <remarks>Note that the ContainerBuilder parameter is unique to this module.</remarks>
        protected override void Load(ContainerBuilder builder)
        {
            // Scan for repositories
            builder.RegisterAssemblyTypes(
                System.Reflection.Assembly.GetAssembly(typeof(ApplicationUserRepository)),
                System.Reflection.Assembly.GetAssembly(typeof(IApplicationUserRepository)))
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();

            // Get a context.
            builder.RegisterType<VVV.EF.VerlofVerzoekVerwerkerContext>().InstancePerLifetimeScope();

            builder.Register(c => new UnitOfWork(c.Resolve<VerlofVerzoekVerwerkerContext>())).As<IUnitOfWork>();
        }
    }
}
