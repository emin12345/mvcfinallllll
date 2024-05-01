using System.ComponentModel.DataAnnotations;

namespace Riode.Models;

public class Review
{
    public int Id { get; set; }
    public string Comment { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public AppUser User { get; set; }
}