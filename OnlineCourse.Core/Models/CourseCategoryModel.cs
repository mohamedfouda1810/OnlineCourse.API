using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Core.Models
{
    public  class CourseCategoryModel
    {
        public int CategoryId { get; set; }

  
        public string CategoryName { get; set; } = null!;

   
        public string? Description { get; set; }

    }
}
