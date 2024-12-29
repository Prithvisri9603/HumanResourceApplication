using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using Microsoft.EntityFrameworkCore;
namespace HumanResourceApplication.Services
{
    public class CountryService : ICountryRepository

    {
        private readonly HrContext _hrContext;
        private readonly IMapper _mapper;
        public CountryService(HrContext hrContext, IMapper mapper)
        {
            _hrContext = hrContext;
            _mapper = mapper;
        }

        #region List All Countries

        /// <summary>
        /// Retrieves a list of all countries from the database.
        /// </summary>
        /// <returns>A list of CountryDTO objects.</returns>

        public async Task<List<CountryDTO>> GetAllCountries()
        {
            
            var countries = await _hrContext.Countries.FromSqlRaw("EXEC GetAllCountries").ToListAsync();

            var countryDTOList = _mapper.Map<List<CountryDTO>>(countries);
            return countryDTOList;

            
            
        }
        #endregion

        #region Get Country By Id

        /// <summary>
        /// Retrieves a specific country by its Id from the database.
        /// </summary>
        /// <param name="Countryid">The Id of the country to retrieve.</param>
        /// <returns>A CountryDTO object if found otherwise null.</returns>

        public async Task<CountryDTO?> GetCountryById(string Countryid)
        {
            
            var country = await _hrContext.Countries .FirstOrDefaultAsync(c => c.CountryId == Countryid);
            if (country == null)
            {
                return null;
            }
            var countryDTO = _mapper.Map<CountryDTO>(country);
            return countryDTO;

            
            
        }
        #endregion


        #region Add Country

        /// <summary>
        /// Adds a new country to the database.
        /// </summary>
        /// <param name="country">The CountryDTO object containing the new country data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddCountry( CountryDTO country)
        {
            var countries = _mapper.Map<Country>(country);
            _hrContext.Countries.Add(countries);
            await _hrContext.SaveChangesAsync();

        }
        #endregion

        #region Update Country
        /// <summary>
        /// Updates an existing country in the database.
        /// </summary>
        /// <param name="Countryid">The ID of the country to update.</param>
        /// <param name="country">The CountryDTO object containing the updated country data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>

        public async Task UpdateCountry(string Countryid, CountryDTO country)
        {
            var countrydata = await _hrContext.Countries.FindAsync(Countryid);
            if (countrydata != null)
            {
                
                _hrContext.Entry(countrydata).State = EntityState.Modified;
                
            }

            await _hrContext.SaveChangesAsync();

        }
        #endregion

        #region Delete Country By Id

        /// <summary>
        /// Deletes a country by its ID from the database.
        /// </summary>
        /// <param name="Countryid">The ID of the country to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteCountryById(string Countryid)
        {
            var country = _hrContext.Countries.Find(Countryid);
            if (country != null)
            {
                _hrContext.Countries.Remove(country);
                await _hrContext.SaveChangesAsync();
            }
        }
        #endregion


    }

}
