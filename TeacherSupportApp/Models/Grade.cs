namespace TeacherSupportApp.Models;

public class Grade
{
    [Key]
    public int Id { get; set; }
    public string Value { get; set; }
    public DateTime Created { get; set; }
    public int StudentId { get; set; }
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }

    public virtual Student Student { get; set; }
    public virtual Teacher Teacher { get; set; }
    public virtual Subject Subject { get; set; }
}
