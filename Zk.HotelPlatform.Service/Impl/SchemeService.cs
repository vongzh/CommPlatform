using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SchemeService : DataService<Scheme>, ISchemeService
    {
        public SchemeService(IDataProvider<Scheme> dataProvider) : base(dataProvider)
        {

        }

        public Scheme GetScheme(int id)
        {
            return base.Get(x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
        }

        /// <summary>
        /// 查询课程
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public IEnumerable<Scheme> QuerySchemes()
        {
            Expression<Func<Scheme, bool>> filter = x => x.Id > 0;

            return base.Select(filter);
        }

        /// <summary>
        /// 查询课程
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public bool SaveScheme(Scheme scheme, int userId)
        {
            if (scheme.Id <= 0)
            {
                var exist = base.Get(x => x.SchemeName == scheme.SchemeName);
                if (exist != null)
                    throw new BusinessException("方案名称已存在");

                scheme.CreateTime = DateTime.Now;
                scheme.IsDelete = (int)GlobalEnum.YESOrNO.N;
                var entity = base.AddAndGetEntity(scheme);
                return entity != null && entity.Id > 0;
            }
            else
            {
                var exist = base.Get(x => x.SchemeName == scheme.SchemeName);
                if (exist != null)
                {
                    if (exist.Id != scheme.Id)
                    {
                        throw new BusinessException("方案名称已存在");
                    }
                }

                var editScheme = base.Get(x => x.Id == scheme.Id);

                editScheme.SchemeName = scheme.SchemeName;
                editScheme.ServiceRate = scheme.ServiceRate;
                editScheme.TotalNumber = scheme.TotalNumber;
                editScheme.UpdateTime = DateTime.Now;
                editScheme.IsDelete = scheme.IsDelete;
                return base.Update(editScheme);
            }
        }
    }
}
