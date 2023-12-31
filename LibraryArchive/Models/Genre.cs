﻿using System.ComponentModel.DataAnnotations;

namespace LibraryArchive.Models
{
    public class Genre
    {
        [Key]
        public string GenreId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
