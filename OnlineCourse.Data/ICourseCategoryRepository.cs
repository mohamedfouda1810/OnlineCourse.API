using OnlineCourse.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Data
{
    public interface ICourseCategoryRepository
    {
        Task<CourseCategory>? GetByIdAsync(int id);    
       Task< List<CourseCategory> >GetCourseCategoriesAsync();


    }
}
