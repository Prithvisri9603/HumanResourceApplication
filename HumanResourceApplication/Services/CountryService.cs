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
        public async Task<List<CountryDTO>> GetAllCountries()
        {
            var countries = await _hrContext.Countries.ToListAsync();
            var countryDTOList = _mapper.Map<List<CountryDTO>>(countries);
            return countryDTOList;
            await _hrContext.SaveChangesAsync();
        }
        #endregion


        #region Get Country By Id
        public async Task<CountryDTO?> GetCountryById(string Countryid)
        {
            
            var country = await _hrContext.Countries .FirstOrDefaultAsync(c => c.CountryId == Countryid);
            if (country == null)
            {
                return null;
            }
            var countryDTO = _mapper.Map<CountryDTO>(country);
            return countryDTO;
            await _hrContext.SaveChangesAsync();
        }
        #endregion

        #region Add Country
        public async Task AddCountry( CountryDTO country)
        {
            var countries = _mapper.Map<Country>(country);
            _hrContext.Countries.Add(countries);
            await _hrContext.SaveChangesAsync();

        }
        #endregion


        
        #region Update Country
        public async Task UpdateCountry(string Countryid, CountryDTO country)
        {
            var countrydata = await _hrContext.Countries.FindAsync(Countryid);
            if (countrydata != null)
            {
                
                _hrContext.Entry(countrydata).State = EntityState.Modified;
                //return "Record Modified Successfully";
            }

            await _hrContext.SaveChangesAsync();

        }
        #endregion

        #region Delete Country By Id
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
