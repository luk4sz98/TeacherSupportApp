namespace TeacherSupportApp.Models;

public class Subject
{
    [Key] 
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual IEnumerable<SubjectGroup> Groups { get; set; }
    public virtual IEnumerable<Grade> Grades { get; set; }
    public virtual IEnumerable<Attendance> Attendances { get; set; }
    public virtual IEnumerable<SubjectTeacher> Teachers { get; set; }
}
