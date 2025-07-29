using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using AppProducts.Data;
using AppProducts.Shared.Models;

namespace AppProducts.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting("Fixed")]
public class ProductsController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<Products>> Get()
    {
        return Ok(ctx.Products);
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Products>> GetAsync(long key)
    {
        var products = await ctx.Products.FirstOrDefaultAsync(x => x.Id == key);

        if (products == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(products);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Products>> PostAsync(Products products)
    {
        var record = await ctx.Products.FindAsync(products.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        await ctx.Products.AddAsync(products);

        await ctx.SaveChangesAsync();

        return Created($"/products/{products.Id}", products);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Products>> PutAsync(long key, Products update)
    {
        var products = await ctx.Products.FirstOrDefaultAsync(x => x.Id == key);

        if (products == null)
        {
            return NotFound();
        }

        ctx.Entry(products).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(products);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Products>> PatchAsync(long key, Delta<Products> delta)
    {
        var products = await ctx.Products.FirstOrDefaultAsync(x => x.Id == key);

        if (products == null)
        {
            return NotFound();
        }

        delta.Patch(products);

        await ctx.SaveChangesAsync();

        return Ok(products);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var products = await ctx.Products.FindAsync(key);

        if (products != null)
        {
            ctx.Products.Remove(products);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}
