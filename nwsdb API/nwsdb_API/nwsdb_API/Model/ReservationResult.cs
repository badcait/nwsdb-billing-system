namespace nwsdb_API.Model;
public class ReservationResult
{
    public ReservationKind Kind { get; set; }
    public PaymentAttempt? Attempt { get; set; }
    public bool NoReservation { get; set; }
    public string? ConflictCode { get; set; }
}
