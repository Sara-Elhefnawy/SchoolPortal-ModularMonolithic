using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Students.Domain.Addresses;

public static class AddressErrors
{
    public static readonly Error InvalidCity =
        Error.Validation("Address.InvalidCity", "City is required and must not exceed 100 characters.");

    public static readonly Error InvalidState =
        Error.Validation("Address.InvalidState", "State is required and must not exceed 100 characters.");

    public static readonly Error InvalidZipCode = Error.Validation("Address.InvalidZipCode",
        "Zip code is required and must not exceed 20 characters.");

    public static readonly Error InvalidCountry = Error.Validation("Address.InvalidCountry",
        "Country is required and must not exceed 100 characters.");

    public static readonly Error InvalidBuildingNumber = Error.Validation("Address.InvalidBuildingNumber",
        "Building Number is required and must not exceed 100 characters.");
}