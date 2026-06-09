using CovidDashboard.Api.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CovidDashboard.Api.Data
{
    public static class DataSeeder
    {
        private const string ConfirmedUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";
        private const string DeathsUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_deaths_global.csv";
        private const string RecoveredUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_recovered_global.csv";
        private const string DailyReportUsUrl = "https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports_us/02-21-2022.csv";

        private static readonly Dictionary<string, string> CountryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "afghanistan", "AFG" },
            { "albania", "ALB" },
            { "algeria", "DZA" },
            { "andorra", "AND" },
            { "angola", "AGO" },
            { "antigua and barbuda", "ATG" },
            { "argentina", "ARG" },
            { "armenia", "ARM" },
            { "australia", "AUS" },
            { "austria", "AUT" },
            { "azerbaijan", "AZE" },
            { "bahamas", "BHS" },
            { "bahrain", "BHR" },
            { "bangladesh", "BGD" },
            { "barbados", "BRB" },
            { "belarus", "BLR" },
            { "belgium", "BEL" },
            { "belize", "BLZ" },
            { "benin", "BEN" },
            { "bhutan", "BTN" },
            { "bolivia", "BOL" },
            { "bosnia and herzegovina", "BIH" },
            { "botswana", "BWA" },
            { "brazil", "BRA" },
            { "brunei", "BRN" },
            { "bulgaria", "BGR" },
            { "burkina faso", "BFA" },
            { "burundi", "BDI" },
            { "cabo verde", "CPV" },
            { "cambodia", "KHM" },
            { "cameroon", "CMR" },
            { "canada", "CAN" },
            { "central african republic", "CAF" },
            { "chad", "TCD" },
            { "chile", "CHL" },
            { "china", "CHN" },
            { "colombia", "COL" },
            { "comoros", "COM" },
            { "congo (brazzaville)", "COG" },
            { "congo (kinshasa)", "COD" },
            { "costa rica", "CRI" },
            { "cote d'ivoire", "CIV" },
            { "croatia", "HRV" },
            { "cuba", "CUB" },
            { "cyprus", "CYP" },
            { "czechia", "CZE" },
            { "denmark", "DNK" },
            { "djibouti", "DJI" },
            { "dominica", "DMA" },
            { "dominican republic", "DOM" },
            { "ecuador", "ECU" },
            { "egypt", "EGY" },
            { "el salvador", "SLV" },
            { "equatorial guinea", "GNQ" },
            { "eritrea", "ERI" },
            { "estonia", "EST" },
            { "eswatini", "SWZ" },
            { "ethiopia", "ETH" },
            { "fiji", "FJI" },
            { "finland", "FIN" },
            { "france", "FRA" },
            { "gabon", "GAB" },
            { "gambia", "GMB" },
            { "georgia", "GEO" },
            { "germany", "DEU" },
            { "ghana", "GHA" },
            { "greece", "GRC" },
            { "grenada", "GRD" },
            { "guatemala", "GTM" },
            { "guinea", "GIN" },
            { "guinea-bissau", "GNB" },
            { "guyana", "GUY" },
            { "haiti", "HTI" },
            { "honduras", "HND" },
            { "hungary", "HUN" },
            { "iceland", "ISL" },
            { "india", "IND" },
            { "indonesia", "IDN" },
            { "iran", "IRN" },
            { "iraq", "IRQ" },
            { "ireland", "IRL" },
            { "israel", "ISR" },
            { "italy", "ITA" },
            { "jamaica", "JAM" },
            { "japan", "JPN" },
            { "jordan", "JOR" },
            { "kazakhstan", "KAZ" },
            { "kenya", "KEN" },
            { "kiribati", "KIR" },
            { "korea, south", "KOR" },
            { "south korea", "KOR" },
            { "kosovo", "XKX" },
            { "kuwait", "KWT" },
            { "kyrgyzstan", "KGZ" },
            { "laos", "LAO" },
            { "latvia", "LVA" },
            { "lebanon", "LBN" },
            { "lesotho", "LSO" },
            { "liberia", "LBR" },
            { "libya", "LBY" },
            { "liechtenstein", "LIE" },
            { "lithuania", "LTU" },
            { "luxembourg", "LUX" },
            { "madagascar", "MDG" },
            { "malawi", "MWI" },
            { "malaysia", "MYS" },
            { "maldives", "MDV" },
            { "mali", "MLI" },
            { "malta", "MLT" },
            { "marshall islands" , "MHL" },
            { "mauritania", "MRT" },
            { "mauritius", "MUS" },
            { "mexico", "MEX" },
            { "micronesia", "FSM" },
            { "moldova", "MDA" },
            { "monaco", "MCO" },
            { "mongolia", "MNG" },
            { "montenegro", "MNE" },
            { "morocco", "MAR" },
            { "mozambique", "MOZ" },
            { "myanmar", "MMR" },
            { "namibia", "NAM" },
            { "nauru", "NRU" },
            { "nepal", "NPL" },
            { "netherlands", "NLD" },
            { "new zealand", "NZL" },
            { "nicaragua", "NIC" },
            { "niger", "NER" },
            { "nigeria", "NGA" },
            { "north macedonia", "MKD" },
            { "norway", "NOR" },
            { "oman", "OMN" },
            { "pakistan", "PAK" },
            { "palau", "PLW" },
            { "panama", "PAN" },
            { "papua new guinea", "PNG" },
            { "paraguay", "PRY" },
            { "peru", "PER" },
            { "philippines", "PHL" },
            { "poland", "POL" },
            { "portugal", "PRT" },
            { "qatar", "QAT" },
            { "romania", "ROU" },
            { "russia", "RUS" },
            { "rwanda", "RWA" },
            { "saint kitts and nevis", "KNA" },
            { "saint lucia", "LCA" },
            { "saint vincent and the grenadines", "VCT" },
            { "samoa", "WSM" },
            { "san marino", "SMR" },
            { "sao tome and principe", "STP" },
            { "saudi arabia", "SAU" },
            { "senegal", "SEN" },
            { "serbia", "SRB" },
            { "seychelles", "SYC" },
            { "sierra leone", "SLE" },
            { "singapore", "SGP" },
            { "slovakia", "SVK" },
            { "slovenia", "SVN" },
            { "solomon islands", "SLB" },
            { "somalia", "SOM" },
            { "south africa", "ZAF" },
            { "south sudan", "SSD" },
            { "spain", "ESP" },
            { "sri lanka", "LKA" },
            { "sudan", "SDN" },
            { "suriname", "SUR" },
            { "sweden", "SWE" },
            { "switzerland", "CHE" },
            { "syria", "SYR" },
            { "taiwan*", "TWN" },
            { "taiwan", "TWN" },
            { "tajikistan", "TJK" },
            { "tanzania", "TZA" },
            { "thailand", "THA" },
            { "timor-leste", "TLS" },
            { "togo", "TGO" },
            { "tonga", "TON" },
            { "trinidad and tobago", "TTO" },
            { "tunisia", "TUN" },
            { "turkey", "TUR" },
            { "turkmenistan", "TKM" },
            { "tuvalu", "TUV" },
            { "uganda", "UGA" },
            { "ukraine", "UKR" },
            { "united arab emirates", "ARE" },
            { "united kingdom", "GBR" },
            { "uk", "GBR" },
            { "uruguay", "URY" },
            { "us", "USA" },
            { "united states", "USA" },
            { "uzbekistan", "UZB" },
            { "vanuatu", "VUT" },
            { "venezuela", "VEN" },
            { "vietnam", "VNM" },
            { "west bank and gaza", "PSE" },
            { "yemen", "YEM" },
            { "zambia", "ZMB" },
            { "zimbabwe", "ZWE" }
        };

        public static async Task SeedAsync(CovidDbContext context)
        {
            // Set longer command timeout to prevent timeout on large bulk inserts
            context.Database.SetCommandTimeout(360);

            if (await context.CovidRecords.AnyAsync() && await context.DailyReportsUs.AnyAsync())
            {
                Console.WriteLine("Database already seeded. Skipping.");
                return;
            }

            var seedDir = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData");
            if (!Directory.Exists(seedDir))
            {
                Directory.CreateDirectory(seedDir);
            }

            // Define local CSV file paths
            var confirmedFile = Path.Combine(seedDir, "time_series_covid19_confirmed_global.csv");
            var deathsFile = Path.Combine(seedDir, "time_series_covid19_deaths_global.csv");
            var recoveredFile = Path.Combine(seedDir, "time_series_covid19_recovered_global.csv");
            var dailyReportUsFile = Path.Combine(seedDir, "02-21-2022.csv");

            // Ensure files are downloaded
            await EnsureFileDownloadedAsync(ConfirmedUrl, confirmedFile);
            await EnsureFileDownloadedAsync(DeathsUrl, deathsFile);
            await EnsureFileDownloadedAsync(RecoveredUrl, recoveredFile);
            await EnsureFileDownloadedAsync(DailyReportUsUrl, dailyReportUsFile);

            // 1. Seed Time Series global data
            if (!await context.CovidRecords.AnyAsync())
            {
                Console.WriteLine("Parsing global time series data...");
                var confirmedData = await ParseTimeSeriesCsvAsync(confirmedFile);
                var deathsData = await ParseTimeSeriesCsvAsync(deathsFile);
                var recoveredData = await ParseTimeSeriesCsvAsync(recoveredFile);

                Console.WriteLine("Merging time series datasets...");
                var allKeys = confirmedData.Keys
                    .Union(deathsData.Keys)
                    .Union(recoveredData.Keys)
                    .ToList();

                var recordsToInsert = new List<CovidRecord>();
                foreach (var key in allKeys)
                {
                    confirmedData.TryGetValue(key, out var confTuple);
                    deathsData.TryGetValue(key, out var deathTuple);
                    recoveredData.TryGetValue(key, out var recTuple);

                    double? lat = confTuple.lat ?? deathTuple.lat ?? recTuple.lat;
                    double? lon = confTuple.lon ?? deathTuple.lon ?? recTuple.lon;

                    long confirmed = confTuple.val;
                    long deaths = deathTuple.val;
                    long recovered = recTuple.val;
                    long active = Math.Max(0, confirmed - deaths - recovered);

                    string countryCode = GetCountryCode(key.country);

                    recordsToInsert.Add(new CovidRecord
                    {
                        CountryRegion = key.country,
                        ProvinceState = string.IsNullOrEmpty(key.province) ? null : key.province,
                        CountryCode = countryCode,
                        Lat = lat,
                        Lon = lon,
                        Confirmed = confirmed,
                        Deaths = deaths,
                        Recovered = recovered,
                        Active = active,
                        Date = key.date
                    });
                }

                Console.WriteLine($"Inserting {recordsToInsert.Count} time-series records in batches...");
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                const int batchSize = 10000;
                for (int i = 0; i < recordsToInsert.Count; i += batchSize)
                {
                    var batch = recordsToInsert.Skip(i).Take(batchSize).ToList();
                    context.CovidRecords.AddRange(batch);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Saved batch {i / batchSize + 1} of {(recordsToInsert.Count - 1) / batchSize + 1}...");
                }
                context.ChangeTracker.AutoDetectChangesEnabled = true;
                Console.WriteLine("Global time series seeding completed.");
            }

            // 2. Seed Daily Reports US data
            if (!await context.DailyReportsUs.AnyAsync())
            {
                Console.WriteLine("Parsing US Daily Report data...");
                var dailyReports = await ParseDailyReportUsCsvAsync(dailyReportUsFile);

                Console.WriteLine($"Inserting {dailyReports.Count} DailyReportUs records...");
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                context.DailyReportsUs.AddRange(dailyReports);
                await context.SaveChangesAsync();
                context.ChangeTracker.AutoDetectChangesEnabled = true;
                Console.WriteLine("US Daily Report seeding completed.");
            }
        }

        private static async Task EnsureFileDownloadedAsync(string url, string localPath)
        {
            if (File.Exists(localPath) && new FileInfo(localPath).Length > 0)
            {
                return;
            }

            Console.WriteLine($"Downloading {url}...");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");
            var bytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(localPath, bytes);
            Console.WriteLine($"Saved to {localPath}");
        }

        private static async Task<Dictionary<(string country, string province, DateTime date), (long val, double? lat, double? lon)>> ParseTimeSeriesCsvAsync(string filePath)
        {
            var result = new Dictionary<(string country, string province, DateTime date), (long val, double? lat, double? lon)>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            await csv.ReadAsync();
            csv.ReadHeader();
            string[] headers = csv.HeaderRecord;

            while (await csv.ReadAsync())
            {
                var province = csv.GetField<string>(0) ?? string.Empty;
                var country = csv.GetField<string>(1) ?? string.Empty;
                var latStr = csv.GetField<string>(2);
                var lonStr = csv.GetField<string>(3);

                double? lat = double.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var lt) ? lt : null;
                double? lon = double.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var ln) ? ln : null;

                for (int i = 4; i < headers.Length; i++)
                {
                    var headerDate = headers[i];
                    if (DateTime.TryParseExact(headerDate, "M/d/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        var valStr = csv.GetField<string>(i);
                        long val = long.TryParse(valStr, out var v) ? v : 0;
                        result[(country, province, date)] = (val, lat, lon);
                    }
                }
            }
            return result;
        }

        private static async Task<List<DailyReportUs>> ParseDailyReportUsCsvAsync(string filePath)
        {
            var result = new List<DailyReportUs>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                var provinceState = GetFieldSafe<string>(csv, "Province_State") ?? string.Empty;
                var countryRegion = GetFieldSafe<string>(csv, "Country_Region") ?? "US";

                var lastUpdateStr = GetFieldSafe<string>(csv, "Last_Update");
                DateTime lastUpdate = DateTime.MinValue;
                if (!string.IsNullOrEmpty(lastUpdateStr))
                {
                    if (!DateTime.TryParse(lastUpdateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out lastUpdate))
                    {
                        lastUpdate = new DateTime(2022, 2, 21);
                    }
                }

                double? lat = GetFieldSafe<double?>(csv, "Lat");
                double? lon = GetFieldSafe<double?>(csv, "Long_");

                long confirmed = GetFieldSafe<long>(csv, "Confirmed");
                long deaths = GetFieldSafe<long>(csv, "Deaths");
                long? recovered = GetFieldSafe<long?>(csv, "Recovered");
                long? active = GetFieldSafe<long?>(csv, "Active");

                string? fips = GetFieldSafe<string>(csv, "FIPS");
                double? incidentRate = GetFieldSafe<double?>(csv, "Incident_Rate");
                double? totalTestResults = GetFieldSafe<double?>(csv, "Total_Test_Results");
                double? caseFatalityRatio = GetFieldSafe<double?>(csv, "Case_Fatality_Ratio");
                string? iso3 = GetFieldSafe<string>(csv, "ISO3") ?? "USA";
                double? testingRate = GetFieldSafe<double?>(csv, "Testing_Rate");

                result.Add(new DailyReportUs
                {
                    ProvinceState = provinceState,
                    CountryRegion = countryRegion,
                    LastUpdate = lastUpdate,
                    Lat = lat,
                    Lon = lon,
                    Confirmed = confirmed,
                    Deaths = deaths,
                    Recovered = recovered,
                    Active = active,
                    FIPS = fips,
                    IncidentRate = incidentRate,
                    TotalTestResults = totalTestResults,
                    CaseFatalityRatio = caseFatalityRatio,
                    ISO3 = iso3,
                    TestingRate = testingRate,
                    Date = new DateTime(2022, 2, 21)
                });
            }

            return result;
        }

        private static T? GetFieldSafe<T>(CsvReader csv, string name)
        {
            try
            {
                if (csv.HeaderRecord == null || !csv.HeaderRecord.Contains(name))
                    return default;

                return csv.GetField<T>(name);
            }
            catch
            {
                return default;
            }
        }

        private static string GetCountryCode(string country)
        {
            if (string.IsNullOrEmpty(country)) return string.Empty;

            string name = country.Trim().ToLowerInvariant();
            if (name.Contains("taiwan")) return "TWN";
            if (name.Contains("korea, south") || name.Contains("south korea")) return "KOR";
            if (name.Contains("us") || name.Contains("united states")) return "USA";
            if (name.Contains("united kingdom") || name.Contains("uk")) return "GBR";
            if (name.Contains("vietnam") || name.Contains("viet nam")) return "VNM";
            if (name.Contains("russia") || name.Contains("russian federation")) return "RUS";
            if (name.Contains("iran")) return "IRN";
            if (name.Contains("syria")) return "SYR";
            if (name.Contains("venezuela")) return "VEN";
            if (name.Contains("laos") || name.Contains("lao")) return "LAO";
            if (name.Contains("moldova")) return "MDA";
            if (name.Contains("congo (kinshasa)")) return "COD";
            if (name.Contains("congo (brazzaville)")) return "COG";
            if (name.Contains("cote d'ivoire") || name.Contains("ivory coast")) return "CIV";
            if (name.Contains("cabo verde") || name.Contains("cape verde")) return "CPV";
            if (name.Contains("west bank and gaza") || name.Contains("palestine")) return "PSE";
            if (name.Contains("burma") || name.Contains("myanmar")) return "MMR";
            if (name.Contains("brunei")) return "BRN";
            if (name.Contains("east timor") || name.Contains("timor-leste")) return "TLS";

            if (CountryMap.TryGetValue(name, out var code))
                return code;

            return string.Empty;
        }
    }
}
