using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CovidDashboard.Api.Models
{
    /// <summary>
    /// Represents a single time-series data point for a country/province on a specific date.
    /// Populated from the JHU CSSE time_series CSV files (confirmed, deaths, recovered).
    /// </summary>
    public class CovidRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string CountryRegion { get; set; } = string.Empty;

        [MaxLength(3)]
        public string? CountryCode { get; set; }

        [MaxLength(200)]
        public string? ProvinceState { get; set; }

        public double? Lat { get; set; }

        [Column("Long")]
        public double? Lon { get; set; }

        public long Confirmed { get; set; }

        public long Deaths { get; set; }

        public long Recovered { get; set; }

        public long Active { get; set; }

        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Represents a US daily report record.
    /// Populated from csse_covid_19_daily_reports_us CSV.
    /// </summary>
    public class DailyReportUs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProvinceState { get; set; } = string.Empty;

        [MaxLength(200)]
        public string CountryRegion { get; set; } = "US";

        public DateTime LastUpdate { get; set; }

        public double? Lat { get; set; }

        [Column("Long_")]
        public double? Lon { get; set; }

        public long Confirmed { get; set; }

        public long Deaths { get; set; }

        public long? Recovered { get; set; }

        public long? Active { get; set; }

        [MaxLength(10)]
        public string? FIPS { get; set; }

        public double? IncidentRate { get; set; }

        public double? TotalTestResults { get; set; }

        public double? CaseFatalityRatio { get; set; }

        [MaxLength(10)]
        public string? ISO3 { get; set; }

        public double? TestingRate { get; set; }

        public DateTime Date { get; set; }
    }
}
