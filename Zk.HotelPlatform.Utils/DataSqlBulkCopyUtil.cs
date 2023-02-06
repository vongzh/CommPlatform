using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Utils
{
    public class DataSqlBulkCopyUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="sqlconn"></param>
        public static void Sqlbulkcopy<T>(List<T> data, string tableName, string connString = "")
        {
            if (string.IsNullOrWhiteSpace(connString))
            {
                connString = @"Server=192.168.1.252;Database=HotelPlatform;user id=sa;password=123456";
            }
            #region 待处理数据初始化处理
            List<PropertyInfo> pList = new List<PropertyInfo>();//创建属性的集合
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列    
            Array.ForEach<PropertyInfo>(typeof(T).GetProperties(), p =>
            {
                pList.Add(p);
                Type colType = p.PropertyType;
                if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    colType = colType.GetGenericArguments()[0];
                }
                dt.Columns.Add(p.Name, colType);
            });  //获得反射的入口（typeof（）） //要对 array 的每个元素执行的 System.Action。
            foreach (var item in data)
            {
                DataRow row = dt.NewRow(); //创建一个DataRow实例  
                pList.ForEach(p =>
                {

                    row[p.Name] = p.GetValue(item, null) ?? DBNull.Value;
                }
                    ); //给row 赋值
                dt.Rows.Add(row); //加入到DataTable    
            }
            #endregion

            #region 批量插入数据库 SqlBulkCopy声明及参数设置
            using (SqlBulkCopy sbc = new SqlBulkCopy(connString, SqlBulkCopyOptions.UseInternalTransaction))
            {
                sbc.DestinationTableName = tableName; /*设置数据库目标表名称*/

                sbc.BatchSize = dt.Rows.Count; /*每一批次中的行数*/
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sbc.ColumnMappings.Add(dt.Columns[i].ColumnName, i);
                }
                try
                {
                    sbc.WriteToServer(dt);
                }
                catch (Exception ex)
                {
                    LogInfoWriter.GetInstance().Error("ex:", ex);
                    throw;
                }
            };


            //bulk.ColumnMappings.Add("Id", "Id"); //设置数据源中的列和目标表中的列之间的映射关系                            
            //bulk.ColumnMappings.Add("Name", "Name");
            //bulk.ColumnMappings.Add("Age", "Age");
            #endregion

        }

        public static void Sqlbulkcopy<T>(ConcurrentBag<T> data, string tableName, string connString = "")
        {
            if (string.IsNullOrWhiteSpace(connString))
            {
                connString = @"Server=192.168.1.252;Database=HotelPlatform;user id=sa;password=123456";
            }
            #region 待处理数据初始化处理
            List<PropertyInfo> pList = new List<PropertyInfo>();//创建属性的集合
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列    
            Array.ForEach<PropertyInfo>(typeof(T).GetProperties(), p =>
            {
                pList.Add(p);
                Type colType = p.PropertyType;
                if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    colType = colType.GetGenericArguments()[0];
                }
                dt.Columns.Add(p.Name, colType);
            });  //获得反射的入口（typeof（）） //要对 array 的每个元素执行的 System.Action。
            foreach (var item in data)
            {

                if (item == null)
                {
                    continue;
                }
                DataRow row = dt.NewRow(); //创建一个DataRow实例 

                pList.ForEach(p =>
                {
                    row[p.Name] = p.GetValue(item, null) ?? DBNull.Value;
                }
                    ); //给row 赋值
                dt.Rows.Add(row); //加入到DataTable    
            }
            #endregion
            #region 批量插入数据库 SqlBulkCopy声明及参数设置
            using (SqlBulkCopy sbc = new SqlBulkCopy(connString, SqlBulkCopyOptions.UseInternalTransaction))
            {
                sbc.DestinationTableName = tableName; /*设置数据库目标表名称*/
                sbc.BatchSize = dt.Rows.Count; /*每一批次中的行数*/
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sbc.ColumnMappings.Add(dt.Columns[i].ColumnName, i);
                }
                sbc.WriteToServer(dt);
            };

            //bulk.ColumnMappings.Add("Id", "Id"); //设置数据源中的列和目标表中的列之间的映射关系                                                 // bulk.ColumnMappings.Add("FlightId", "FlightId");//ColumnMappings.Add("源数据表列名称", "目标表数据列名称");
            //bulk.ColumnMappings.Add("Name", "Name");
            //bulk.ColumnMappings.Add("Age", "Age");
            #endregion
        }



        /// <summary>
        /// SqlBulkCopy 批量更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="crateTemplateSql"></param>
        /// <param name="updateSql"></param>
        public static void BulkUpdateData<T>(List<T> list, string crateTemplateSql, string updateSql)
        {
            var dataTable = ConvertToDataTable(list);
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["HotelPlatform"].ConnectionString))
            {
                using (var command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();
                        //数据库并创建一个临时表来保存数据表的数据
                        command.CommandText = $" CREATE TABLE #TmpTable ({crateTemplateSql})";
                        command.ExecuteNonQuery();

                        //使用SqlBulkCopy 加载数据到临时表中
                        using (var bulkCopy = new SqlBulkCopy(conn))
                        {
                            foreach (DataColumn dcPrepped in dataTable.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(dcPrepped.ColumnName, dcPrepped.ColumnName);
                            }

                            bulkCopy.BulkCopyTimeout = 660;
                            bulkCopy.DestinationTableName = "#TmpTable";
                            bulkCopy.WriteToServer(dataTable);
                            bulkCopy.Close();
                        }

                        // 执行Command命令 使用临时表的数据去更新目标表中的数据  然后删除临时表
                        command.CommandTimeout = 30000;
                        command.CommandText = updateSql;
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        /// <summary>
        /// List转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
