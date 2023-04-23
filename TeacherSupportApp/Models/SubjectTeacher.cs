namespace TeacherSupportApp.Models;

public class SubjectTeacher
{
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }


    public virtual Teacher Teacher { get; set; }
    public virtual Subject Subject { get; set; }
}
