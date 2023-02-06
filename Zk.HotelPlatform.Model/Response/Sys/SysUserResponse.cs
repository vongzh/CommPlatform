using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Response
{
    public class SysUserResponse : BaseSysUser
    {
        public int UserType { get; set; }
        public string UserTypeDesc { get; set; }
        public string CustomerNo { get; set; }
        public new string Avatar { get; set; } = "https://avatars0.githubusercontent.com/u/12293185?s=40&amp;v=4";
        public int Status { get; set; }
        public bool IsDefaultPwd { get; set; }
        public string StatusDesc { get; set; }
        public int Disabled { get; set; }
        public string DepartmentId { get; set; }
        public string QyWorkUserId { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public IEnumerable<SysModuleResponse> Modules { get; set; }
        public IEnumerable<SysRoleResponse> UserRoles { get; set; }
        public IEnumerable<SysMenuResponse> RoleMenus { get; set; }
    }

    public class SysUserClient : BaseSysUser
    {
        private string _mail;
        public new string Mail
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(_mail))
                    {
                        return string.Empty;
                    }

                    var idx = _mail.IndexOf("@");
                    var cnt = _mail.Substring(idx - 3, 3);
                    return _mail.Replace(cnt, "***");

                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            set
            {
                _mail = value;
            }
        }

        private string _mobile;
        public new string Mobile
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(_mobile))
                    {
                        return string.Empty;
                    }
                    var idx = _mobile.Substring(3, 4);
                    return _mobile.Replace(idx, "****");
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            set
            {
                _mobile = value;
            }
        }
    }
}
