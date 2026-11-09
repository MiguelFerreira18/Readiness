using System.Collections.Generic;
using System.Linq;

namespace readiness.repository;

public class BookRepository
{
    // 1. Listas para guardar os dados em memória
    private List<Book> _books = new List<Book>();
    private List<Book> _wishes = new List<Book>();
    private List<Book> _readBooks = new List<Book>();
    private List<Book> _toReadBooks = new List<Book>();

    // ==========================================
    // GERIR LIVROS (Biblioteca Principal)
    // ==========================================
    public void AddBook(Book newBook)
    {
        if (newBook != null)
        {
            _books.Add(newBook);
        }
    }

    public void RemoveBook(Book bookToRemove)
    {
        if (bookToRemove != null)
        {
            _books.Remove(bookToRemove);
        }
    }

    public List<Book> GetAllBooks()
    {
        return _books;
    }

    // ==========================================
    // GERIR DESEJOS (Wishes)
    // ==========================================
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

    // ==========================================
    // GERIR LIVROS LIDOS (Read)
    // ==========================================
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

    // ==========================================
    // GERIR LIVROS PARA LER (To Read)
    // ==========================================
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