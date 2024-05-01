namespace Riode.Models;

public class BlogTopic
{
    public int Id { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;

    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;
}