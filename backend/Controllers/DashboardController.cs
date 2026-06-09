using CovidDashboard.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CovidDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly CovidDbContext _context;

        public DashboardController(CovidDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/dashboard/summary
        /// Returns global totals for the latest available date.
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            if (!await _context.CovidRecords.AnyAsync())
            {
                return Ok(new
                {
                    TotalConfirmed = 0,
                    TotalDeaths = 0,
                    TotalRecovered = 0,
                    TotalActive = 0,
                    Countries = 0
                });
            }

            var latestDate = await _context.CovidRecords.MaxAsync(r => r.Date);

            var summary = await _context.CovidRecords
                .Where(r => r.Date == latestDate)
                .GroupBy(r => 1)
                .Select(g => new
                {
                    TotalConfirmed = g.Sum(r => r.Confirmed),
                    TotalDeaths = g.Sum(r => r.Deaths),
                    TotalRecovered = g.Sum(r => r.Recovered),
                    TotalActive = g.Sum(r => r.Active),
                    Countries = g.Select(r => r.CountryRegion).Distinct().Count()
                })
                .FirstOrDefaultAsync();

            return Ok(summary);
        }

        /// <summary>
        /// GET /api/dashboard/by-country
        /// Returns aggregated data grouped by country for the latest date, including DailyIncrease.
        /// </summary>
        [HttpGet("by-country")]
        public async Task<IActionResult> GetByCountry()
        {
            if (!await _context.CovidRecords.AnyAsync())
            {
                return Ok(new List<object>());
            }

            var latestDate = await _context.CovidRecords.MaxAsync(r => r.Date);

            // Fetch latest records grouped by country
            var latestRecords = await _context.CovidRecords
                .Where(r => r.Date == latestDate)
                .GroupBy(r => new { r.CountryRegion, r.CountryCode, r.Lat, r.Lon })
                .Select(g => new
                {
                    Country = g.Key.CountryRegion,
                    CountryCode = g.Key.CountryCode,
                    Lat = g.Key.Lat,
                    Lon = g.Key.Lon,
                    Confirmed = g.Sum(r => r.Confirmed),
                    Deaths = g.Sum(r => r.Deaths),
                    Recovered = g.Sum(r => r.Recovered),
                    Active = g.Sum(r => r.Active)
                })
                .ToListAsync();

            // Find the date immediately before the latest date to compute DailyIncrease
            var dayBeforeLatestDate = await _context.CovidRecords
                .Where(r => r.Date < latestDate)
                .OrderByDescending(r => r.Date)
                .Select(r => r.Date)
                .FirstOrDefaultAsync();

            var previousConfirmedMap = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

            if (dayBeforeLatestDate != default)
            {
                var prevRecords = await _context.CovidRecords
                    .Where(r => r.Date == dayBeforeLatestDate)
                    .GroupBy(r => r.CountryRegion)
                    .Select(g => new
                    {
                        Country = g.Key,
                        Confirmed = g.Sum(r => r.Confirmed)
                    })
                    .ToListAsync();

                previousConfirmedMap = prevRecords.ToDictionary(
                    r => r.Country,
                    r => r.Confirmed,
                    StringComparer.OrdinalIgnoreCase
                );
            }

            // Calculate daily increase relative to the day before
            var result = latestRecords.Select(r =>
            {
                previousConfirmedMap.TryGetValue(r.Country, out var prevConfirmed);
                long dailyIncrease = Math.Max(0, r.Confirmed - prevConfirmed);

                return new
                {
                    Country = r.Country,
                    CountryCode = r.CountryCode,
                    Lat = r.Lat,
                    Lon = r.Lon,
                    Confirmed = r.Confirmed,
                    Deaths = r.Deaths,
                    Recovered = r.Recovered,
                    Active = r.Active,
                    DailyIncrease = dailyIncrease
                };
            })
            .OrderByDescending(r => r.Confirmed)
            .ToList();

            return Ok(result);
        }

        /// <summary>
        /// GET /api/dashboard/daily-trend?country=US
        /// Returns time series data aggregated at country level for line charts.
        /// </summary>
        [HttpGet("daily-trend")]
        public async Task<IActionResult> GetDailyTrend([FromQuery] string? country)
        {
            var query = _context.CovidRecords.AsQueryable();

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(r => r.CountryRegion == country);
            }

            var data = await query
                .GroupBy(r => new { r.Date, r.CountryRegion })
                .Select(g => new
                {
                    Date = g.Key.Date,
                    CountryRegion = g.Key.CountryRegion,
                    Confirmed = g.Sum(r => r.Confirmed),
                    Deaths = g.Sum(r => r.Deaths),
                    Recovered = g.Sum(r => r.Recovered),
                    Active = g.Sum(r => r.Active)
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            return Ok(data);
        }
    }
}
