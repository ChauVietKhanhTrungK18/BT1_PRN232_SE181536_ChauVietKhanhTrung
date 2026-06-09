using CovidDashboard.Api.Data;
using CovidDashboard.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace CovidDashboard.Api.Controllers
{
    public class DailyReportsUsController : ODataController
    {
        private readonly CovidDbContext _context;

        public DailyReportsUsController(CovidDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /odata/DailyReportsUs
        /// Supports OData query: $filter, $select, $orderby, $top, $skip, $count
        /// </summary>
        [HttpGet]
        [EnableQuery(PageSize = 1000)]
        public IActionResult Get()
        {
            return Ok(_context.DailyReportsUs);
        }

        /// <summary>
        /// GET /odata/DailyReportsUs(5)
        /// </summary>
        [HttpGet]
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var record = _context.DailyReportsUs.Where(r => r.Id == key);
            if (!record.Any())
                return NotFound();
            return Ok(SingleResult.Create(record));
        }
    }
}
