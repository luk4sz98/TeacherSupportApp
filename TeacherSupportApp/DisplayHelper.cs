using System.Globalization;

namespace TeacherSupportApp;

public static class DisplayHelper
{

    public static void ShowTeacherMenu()
    {
        Console.Clear();
        Console.WriteLine("1. Sprawdzenie obecności\n" +
            "2. Przypisz studenta do grupy zajęciowej\n" +
            "3. Przejrzenie studentów z danej grupy zajęciowej\n" +
            "4. Wyświetlenie prowadzonych zajęć\n" +
            "5. Tworzenie zajęć\n" +
            "6. Przypisz grupy do przedmiotu\n" +
            "7. Tworzenie grup zajęciowych\n" +
            "8. Wstawienie oceny\n" +
            "9. Wyświetlenie ocen danego studenta z podziałem na przedmioty\n" +
            "10. Wyświetlenie średniej ocen danego studenta\n" +
            "11. Wyświetlenie ocen dla danej grupy zajęciowej\n" +
            "12. Statystyki obecności danego studenta\n" +
            "13. Wyloguj\n\nOpcja nr:");
    }

    public static void ShowPauseKey(string message = "")
    {
        const string pauseMsg = "Naciśnij przycisk by przejść dalej...";
        var msg = string.IsNullOrEmpty(message)
            ? pauseMsg
            : message + "\n" + pauseMsg;
        
        Console.WriteLine(msg);
        Console.ReadKey();
        Console.Clear();
    }

    public static void ShowStudentAttendances(IEnumerable<Attendance> attendances, string credentials)
    {
        Console.Clear();
        Console.WriteLine($"Statystyka obecności studenta {credentials} wg przedmiotu");

        foreach (var att in attendances.GroupBy(a => a.Subject).ToList())
        {
            Console.WriteLine(att.Key.Name);
            Console.WriteLine("Data zajęć\t\tObecność");
            
            foreach (var @class in att)
            {
                Console.WriteLine($"{@class.ClassDate:dd/MM/yyyy HH:mm}\t\t{@class.WasPresent.GetFormattedText()}");
            }

            Console.WriteLine(GetAttendancePercentages(att));
        }
        ShowPauseKey();
    }

    public static void ShowGroupGrades(IEnumerable<Grade> gradesGroup)
    {
        Console.Clear();
        Console.WriteLine("Statystyka poszczególnych ocen w danej grupie wg przedmiotu");

        foreach (var subjectKey in gradesGroup.GroupBy(g => g.Subject))
        {
            Console.WriteLine(subjectKey.Key.Name);
            Console.WriteLine(GetGroupGradesPercentages(subjectKey));
        }
        ShowPauseKey();
    }

    public static void ShowStudents(IEnumerable<Student> students)
    => ShowCollection(students, "Lista studentów", student => $"{student.Id} {student.FirstName} {student.LastName}");

    public static void ShowTeacherSubjects(IEnumerable<Subject> teacherSubjects)
        => ShowCollection(teacherSubjects, "Prowadzone przedmioty", subject => $"{subject.Id}. {subject.Name}");

    public static void ShowSubjectGroups(IEnumerable<Group> subjectGroups)
        => ShowCollection(subjectGroups, "Grupy przypisane do przedmiotu", group => $"Grupa {group.Id}");

    public static void ShowTeacherGroups(IEnumerable<Group> teacherGroups)
        => ShowCollection(teacherGroups, "Grupty, które uczysz", group => $"Grupa {group.Id}");

    private static void ShowCollection<T>(IEnumerable<T> collection, string message, Func<T, string> itemFormatter)
    {
        Console.WriteLine($"\n{message}");
        foreach (var item in collection)
        {
            Console.WriteLine(itemFormatter(item));
        }
    }

    private static string GetGroupGradesPercentages(IEnumerable<Grade> grades)
    {
        var gradesCount = grades.Count();

        GetGradesCount(grades, out var fiveNumber, out var fourHalfNumber, 
            out var fourNumber, out var threeHalfNumber, out var threeNumber, out var twoNumber);

        return $"Ilość wszystkich ocen: {gradesCount}\n" +
               $"Ilość 5.0: {fiveNumber} ({Math.Round((double)fiveNumber / gradesCount * 100, 2)}%)\n" +
               $"Ilość 4.5: {fourHalfNumber} ({Math.Round((double)fourHalfNumber / gradesCount * 100, 2)}%)\n" +
               $"Ilość 4.0: {fourNumber} ({Math.Round((double)fourNumber / gradesCount * 100, 2)}%)\n" +
               $"Ilość 3.5: {threeHalfNumber} ({Math.Round((double)threeHalfNumber / gradesCount * 100, 2)}%)\n" +
               $"Ilość 3.0: {threeNumber} ({Math.Round((double)threeNumber / gradesCount * 100, 2)}%)\n" +
               $"Ilość 2.0: {twoNumber} ({Math.Round((double)twoNumber / gradesCount * 100, 2)}%)\n";
    }

    private static void GetGradesCount(IEnumerable<Grade> grades, out int fiveNumber, out int  fourHalfNumber, 
        out int fourNumber, out int threeHalfNumber, out int threeNumber, out int twoNumber)
    {
        fiveNumber = 0; fourHalfNumber = 0; fourNumber = 0; threeHalfNumber = 0; threeNumber = 0; twoNumber = 0;
        foreach (var grade in grades)
        {
            switch (float.Parse(grade.Value, new CultureInfo("en-US")))
            {
                case 5.00f:
                    fiveNumber++;
                    break;
                case 4.5f:
                    fourHalfNumber++;
                    break;
                case 4.00f:
                    fourNumber++;
                    break;
                case 3.5f:
                    threeHalfNumber++;
                    break;
                case 3.00f:
                    threeNumber++;
                    break;
                case 2.00f:
                    twoNumber++;
                    break;
                default:
                    break;
            }
        }
    }

    private static string GetAttendancePercentages(IEnumerable<Attendance> attendances)
    {
        var attCount = attendances.Count();
        var presentAtt = attendances.Count(a => a.WasPresent);
        var nonPresentAtt = attCount - presentAtt;
        var presentPercent = Math.Round((double)presentAtt / attCount * 100, 2);
        var nonPresentPercent = Math.Round((double)nonPresentAtt / attCount * 100, 2);

        return $"Ilość odbytych zajęć: {attCount}\n" +
               $"Ilość obecności: {presentAtt} ({presentPercent}%)\n" +
               $"Ilość nieobecności: {nonPresentAtt} ({nonPresentPercent}%)";
    }

    private static string GetFormattedText(this bool value)
        => value ? "TAK" : "NIE";
}
