using System.ComponentModel.DataAnnotations;
using website.Model;

namespace website.DTO;

public class CreateReadingDto
{
    public DateTime ReadingDate { get; set; }
    public decimal MeterValue { get; set; }
}
