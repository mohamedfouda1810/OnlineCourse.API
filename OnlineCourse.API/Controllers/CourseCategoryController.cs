using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Service;

namespace OnlineCourse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseCategoryController : ControllerBase
    {
        private readonly ICourseCategoryService _service;

        public CourseCategoryController(ICourseCategoryService service)
        {
            _service = service; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetCourseCategoriesAsync();
           if(data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get( int id)
        {
            var data = await _service.GetByIdAsync(id);
          
            return Ok(data);
        }
    }
}
