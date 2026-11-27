using System.Collections.Generic;
using System.Threading.Tasks;
using readiness.model;
using readiness.repository;

namespace readiness.service;

public class BookService
{
    private readonly BookRepository _bookRepository;

    public BookService(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    // --- BIBLIOTECA GERAL ---

    public async Task AddBookAsync(Book newBook)
    {
        await _bookRepository.AddBookAsync(newBook);
    }

    public async Task RemoveBookAsync(Book bookToRemove)
    {
        await _bookRepository.RemoveBookAsync(bookToRemove);
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllBooksAsync();
    }

    // --- DESEJOS (WISHES) ---

    public async Task AddWishAsync(long userId, long bookId)
    {
        await _bookRepository.SetBookStatusAsync(userId, bookId, ReadingStatus.Wish);
    }

    public async Task RemoveWishAsync(long userId, long bookId)
    {
        await _bookRepository.RemoveBookStatusAsync(userId, bookId, ReadingStatus.Wish);
    }

    public async Task<List<Book>> GetAllWishesAsync(long userId)
    {
        return await _bookRepository.GetBooksByStatusAsync(userId, ReadingStatus.Wish);
    }

    // --- LIVROS LIDOS (READ) ---

    public async Task MarkAsReadAsync(long userId, long bookId)
    {
        await _bookRepository.SetBookStatusAsync(userId, bookId, ReadingStatus.Read);
    }

    public async Task RemoveFromReadAsync(long userId, long bookId)
    {
        await _bookRepository.RemoveBookStatusAsync(userId, bookId, ReadingStatus.Read);
    }

    public async Task<List<Book>> GetAllReadBooksAsync(long userId)
    {
        return await _bookRepository.GetBooksByStatusAsync(userId, ReadingStatus.Read);
    }

    // --- LIVROS PARA LER (TO READ) ---

    public async Task MarkAsToReadAsync(long userId, long bookId)
    {
        await _bookRepository.SetBookStatusAsync(userId, bookId, ReadingStatus.ToRead);
    }

    public async Task RemoveFromToReadAsync(long userId, long bookId)
    {
        await _bookRepository.RemoveBookStatusAsync(userId, bookId, ReadingStatus.ToRead);
    }

    public async Task<List<Book>> GetAllToReadBooksAsync(long userId)
    {
        return await _bookRepository.GetBooksByStatusAsync(userId, ReadingStatus.ToRead);
    }
}