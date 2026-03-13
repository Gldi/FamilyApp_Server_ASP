using System.IdentityModel.Tokens.Jwt;
using Fm.Api.Data;
using Fm.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionsController(AppDbContext db)
    {
        _db = db;
    }

    private Guid GetUserId()
    {
        var uid = User.FindFirst("uid")?.Value
                  ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return Guid.Parse(uid!);
    }

    public record CreateTransactionRequest(
        TxType Type,
        string Category,
        decimal Amount,
        string? Memo,
        DateTime SpentAt
    );

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionRequest request)
    {
        var userId = GetUserId();

        var transaction = new Transaction
        {
            UserId = userId,
            Type = request.Type,
            Category = request.Category,
            Amount = request.Amount,
            Memo = request.Memo,
            SpentAt = request.SpentAt
        };

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        return Ok(transaction);
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var userId = GetUserId();

        var list = await _db.Transactions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SpentAt)
            .ToListAsync();

        return Ok(list);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();

        var transaction = await _db.Transactions
            .SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (transaction == null)
            return NotFound();

        _db.Transactions.Remove(transaction);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}