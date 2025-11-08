using Microsoft.EntityFrameworkCore;
using OnlineCourse.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Data
{
    public class CourseCategoryRepository : ICourseCategoryRepository
    {
         
        private readonly OnlineCourseDbContext _db ;

        public CourseCategoryRepository(OnlineCourseDbContext db)
        {
            _db = db;
        }

        public async  Task<CourseCategory>? GetByIdAsync(int id)
        {
                var data = await  _db.CourseCategories.FindAsync(id).AsTask();
            return data;

             
        }

        public async  Task<List<CourseCategory>> GetCourseCategoriesAsync()
        {
            var data = await _db.CourseCategories.ToListAsync();
            return data;
        }
    }
}
