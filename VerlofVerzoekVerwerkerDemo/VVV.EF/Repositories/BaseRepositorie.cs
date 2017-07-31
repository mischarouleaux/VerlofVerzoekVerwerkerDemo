using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VVV.Interfaces.Repositories;
using VVV.Models.Interfaces;

namespace VVV.EF.Repositories
{
    /// <summary>
    /// The base repository 
    /// </summary>
    /// <typeparam name="T">The type of the T.</typeparam>
    public class BaseRepository<T> : IRepository<T> where T : class
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        public BaseRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.Context = context;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected DbContext Context
        {
            get;
        }

        /// <summary>
        /// Gets the database set.
        /// </summary>
        /// <value>The database set.</value>
        protected DbSet<T> DbSet
        {
            get { return this.Context.Set<T>(); }
        }

        #region retrieve methods

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>Result set.</returns>
        public virtual IQueryable<T> Get()
        {
            return this.DbSet;
        }

        /// <summary>
        /// Gets all entities with specified dependent objects.
        /// </summary>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Result set.</returns>
        public IQueryable<T> Get(params Expression<Func<T, object>>[] includes)
        {
            var get = this.Get();
            foreach (var include in includes)
            {
                get = get.Include(include);
            }

            return get;
        }

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <returns>Result set.</returns>
        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter)
        {
            return this.Get().Where(filter);
        }

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression and
        /// loaded with specified dependent objects.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Materialized result set.</returns>
        public IQueryable<T> Get(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            return this.Get(includes).Where(filter);
        }

        /// <summary>
        /// Gets the read only records only.
        /// </summary>
        /// <returns>List of read-only records.</returns>
        public virtual IQueryable<T> GetReadOnly()
        {
            return this.DbSet.AsNoTracking();
        }

        /// <summary>
        /// Gets the read only records only.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>List of read-only records.</returns>
        public virtual IQueryable<T> GetReadOnly(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            return this.Get(includes).Where(filter).AsNoTracking();
        }

        /// <summary>
        /// Gets the read only records only.
        /// </summary>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>List of read-only records.</returns>
        public virtual IQueryable<T> GetReadOnly(params Expression<Func<T, object>>[] includes)
        {
            return this.Get(includes).AsNoTracking();
        }

        /// <summary>
        /// Finds the entity by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Found entity.</returns>
        public virtual T Find(object id)
        {
            return this.DbSet.Find(id);
        }

        /// <summary>
        /// Finds the entity by its key values.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>Found entity.</returns>
        public virtual T Find(params object[] keyValues)
        {
            return this.DbSet.Find(keyValues);
        }

        /// <summary>
        /// Loads specified entity with related collection.
        /// </summary>
        /// <typeparam name="TElement">The type of the collection element.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The collection expression.</param>
        public virtual void LoadCollection<TElement>(T entity, Expression<Func<T, ICollection<TElement>>> expression) where TElement : class
        {
            this.Context.Entry(entity).Collection(expression).Load();
        }

        /// <summary>
        /// Loads specified entity with related object.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related object.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The object expression.</param>
        public virtual void LoadReference<TProperty>(T entity, Expression<Func<T, TProperty>> expression) where TProperty : class
        {
            this.Context.Entry(entity).Reference(expression).Load();
        }

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression asynchronously.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <returns>Materialized result set.</returns>
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await this.Get().Where(filter).ToListAsync();
        }

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression and
        /// loaded with specified dependent objects asynchronously.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Materialized result set.</returns>
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var get = this.Get();
            foreach (var include in includes)
            {
                get.Include(include);
            }

            return await get.Where(filter).ToListAsync();
        }

        public Task<T> FindAsync(object id)
        {
            return this.DbSet.FindAsync(id);
        }

        /// <summary>
        /// Finds the entity by its key values asynchronously.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>Found entity.</returns>
        public Task<T> FindAsync(params object[] keyValues)
        {
            return this.DbSet.FindAsync(keyValues);
        }

        #endregion retrieve methods

        #region modification methods

        /// <summary>
        /// Adds the specified entity to database context.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <returns>The added entity.</returns>
        public virtual T Add(T entity)
        {
            return DbSet.Add(entity);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        public virtual T Edit(T entityToUpdate)
        {
            T result = entityToUpdate;
            DbEntityEntry dbEntityEntry = this.Context.Entry(entityToUpdate);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                result = DbSet.Attach(entityToUpdate);
            }
            dbEntityEntry.State = EntityState.Modified;

            return result;
        }

        /// <summary>
        /// Deactivates the specified entity.
        /// </summary>
        /// <remarks>
        /// Entity must implement the ISoftDelete interface.
        /// </remarks>
        /// <param name="entityToDeactivate">The entity to deactivate.</param>
        public virtual void SoftDelete(T entityToDeactivate)
        {
            var baseEntity = entityToDeactivate as ISoftDelete;

            if (baseEntity == null)
            {
                return;
            }

            DbEntityEntry dbEntityEntry = this.Context.Entry(entityToDeactivate);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToDeactivate);
            }
            dbEntityEntry.State = EntityState.Modified;

            baseEntity.IsActive = false;
        }

        /// <summary>
        /// Deactivates the specified entity.
        /// </summary>
        /// <remarks>
        /// Entity must implement the ISoftDelete interface.
        /// </remarks>
        /// <param name="entityToDeactivate">The entity to deactivate.</param>
        public virtual void SoftDelete(object id)
        {
            T entityToDeactivate = this.Find(id);

            var baseEntity = entityToDeactivate as ISoftDelete;

            if (baseEntity == null)
            {
                return;
            }

            DbEntityEntry dbEntityEntry = this.Context.Entry(entityToDeactivate);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToDeactivate);
            }
            dbEntityEntry.State = EntityState.Modified;

            baseEntity.IsActive = false;
        }

        /// <summary>
        /// Reactivates the specified entity by setting IsActive to true.
        /// </summary>
        /// <remarks>
        /// Entity must implement the ISoftDelete interface.
        /// </remarks>
        /// <param name="entityToActivate">The entity to activate.</param>
        /// <returns>The deleted entity.</returns>
        public virtual T UndoDelete(T entityToActivate)
        {
            var baseEntity = entityToActivate as ISoftDelete;

            if (baseEntity == null)
            {
                throw new DbUpdateException("Entity does not implement ISoftDelete interface");
            }

            DbEntityEntry dbEntityEntry = this.Context.Entry(entityToActivate);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToActivate);
            }
            dbEntityEntry.State = EntityState.Modified;

            baseEntity.IsActive = true;

            return entityToActivate;
        }

        /// <summary>
        /// Reactivates the specified entity by setting IsActive to true.
        /// </summary>
        /// <remarks>
        /// Entity must implement the ISoftDelete interface.
        /// </remarks>
        /// <param name="entityToActivate">The entity to activate.</param>
        /// <returns>The deleted entity.</returns>
        public virtual T UndoDelete(object id)
        {
            T entityToActivate = this.Find(id);
            var baseEntity = entityToActivate as ISoftDelete;

            if (baseEntity == null)
            {
                throw new DbUpdateException("Entity does not implement ISoftDelete interface");
            }

            DbEntityEntry dbEntityEntry = this.Context.Entry(entityToActivate);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToActivate);
            }
            dbEntityEntry.State = EntityState.Modified;

            baseEntity.IsActive = true;

            return entityToActivate;
        }


        /// <summary>
        /// Deletes the specified entity by id.
        /// </summary>
        /// <param name="id">The entity id.</param>
        public virtual void Delete(object id)
        {
            T entityToDelete = this.Find(id);
            this.Delete(entityToDelete);
        }

        /// <summary>
        /// Deletes the specified entity by its key values.
        /// </summary>
        /// <param name="keyValues">The key values of the entity.</param>
        public void Delete(params object[] keyValues)
        {
            T entityToDelete = this.Find(keyValues);
            this.Delete(entityToDelete);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        public virtual void Delete(T entityToDelete)
        {
            if (this.Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToDelete);
            }

            this.DbSet.Remove(entityToDelete);
        }

        #endregion

    }

}

