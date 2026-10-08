using System;

namespace EF_Lab.Models;

public class Student
{
    public int StudentID { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}


