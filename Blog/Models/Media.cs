using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Media
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Path { get; set; }
        [Required]
        public MediaTypesEnum Type { get; set; }
        public string Uri { get; set; }
        [Required]
        public string Mime { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }

    public class MediaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public MediaTypesEnum Type { get; set; }
        public string Mime { get; set; }
        public string UserId { get; set; }
        public string Uri { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}