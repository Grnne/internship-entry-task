using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services.Interfaces;

namespace TicTacToe.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Создает новую игру.
    /// </summary>
    /// <param name="request">Данные для создания игры.</param>
    /// <returns>Созданная игра с деталями.</returns>
    /// <response code="201">Игра успешно создана.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateGameDto request)
    {
        var response = await _gameService.CreateAsync(request);

        if (response == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetById), new { gameId = response.Id }, response);
    }

    /// <summary>
    /// Получить игру по идентификатору.
    /// </summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <returns>Данные игры.</returns>
    /// <response code="200">Игра найдена и возвращена.</response>
    /// <response code="404">Игра с указанным идентификатором не найдена.</response>
    [HttpGet("{gameId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int gameId)
    {
        var game = await _gameService.GetByIdAsync(gameId);

        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    /// <summary>
    /// Удалить игру по идентификатору.
    /// </summary>
    /// <param name="gameId">Идентификатор игры.</param>
    /// <response code="204">Игра успешно удалена.</response>
    /// <response code="404">Игра с указанным идентификатором не найдена.</response>
    [HttpDelete("{gameId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int gameId)
    {
        var success = await _gameService.DeleteAsync(gameId);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}