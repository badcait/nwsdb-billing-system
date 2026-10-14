using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class UsageReadDto
{
    public int ReadingId { get; set; }
    public DateTime ReadingDate { get; set; }
    public decimal MeterValue { get; set; }
    public decimal? Consumption { get; set; }
}
