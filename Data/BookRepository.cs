using Lesson3_CNLTWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace Lesson3_CNLTWeb.Data
{
    /// <summary>
    /// Thực hiện các thao tác CRUD với bảng Book qua Entity Framework Core.
    /// </summary>
    public class BookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Book> GetAll()
        {
            return _context.Books
                .OrderBy(b => b.Id)
                .ToList(); // SELECT * FROM Books ORDER BY Id
        }

        public Book? GetById(int id)
        {
            return _context.Books.Find(id); // SELECT * FROM Books WHERE Id = @id
        }

        public void Add(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public bool Update(Book book)
        {
            var existing = _context.Books.Find(book.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Name = book.Name;
            existing.Price = book.Price;
            _context.SaveChanges(); // lưu thay đổi vào database
            return true;
        }

        public bool Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book); // DELETE FROM Books WHERE Id = @id
            _context.SaveChanges();
            return true;
        }
    }
}
