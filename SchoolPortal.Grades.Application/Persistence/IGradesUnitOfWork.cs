using SchoolPortal.SharedKernel.Persistence;

namespace SchoolPortal.Grades.Application.Persistence;

// Empty on purpose — it exists only to give DI something unambiguous to resolve.
// A GradesCommandHandler depends on THIS, never on the bare IUnitOfWork, so
//      the container never has to guess which DbContext you meant.
public interface IGradesUnitOfWork : IUnitOfWork;