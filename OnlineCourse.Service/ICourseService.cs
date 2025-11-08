using OnlineCourse.Core.Models;
using OnlineCourse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Service
{
    public interface ICourseService
    {
        Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null);
        Task<CourseDetailModel> GetCourseDetailAsync(int courseId);
    }
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository repo;
        public CourseService(ICourseRepository repository)
        {
            repo = repository;
        }   
        public Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null)
        {
           return repo.GetAllCoursesAsync(categoryId);  
        }

        public Task<CourseDetailModel> GetCourseDetailAsync(int courseId)
        {
           
            return repo.GetCourseDetailAsync( courseId);
        }
    }
}
