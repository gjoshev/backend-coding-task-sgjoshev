using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

/// <summary>
/// REST API controller for insurance claims.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;

    public ClaimsController(IClaimsService claimsService)
    {
        _claimsService = claimsService;
    }

    /// <summary>
    /// Returns all claims.
    /// </summary>
    /// <returns>A list of all claims.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Claim>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Claim>>> GetAsync()
    {
        var claims = await _claimsService.GetAllAsync();
        return Ok(claims);
    }

    /// <summary>
    /// Returns a single claim by id.
    /// </summary>
    /// <param name="id">The unique identifier of the claim.</param>
    /// <returns>The matching claim, or 404 if not found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Claim), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Claim>> GetAsync(string id)
    {
        var claim = await _claimsService.GetByIdAsync(id);
        if (claim is null) return NotFound();
        return Ok(claim);
    }

    /// <summary>
    /// Creates a new claim.
    /// </summary>
    /// <param name="claim">The claim to create. DamageCost must not exceed 100,000 and Created must fall within the related cover period.</param>
    /// <returns>The created claim with a generated Id.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Claim), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Claim>> CreateAsync(Claim claim)
    {
        try
        {
            var created = await _claimsService.CreateAsync(claim);
            return Ok(created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a claim by id.
    /// </summary>
    /// <param name="id">The unique identifier of the claim to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _claimsService.DeleteAsync(id);
        return NoContent();
    }
}
