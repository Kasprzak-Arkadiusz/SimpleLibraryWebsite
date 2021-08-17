using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data.DAL
{
    public class BookRepository : IBookRepository
    {
        private ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Book> GetBooks()
        {
            return _context.Books.ToList();
        }

        public Book GetBookById(int bookId)
        {
            return _context.Books.Find(bookId);
        }

        public void InsertBook(Book book)
        {
            _context.Books.Add(book);
        }

        public void DeleteBook(int bookId)
        {
           Book book = _context.Books.Find(bookId);
           _context.Books.Remove(book);
        }

        public void UpdateBook(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
