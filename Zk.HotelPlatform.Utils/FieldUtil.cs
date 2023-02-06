using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Utils
{
    public static class FieldUtil
    {
        /// <summary>
        /// 忽略的列名列表
        /// </summary>
        private static List<string> ignoreColumnList = new List<string>()
        {
            "CreateTime",
            "UpdateTime",
            "ModifyTime"
        };

        private static HashSet<Type> PrimitiveTypes = null;

        static FieldUtil()
        {
            PrimitiveTypes = new HashSet<Type>()
                {
                    typeof(String),
                    typeof(Byte[]),
                    typeof(Byte),
                    typeof(Int16),
                    typeof(Int32),
                    typeof(Int64),
                    typeof(Single),
                    typeof(Double),
                    typeof(decimal),
                    typeof(DateTime),
                    typeof(Guid),
                    typeof(Boolean),
                    typeof(TimeSpan),
                    typeof(Byte?),
                    typeof(Int16?),
                    typeof(Int32?),
                    typeof(Int64?),
                    typeof(Single?),
                    typeof(Double?),
                    typeof(decimal?),
                    typeof(DateTime?),
                    typeof(Guid?),
                    typeof(Boolean?),
                    typeof(TimeSpan?)
                };
        }

        public static T DeepClone<T>(this T newEntity) where T : class
        {
            var jsonObj = JsonConvert.SerializeObject(newEntity);
            return JsonConvert.DeserializeObject<T>(jsonObj);
        }

        public static string GetChangedFields<T>(this T newEntity, T oldEntity) where T : class
        {
            try
            {
                Type entityType = typeof(T);
                PropertyInfo[] properties = entityType.GetProperties().Where(o => o.CanWrite
                && PrimitiveTypes.Contains(o.PropertyType)
                && !o.GetCustomAttributes(false).OfType<NotMappedAttribute>().Any()).ToArray();

                dynamic changedFields = new ExpandoObject();
                foreach (var p in properties)
                {
                    object oldValue = p.GetValue(oldEntity, null);
                    object newValue = p.GetValue(newEntity, null);

                    if ((oldValue == null && newValue == null))
                    {
                        continue;
                    }
                    else if (oldValue == null && newValue != null || oldValue != null && newValue == null || !Eq(p.PropertyType, oldValue, newValue))
                    {
                       
                        ((IDictionary<string, object>)changedFields).Add(p.Name, new
                        {
                            old = oldValue ?? "NULL",
                            now = newValue ?? "NULL"
                        });
                    }
                }
                return JsonConvert.SerializeObject(changedFields);
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);

                return string.Empty;
            }
        }

        private static bool Eq(Type propertyType, object oldValue, object newValue)
        {
            if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            {
                return decimal.Parse(oldValue.ToString()) == decimal.Parse(newValue.ToString());
            }
            else
            {
                return string.Equals(oldValue.ToString(), newValue.ToString());
            }
        }
    }
}
