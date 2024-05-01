namespace Riode.Models;

public class ContactComment
{
    public int Id { get; set; }
    public string Comment { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}
