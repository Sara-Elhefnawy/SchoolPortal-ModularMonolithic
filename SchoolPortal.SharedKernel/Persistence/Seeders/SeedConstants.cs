namespace SchoolPortal.SharedKernel.Persistence.Seeders;

public static class SeedConstants
{
    public static readonly int[] StudentIds = [1, 2, 3, 4, 5];

    // Sections that get assessments, grades, sessions and attendance.
    // The Grades seeder creates 3 sections in total, so IDs 1..3 exist.
    public static readonly int[] FullyEnrolledSectionIds = [1, 2];
}