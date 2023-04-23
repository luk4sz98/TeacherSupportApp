namespace TeacherSupportApp.DAL;

public class AppDbContext : DbContext
{
    public virtual DbSet<Teacher> Teachers { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<Grade> Grades { get; set; }
    public virtual DbSet<Subject> Subjects { get; set; }
    public virtual DbSet<Group> Groups { get; set; }
    public virtual DbSet<Attendance> Attendances { get; set; }
    public virtual DbSet<SubjectTeacher> SubjectTeachers { get; set; }
    public virtual DbSet<SubjectGroup> SubjectGroups { get; set; }
    public virtual DbSet<TeacherGroup> TeacherGroups { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Fluent API commands
        builder.Entity<SubjectGroup>()
            .HasKey(subjectGroup => new { subjectGroup.GroupId, subjectGroup.SubjectId });
        builder.Entity<SubjectTeacher>()
            .HasKey(subjectTeacher => new { subjectTeacher.SubjectId, subjectTeacher.TeacherId });
        builder.Entity<TeacherGroup>()
            .HasKey(teacherGroup => new { teacherGroup.TeacherId, teacherGroup.GroupId });

        builder.Entity<Attendance>()
            .HasOne(attendance => attendance.Student)
            .WithMany(student => student.Attendances)
            .HasForeignKey(attendance => attendance.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Attendance>()
            .HasOne(attendance => attendance.Subject)
            .WithMany(subject => subject.Attendances)
            .HasForeignKey(attendance => attendance.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Grade>()
            .HasOne(grade => grade.Subject)
            .WithMany(subject => subject.Grades)
            .HasForeignKey(grade => grade.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Grade>()
            .HasOne(grade => grade.Student)
            .WithMany(student => student.Grades)
            .HasForeignKey(grade => grade.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Grade>()
            .HasOne(grade => grade.Teacher)
            .WithMany(teacher => teacher.Grades)
            .HasForeignKey(grade => grade.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Group>()
            .HasMany(group => group.Students)
            .WithOne(student => student.Group)
            .HasForeignKey(student => student.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Group>()
            .HasMany(group => group.Subjects)
            .WithOne(subject => subject.Group)
            .HasForeignKey(subject => subject.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Subject>()
            .HasMany(subject => subject.Groups)
            .WithOne(group => group.Subject)
            .HasForeignKey(group => group.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Subject>()
            .HasMany(subject => subject.Teachers)
            .WithOne(teacher => teacher.Subject)
            .HasForeignKey(teacher => teacher.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Teacher>()
            .HasMany(teacher => teacher.Subjects)
            .WithOne(subject => subject.Teacher)
            .HasForeignKey(subject => subject.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Group>()
            .HasMany(group => group.Teachers)
            .WithOne(teacher => teacher.Group)
            .HasForeignKey(teacher => teacher.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Teacher>()
            .HasMany(teacher => teacher.Groups)
            .WithOne(group => group.Teacher)
            .HasForeignKey(group => group.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
