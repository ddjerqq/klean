using Application.Dtos;
using AutoMapper.QueryableExtensions;
using Domain.Aggregates;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Common.Abstractions;

namespace Presentation.Controllers;

/// <summary>
/// controller for authentication
/// </summary>
public sealed class HomeController : ApiController
{
    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet("/users")]
    public async Task<IActionResult> Users(CancellationToken ct)
    {
        var users = await DbContext.Set<User>()
            .ProjectTo<UserDto>(Mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Ok(users);
    }

    /// <summary>
    /// Get all items
    /// </summary>
    [HttpGet("/items")]
    public async Task<IActionResult> Items(CancellationToken ct)
    {
        var users = await DbContext.Set<Item>()
            .ToListAsync(ct);

        return Ok(users);
    }

    /// <summary>
    /// Get all item types
    /// </summary>
    [HttpGet("/item-types")]
    public async Task<IActionResult> ItemTypes(CancellationToken ct)
    {
        var users = await DbContext.Set<ItemType>()
            .ToListAsync(ct);

        return Ok(users);
    }
}