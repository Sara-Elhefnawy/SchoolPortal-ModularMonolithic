namespace SchoolPortal.SharedKernel.Identity;

public static class Permissions
{
    public static class Users
    {
        public const string Create = "Users.Create";
        public const string View = "Users.View";
        public const string Update = "Users.Update";
        public const string Delete = "Users.Delete";
    }

    public static class Grades
    {
        public const string View = "Grades.View";
        public const string Create = "Grades.Create";
        public const string Update = "Grades.Update";
        public const string Delete = "Grades.Delete";
    }

    public static class Attendance
    {
        public const string View = "Attendance.View";
        public const string Mark = "Attendance.Mark";
        public const string Update = "Attendance.Update";
    }

    public static class Students
    {
        public const string View = "Students.View";
        public const string Create = "Students.Create";
        public const string Update = "Students.Update";
        public const string Delete = "Students.Delete";
    }
    
    /*
     * This is the complete list of permissions that exist in the application.
     *
     * The database decides which role receives each permission.
     *
     * So:
     *      Code = "What permissions exist?"
     *      Database = "Who has those permissions?"
     */
    public static IEnumerable<string> All =>
    [
        Users.Create,
        Users.View,
        Users.Update,
        Users.Delete,

        Grades.View,
        Grades.Create,
        Grades.Update,
        Grades.Delete,

        Attendance.View,
        Attendance.Mark,
        Attendance.Update,

        Students.View,
        Students.Create,
        Students.Update,
        Students.Delete
    ];
}