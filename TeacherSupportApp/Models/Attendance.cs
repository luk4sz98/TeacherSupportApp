namespace TeacherSupportApp.Models;

public class Attendance
{
    [Key]
    public int Id { get; set; }
    public DateTime ClassDate { get; set; }
    public bool WasPresent { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }

    public virtual Student Student { get; set; }
    public virtual Subject Subject { get; set; }
}
