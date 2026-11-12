namespace HilltopApi.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? School { get; set; }
    public DateOnly DateOfBirth { get; set; }

    // Contact and academic records
    public string Email { get; set; } = "";
    public string HomeAddress { get; set; } = "";
    public double Gpa { get; set; }
}
