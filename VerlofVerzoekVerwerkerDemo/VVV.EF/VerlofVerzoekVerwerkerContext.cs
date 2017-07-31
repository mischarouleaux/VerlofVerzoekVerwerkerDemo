using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VVV.EF.Mapping;
using VVV.Models;
using VVV.Models.Interfaces;

namespace VVV.EF
{
    public partial class VerlofVerzoekVerwerkerContext : DbContext
    {
        static VerlofVerzoekVerwerkerContext()
        {
            Database.SetInitializer<VerlofVerzoekVerwerkerContext>(null);
        }

        public VerlofVerzoekVerwerkerContext()
            : base("name=VerlofVerzoekVerwerkerContext")
        {
            // Do NOT enable proxied entities, else serialization fails
            base.Configuration.ProxyCreationEnabled = false;

            // We don't do lazy loading due to unexpected queries being fired on the front end.
            base.Configuration.LazyLoadingEnabled = false;

            //? Query logging
#if DEBUG
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
        }

        public IDbSet<ApplicationUser> ApplicationUsers { get; set; }
        public IDbSet<Department> Departments { get; set; }
        public IDbSet<Holiday> Holidays { get; set; }
        public IDbSet<Manager> Managers { get; set; }
        public IDbSet<Message> Messages { get; set; }
        public IDbSet<MutationsVacation> MutationsVacations { get; set; }
        public IDbSet<sysdiagram> sysdiagrams { get; set; }
        public IDbSet<VacationRequest> VacationRequests { get; set; }

        #region methods

        /// <summary>
        /// Creates a new context.
        /// </summary>
        /// <returns></returns>
        public static VerlofVerzoekVerwerkerContext Create()
        {
            return new VerlofVerzoekVerwerkerContext();
        }

        /// <summary>
        /// Handles creating our models.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Call our base, needed for Identity related stuff.
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Remove this convention, we don't do a cascade delete all too often and if we need to, we'll handle it ourselves.
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            {
                modelBuilder.Configurations.Add(new ApplicationUserMap());
                modelBuilder.Configurations.Add(new DepartmentMap());
                modelBuilder.Configurations.Add(new HolidayMap());
                modelBuilder.Configurations.Add(new ManagerMap());
                modelBuilder.Configurations.Add(new MessageMap());
                modelBuilder.Configurations.Add(new MutationsVacationMap());
                modelBuilder.Configurations.Add(new sysdiagramMap());
                modelBuilder.Configurations.Add(new VacationRequestMap());
            }

        }


        /// <summary>
        /// Override for SaveChangesAsync to support soft deleting items.
        /// </summary>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync()
        {
            // Audit the entries.
            //AuditEntries();

            return base.SaveChangesAsync();
        }

        /// <summary>
        /// Override for SaveChanges to support soft deleting items.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        /// <summary>
        /// Logs some basic audit info for entities that we want to audit.
        /// </summary>
        /*private void AuditEntries()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is IAuditable && (x.State == EntityState.Added || x.State == EntityState.Modified));

            // TODO: Figure out what the best way is to retrieve the user.
            // The following is based on HTTP and is no guarantee for an Id.

            var currentUsername = HttpContext.Current != null && HttpContext.Current.User != null
                ? HttpContext.Current.User.Identity.Name
                : "anonymous";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IAuditable)entity.Entity).DateCreated = DateTime.Now;
                    ((IAuditable)entity.Entity).CreateUser = currentUsername;
                }

                ((IAuditable)entity.Entity).DateChanged = DateTime.Now;
                ((IAuditable)entity.Entity).ChangeUser = currentUsername;
            }
        }*/

        #endregion
    }
}
