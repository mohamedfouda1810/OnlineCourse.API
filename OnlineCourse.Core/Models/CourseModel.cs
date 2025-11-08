using OnlineCourse.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Core.Models
{
    public class CourseDetailModel : CourseModel
    {
        public List<SessionDetailModel> SessionDetails { get; set; } = new List<SessionDetailModel>();
        public List<UserReviewModel> Reviews { get; set; } = new List<UserReviewModel>();
    }
    public class CourseModel
    {
        public int CourseId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;
        public decimal Price { get; set; }

        public string CourseType { get; set; } = null!;

        public int? SeatsAvailable { get; set; }

        public decimal Duration { get; set; }

        public int CategoryId { get; set; }

        public int InstructorId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        public virtual CourseCategoryModel Category { get; set; } = null!;

        public UserRatingModel UserRating { get; set; } = null!;

    }

    public class UserReviewModel
    {
        public int CourseId { get; set; }
      public string UserName { get; set; } = string.Empty;
        public string? Comments { get; set; } 
        public int Rating { get; set; } 
        public DateTime ReviewDate { get; set; }    

    }
    public class UserRatingModel
    {
        public int CourseId { get; set; }
        public int TotalRatings { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class SessionDetailModel
    {
        public int SessionId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; } 
        public string? VideoUrl { get; set; }
        public int VideoOrder { get; set; }

    }   
}
