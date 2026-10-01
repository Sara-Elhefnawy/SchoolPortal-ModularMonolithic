using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Domain.Addresses;

public sealed class Address
{
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string ZipCode { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public int BuildingNumber { get; private set; }

    private Address() {}

    private Address(
        string city,
        string state,
        string zipCode,
        string country,
        int buildingNumber)
    {
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
        BuildingNumber = buildingNumber;
    }

    public static Result<Address> Create(
        string city,
        string state,
        string zipCode,
        string country,
        int buildingNumber)
    {
        var validate = ValidateAll(city, state, zipCode, country, buildingNumber);

        return Result<Address>.Success(
            new Address(
                city.Trim(),
                state.Trim(),
                zipCode.Trim(),
                country.Trim(),
                buildingNumber));
    }

    public Result UpdateAddress(Address address)
    {
        var addressResult = ValidateAll(address.City, address.State, address.ZipCode, address.Country, address.BuildingNumber);
        if (addressResult.IsFailure) return addressResult;

        City = address.City.Trim();
        State = address.State.Trim();
        ZipCode = address.ZipCode.Trim();
        Country = address.Country.Trim();
        BuildingNumber = address.BuildingNumber;
        
        return Result.Success();
    }
    
    private static Result ValidateAll(string city, string state, string zipCode, string country, int buildingNumber)
    {
        if (string.IsNullOrWhiteSpace(city) ||
            city.Trim().Length > Constants.MaxCityLength)
            return Result<Address>.Failure(AddressErrors.InvalidCity);

        if (string.IsNullOrWhiteSpace(state) ||
            state.Trim().Length > Constants.MaxStateLength)
            return Result<Address>.Failure(AddressErrors.InvalidState);

        if (string.IsNullOrWhiteSpace(zipCode) ||
            zipCode.Trim().Length > Constants.MaxZipCodeLength)
            return Result<Address>.Failure(AddressErrors.InvalidZipCode);

        if (string.IsNullOrWhiteSpace(country) ||
            country.Trim().Length > Constants.MaxCountryLength)
            return Result<Address>.Failure(AddressErrors.InvalidCountry);

        if (buildingNumber <= 0)
            return Result<Address>.Failure(AddressErrors.InvalidBuildingNumber);
        
        return Result.Success();
    }
}