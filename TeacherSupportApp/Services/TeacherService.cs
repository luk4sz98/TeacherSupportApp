namespace TeacherSupportApp.Services;

public class TeacherService : ITeacherService
{
    private readonly AppDbContext _context;

    public TeacherService(AppDbContext context)
    {
        _context = context;
    }

    public void AddAttendanceForGroup(Group choosenGroup, Subject choosenSubject)
    {
        var currentDate = DateTime.Now;
        var attendanceExist = _context.Attendances
            .Where(a => a.SubjectId == choosenSubject.Id)
            .Where(a => a.Student.GroupId == choosenGroup.Id)
            .Where(a => a.ClassDate >= currentDate.AddMinutes(-45) && a.ClassDate <= currentDate.AddMinutes(45))
            .Any();
        if (attendanceExist)
        {
            DisplayHelper.ShowPauseKey("Już była sprawdzana obecność w tym terminie.");
            return;
        }

        Console.WriteLine("Dla każdego studenta w grupie podaj informację czy jest obecny\n1 - obecny\n2 - nieobecny");
        var attendances = new List<Attendance>();

        foreach (var student in choosenGroup.Students)
        {
            Console.WriteLine($"Student {student.FirstName} {student.LastName} jest:\n");
            var attendance = new Attendance
            {
                ClassDate = currentDate,
                StudentId = student.Id,
                SubjectId = choosenSubject.Id,
            };

            var keyPresent = Console.ReadKey();
            Console.WriteLine();
            if (keyPresent.Key == ConsoleKey.D1)
                attendance.WasPresent = true;
            else
                attendance.WasPresent = false;

            attendances.Add(attendance);
        }

        _context.Attendances.AddRange(attendances);
        _context.SaveChanges();
        DisplayHelper.ShowPauseKey("Obecności zapisane!");
    }

    public void AssignStudentToGroup(int studentId, int groupId)
    {
        var group = _context.Groups.FirstOrDefault(g => g.Id == groupId) ?? throw new ArgumentException("Nie ma takiej grupy");
        var student = _context.Students.FirstOrDefault(s => s.Id == studentId) ?? throw new ArgumentException("Nie ma takiego studenta");
        student.Group = group;
        _context.SaveChanges();
        DisplayHelper.ShowPauseKey("Student został dodany do grupy!");
    }

    public int AddSubject(string subjectName)
    {
        var subject = new Subject
        {
            Name = subjectName
        };
        _context.Subjects.Add(subject);
        _context.SaveChanges();
        
        DisplayHelper.ShowPauseKey("Przedmiot został utworzony");
        return subject.Id;
    }

    public void AssignTeacherToSubject(int subjectId, int teacherId)
    {
        _context.SubjectTeachers.Add(new SubjectTeacher
        {
            SubjectId = subjectId,
            TeacherId = teacherId
        });
        DisplayHelper.ShowPauseKey("Nauczyciel został przypisany do przedmiotu");
    }

    public void AssignSubjectToGroups(int subjectId, IEnumerable<int> groupIds)
    {
        var subjectGroups = new List<SubjectGroup>();
        foreach (var groupId in groupIds)
        {
            subjectGroups.Add(new SubjectGroup
            {
                SubjectId = subjectId,
                GroupId = groupId
            });
        }

        _context.SubjectGroups.AddRange(subjectGroups);
        _context.SaveChanges();
        DisplayHelper.ShowPauseKey("Przedmiot zostął przypisany do poszczególnych grup.");
    }

    public int AddGroup()
    {
        var group = new Group();
        _context.Groups.Add(group);
        _context.SaveChanges();
        DisplayHelper.ShowPauseKey("Została utworzona nowa grupa");
        return group.Id;
    }

    public void AssignTeacherToGroup(int teacherId, int groupId)
    {
        _context.TeacherGroups.Add(new TeacherGroup
        {
            TeacherId = teacherId,
            GroupId = groupId
        });
        _context.SaveChanges();
        DisplayHelper.ShowPauseKey("Grupa została przypisana do ciebie.");
    }

    public void AddGrade(int studentId, int subjectId, int teacherId, string value)
    {
        var grade = new Grade
        {
            StudentId = studentId,
            SubjectId = subjectId,
            TeacherId = teacherId,
            Value = value,
            Created = DateTime.Now
        };
        _context.Grades.Add(grade); 
        _context.SaveChanges();
        DisplayHelper.ShowPauseKey("Ocena została dodana.");
    }
}
