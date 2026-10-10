using System;

namespace EF_Lab.Models;

public class Enrollment
{
    public int StudentID { get; set; }

    // navigation property should be virtual for lazy loading
    // navigation property should be nullable for model-binding when creating nested objects
    public virtual Student Student { get; set; }

    public int CourseID { get; set; }

    // navigation property should be virtual for lazy loading
    // navigation property should be nullable for model-binding when creating nested objects
    public virtual Course Course { get; set; }

    public DateTime EnrollmentDate { get; set; }
}
