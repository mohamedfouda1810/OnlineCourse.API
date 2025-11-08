using Microsoft.EntityFrameworkCore;
using OnlineCourse.Core.Models;
using OnlineCourse.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCourse.Data
{
    public class CourseRepository : ICourseRepository
    {
        private readonly OnlineCourseDbContext db;

        public CourseRepository(OnlineCourseDbContext db ) {
            this.db = db;
        }   
        public async Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null)
        {

            var query=db.Courses.
                Include(c => c.Category)
                .AsQueryable();


            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }
       var courses=await  query.Select(c => new CourseModel
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                Price = c.Price,
                CourseType = c.CourseType,
                SeatsAvailable = c.SeatsAvailable,
                Duration = c.Duration,
                CategoryId = c.CategoryId,
                InstructorId = c.InstructorId,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Category = new CourseCategoryModel
                {
                    CategoryId = c.Category.CategoryId,
                    CategoryName = c.Category.CategoryName,
                    Description = c.Category.Description
                },
                UserRating = new UserRatingModel
                {
                    CourseId = c.CourseId,
                    TotalRatings = c.Reviews.Count,
                    AverageRating = c.Reviews.Any() ? Convert.ToDecimal(c.Reviews.Average(r => r.Rating)) : 0
                }
            }).ToListAsync();
            return courses;

        }

        public async  Task<CourseDetailModel> GetCourseDetailAsync(int courseId)
        {
           var course=await db.Courses.Include(c=>c.Category)
                .Include(c=>c.Reviews)
                .Include(c=>c.SessionDetails)
                .Where(c=>c.CourseId==courseId)
                .Select(c=> new CourseDetailModel {
                    CourseId=c.CourseId,
                    Title=c.Title,
                    Description=c.Description,
                    Price=c.Price,
                    CourseType=c.CourseType,
                    SeatsAvailable=c.SeatsAvailable,
                    Duration=c.Duration,
                    CategoryId=c.CategoryId,
                    InstructorId=c.InstructorId,
                    StartDate=c.StartDate,
                    EndDate=c.EndDate,
                    Category=new CourseCategoryModel {
                        CategoryId=c.Category.CategoryId,
                        CategoryName=c.Category.CategoryName,
                        Description=c.Category.Description
                    },
                    SessionDetails=c.SessionDetails.Select(s=> new SessionDetailModel {
                        SessionId=s.SessionId,
                        CourseId=s.CourseId,
                        Title=s.Title,
                        Description=s.Description,
                        VideoUrl=s.VideoUrl,
                        VideoOrder=s.VideoOrder
                    }).OrderBy(o=>o.VideoOrder).ToList(),
                    Reviews=c.Reviews.Select(r=> new UserReviewModel {
                        CourseId=r.CourseId,
                        UserName=r.User.DisplayName,
                        Comments=r.Comments,
                        Rating=r.Rating,
                        ReviewDate=r.ReviewDate
                    }).OrderByDescending(o=>o.Rating).Take(10).ToList(),
                    UserRating=new UserRatingModel {
                        CourseId=c.CourseId,
                        TotalRatings=c.Reviews.Count,
                        AverageRating=c.Reviews.Any()? Convert.ToDecimal(c.Reviews.Average(r=>r.Rating)):0
                    }
                }).FirstOrDefaultAsync();
            return course; 

        }
    }
}
