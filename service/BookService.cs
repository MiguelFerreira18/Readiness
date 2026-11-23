using System.Collections.Generic;
using readiness.model;
using readiness.repository;

namespace readiness.service;

public class BookService
{
    // O Service precisa do Repositório para conseguir guardar e buscar dados
    private readonly BookRepository _bookRepository;

    // Construtor: recebe o repositório automaticamente
    public BookService(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    // --- GERIR BIBLIOTECA ---

    public void AddBook(Book newBook)
    {

        _bookRepository.AddBook(newBook);
    }

    public void RemoveBook(Book bookToRemove)
    {
        _bookRepository.RemoveBook(bookToRemove);
    }

    public List<Book> GetAllBooks()
    {
        return _bookRepository.GetAllBooks();
    }

    // --- GERIR DESEJOS (WISHES) ---

    public void AddWish(Book book)
    {
        _bookRepository.AddWish(book);
    }

    public void RemoveWish(Book book)
    {
        _bookRepository.RemoveWish(book);
    }

    public List<Book> GetAllWishes()
    {
        return _bookRepository.GetAllWishes();
    }

    // --- GERIR LIVROS LIDOS (READ) ---

    public void MarkAsRead(Book book)
    {
        _bookRepository.MarkAsRead(book);
    }

    public void RemoveFromRead(Book book)
    {
        _bookRepository.RemoveFromRead(book);
    }

    public List<Book> GetAllReadBooks()
    {
        return _bookRepository.GetAllReadBooks();
    }

    // --- GERIR LIVROS PARA LER (TO READ) ---

    public void MarkAsToRead(Book book)
    {
        _bookRepository.MarkAsToRead(book);
    }

    public void RemoveFromToRead(Book book)
    {
        _bookRepository.RemoveFromToRead(book);
    }

    public List<Book> GetAllToReadBooks()
    {
        return _bookRepository.GetAllToReadBooks();
    }
}