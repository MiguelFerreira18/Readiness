using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using readiness.model;

namespace readiness.repository;

public class BookRepository
{
    private readonly AppDbContext _context;

    // Listas para gerir os estados (Desejo, Lido, Para Ler)

    private List<Book> _wishes = new List<Book>();
    private List<Book> _readBooks = new List<Book>();
    private List<Book> _toReadBooks = new List<Book>();

    // O construtor recebe a ligação à base de dados
    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // GERIR LIVROS (Ligado à Base de Dados)
    // ==========================================

    public void AddBook(Book newBook)
    {
        if (newBook != null)
        {
            _context.Books.Add(newBook);
            _context.SaveChanges(); // Guarda na BD
        }
    }

    public void RemoveBook(Book bookToRemove)
    {
        if (bookToRemove != null)
        {
            _context.Books.Remove(bookToRemove);
            _context.SaveChanges(); // Remove da BD
        }
    }

    public List<Book> GetAllBooks()
    {
        return _context.Books.ToList(); // Busca da BD
    }

    // ==========================================
    // GERIR ESTADOS
    // ==========================================

    // Desejos (Wishes)
    public void AddWish(Book book)
    {
        if (book != null && !_wishes.Contains(book))
        {
            _wishes.Add(book);
        }
    }

    public void RemoveWish(Book book)
    {
        if (book != null)
        {
            _wishes.Remove(book);
        }
    }

    public List<Book> GetAllWishes()
    {
        return _wishes;
    }

    // Lidos (Read)
    public void MarkAsRead(Book book)
    {
        if (book != null && !_readBooks.Contains(book))
        {
            _readBooks.Add(book);
        }
    }

    public void RemoveFromRead(Book book)
    {
        if (book != null)
        {
            _readBooks.Remove(book);
        }
    }

    public List<Book> GetAllReadBooks()
    {
        return _readBooks;
    }

    // Para Ler (To Read)
    public void MarkAsToRead(Book book)
    {
        if (book != null && !_toReadBooks.Contains(book))
        {
            _toReadBooks.Add(book);
        }
    }

    public void RemoveFromToRead(Book book)
    {
        if (book != null)
        {
            _toReadBooks.Remove(book);
        }
    }

    public List<Book> GetAllToReadBooks()
    {
        return _toReadBooks;
    }
}