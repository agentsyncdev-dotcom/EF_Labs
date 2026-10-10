using System;

namespace EF_Lab.Models;

public class Student
{
    public int StudentID { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    // Make navigation property virtual to enable lazy loading
    public virtual ICollection<Enrollment> Enrollments { get; set; }
}


