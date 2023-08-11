using System.ComponentModel.DataAnnotations;

namespace Mage.Models
{
    public enum GameSizeUnit //predefined values for size
    {
        Bytes,
        Kilobytes,
        Megabytes,
        Gigabytes
    }
    public class Game
    {
        public int Id { get; set; } //primary key

        [Required()]
        [Display(Name = "Category")]
        public int CategoryId { get; set; } //foreign key

        [Required()]
        [Display(Name = "Game Name")]
        public string Name { get; set; }

        [Display(Name = "Game Description")]
        public string? Description { get; set; } //? allows fields to be null

        public string? Genre { get; set; }

        [Required()]
        public decimal Price { get; set; }

        [Required()]
        public decimal Size { get; set; } //file size in GB

        [Required()]
        public GameSizeUnit SizeUnit { get; set; }
        public string? Photo { get; set; }

        //Parent reference (primary key)
        public Category? Category { get; set; } 
    }
}
