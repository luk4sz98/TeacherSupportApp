namespace TeacherSupportApp.Models;

public class Group
{
    [Key]
    public int Id { get; set; }

    public virtual IEnumerable<Student> Students { get; set; }
    public virtual IEnumerable<SubjectGroup> Subjects { get; set; }
    public virtual IEnumerable<TeacherGroup> Teachers { get; set; }
}
