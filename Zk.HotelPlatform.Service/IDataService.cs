using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider;

namespace Zk.HotelPlatform.Service
{
    public interface IDataService<T> where T : class, new()
    {
        long GetSequence(string sequenceName);

        T Get(object col);

        T Get(Expression<Func<T, bool>> filter, string includeProperties = "");

        int Count(Expression<Func<T, bool>> filter);

        IEnumerable<T> Select(Expression<Func<T, dynamic>> dynamicPredicate, Func<dynamic, T> dynamicFunc, Expression<Func<T, bool>> fiter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includeProperties = "");

        IEnumerable<T> Select(Expression<Func<T, bool>> fiter = null,
               Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includeProperties = "", int count = 0);

        IEnumerable<T> Select(out int totalcount, Expression<Func<T, bool>> fiter = null, int pi = 1, int pz = 0,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includeProperties = "");

        bool AddEntity(T t_entity);
        T AddGetEntity(T t_entity);
        bool Update(T t_enttiy);

        IEnumerable<TO> SelectWithSql<TO>(string query, params object[] parameters) where TO : class, new();

        IEnumerable<TO> SelectWithSql<TO>(out int totalCount, string query) where TO : class, new();

        IEnumerable<TO> SelectWithSql<TO>(out int totalCount, string query, int pageIndex, int pageSize) where TO : class, new();

        IEnumerable<TO> SelectWithSql<TO>(out int totalCount, string query, string sort, int pageIndex, int pageSize) where TO : class, new();
        IEnumerable<dynamic> SelectWithSql(string query);

        IEnumerable<T> DeleteAll(Expression<Func<T, bool>> predicate);

        IEnumerable<T> UpdateAll(List<T> list);

        IEnumerable<T> InsertAll(List<T> list);
        void BulkUpdate(IEnumerable<T> models);
        void BulkInsert(IEnumerable<T> models);
    }
}
