using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services.Interfaces;

namespace TicTacToe.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoveController : ControllerBase
{
    private readonly IMoveService _service;

    public MoveController(IMoveService service)
    {
        _service = service;
    }

    /// <summary>
    /// Делает ход.
    /// </summary>
    /// <param name="request">Данные для создания нового хода.</param>
    /// <returns>GameId, состояние игры, игрока и список с клетками.</returns>
    /// <response code="200">Ход уже был сделан.</response>
    /// <response code="201">Ход успешно создан.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)] //TODO Etag
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GameStateDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMoveDto request)
    {
        var result = await _service.CreateAsync(request);

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

        // Возвращаем 201 Created с телом ответа
        return StatusCode(StatusCodes.Status201Created, result.Response);
    }
}