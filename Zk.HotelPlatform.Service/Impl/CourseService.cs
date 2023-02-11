using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class CourseService : DataService<Course>, ICourseService
    {
        public CourseService(IDataProvider<Course> dataProvider) : base(dataProvider)
        {

        }

        public Course GetCourse(int id)
        {
            return base.Get(x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
        }

        /// <summary>
        /// 查询课程
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public IEnumerable<Course> QueryCourses(CourseQueryRequest queryRequest)
        {
            Expression<Func<Course, bool>> filter = x => x.Id > 0;

            if (!string.IsNullOrWhiteSpace(queryRequest.CourseName))
            {
                filter = filter.And(x => x.CourseName.Contains(queryRequest.CourseName));
            }
            if (queryRequest.IsDelete.HasValue)
            {
                filter = filter.And(x => x.IsDelete == queryRequest.IsDelete);
            }

            return base.Select(filter);
        }

        /// <summary>
        /// 查询课程
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public bool SaveCourse(Course course, int userId)
        {
            if (course.Id <= 0)
            {
                var exist = base.Get(x => x.CourseName == course.CourseName);
                if (exist != null)
                    throw new BusinessException("课程名称已存在");

                course.CreateTime = DateTime.Now;
                course.IsDelete = (int)GlobalEnum.YESOrNO.N;
                var entity = base.AddAndGetEntity(course);
                return entity != null && entity.Id > 0;
            }
            else
            {
                var exist = base.Get(x => x.CourseName == course.CourseName);
                if (exist != null)
                {
                    if (exist.Id != course.Id)
                    {
                        throw new BusinessException("课程名称已存在");
                    }
                }

                exist.CourseName = course.CourseName;
                exist.Duration = course.Duration;
                exist.Price = course.Price;
                exist.UpdateTime = DateTime.Now;
                exist.IsDelete = course.IsDelete;
                return base.Update(exist);
            }
        }
    }
}
