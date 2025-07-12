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
    /// <response code="201">Ход успешно создан.</response>
    /// <response code="200">Ход уже был сделан (дубликат)</response>
    /// <response code="400">Некорректные данные запроса.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GameStateDto))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMoveDto request)
    {
        var response = await _service.CreateAsync(request);

        if (response == null)
        {
            return BadRequest();
        }

        return Ok(response);
    }
}