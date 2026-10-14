using System.ComponentModel.DataAnnotations;
using nwsdb_API.Model;

namespace nwsdb_API.DTO;

public class GenerateBillDto
{
    public int StartReadingId { get; set; }
    public int EndReadingId { get; set; }
    public DateTime DueDate { get; set; }
}
