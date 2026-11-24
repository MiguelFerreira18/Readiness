using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using readiness.model;

namespace readiness.repository;

public class BookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- BIBLIOTECA GERAL (Adicionar/Remover livros do sistema) ---

    public async Task AddBookAsync(Book newBook)
    {
        await _context.Books.AddAsync(newBook);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveBookAsync(Book bookToRemove)
    {
        _context.Books.Remove(bookToRemove);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }

    // --- GESTÃO DE STATUS (Lido, Para Ler, Desejo) ---

    // Adicionar ou atualizar o estado de um livro para um utilizador específico
    public async Task SetBookStatusAsync(long userId, long bookId, ReadingStatus status)
    {
        // Verifica se este utilizador já tem este livro associado
        var userBook = await _context.UserBooks
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == bookId);

        if (userBook != null)
        {
            // Se já existe, apenas atualizamos o estado
            userBook.Status = status;
        }
        else
        {
            // Se não existe, criamos um novo registo na tabela UserBook
            _context.UserBooks.Add(new UserBook
            {
                UserId = userId,
                BookId = bookId,
                Status = status
            });
        }

        await _context.SaveChangesAsync();
    }

    // Remover o estado de um livro
    public async Task RemoveBookStatusAsync(long userId, long bookId, ReadingStatus status)
    {
        var userBook = await _context.UserBooks
            .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == bookId && ub.Status == status);

        if (userBook != null)
        {
            _context.UserBooks.Remove(userBook);
            await _context.SaveChangesAsync();
        }
    }

    // Obter a lista de livros de um utilizador com um determinado estado
    public async Task<List<Book>> GetBooksByStatusAsync(long userId, ReadingStatus status)
    {
        // 1. Busca todos os registos UserBook que pertencem a este user e têm este estado
        var userBooks = await _context.UserBooks
            .Where(ub => ub.UserId == userId && ub.Status == status)
            .ToListAsync();

        // 2. Pega nos IDs dos livros encontrados
        var bookIds = userBooks.Select(ub => ub.BookId).ToList();

        // 3. Devolve os livros completos da base de dados que correspondem a esses IDs
        return await _context.Books.Where(b => bookIds.Contains(b.Id)).ToListAsync();
    }
}