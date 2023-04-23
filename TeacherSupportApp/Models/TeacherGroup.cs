namespace TeacherSupportApp.Models;

public class TeacherGroup
{
    public int TeacherId { get; set; }
    public int GroupId { get; set; }


    public virtual Teacher Teacher { get; set; }
    public virtual Group Group { get; set; }
}