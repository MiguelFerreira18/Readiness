using Microsoft.AspNetCore.Mvc;
using readiness.model;
using readiness.service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace readiness.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly BookService _bookService;

    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }

    // --- BIBLIOTECA GERAL ---

    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        return Ok(await _bookService.GetAllBooksAsync());
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] Book newBook)
    {
        await _bookService.AddBookAsync(newBook);
        return Ok("Livro adicionado com sucesso!");
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveBook([FromBody] Book bookToRemove)
    {
        await _bookService.RemoveBookAsync(bookToRemove);
        return Ok("Livro removido com sucesso!");
    }

    // --- DESEJOS (WISHES) ---

    [HttpGet("wish")]
    public async Task<IActionResult> GetAllWishes([FromQuery] long userId)
    {
        return Ok(await _bookService.GetAllWishesAsync(userId));
    }

    [HttpPost("wish")]
    public async Task<IActionResult> AddWish([FromQuery] long userId, [FromQuery] long bookId)
    {
        await _bookService.AddWishAsync(userId, bookId);
        return Ok("Adicionado aos desejos!");
    }

    [HttpDelete("wish")]
    public async Task<IActionResult> RemoveWish([FromQuery] long userId, [FromQuery] long bookId)
    {
        await _bookService.RemoveWishAsync(userId, bookId);
        return Ok("Removido dos desejos!");
    }

    // --- LIVROS LIDOS (READ) ---

    [HttpGet("read")]
    public async Task<IActionResult> GetAllReadBooks([FromQuery] long userId)
    {
        return Ok(await _bookService.GetAllReadBooksAsync(userId));
    }

    [HttpPost("read")]
    public async Task<IActionResult> MarkAsRead([FromQuery] long userId, [FromQuery] long bookId)
    {
        await _bookService.MarkAsReadAsync(userId, bookId);
        return Ok("Marcado como lido!");
    }

    [HttpDelete("read")]
    public async Task<IActionResult> RemoveFromRead([FromQuery] long userId, [FromQuery] long bookId)
    {
        await _bookService.RemoveFromReadAsync(userId, bookId);
        return Ok("Removido dos lidos!");
    }

    // --- LIVROS PARA LER (TO READ) ---

    [HttpGet("toread")]
    public async Task<IActionResult> GetAllToReadBooks([FromQuery] long userId)
    {
        return Ok(await _bookService.GetAllToReadBooksAsync(userId));
    }

    [HttpPost("toread")]
    public async Task<IActionResult> MarkAsToRead([FromQuery] long userId, [FromQuery] long bookId)
    {
        await _bookService.MarkAsToReadAsync(userId, bookId);
        return Ok("Marcado para ler!");
    }

    [HttpDelete("toread")]
    public async Task<IActionResult> RemoveFromToRead([FromQuery] long userId, [FromQuery] long bookId)
    {
        await _bookService.RemoveFromToReadAsync(userId, bookId);
        return Ok("Removido da lista de leitura!");
    }
}