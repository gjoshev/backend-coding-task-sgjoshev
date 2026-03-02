using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

/// <summary>
/// REST API controller for insurance covers.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class CoversController : ControllerBase
{
    private readonly ICoversService _coversService;

    public CoversController(ICoversService coversService)
    {
        _coversService = coversService;
    }

    /// <summary>
    /// Computes the premium for the given date range and cover type.
    /// </summary>
    /// <param name="startDate">Insurance period start date.</param>
    /// <param name="endDate">Insurance period end date.</param>
    /// <param name="coverType">The type of the covered object (Yacht, PassengerShip, Tanker, etc.).</param>
    /// <returns>The computed premium amount.</returns>
    [HttpPost("compute")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    public ActionResult<decimal> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }

    /// <summary>
    /// Returns all covers.
    /// </summary>
    /// <returns>A list of all covers.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Cover>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coversService.GetAllAsync();
        return Ok(results);
    }

    /// <summary>
    /// Returns a single cover by id.
    /// </summary>
    /// <param name="id">The unique identifier of the cover.</param>
    /// <returns>The matching cover, or 404 if not found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Cover), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        var cover = await _coversService.GetByIdAsync(id);
        if (cover is null) return NotFound();
        return Ok(cover);
    }

    /// <summary>
    /// Creates a new cover.
    /// </summary>
    /// <param name="cover">The cover to create.</param>
    /// <returns>The created cover with a generated Id and computed premium.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Cover), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Cover>> CreateAsync(Cover cover)
    {
        try
        {
            var created = await _coversService.CreateAsync(cover);
            return Ok(created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a cover by id.
    /// </summary>
    /// <param name="id">The unique identifier of the cover to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        await _coversService.DeleteAsync(id);
        return NoContent();
    }
}
