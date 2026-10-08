using System;

namespace EF_Lab.Models;

public class Course
{
    public int CourseID { get; set; }

    public string CourseName { get; set; }

    public int Credits { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();

}
