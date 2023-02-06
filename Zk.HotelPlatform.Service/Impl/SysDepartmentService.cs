using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.Containers;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SysDepartmentService : DataService<SysDepartment>, ISysDepartmentService
    {
        private readonly IMapper _mapper = null;
        public SysDepartmentService(IMapper mapper, IDataProvider<SysDepartment> dataProvider) : base(dataProvider)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 查询部门信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SysDepartment> GetDepartments(int[] deptIds)
        {
            Expression<Func<SysDepartment, bool>> filter = x => x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            if (deptIds != null && deptIds.Length > 0)
            {
                filter = filter.And(x => deptIds.Contains(x.Id));
            }

            return base.Select(filter);
        }

        /// <summary>
        /// 从企业微信同步部门
        /// </summary>
        /// <returns></returns>
        public void SyncDepartments()
        {
            var weixinSetting = Senparc.Weixin.Config.SenparcWeixinSetting.WorkSetting;
            var qyDepartmentsRes = MailListApi.GetDepartmentList(AccessTokenContainer.TryGetToken(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret));

            if (qyDepartmentsRes.errcode != Senparc.Weixin.ReturnCode_Work.请求成功)
            {
                throw new BusinessException(qyDepartmentsRes.errmsg);
            }

            var qyDepartments = qyDepartmentsRes.department;
            var departments = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            foreach (var qyDept in qyDepartments)
            {
                var dept = departments.FirstOrDefault(x => x.ThirdId == qyDept.id);

                // 上级部门
                var patentDept = departments.FirstOrDefault(x => x.ThirdId == qyDept.parentid);
                if (dept == null)
                {
                    base.AddEntity(new SysDepartment()
                    {
                        ThirdId = qyDept.id,
                        ParentId = patentDept?.Id,
                        ThirdParentId = qyDept.parentid,
                        DepartmentName = qyDept.name,
                        Order = qyDept.order,
                        CreateTime = DateTime.Now,
                        SyncTime = DateTime.Now,
                        IsDelete = (int)GlobalEnum.YESOrNO.N
                    });
                }
                else
                {
                    dept.DepartmentName = qyDept.name;
                    dept.Order = qyDept.order;
                    dept.ParentId = patentDept?.Id;
                    dept.ThirdParentId = qyDept.parentid;
                    dept.UpdateTime = DateTime.Now;
                    dept.SyncTime = DateTime.Now;
                    base.Update(dept);
                }
            }
        }

        public IEnumerable<SysDepartmentResponse> GetSysDepartments(SysDepartmentQueryRequest queryRequest)
        {
            Expression<Func<SysDepartment, bool>> filter = x => x.IsDelete == (int)GlobalEnum.YESOrNO.N;

            if (!string.IsNullOrWhiteSpace(queryRequest.DeptName))
            {
                filter = filter.And(x => x.DepartmentName.Contains(queryRequest.DeptName));
            }
            
            var departments = base.Select(filter);
            if (departments == null)
            {
                return new List<SysDepartmentResponse>();
            }

            var departmentResponses = _mapper.Map<IEnumerable<SysDepartment>, IEnumerable<SysDepartmentResponse>>(departments);
            foreach (var departmentResponse in departmentResponses)
            {
                if (departmentResponse.ParentId != null)
                {
                    var parent = departmentResponses.FirstOrDefault(x => x.Id == departmentResponse.ParentId);
                    departmentResponse.ParentDepartment = parent == null ? new SysDepartmentResponse() : parent;
                }
            }
            return departmentResponses;
        }
    }
}
