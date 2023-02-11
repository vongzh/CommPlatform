using System;
using System.Collections.Generic;
using System.Web.Http;
using Zk.HotelPlatform.Api.Filters;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Service;

namespace Zk.HotelPlatform.Api.Controllers
{
#if !DEBUG
    [Authorize(Roles = "Client")]
#endif
    [ResponseHandler]
    public class DataController : BaseController
    {
        private readonly ICourseService _courseService = null;
        private readonly ISchemeService _schemeService = null;

        public DataController(ICourseService courseService, ISchemeService schemeService)
        {
            this._courseService = courseService;
            this._schemeService = schemeService;
        }

        [HttpPost]
        [Route("Course/Query")]
        public IEnumerable<Course> GetCourses(CourseQueryRequest queryRequest)
        {
            if (queryRequest == null)
                queryRequest = new CourseQueryRequest();

            return this._courseService.QueryCourses(queryRequest);
        }

        [HttpPost]
        [SysAuthorize]
        [Route("Course/Save")]
        public bool SaveCourse(Course course)
        {
            if (string.IsNullOrWhiteSpace(course.CourseName))
                throw new ArgumentNullException("课程名称");
            if (course.Duration <= 0)
                throw new ArgumentNullException("课程周期");
            if (course.Price <= 0)
                throw new ArgumentNullException("课程价格");

            return this._courseService.SaveCourse(course, CurrentSysUser.UserId);
        }

        [HttpPost]
        [Route("Scheme/Query")]
        public IEnumerable<Scheme> GetSchemes()
        {
            return this._schemeService.QuerySchemes();
        }

        [HttpPost]
        [SysAuthorize]
        [Route("Scheme/Save")]
        public bool SaveScheme(Scheme scheme)
        {
            if (string.IsNullOrWhiteSpace(scheme.SchemeName))
                throw new ArgumentNullException("课程名称");
            if (scheme.ServiceRate <= 0)
                throw new ArgumentNullException("服务费比例");
            if (scheme.TotalNumber <= 0)
                throw new ArgumentNullException("课程价格");
            return this._schemeService.SaveScheme(scheme, CurrentSysUser.UserId);
        }
    }
}
