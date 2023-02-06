using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class RegexUtil
    {


        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static int GetStringLength(string stringValue)
        {
            return Encoding.Default.GetBytes(stringValue).Length;
        }

        /// <summary>
        /// 用户名正则验证
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool UserRegexMatch(string userName)
        {
            return Regex.IsMatch(userName, @"^[a-zA-Z][A-Za-z0-9]{5,18}$");
            //@"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,18}$"
            // @"[a-zA-Z0-9]{6,18}$"
        }
        /// <summary>
        /// 密码正则验证
        /// </summary>
        /// <param name="userPwd"></param>
        /// <returns></returns>
        public static bool PwdRegexMatch(string userPwd)
        {
            //return Regex.IsMatch(userPwd, @"^(?![^a-zA-Z]+$)^(?![^0-9]+$)^(?![^\W]+$)^[^\s\u4e00-\u9fa5]{7,20}$");
            //@"(?![A-Z]+$)(?![a-z]+$)(?!\d+$)(?![\W]+$){7,20}$"
            // @"^(?![a-zA-Z]+$)(?![0-9]+$)(?![!@#$\%\^\&\*\(\)])[0-9A-Za-z!@#$\%\^\&\*\(\)]{8,20}$"；
            return Regex.IsMatch(userPwd, @"^(?![a-zA-Z]+$)(?![0-9]+$)(?![!@#$\%\^\&\*\(\)])[0-9A-Za-z!@#$\%\^\&\*\(\)]{8,20}$");
            //@"[a-zA-Z0-9]{7,20}$"
        }

        /// <summary>
        /// 手机号正则验证
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool PhoneRegexMatch(string phone)
        {
            return Regex.IsMatch(phone, @"^[1](([3][0-9])|([4][1-9])|([5][0-3,4-9])|([6][2,5,6,7])|([7][0-8])|([8][0-9])|([9][0,1,2,3,5,6,7,8,9]))[0-9]{8}$");
            // return Regex.IsMatch(phone, @"^[1](([3][0-9])|([4][5-9])|([5][0-3,5-9])|([6][5,6])|([7][0-8])|([8][0-9])|([9][1,8,9]))[0-9]{8}$");
            //@"^[1]+[3,5,6,7,8,9]+\d{9}$"
        }
 

        public static bool MailRegexMatch(string mail)
        {
            return Regex.IsMatch(mail, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

        }

        /// <summary>
        /// 营业执照号验证
        /// </summary>
        /// <param name="businessLicenseNo"></param>
        /// <returns></returns>
        public static bool BusinessLicenseNoRegexMatch(string businessLicenseNo)
        {
            return Regex.IsMatch(businessLicenseNo, @"^(?![a-zA-Z]+$)[A-Za-z0-9]{18,18}$");
        }

        /// <summary>
        /// 身份证号验证
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public static bool IDNumberRegexMatch(string IDNo)
        {
            return Regex.IsMatch(IDNo, @"^[1-9]\d{5}(18|19|20|(3\d))\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$");
        }

        /// <summary>
        /// 姓名的规则验证
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool NameMathch(string name)
        {
            return Regex.IsMatch(name, @"^[\u4E00-\u9FA5]+[·\u4E00-\u9FA5]+$");
        }
    }
}
