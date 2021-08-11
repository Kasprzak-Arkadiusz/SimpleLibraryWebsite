﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleLibraryWebsite.Models
{
    public enum Genres { Adventure, Novel, SciFi, Fantasy, Romance, Thriller, Horror, Biography, Poetry, Scientific }
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Display(Name = "Book author")]
        public string Author { get; set; }
        [Display(Name = "Book title")]
        public string Title { get; set; }
        [Required]
        public Genres? Genre { get; set; }
        [Display(Name = "Date of adding")]
        [DataType(DataType.Date)]
        public DateTime DateOfAdding { get; set; }
        [Display(Name = "Is book borrowed?")]
        public bool IsBorrowed { get; set; }

        public Book(string author, string title, Genres genre)
        {
            Author = author;
            Title = title;
            Genre = genre;
            DateOfAdding = DateTime.Now;
            IsBorrowed = false;
        }

        public Book()
        {
        }

        public void FillMissingProperties()
        {
            DateOfAdding = DateTime.Now;
            IsBorrowed = false;
        }
    }
}
