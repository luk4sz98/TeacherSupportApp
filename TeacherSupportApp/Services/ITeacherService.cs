namespace TeacherSupportApp.Services;

public interface ITeacherService
{
    void AddAttendanceForGroup(Group choosenGroup, Subject choosenSubject);
    void AssignStudentToGroup(int studentId, int groupId);
    int AddSubject(string subjectName);
    void AssignTeacherToSubject(int subjectId, int teacherId);
    void AssignSubjectToGroups(int subjectId, IEnumerable<int> groupIds);
    int AddGroup();
    void AssignTeacherToGroup(int teacherId, int groupId);
    void AddGrade(int studentId, int subjectId, int teacherId, string value);
}
