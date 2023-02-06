using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;
using Zk.HotelPlatform.Utils;

namespace Zk.HotelPlatform.DBProvider.Base
{
    public class DataProvider<TEntity> : IDataProvider<TEntity> where TEntity : class, new()
    {
        protected DbSet<TEntity> EntitySet => this.DataContext.Set<TEntity>();

        private readonly DataContext DataContext = null;

        public DataProvider(DataContext dataContext)
        {
            DataContext = dataContext;

        }

        #region Sequence
        public virtual long GetSequence(string sequenceName)
        {
            using (var db = new DataContext())
            {
                return db.Database.SqlQuery<long>($"SELECT NEXT VALUE FOR[{ sequenceName}] Id").FirstOrDefault();
            }
        }
        #endregion

        #region Query Insert Modify Delete方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col">条件</param>
        /// <returns></returns>
        public virtual TEntity Get(object col)
        {
            using (var db = new DataContext())
            {
                return db.Set<TEntity>().Find(col);
            }
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> filter)
        {
            using (var db = new DataContext())
            {
                IQueryable<TEntity> query = db.Set<TEntity>();

                if (filter != null)
                {
                    return query.Count(filter);
                }
                return default;
            }
        }

        /// <summary>
        /// 查询总和
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual decimal Sum(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, decimal>> selector)
        {
            using (var db = new DataContext())
            {
                IQueryable<TEntity> query = db.Set<TEntity>();

                if (filter != null)
                {
                    return query.Where(filter).Select(selector).DefaultIfEmpty().Sum();
                }
                return default;
            }
        }

        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
        {
            using (var db = new DataContext())
            {
                IQueryable<TEntity> query = db.Set<TEntity>();

                if (filter != null)
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrWhiteSpace(includeProperties) || !string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includePropertie in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includePropertie);
                    }
                }
                return query.AsNoTracking().FirstOrDefault();

            }
        }

        /// <summary>
        /// 获取相应字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="dynamicPredicate"></param>
        /// <param name="dynamicFunc"></param>
        /// <returns></returns>
        public TEntity GetField(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, dynamic>> dynamicPredicate, Func<dynamic, TEntity> dynamicFunc)
        {
            using (var db = new DataContext())
            {
                db.Set<TEntity>().Where(predicate).AsNoTracking().Select(dynamicPredicate).AsEnumerable()
                                     .Select(dynamicFunc).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 获取字段值(获取部分字段)
        /// </summary>
        /// <param name="dynamicPredicate"></param>
        /// <param name="dynamicFunc"></param>
        /// <param name="fiter"></param>
        /// <param name="orderby"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        
        public virtual IEnumerable<TEntity> Select(Expression<Func<TEntity, dynamic>> dynamicPredicate, Func<dynamic, TEntity> dynamicFunc, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string includeProperties = "")
        {
            using (var db = new DataContext())
            {
                IQueryable<TEntity> query = db.Set<TEntity>();
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrWhiteSpace(includeProperties) || !string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includePropertie in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includePropertie);
                    }
                }

                if (orderby != null)
                {
                    query = orderby(query);
                }
                if (dynamicPredicate != null && dynamicFunc != null)
                {
                    return query.Select(dynamicPredicate).AsEnumerable().Select(dynamicFunc);
                }
                return query.AsNoTracking().ToList();
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="fiter">条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="includeProperties">需要贪婪加载哪一个导航属性</param>
        /// <returns></returns>
        
        public virtual IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> filter = null,
               Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string includeProperties = "", int count = 0)
        {
            using (var db = new DataContext())
            {
                IQueryable<TEntity> query = db.Set<TEntity>();
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                if (!string.IsNullOrWhiteSpace(includeProperties) || !string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includePropertie in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includePropertie);
                    }
                }

                if (orderby != null)
                {
                    query = orderby(query).AsNoTracking();
                }

                if (count != 0)
                {
                    query = query.Take(count);
                }

                return query.AsNoTracking().ToList();
            }
        }

        /// <summary>
        /// 获取分页
        /// </summary>
        /// <param name="totalcount"></param>
        /// <param name="fiter"></param>
        /// <param name="pi"></param>
        /// <param name="pz"></param>
        /// <param name="orderby"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        
        public virtual IEnumerable<TEntity> Select(out int totalcount, Expression<Func<TEntity, bool>> filter = null, int pi = 1, int pz = 0, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null, string includeProperties = "")
        {

            using (var db = new DataContext())
            {
                IQueryable<TEntity> query = db.Set<TEntity>();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includePropertie in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includePropertie);
                }

                if (orderby != null)
                {
                    query = orderby(query).AsNoTracking();
                }
                else
                {
                    query = query.AsNoTracking();
                }

                totalcount = query.Count();

                if (pz != 0)
                {
                    query = query.Skip((pi - 1) * pz).Take(pz);
                }

                return query.AsNoTracking().ToList();
            }
        }

        /// <summary>
        /// 根据sql获取数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetWithDbSetSql(string query, params object[] parameters)
        {
            using (var db = new DataContext())
            {
                var dbSet = db.Set<TEntity>();
                return dbSet.SqlQuery(query, parameters).AsNoTracking().AsEnumerable();
            }
        }


        /// <summary>
        /// 根据sql获取数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> SelectWithDbContextSql<T>(string query, params object[] parameters)
        {
            using (var db = new DataContext())
            {
                return db.Database.SqlQuery<T>(query, parameters);
            }
        }


        /// <summary>
        /// 根据sql获取数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> SelectWithSql<T>(string query, params object[] parameters) where T : class, new()
        {
            using (var db = new DataContext())
            {
                return db.Database.SqlQuery<T>(query, parameters).ToList();
            }
        }

        /// <summary>
        /// 根据sql获取数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IEnumerable<dynamic> SelectWithSql(string query)
        {
            using (var db = new DataContext())
            {
                return db.Database.ExecuteExpandoObjects(query).ToList();
            }
        }

        public virtual IEnumerable<T> SelectWithSql<T>(out int totalCount, string query) where T : class, new()
        {
            using (var db = new DataContext())
            {
                totalCount = db.Database.ExecuteEntities<T>(query).Count();
                return db.Database.ExecuteEntities<T>(query).ToList();
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="totalCount"></param>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> SelectWithSql<T>(out int totalCount, string query, string sort, int pageIndex, int pageSize) where T : class, new()
        {
            using (var db = new DataContext())
            {
                totalCount = db.Database.ExecuteScalarAs<int>(string.Format("select count(*) from ({0}) a", query));

                int pageStart = pageSize * (pageIndex - 1);
                int pageEnd = pageSize * pageIndex;
                string sqlPage =
                    string.Format(@"SELECT * FROM ( 
                SELECT ROW_NUMBER() OVER( {0}) AS RowId, * FROM ({1}) a  
                ) AS t  WHERE t.RowId BETWEEN {2} AND {3}", sort, query, pageStart, pageEnd);

                return db.Database.ExecuteEntities<T>(sqlPage).ToList();
            }
        }

        /// <summary>
        /// 根据sql获取分页数据
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns>yp</returns>
        public virtual IEnumerable<T> SelectWithSql<T>(out int totalCount, string query, int pageIndex, int pageSize) where T : class, new()
        {
            using (var db = new DataContext())
            {
                totalCount = db.Database.ExecuteEntities<T>(query).Count();
                return db.Database.ExecuteEntities<T>(query).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }

            //using (var db = new DataContext())
            //{
            //    totalCount = db.Database.SqlQuery<T>(query).Count();
            //    return db.Database.SqlQuery<T>(query).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            //}
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool Insert(TEntity entity)
        {
            try
            {
                using (var db = new DataContext())
                {
                    db.Set<TEntity>().Add(entity);

                    return db.SaveChanges() > 0;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var msg = string.Empty;
                var errors = (from u in ex.EntityValidationErrors select u.ValidationErrors).ToList();
                foreach (var item in errors)
                    msg += item.FirstOrDefault().ErrorMessage;
                Utils.Log.LogInfoWriter.GetInstance().Error(msg);
                return false;
            }
            
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity InsertGet(TEntity entity)
        {
            using (var db = new DataContext())
            {
                db.Set<TEntity>().Add(entity);
                db.SaveChanges();
                return entity;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        public virtual bool Delete(object col)
        {
            using (var db = new DataContext())
            {
                TEntity entityToDelete = db.Set<TEntity>().Find(col);
                if (entityToDelete != null)
                {
                    return Delete(entityToDelete);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="predicate"></param>
        public virtual bool Delete(Expression<Func<TEntity, bool>> predicate)
        {
            TEntity entityToDelete = Get(predicate);
            if (entityToDelete != null)
            {
                return Delete(entityToDelete);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entityToDelete">删除的实体对象</param>
        public virtual bool Delete(TEntity entityToDelete)
        {
            using (var db = new DataContext())
            {
                var dbSet = db.Set<TEntity>();
                if (db.Entry(entityToDelete).State == EntityState.Detached)
                {
                    dbSet.Attach(entityToDelete);
                }
                dbSet.Remove(entityToDelete);
                return db.SaveChanges() > 0;
            }

        }

        /// <summary>
        /// 批量删除。删除失败的以 IEnumerable<T> 形式返回
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>IEnumerable<T>：失败数据</returns>
        public virtual IEnumerable<TEntity> DeleteAll(Expression<Func<TEntity, bool>> predicate)
        {
            using (var db = new DataContext())
            {
                var dbSet = db.Set<TEntity>();
                List<TEntity> tList = new List<TEntity>();
                var collect = dbSet.Where(predicate);
                try
                {
                    foreach (var item in collect)
                    {
                        Delete(item);
                        tList.Add(item);
                    }
                    db.SaveChanges();
                    return null;
                }
                catch (Exception)
                {
                    db.SaveChanges();
                    return collect.ToList().Except(tList);
                }
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entity"></param>
        public virtual IEnumerable<TEntity> InsertAll(List<TEntity> list)
        {
            using (var db = new DataContext())
            {
                var dbSet = db.Set<TEntity>();
                List<TEntity> tList = new List<TEntity>();
                foreach (var item in list)
                {
                    try
                    {
                        db.Set<TEntity>().Add(item);

                    }
                    catch (Exception)
                    {
                        tList.Add(item);
                        throw;
                    }
                }
                db.SaveChanges();
                return tList;
            }

        }

        /// <summary>
        /// 批量更新（list 中实体必须为 数据库中查找到的实体）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> UpdateAll(List<TEntity> list)
        {
            using (var db = new DataContext())
            {
                var dbSet = db.Set<TEntity>();
                List<TEntity> tList = new List<TEntity>();
                foreach (var item in list)
                {

                    try
                    {
                        var entity = dbSet.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                    }
                    catch (Exception)
                    {
                        tList.Add(item);
                        throw;
                    }
                }
                db.SaveChanges();
                return tList;
            }
        }

        /// <summary>
        /// 全部实体更新
        /// </summary>
        /// <param name="entityToUpdate">更新的实体(必须为数据库中查找到的实体)</param>
        public virtual bool Update(TEntity entityToUpdate)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var dbSet = db.Set<TEntity>();
                    var entity = dbSet.Attach(entityToUpdate);
                    db.Entry(entityToUpdate).State = EntityState.Modified;
                    return db.SaveChanges() > 0;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var msg = string.Empty;
                var errors = (from u in ex.EntityValidationErrors select u.ValidationErrors).ToList();
                foreach (var item in errors)
                    msg += item.FirstOrDefault().ErrorMessage;
                Utils.Log.LogInfoWriter.GetInstance().Error(msg);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// 更新部分字段的实体
        /// </summary>
        /// <param name="model"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public virtual bool Update(TEntity model, string[] arr)
        {
            using (var db = new DataContext())
            {
                db.Set<TEntity>().Attach(model);

                var stateEntry = ((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.GetObjectStateEntry(model);

                stateEntry.SetModified();

                foreach (string item in arr)
                    stateEntry.SetModifiedProperty(item);

                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="models"></param>
        public virtual void BulkUpdate(IEnumerable<TEntity> models)
        {
            using (var db = new DataContext())
            {
                db.BulkUpdate(models);
            }
        }

        public virtual void BulkInsert(IEnumerable<TEntity> models)
        {
            using (var db = new DataContext())
            {
                db.BulkInsert(models);
            }
        }
        #endregion
    }
}
