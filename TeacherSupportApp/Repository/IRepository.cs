namespace TeacherSupportApp.Repository;

public interface IRepository
{
    bool IsGroupAssignedToSubject(int groupId, int subjectId);
    Subject? GetSubject(int subjectId);
    Teacher? GetTeacherById(int teacherId);
    Student? GetStudentById(int studentId);
    Group? GetGroupById(int groupId);
    IEnumerable<Subject> GetSubjectsByTeacherId(int teacherId);
    IEnumerable<Group> GetGroups();
    IEnumerable<Group> GetGroupsBySubjectId(int subjectId);
    IEnumerable<Group> GetGroupsByTeacherId(int teacherId);
    IEnumerable<Student> GetStudentsWithoutGroup();
    IEnumerable<Attendance> GetStudentAttendances(int studentId);
    IEnumerable<Grade> GetGroupGrades(int groupId);
}
