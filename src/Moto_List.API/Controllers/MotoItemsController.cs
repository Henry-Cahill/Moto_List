using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moto_List.API.Data;
using Moto_List.API.Models;
using Moto_List.Shared.DTOs;

namespace Moto_List.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MotoItemsController : ControllerBase
{
    private readonly AppDbContext _db;

    public MotoItemsController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<MotoItemDto>>> GetAll()
    {
        var userId = GetUserId();
        var items = await _db.MotoItems
            .Where(m => m.UserId == userId)
            .Include(m => m.Category)
            .Select(m => new MotoItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                IsChecked = m.IsChecked,
                CategoryId = m.CategoryId,
                CategoryName = m.Category.Name
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MotoItemDto>> Get(int id)
    {
        var userId = GetUserId();
        var item = await _db.MotoItems
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (item is null) return NotFound();

        return Ok(new MotoItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsChecked = item.IsChecked,
            CategoryId = item.CategoryId,
            CategoryName = item.Category.Name
        });
    }

    [HttpPost]
    public async Task<ActionResult<MotoItemDto>> Create([FromBody] CreateMotoItemRequest request)
    {
        var userId = GetUserId();
        var item = new MotoItem
        {
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            UserId = userId
        };

        _db.MotoItems.Add(item);
        await _db.SaveChangesAsync();

        await _db.Entry(item).Reference(m => m.Category).LoadAsync();

        return CreatedAtAction(nameof(Get), new { id = item.Id }, new MotoItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsChecked = item.IsChecked,
            CategoryId = item.CategoryId,
            CategoryName = item.Category.Name
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMotoItemRequest request)
    {
        var userId = GetUserId();
        var item = await _db.MotoItems.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (item is null) return NotFound();

        if (request.Name is not null) item.Name = request.Name;
        if (request.Description is not null) item.Description = request.Description;
        if (request.IsChecked.HasValue) item.IsChecked = request.IsChecked.Value;
        if (request.CategoryId.HasValue) item.CategoryId = request.CategoryId.Value;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var userId = GetUserId();
        var item = await _db.MotoItems.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (item is null) return NotFound();

        item.IsChecked = !item.IsChecked;
        await _db.SaveChangesAsync();

        return Ok(new { item.IsChecked });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var item = await _db.MotoItems.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (item is null) return NotFound();

        _db.MotoItems.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
