using OnlineCourse.Core.Models;
using OnlineCourse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Service
{
    public  interface ICourseCategoryService
    {
        Task<CourseCategoryModel>? GetByIdAsync(int id);
        Task<List<CourseCategoryModel>> GetCourseCategoriesAsync();

    }

    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly ICourseCategoryRepository _courseCategoryRepository;   
        public CourseCategoryService(ICourseCategoryRepository courseCategoryRepository)
        {
            _courseCategoryRepository = courseCategoryRepository;
        }
        public async  Task<CourseCategoryModel>? GetByIdAsync(int id)
        {
            var data = await _courseCategoryRepository.GetByIdAsync(id); 
          

            return new CourseCategoryModel
            {
                CategoryId = data.CategoryId,
                CategoryName = data.CategoryName,
                Description = data.Description
            };




        }

        public async Task<List<CourseCategoryModel>> GetCourseCategoriesAsync()
        {
            var data = await _courseCategoryRepository.GetCourseCategoriesAsync();  
            var result = data.Select(c => new CourseCategoryModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToList();    

            return result;
        }
    }
}
