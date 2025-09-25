using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetName";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
            .WithParameterValidation();

        // GET /games
        group.MapGet("/", async (AppDbContext dbContext) =>
        {
            return await dbContext.Games
                .Select(g => g.ToDto())
                .ToListAsync();
        });

        // GET /games/1
        group.MapGet("/{id}", async (int id, AppDbContext dbContext) =>
            {
                GameDto? game = await dbContext.Games
                    .Where(g => g.Id == id)
                    .Select(g => g.ToDto())
                    .FirstOrDefaultAsync();

                return game is null ? Results.NotFound() : Results.Ok(game);
            })
            .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", async (CreateGameDto newGame, AppDbContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            game.Genre = await dbContext.Genres.FindAsync(newGame.GenreId);

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToDto());
        });

        // PUT /games/{id}
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, AppDbContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);
            if (game is null)
            {
                return Results.NotFound();
            }

            game.Name = updatedGame.Name;
            game.Genre = dbContext.Genres.Find(updatedGame.GenreId);
            game.Price = updatedGame.Price;
            game.ReleaseDate = updatedGame.ReleaseDate;
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/{id}
        group.MapDelete("/{id}", async (int id, AppDbContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);
            if (game is null)
            {
                return Results.NotFound();
            }

            dbContext.Games.Remove(game);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        return group;
    }
}
