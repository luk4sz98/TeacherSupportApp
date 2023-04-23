namespace TeacherSupportApp.Models;

public class Teacher : Person
{
    [Key]
    public int Id { get; set; }

    public virtual IEnumerable<SubjectTeacher> Subjects { get; set; }
    public virtual IEnumerable<Grade> Grades { get; set; }
    public virtual IEnumerable<TeacherGroup> Groups { get; set; }
}
