using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider.Base;

namespace Zk.HotelPlatform.DBProvider
{
    public interface IDataProvider<TEntity> where TEntity : class
    {
        long GetSequence(string sequenceName);
        TEntity Get(object col);
        int Count(Expression<Func<TEntity, bool>> filter);

        decimal Sum(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, decimal>> selector);

        TEntity Get(Expression<Func<TEntity, bool>> filter, string includeProperties = "");

        TEntity GetField(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, dynamic>> dynamicPredicate, Func<dynamic, TEntity> dynamicFunc);

        IEnumerable<TEntity> Select(Expression<Func<TEntity, dynamic>> dynamicPredicate, Func<dynamic, TEntity> dynamicFunc, Expression<Func<TEntity, bool>> filter = null,
              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string includeProperties = "");

        IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> filter = null,
               Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string includeProperties = "", int count = 0);

        IEnumerable<TEntity> Select(out int totalcount, Expression<Func<TEntity, bool>> filter = null, int pi = 1, int pz = 0,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string includeProperties = "");

        IEnumerable<TEntity> GetWithDbSetSql(string query, params object[] parameters);

        IEnumerable<T> SelectWithDbContextSql<T>(string query, params object[] parameters);

        IEnumerable<T> SelectWithSql<T>(string query, params object[] parameters) where T : class, new();

        IEnumerable<T> SelectWithSql<T>(out int totalCount, string query) where T : class, new();

        IEnumerable<T> SelectWithSql<T>(out int totalCount, string query, int pageIndex, int pageSize) where T : class, new();

        IEnumerable<T> SelectWithSql<T>(out int totalCount, string query, string sort, int pageIndex, int pageSize) where T : class, new();

        IEnumerable<dynamic> SelectWithSql(string query);

        bool Insert(TEntity entity);

        TEntity InsertGet(TEntity entity);

        bool Delete(object col);

        bool Delete(Expression<Func<TEntity, bool>> predicate);

        bool Delete(TEntity entityToDelete);

        bool Update(TEntity entityToUpdate);

        IEnumerable<TEntity> InsertAll(List<TEntity> list);

        IEnumerable<TEntity> DeleteAll(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> UpdateAll(List<TEntity> list);
        void BulkUpdate(IEnumerable<TEntity> models);
        void BulkInsert(IEnumerable<TEntity> models);
    }
}
