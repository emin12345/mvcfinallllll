namespace Riode.Models;

public class Topic
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<BlogTopic> BlogTopic { get; set; } = null!;
}