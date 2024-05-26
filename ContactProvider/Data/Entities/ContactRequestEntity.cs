namespace ContactProvider.Data.Entities;

public class ContactRequestEntity
{
    public int Id { get; set; }
    public string Fullname { get; set; } = null!;
    public string Email { get; set; } = "";
    public int? Service { get; set; }
    public string Message { get; set; } = "";
}
