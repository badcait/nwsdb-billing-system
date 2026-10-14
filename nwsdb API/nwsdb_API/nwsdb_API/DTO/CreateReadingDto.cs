using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class CreateReadingDto
{
    public DateTime ReadingDate { get; set; }
    public decimal MeterValue { get; set; }
}
