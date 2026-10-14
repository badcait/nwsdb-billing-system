using System.ComponentModel.DataAnnotations;
using website.Model;

namespace website.DTO;

public class GenerateBillDto
{
    public int StartReadingId { get; set; }
    public int EndReadingId { get; set; }
    public DateTime DueDate { get; set; }
}
