namespace HilltopApi.Models;

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public string School { get; set; } = "";

    public List<Enrollment> Enrollments { get; set; } = [];
}
