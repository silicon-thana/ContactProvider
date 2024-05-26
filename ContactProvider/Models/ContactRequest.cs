namespace ContactProvider.Models;

public class ContactRequest
{
    public string Fullname { get; set; } = null!;
    public string Email { get; set; } = "";
    public int? Service { get; set; }
    public string Message { get; set; } = "";
}
