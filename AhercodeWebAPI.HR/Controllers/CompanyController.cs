using AhercodeWebAPI.HR.Dtos.Company;
using AhercodeWebAPI.HR.Dtos.Currency;
using AhercodeWebAPI.HR.UserModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AhercodeWebAPI.HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly FrankiesHotelContext _context;
        private readonly IMapper mapper;

        public static IWebHostEnvironment _environment;
        public CompanyController(IWebHostEnvironment environment, FrankiesHotelContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
            _environment = environment;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompany()
        {

            var company = await _context.Companies.ToListAsync();

            return Ok(company);
        }
        [HttpPost]
        public async Task<ActionResult<Company>> AddCompany(CompanyCreateDto companyDto)
        {

            var company = mapper.Map<Company>(companyDto);
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();

            return Ok(company.Id);
        }
        [HttpPut]
        public async Task<ActionResult<IEnumerable<Company>>> PutCompany(CompanyPutDto companyPut)
        {
            //var tax = _context.TaxTables.Where(x => x.Id == taxPut.Id).ToList();
            var company = mapper.Map<Company>(companyPut);

            //var mapTaxes = mapper.Map
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();

            return Ok(company.Id);
        }

        [HttpDelete("id")]
        public async Task<ActionResult<Company>> DeleteCurrency(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
