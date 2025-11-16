using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Service;

namespace OnlineCourse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _service;
        public CourseController(ICourseService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId = null)
        {
            var data = await _service.GetAllCoursesAsync();
        
            return Ok(data);
        }
        [HttpGet("Category/{categoryId}")]
        public async Task<IActionResult> GetByCategory([FromRoute]int categoryId)
        {
            var data = await _service.GetAllCoursesAsync(categoryId);
        
            return Ok(data);
        }


        [HttpGet("Detail/{courseId}")]
        public async Task<IActionResult> Get(int courseId)
        {
            var data = await _service.GetCourseDetailAsync(courseId);
         
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
