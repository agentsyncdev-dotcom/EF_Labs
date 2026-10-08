using System;

namespace EF_Lab.Models;

public class Enrollment
{
    public int StudentID { get; set; }

    public Student Student { get; set; }

    public int CourseID { get; set; }

    public Course Course { get; set; }

    public DateTime EnrollmentDate { get; set; }
}
