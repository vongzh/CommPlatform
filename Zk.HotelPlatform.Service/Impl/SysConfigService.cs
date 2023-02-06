using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SysConfigService : DataService<SysConfig>, ISysConfigService
    {
        private IOperationLogService _operationLogService = null;

        public ISysUserService SysUserService { get; set; }

        public SysConfigService(IDataProvider<SysConfig> dataProvider, IOperationLogService operationLogService)
          : base(dataProvider)
        {
            _operationLogService = operationLogService;
        }

        /// <summary>
        /// 系统默认密码
        /// </summary>
        /// <returns></returns>

        public string GetSysUserDefaultPwd()
        {
            string sysUserDefaultPwd = this.GetConfig<string>("business", "SysUserDefaultPwd");
            if (string.IsNullOrWhiteSpace(sysUserDefaultPwd))
            {
                sysUserDefaultPwd = "zknet";
            }
            return sysUserDefaultPwd;
        }

        /// <summary>
        /// 是否允许注册
        /// </summary>
        /// <returns></returns>
        public bool GetAllowRegister()
        {
            int allowRegister = this.GetConfig<int>("business", "AllowRegister");
            return Convert.ToBoolean(allowRegister);
        }

        /// <summary>
        /// 邮箱发送限制次数
        /// </summary>
        /// <returns></returns>
        public int GetMailSendLimitCount()
        {
            var count = this.GetConfig<int>("business", "MailSendLimitCount");
            if (count == 0) count = 10;
            return count;
        }


        /// <summary>
        /// 登录错误次数
        /// </summary>
        /// <returns></returns>
        public int GetLoginErrorLimitCount()
        {
            int loginErrorCount = this.GetConfig<int>("business", "LoginErrorLimitCount");
            if (loginErrorCount <= 0)
            {
                loginErrorCount = 10;
            }
            return loginErrorCount;
        }

        /// <summary>
        /// 取消时间
        /// </summary>
        /// <returns></returns>
        public int GetCancelEarlyTime()
        {
            int cancelEarlyTime = this.GetConfig<int>("business", "CancelEarlyTime");
            if (cancelEarlyTime <= 0)
            {
                cancelEarlyTime = 10;
            }
            return cancelEarlyTime;
        }


        /// <summary>
        /// 是否允许注册
        /// </summary>
        /// <returns></returns>
        public bool GetQunarIncludeNotInstanceConfim()
        {
            try
            {
                int includeNotInstanceConfim = this.GetConfig<int>("qunar", "IncludeNotInstanceConfim");
                return Convert.ToBoolean(includeNotInstanceConfim);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public T GetConfig<T>(string domain, string keyName)
        {
            try
            {
                var config = base.Get(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N && x.Domain.ToLower() == domain.ToLower() && x.Key.ToLower() == keyName.ToLower());
                if (config == null)
                    return default;

                var val = config.Value;
                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch (Exception)
            {
                return default;
            }
        }

        public PageResult<SysConfig> GetDataConfigs(SysConfigQueryRequest queryRequest)
        {
            Expression<Func<SysConfig, bool>> filter = x => x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            if (!string.IsNullOrWhiteSpace(queryRequest.Key))
            {
                filter = filter.And(x => x.Key.Contains(queryRequest.Key.Trim()));
            }

            IOrderedQueryable<SysConfig> orderby(IQueryable<SysConfig> x) => x.OrderByDescending(z => z.Id);

            var logs = base.Select(out int totalCount, filter, queryRequest.PageIndex, queryRequest.PageSize, orderby);
            if (logs == null || logs.Count() == 0)
                return new PageResult<SysConfig>()
                {
                    Rows = new List<SysConfig>(),
                    Total = 0
                };

            return new PageResult<SysConfig>()
            {
                Total = totalCount,
                Rows = logs
            };
        }

        public bool SaveDataConfig(SysConfig sysConfig, int userId)
        {
            if (sysConfig.Id <= 0)
            {
                var cfg = base.Get(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N && x.Domain == sysConfig.Domain && x.Key == sysConfig.Key);
                if (cfg != null)
                {
                    throw new BusinessException("数据配置已存在");
                }

                sysConfig.Creator = userId;
                sysConfig.CreateTime = DateTime.Now;
                var entity = base.AddAndGetEntity(sysConfig);
                var ret = entity != null && entity.Id > 0;
                if (ret)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = entity.Id.ToString(),
                        LogContent = $"新增系统配置{ entity.KeyName }",
                        ChangedFields = JsonConvert.SerializeObject(new
                        {
                            sysConfig.Id,
                            sysConfig.KeyName,
                            sysConfig.Key,
                            sysConfig.ValueName,
                            sysConfig.Value,
                            sysConfig.Group,
                        }),
                        ResourceType = (int)GlobalEnum.ResourceType.NEW_SYSCONFIG,
                    });
                }
                return ret;
            }
            else
            {
                var cfg = base.Get(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N && x.Domain == sysConfig.Domain && x.Key == sysConfig.Key && x.Id != sysConfig.Id);
                if (cfg != null)
                {
                    throw new BusinessException("数据配置已存在");
                }

                var tempCfg = cfg.DeepClone();

                sysConfig.ModifyTime = DateTime.Now;
                sysConfig.Modifer = userId;
                var ret = base.Update(sysConfig);
                if (ret)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = sysConfig.Id.ToString(),
                        LogContent = $"修改系统配置{ sysConfig.KeyName }",
                        ChangedFields = tempCfg.GetChangedFields(sysConfig),
                        ResourceType = (int)GlobalEnum.ResourceType.EDIT_SYSCONFIG,
                    });
                }
                return ret;
            }
        }

        public bool Delete(int id, int userId)
        {
            var sysConfig = base.Get(x => x.Id == id);
            if (sysConfig == null)
                throw new BusinessException("未找到系统配置");
            if (sysConfig.IsDelete == (int)GlobalEnum.YESOrNO.Y)
                throw new BusinessException("系统配置已经删除");

            sysConfig.Modifer = userId;
            sysConfig.ModifyTime = DateTime.Now;
            sysConfig.IsDelete = (int)GlobalEnum.YESOrNO.Y;
            var ret = base.Update(sysConfig);
            if (ret)
            {
                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = sysConfig.Id.ToString(),
                    LogContent = $"删除系统配置{ sysConfig.KeyName }",
                    ChangedFields =
                    JsonConvert.SerializeObject(new
                    {
                        sysConfig.Id,
                        sysConfig.KeyName,
                        sysConfig.Key,
                        sysConfig.ValueName,
                        sysConfig.Value,
                        sysConfig.Group,
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.DELETE_SYSCONFIG,
                });
            }
            return ret;
        }
    }
}
