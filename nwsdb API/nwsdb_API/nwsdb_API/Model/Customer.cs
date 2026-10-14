namespace nwsdb_API.Model;

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}
