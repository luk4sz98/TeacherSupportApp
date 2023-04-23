global using Microsoft.EntityFrameworkCore;
global using TeacherSupportApp.DAL;
global using TeacherSupportApp.Models;
global using System.ComponentModel.DataAnnotations;
global using TeacherSupportApp.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using TeacherSupportApp.Services;
using TeacherSupportApp;
using System.Globalization;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = RegisterServices(args);
        var repo = host.Services.GetRequiredService<IRepository>();
        var teacherService = host.Services.GetRequiredService<ITeacherService>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Wybierz jedną z dostepnych akcji");
            Console.WriteLine("1.Zaloguj\n2.Wyjdź");

            var option = Console.ReadKey();
            if (option.Key == ConsoleKey.D2)
            {
                break;
            }
            else if (option.Key == ConsoleKey.D1)
            {
                Console.Clear();
                Console.WriteLine("Podaj nr id by się zalogować\n");

                var result = int.TryParse(Console.ReadLine(), out var id);
                if (!result)
                {
                    DisplayHelper.ShowPauseKey("Niepoprawny nr id!");
                    continue;
                }

                var teacher = repo.GetTeacherById(id);
                if (teacher is null)
                {
                    DisplayHelper.ShowPauseKey("Brak nauczyciela o tym nr id!");
                    continue;
                }

                TeacherAction(teacher, repo, teacherService);
            }
        }
    }

    private static void TeacherAction(Teacher teacher, IRepository repo, ITeacherService teacherService)
    {
        bool breakLoop = false;
        while (!breakLoop)
        {
            DisplayHelper.ShowTeacherMenu();
            var result = int.TryParse(Console.ReadLine(), out var operation);
            if (!result)
                continue;
            switch (operation)
            {
                case 1:
                    CheckStudentAttendance(teacherService, repo, teacher.Id);
                    break;
                case 2:
                    AssignStudentToGroup(teacherService, repo, teacher.Id);
                    break;
                case 3:
                    GetStudentsForGroup(repo, teacher.Id);
                    break;
                case 4:
                    GetTeacherSubjects(repo, teacher.Id);
                    break;
                case 5:
                    CreateSubject(teacherService, repo, teacher.Id);
                    break;
                case 6:
                    AssignGroupsToSubject(teacherService, repo, teacher.Id);
                    break;
                case 7:
                    CreateGroup(teacherService, repo, teacher.Id);
                    break;
                case 8:
                    InsertGrade(teacherService, repo, teacher.Id);
                    break;
                case 9:
                    GetStudentGrades(repo, teacher.Id);
                    break;
                case 10:
                    GetStudentAverageGrade(repo, teacher.Id);
                    break;
                case 11:
                    GetGroupGradesStats(repo, teacher.Id);
                    break;
                case 12:
                    GetStudentAttendance(repo, teacher.Id);
                    break;
                case 13:
                    breakLoop = true;
                    break;
                default:
                    DisplayHelper.ShowPauseKey("Podaj jedną z dozwolonych operacji!");
                    break;
            }
        }
    }

    private static void InsertGrade(ITeacherService teacherService, IRepository repo, int teacherId)
    {
        try
        {
            var subjects = repo.GetSubjectsByTeacherId(teacherId);
            if (subjects is null || !subjects.Any())
                throw new InvalidDataException("Nie masz przypisanych żadnych przedmiotów.");
            DisplayHelper.ShowTeacherSubjects(subjects);
            
            var subjectId = GetIdFromUser();
            var student = GetStudent(repo, teacherId);
            var groupId = student.Group.Id;

            if (!repo.IsGroupAssignedToSubject(groupId, subjectId))
                throw new InvalidDataException("Grupa do której należy student, nie jest przypisana do przedmiotu.");

            Console.Clear();
            Console.WriteLine("Podaj wartość oceny w formacie X.XX lub wpisz exit jeśli chcesz przerwać proces.");
            string value = "";
            Regex regex = new(@"^(2\.00|5\.00|3\.(00|50)|4\.(00|50))$");
            do
            {
                Console.WriteLine("Ocena:");
                value = Console.ReadLine() ?? "";
                var isValid = regex.IsMatch(value);
                if (isValid)
                {
                    teacherService.AddGrade(student.Id, subjectId, teacherId, value);
                    break;
                }
            }
            while (value != "exit");
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
        }
    }

    private static void GetGroupGradesStats(IRepository repo, int teacherId)
    {
        try
        {
            Console.Clear();
            var groups = repo.GetGroupsByTeacherId(teacherId);
            if (groups is null || !groups.Any())
                throw new InvalidDataException("Nie masz przypisanych żadnych grup.");

            DisplayHelper.ShowTeacherGroups(groups);
            var groupId = GetIdFromUser();

            var group = groups.FirstOrDefault(g => g.Id == groupId) 
                ?? throw new InvalidDataException("Nieprawidłowy nr id grupy.");

            var gradesGroup = repo.GetGroupGrades(group.Id);

            if (gradesGroup is null || !gradesGroup.Any())
                throw new InvalidDataException("Studenci należący do grupy nie mają ocen");

            DisplayHelper.ShowGroupGrades(gradesGroup);
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
        }
    }

    private static void GetStudentAttendance(IRepository repo, int teacherId)
    {
        try
        {
            var student = GetStudent(repo, teacherId);
            var attendances = repo.GetStudentAttendances(student.Id);
            if (attendances is null || !attendances.Any())
                throw new InvalidDataException("Student nie ma żadnych wpisów w obecności.");

            DisplayHelper.ShowStudentAttendances(attendances, $"{student.FirstName} {student.LastName}");
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
        }
    }

    private static void GetStudentAverageGrade(IRepository repo, int teacherId)
    {
        try
        {
            var student = GetStudent(repo, teacherId);
            var studentGrades = student.Grades;
            if (studentGrades is null || !studentGrades.Any())
                throw new ArgumentException("Student nie ma ocen");

            var avg = studentGrades.Average(g => float.Parse(g.Value, new CultureInfo("en-US")));

            Console.Clear();
            Console.WriteLine($"Średnia ogólna ocen studenta {student.FirstName} {student.LastName} to: {avg}");
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
        }
    }

    private static void GetStudentGrades(IRepository repo, int teacherId)
    {
        try
        {
            var student = GetStudent(repo, teacherId);
            var studentGrades = student.Grades;
            if (studentGrades is null || !studentGrades.Any())
                throw new ArgumentException("Student nie ma ocen");

            var gradesBySubject = studentGrades
                .GroupBy(g => g.Subject)
                .ToList();

            foreach (var subject in gradesBySubject)
            {
                Console.WriteLine($"\n{subject.Key.Name}");
                var grades = subject.Select(g => float.Parse(g.Value, new CultureInfo("en-US")));
                foreach (var grade in grades)
                {
                    Console.Write(grade + ", ");
                }
                Console.WriteLine($"Średnia:{grades.Average()}");
            }
            DisplayHelper.ShowPauseKey();
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
        }
    }

    private static Student GetStudent(IRepository repo, int teacherId)
    {
        var teacherGroups = repo.GetGroupsByTeacherId(teacherId);
        if (!teacherGroups.Any())
            throw new InvalidDataException("Nie prowadzisz żadnych grup");

        DisplayHelper.ShowTeacherGroups(teacherGroups);
        var groupId = GetIdFromUser();

        var group = teacherGroups.FirstOrDefault(g => g.Id == groupId);
        if (group is null || group.Students is null)
            throw new ArgumentException("Podałeś zły numer id lub grupa nie ma przypisanych żadnych studentów.");

        var students = group.Students.ToList();
        DisplayHelper.ShowStudents(students);

        var studentId = GetIdFromUser();
        var student = students.FirstOrDefault(s => s.Id == studentId);

        return student ?? throw new InvalidDataException("Dany student nie istnieje.");
    }

    private static void CreateGroup(ITeacherService teacherService,IRepository repo, int teacherId)
    {
        try
        {
            Console.Clear();
            Console.WriteLine("Czy chcesz utworzyć nową grupę zajęciową?\nTAK - naciśnij 1\nNIE - naciśnij 2");
            
            var userKey = Console.ReadKey();
            if (userKey.Key != ConsoleKey.D1)
                return;

            var groupId = teacherService.AddGroup();
            
            var group = repo.GetGroupById(groupId) ?? throw new InvalidDataException("Coś poszło nie tak");
            teacherService.AssignTeacherToGroup(teacherId, group.Id);
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
            return;
        }
    }

    private static void AssignGroupsToSubject(ITeacherService teacherService, IRepository repo, int teacherId)
    {
        try
        {
            var teacherSubjects = repo.GetSubjectsByTeacherId(teacherId);
            if (!teacherSubjects.Any())
            {
                DisplayHelper.ShowPauseKey("Nie jesteś przypisany do żadnego z przedmiotów.");
                return;
            }

            DisplayHelper.ShowTeacherSubjects(teacherSubjects);
            var subjectId = GetIdFromUser();

            var teacherGroups = repo.GetGroupsByTeacherId(teacherId);
            Console.WriteLine("Podaj id grup, które chcesz przypisać do tego przedmiotu. Podawaj w formacie: 21,23,11 itp...");
            foreach (var g in teacherGroups)
            {
                Console.WriteLine($"Grupa {g.Id}");
            }

            var gr = Console.ReadLine();
            if (string.IsNullOrEmpty(gr))
            {
                DisplayHelper.ShowPauseKey("Nie podałeś żadnej grupy");
                return;
            }

            var groups = gr
                .Split(",")
                .Select(g => int.Parse(g))
                .ToList();
            if (groups.Count == 0)
            {
                DisplayHelper.ShowPauseKey("Podałeś niepoprawny format, spróbuj ponownie");
                return;
            }

            teacherService.AssignSubjectToGroups(subjectId, groups);
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
            return;
        }
    }

    private static void CreateSubject(ITeacherService teacherService, IRepository repo, int teacherId)
    {
        try
        {
            Console.Clear();
            Console.WriteLine("Podaj nazwę przedmiotu:");
            
            var subjectName = Console.ReadLine();
            if (string.IsNullOrEmpty(subjectName) || subjectName.Length < 3)
            {
                DisplayHelper.ShowPauseKey("Nazwa przedmiotu musi składać się conajmniej z trzech liter.");
                return;
            }
            if (subjectName.Any(c => char.IsDigit(c) || char.IsSymbol(c)))
            {
                DisplayHelper.ShowPauseKey("Nazwa przedmiotu nie może zawierać cyfr lub znaków specjalnych!");
                return;
            }

            var subjectId = teacherService.AddSubject(subjectName);
            var subject = repo.GetSubject(subjectId) ?? throw new ArgumentException("Coś poszło nie tak, spróbuj ponownie.");
            teacherService.AssignTeacherToSubject(subject.Id, teacherId);
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
            return;
        }
    }

    private static void AssignStudentToGroup(ITeacherService teacherService, IRepository repo, int id)
    {   
        try
        {
            var nonGroupStudents = repo.GetStudentsWithoutGroup();
            var teacherGroups = repo.GetGroupsByTeacherId(id);
            if (!nonGroupStudents.Any() || !teacherGroups.Any())
            {
                DisplayHelper.ShowPauseKey("Nie ma studenta, który nie miałby przypisanej grupy zajęciowej " +
                    "lub nie prowadzisz żadnej grupy");
                return;
            }

            DisplayHelper.ShowStudents(nonGroupStudents);
            var studentId = GetIdFromUser();

            DisplayHelper.ShowTeacherGroups(teacherGroups);
            var groupId = GetIdFromUser();

            teacherService.AssignStudentToGroup(studentId, groupId);
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
            return;
        }
    }

    private static void GetTeacherSubjects(IRepository repo, int teacherId)
    {
        var teacherSubjects = repo.GetSubjectsByTeacherId(teacherId);
        if (!teacherSubjects.Any())
        {
            DisplayHelper.ShowPauseKey("Nie prowadzisz żadnych zajęć");
            return;
        }
        Console.Clear();
        Console.WriteLine("Zajęcia prowadzone przez Ciebie to:\n");
        foreach (var subject in teacherSubjects)
        {
            Console.WriteLine($"{subject.Name}");
        }
        DisplayHelper.ShowPauseKey();
    }

    private static void GetStudentsForGroup(IRepository repo, int teacherId)
    {
        try
        {
            var teacherGroups = repo.GetGroupsByTeacherId(teacherId);
            if (!teacherGroups.Any())
            {
                DisplayHelper.ShowPauseKey("Nie masz przypisanych żadnych grup zajęciowych");
                return;
            }
            DisplayHelper.ShowTeacherGroups(teacherGroups);

            var groupId = GetIdFromUser();

            var group = teacherGroups.FirstOrDefault(g => g.Id == groupId);
            if (group is null)
            {
                DisplayHelper.ShowPauseKey("Nie uczysz tej grupy");
                return;
            }

            Console.Clear();
            Console.WriteLine($"Studenci przypisani do grupy nr {groupId}");
            foreach (var student in group.Students)
            {
                Console.WriteLine($"{student.Id}\t{student.FirstName} {student.LastName}\t{student.Age}");
            }
            DisplayHelper.ShowPauseKey();
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
            return;
        }
    }

    private static void CheckStudentAttendance(ITeacherService teacherService, IRepository repository, int teacherId)
    {
        try
        {
            var subjects = repository.GetSubjectsByTeacherId(teacherId);
            if (!subjects.Any())
            {
                DisplayHelper.ShowPauseKey("Nie prowadzisz żadnych zajęć!");
                return;
            }

            DisplayHelper.ShowTeacherSubjects(subjects);

            var subjectId = GetIdFromUser();

            var choosenSubject = subjects.FirstOrDefault(s => s.Id == subjectId);
            if (choosenSubject is null)
            {
                DisplayHelper.ShowPauseKey($"Nie prowadzisz przedmiotu o podanym nr id {subjectId}");
                return;
            }

            var subjectGroups = repository.GetGroupsBySubjectId(choosenSubject.Id);
            if (!subjectGroups.Any())
            {
                DisplayHelper.ShowPauseKey("Do podanego przedmiotu nie jest zapisana żadna grupa zajęciowa.");
                return;
            }
            DisplayHelper.ShowSubjectGroups(subjectGroups);

            var groupId = GetIdFromUser();

            var choosenGroup = subjectGroups.FirstOrDefault(g => g.Id == groupId);
            if (choosenGroup is null)
            {
                DisplayHelper.ShowPauseKey($"Ta grupa nie jest przypisana do przedmiotu {choosenSubject.Name}.");
                return;
            }

            teacherService.AddAttendanceForGroup(choosenGroup, choosenSubject);
        }
        catch (Exception ex)
        {
            DisplayHelper.ShowPauseKey(ex.Message);
            return;
        }
    }

    private static int GetIdFromUser()
    {
        Console.WriteLine("Podaj nr id:");
        var validId = int.TryParse(Console.ReadLine(), out var id);
        return validId ? id : throw new ArgumentException("Id musi być liczbą!");
    }

    private static IHost RegisterServices(string[] args)
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        var _configuration = configBuilder.Build();
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
                });
                services.AddScoped<IRepository, Repository>();
                services.AddScoped<ITeacherService, TeacherService>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning); // lub LogLevel.Error
            })
            .Build();
    }
}