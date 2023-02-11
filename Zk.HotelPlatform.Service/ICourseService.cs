using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface ICourseService
    {
        IEnumerable<Course> QueryCourses(CourseQueryRequest queryRequest);
        bool SaveCourse(Course course, int userId);
        Course GetCourse(int id);
    }
}
