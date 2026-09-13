namespace SchoolPortal.Web.Endpoints.Students.Requests.Students;

public sealed record UpdateStudentAddressRequest(string City, string State, string ZipCode, string Country, int BuildingNumber);