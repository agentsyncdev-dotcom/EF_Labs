using EF_Lab.Data;
using EF_Lab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using EF_Lab.DTO;

namespace EF_Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCourseController : ControllerBase
    {
        private readonly AppDBContext _context;

        public StudentCourseController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }

        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudentByIdWithEnrollment(int id)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(courses);
        }

        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourseByIdWithEnrollment(int id)
        {
            // Load course without eager loading. With lazy-loading proxies enabled and
            // navigation properties declared virtual, accessing navigation properties
            // will trigger lazy loading on demand.
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            // Access Enrollments to trigger lazy loading
            var enrollments = course.Enrollments;
            // Optionally access each enrollment's Student to trigger lazy loading for that navigation
            foreach (var e in enrollments)
            {
                var _ = e.Student;
            }

            return Ok(course);
        }

        [HttpPost("students")]
        public async Task<IActionResult> CreateStudent(CreateStudentDTO studentDTO)
        {
            if (studentDTO == null)
            {
                return BadRequest("Student data is null.");
            }
            // Create student but ignore any nested navigation properties (Enrollments) supplied by the client.
            var newStudent = new Student
            {
                FirstName = studentDTO.FirstName,
                LastName = studentDTO.LastName,
                Email = studentDTO.Email
            };

            await _context.Students.AddAsync(newStudent);
            await _context.SaveChangesAsync();
            return Ok(newStudent);
        }

        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse(CreateCourseDTO courseDTO)
        {
            if(courseDTO == null)
            {
                return BadRequest("Course data is null.");
            }  
            var newCourse = new Course
            {
                CourseName = courseDTO.CourseName,
                Credits = courseDTO.Credits
            };

            
            await _context.Courses.AddAsync(newCourse);
            await _context.SaveChangesAsync();
            return Ok(newCourse);
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollStudentInCourse([FromQuery] int studentId, [FromQuery] int courseId)
        {
            var student = await _context.Students.FindAsync(studentId);
            var course = await _context.Courses.FindAsync(courseId);
            if (student == null || course == null)
            {
                return NotFound("Student or Course not found.");
            }
            var enrollment = new Enrollment
            {
                StudentID = studentId,
                CourseID = courseId,
                EnrollmentDate = System.DateTime.Now
            };
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return Ok("Student enrolled in course successfully.");
        }
    }
}
