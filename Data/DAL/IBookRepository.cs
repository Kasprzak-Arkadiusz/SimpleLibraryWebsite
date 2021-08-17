using System;
using System.Collections.Generic;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data.DAL
{
    public interface IBookRepository : IDisposable
    {
        IEnumerable<Book> GetBooks();
        Book GetBookById(int bookId);
        void InsertBook(Book book);
        void DeleteBook(int bookId);
        void UpdateBook(Book book);
        void Save();
    }
}
