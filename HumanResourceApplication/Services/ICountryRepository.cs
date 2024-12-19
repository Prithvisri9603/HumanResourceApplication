using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Services
{
    public interface ICountryRepository
    {

        Task<List<CountryDTO>> GetAllCountries();
        Task<CountryDTO?> GetCountryById(string Countryid);
        Task AddCountry(CountryDTO country);
        Task UpdateCountry( string Countryid, CountryDTO country);
        Task DeleteCountryById(string Countryid);

    }
}
