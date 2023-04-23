using System.Linq.Expressions;

namespace TeacherSupportApp.Repository;

public class Repository : IRepository
{
    private readonly AppDbContext _dbContext;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool IsGroupAssignedToSubject(int groupId, int subjectId)
    {
        return _dbContext.SubjectGroups.Any(s => s.GroupId == groupId && s.SubjectId == subjectId);
    }

    public Teacher? GetTeacherById(int teacherId)
        => _dbContext.Teachers.FirstOrDefault(teacher => teacher.Id == teacherId);

    public Student? GetStudentById(int studentId)
        => _dbContext.Students.FirstOrDefault(student => student.Id == studentId);

    public Group? GetGroupById(int groupId)
        => _dbContext.Groups.FirstOrDefault(group => group.Id == groupId);

    public IEnumerable<Subject> GetSubjectsByTeacherId(int teacherId) 
        => _dbContext.SubjectTeachers
                .Include(s => s.Subject)
                .Where(st => st.TeacherId == teacherId)
                .Select(st => st.Subject)
                .ToList();

    public IEnumerable<Group> GetGroups() 
        => _dbContext.SubjectGroups
                .Include(s => s.Group)
                .Select(st => st.Group)
                .ToList();

    public IEnumerable<Group> GetGroupsBySubjectId(int subjectId) 
        => _dbContext.SubjectGroups
                .Include(s => s.Group)
                .Where(st => st.SubjectId == subjectId)
                .Select(st => st.Group)
                .ToList();

    public IEnumerable<Group> GetGroupsByTeacherId(int teacherId) 
        => _dbContext.TeacherGroups
                .Where(t => t.TeacherId == teacherId)
                .Select(t => t.Group);

    public IEnumerable<Student> GetStudentsWithoutGroup()
        => GetStudentsByCondition(student => student.Group == null);
    
    public Subject? GetSubject(int subjectId)
    => _dbContext.Subjects.FirstOrDefault(s => s.Id == subjectId);

    public IEnumerable<Attendance> GetStudentAttendances(int studentId)
        => _dbContext.Attendances
                 .Include(a => a.Subject)
                 .Where(a => a.StudentId == studentId);

    public IEnumerable<Grade> GetGroupGrades(int groupId)
        => _dbContext.Grades
                 .Include(g => g.Student)
                 .Include(g => g.Subject)
                 .Where(g => g.Student.GroupId == groupId);

    private IEnumerable<Student> GetStudentsByCondition(Expression<Func<Student, bool>> condition) 
        => _dbContext.Students.Where(condition);
}
