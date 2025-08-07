namespace CYR.User;

public record class UserCompany(int UserId,string? Username, string? Role, DateTime? CreatedAt, string? UserLogo,
    int CompanyId, string? CompanyName, string? CompanyStreet, string? CompanyCity, string? CompanyPlz,
    string? CompanyHouseNumber, string? CompanyTelefonNumber, string? CompanyEmailAddress, string? CompanyBankName,
    string? CompanyIban, string? CompanyBic, string? CompanyUstidnr, string? CompanyStnr, string? CompanyLogo);
