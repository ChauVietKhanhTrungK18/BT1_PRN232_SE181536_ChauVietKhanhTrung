using CovidDashboard.Api.Data;
using CovidDashboard.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace CovidDashboard.Api.Controllers
{
    public class CovidRecordsController : ODataController
    {
        private readonly CovidDbContext _context;

        public CovidRecordsController(CovidDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /odata/CovidRecords
        /// Supports OData query options: $filter, $select, $orderby, $top, $skip, $count, $apply
        /// </summary>
        [HttpGet]
        [EnableQuery(PageSize = 1000)]
        public IActionResult Get()
        {
            return Ok(_context.CovidRecords);
        }

        /// <summary>
        /// GET /odata/CovidRecords(5)
        /// </summary>
        [HttpGet]
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var record = _context.CovidRecords.Where(r => r.Id == key);
            if (!record.Any())
                return NotFound();
            return Ok(SingleResult.Create(record));
        }
    }
}
