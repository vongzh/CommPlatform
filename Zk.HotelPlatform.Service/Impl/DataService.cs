using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zk.HotelPlatform.DBProvider;

namespace Zk.HotelPlatform.Service.Impl
{
    public class DataService<T> : IDataService<T> where T : class, new()
    {
        protected readonly IDataProvider<T> _dataProvider = null;
        public DataService(IDataProvider<T> dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public T Get(object col)
        {
            return _dataProvider.Get(col);
        }

        public int Count(Expression<Func<T, bool>> filter)
        {
            return _dataProvider.Count(filter);
        }

        public decimal Sum(Expression<Func<T, bool>> filter, Expression<Func<T, decimal>> selector)
        {
            return _dataProvider.Sum(filter, selector);
        }

        public T Get(Expression<Func<T, bool>> filter, string includeProperties = "")
        {
            return _dataProvider.Get(filter, includeProperties);
        }

        public IEnumerable<T> Select(Expression<Func<T, dynamic>> dynamicPredicate, Func<dynamic, T> dynamicFunc, Expression<Func<T, bool>> fiter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includeProperties = "")
        {
            return _dataProvider.Select(dynamicPredicate, dynamicFunc, fiter, orderby, includeProperties);
        }

        public IEnumerable<T> Select(Expression<Func<T, bool>> fiter = null,
               Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includeProperties = "", int count = 0)
        {
            return _dataProvider.Select(fiter, orderby, includeProperties, count);
        }

        public IEnumerable<T> Select(out int totalcount, Expression<Func<T, bool>> fiter = null, int pi = 1, int pz = 0, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, string includeProperties = "")
        {
            return _dataProvider.Select(out totalcount, fiter, pi, pz, orderby, includeProperties);
        }

        public bool AddEntity(T t_entity)
        {
            return _dataProvider.Insert(t_entity);
        }

        public T AddGetEntity(T t_entity)
        {
            return _dataProvider.InsertGet(t_entity);
        }

        public T AddAndGetEntity(T t_entity)
        {
            return _dataProvider.InsertGet(t_entity);
        }

        public bool Update(T t_enttiy)
        {
            return _dataProvider.Update(t_enttiy);
        }

        public IEnumerable<TO> SelectWithSql<TO>(string query, params object[] parameters) where TO : class, new()
        {
            return _dataProvider.SelectWithSql<TO>(query, parameters);
        }

        public IEnumerable<dynamic> SelectWithSql(string query)
        {
            return _dataProvider.SelectWithSql(query);
        }

        public IEnumerable<TO> SelectWithSql<TO>(out int totalCount, string query) where TO : class, new()
        {
            return _dataProvider.SelectWithSql<TO>(out totalCount, query);
        }

        public IEnumerable<TO> SelectWithSql<TO>(out int totalCount, string query, int pageIndex, int pageSize) where TO : class, new()
        {
            return _dataProvider.SelectWithSql<TO>(out totalCount, query, pageIndex, pageSize);
        }

        public IEnumerable<TO> SelectWithSql<TO>(out int totalCount, string query, string sort, int pageIndex, int pageSize) where TO : class, new()
        {
            return _dataProvider.SelectWithSql<TO>(out totalCount, query, sort, pageIndex, pageSize);
        }

        public IEnumerable<T> DeleteAll(Expression<Func<T, bool>> predicate)
        {
            return _dataProvider.DeleteAll(predicate);
        }

        public IEnumerable<T> UpdateAll(List<T> list)
        {
            return _dataProvider.UpdateAll(list);
        }

        public IEnumerable<T> InsertAll(List<T> list)
        {
            return _dataProvider.InsertAll(list);
        }

        public long GetSequence(string sequenceName)
        {
            return _dataProvider.GetSequence(sequenceName);
        }

        public void BulkUpdate(IEnumerable<T> models)
        {
            _dataProvider.BulkUpdate(models);
        }

        public void BulkInsert(IEnumerable<T> models)
        {
            _dataProvider.BulkInsert(models);
        }
    }
}
