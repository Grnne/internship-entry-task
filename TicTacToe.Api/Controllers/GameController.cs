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
    /// <param name="request">Данные для создания игры. Размеры доски и длина линии победы - необязательные значения.</param>
    /// <returns>Созданная игра с деталями.</returns>
    /// <response code="201">Игра успешно создана.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GameDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateGameDto request)
    {
        var result = await _gameService.CreateAsync(request);

        if (!result.Success)
        {
            var error = result.Error;
            var problemDetails = new ProblemDetails
            {
                Title = error?.ErrorCode ?? "InvalidRequest",
                Detail = error?.Message ?? "Некорректные данные запроса.",
                Status = error?.StatusCode ?? StatusCodes.Status400BadRequest
            };
            return BadRequest(problemDetails);
        }

        return CreatedAtAction(nameof(GetById), new { gameId = result.Response?.Id }, result.Response);
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
        var result = await _gameService.GetByIdAsync(gameId);

        if (!result.Success)
        {
            var error = result.Error;
            var problemDetails = new ProblemDetails
            {
                Title = error?.ErrorCode ?? "NotFound",
                Detail = error?.Message ?? "Игра с указанным идентификатором не найдена.",
                Status = error?.StatusCode ?? StatusCodes.Status404NotFound
            };
            return NotFound(problemDetails);
        }

        return Ok(result.Response);
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
        var result = await _gameService.DeleteAsync(gameId);

        if (!result.Success)
        {
            var error = result.Error;
            var problemDetails = new ProblemDetails
            {
                Title = error?.ErrorCode ?? "NotFound",
                Detail = error?.Message ?? "Игра с указанным идентификатором не найдена.",
                Status = error?.StatusCode ?? StatusCodes.Status404NotFound
            };
            return NotFound(problemDetails);
        }

        return NoContent();
    }
}