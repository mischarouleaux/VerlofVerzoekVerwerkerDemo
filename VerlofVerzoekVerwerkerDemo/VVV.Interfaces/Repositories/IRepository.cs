using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VVV.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface that follows Repository pattern. Exposes basic CRUD
    /// operations for specified entity object.
    /// </summary>
    /// <typeparam name="TEntity">Entity to be used for CRUD operation.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
    {

        #region retrieve methods

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>Result set.</returns>
        IQueryable<TEntity> Get();

        /// <summary>
        /// Gets all entities with specified dependent objects.
        /// </summary>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Result set.</returns>
        IQueryable<TEntity> Get(params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <returns>Result set.</returns>
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression and
        /// loaded with specified dependent objects.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Materialized result set.</returns>
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the read-only records.
        /// </summary>
        /// <returns>List of read-only records.</returns>
        IQueryable<TEntity> GetReadOnly();

        /// <summary>
        /// Gets the read only records only.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>List of read-only records.</returns>
        IQueryable<TEntity> GetReadOnly(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets the read-only records.
        /// </summary>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>List of read-only records.</returns>
        IQueryable<TEntity> GetReadOnly(params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Finds the entity by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Found entity</returns>
        TEntity Find(object id);

        /// <summary>
        /// Finds the entity by its key values.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>Found entity</returns>
        TEntity Find(params object[] keyValues);

        /// <summary>
        /// Loads specified entity with related collection.
        /// </summary>
        /// <typeparam name="TElement">The type of the collection element.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The collection expression.</param>
        void LoadCollection<TElement>(TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> expression)
            where TElement : class;

        /// <summary>
        /// Loads specified entity with related object.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related object.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The object expression.</param>
        void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expression)
            where TProperty : class;

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression asynchronously.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <returns>Materialized result set.</returns>
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression and
        /// loaded with specified dependent objects asynchronously.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Materialized result set.</returns>
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Finds the entity by id asynchronously.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Found entity.</returns>
        Task<TEntity> FindAsync(object id);

        /// <summary>
        /// Finds the entity by its key values asynchronously.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>Found entity.</returns>
        Task<TEntity> FindAsync(params object[] keyValues);

        #endregion retrieve methods

        #region modification methods

        /// <summary>
        /// Adds the specified entity to database context.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        TEntity Edit(TEntity entityToUpdate);

        /// <summary>
        /// Soft deletes the specified entity.
        /// </summary>
        /// <remarks>
        /// Entity must implement the ISoftDelete interface.
        /// </remarks>
        /// <param name="entityToDeactivate">The entity to soft delete.</param>
        void SoftDelete(TEntity entityToDeactivate);

        /// <summary>
        /// The soft delete of the specified entity is undone by setting IsActive to true.
        /// </summary>
        /// <remarks>
        /// Entity must implement the ISoftDelete interface.
        /// </remarks>
        /// <param name="entityToActivate">The entity to undelete.</param>
        TEntity UndoDelete(TEntity entityToActivate);

        /// <summary>
        /// Soft deletes the specified entity.
        /// </summary>
        /// <remarks>
        /// Entity must implement the ISoftDelete interface.
        /// </remarks>
        /// <param name="entityToActivate">The entity to soft delete.</param>
        void SoftDelete(object id);

        /// <summary>
        /// The soft delete of the specified entity is undone by setting IsActive to true.
        /// </summary>
        /// <remarks>
        /// Entity must implement the ISoftDelete interface.
        /// </remarks>
        /// <param name="entityToDectivate">The entity to undelete.</param>
        TEntity UndoDelete(object id);

        /// <summary>
        /// Deletes the specified entity by id.
        /// </summary>
        /// <param name="id">The entity id.</param>
        void Delete(object id);

        /// <summary>
        /// Deletes the specified entity by its key values.
        /// </summary>
        /// <param name="keyValues">The key values of the entity.</param>
        void Delete(params object[] keyValues);

        #endregion
    }
}

