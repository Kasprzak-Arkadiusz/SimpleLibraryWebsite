using System;
using System.Threading.Tasks;
using SimpleLibraryWebsite.Models;

namespace SimpleLibraryWebsite.Data.DAL
{
    public sealed class UnitOfWork : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private GenericRepository<Book> _bookRepository;
        private GenericRepository<Loan> _loanRepository;
        private GenericRepository<Reader> _readerRepository;
        private GenericRepository<Request> _requestRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public GenericRepository<Book> BookRepository => _bookRepository ??= new GenericRepository<Book>(_context);

        public GenericRepository<Loan> LoanRepository => _loanRepository ??= new GenericRepository<Loan>(_context);

        public GenericRepository<Reader> ReaderRepository => _readerRepository ??= new GenericRepository<Reader>(_context);

        public GenericRepository<Request> RequestRepository => _requestRepository ??= new GenericRepository<Request>(_context);

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        private bool _disposed;

        private void Dispose(bool disposing)
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
