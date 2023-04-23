namespace TeacherSupportApp.Models;

public class Student : Person
{
    [Key]
    public int Id { get; set; }
    public int GroupId { get; set; }

    public virtual Group Group { get; set; }
    public virtual IEnumerable<Grade> Grades { get; set;}
    public virtual IEnumerable<Attendance> Attendances { get; set; }
}
