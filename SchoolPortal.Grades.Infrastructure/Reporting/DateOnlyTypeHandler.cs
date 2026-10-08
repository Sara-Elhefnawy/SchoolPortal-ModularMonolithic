using System.Data;
using Dapper;

namespace SchoolPortal.Grades.Infrastructure.Reporting;

// WHY this exists: Dapper doesn't know how to build a DateOnly from a Postgres "date" column.
// Without it, every Row record that has a DateOnly property fails at runtime with a type-mismatch error.
internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    // Called when reading: accept whichever type the driver produced.
    public override DateOnly Parse(object value) =>
        value switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            _ => throw new DataException($"Cannot convert {value.GetType()} to DateOnly.")
        };

    // Called when sending a DateOnly as a query parameter (none of the current queries do,
    // but without this a future query with a DateOnly parameter would break).
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value;
    }
}