using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetName";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
            .WithParameterValidation();

        // GET /games
        group.MapGet("/", () => Results.Ok());

        // GET /games/1
        group.MapGet("/{id}", (int id, AppDbContext dbContext) =>
            {
                GameDto? game = dbContext.Games
                    .Where(g => g.Id == id)
                    .Select(g => g.ToDto())
                    .FirstOrDefault();

                return game is null ? Results.NotFound() : Results.Ok(game);
            })
            .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", (CreateGameDto newGame, AppDbContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            game.Genre = dbContext.Genres.Find(newGame.GenreId);

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToDto());
        });

        // PUT /games/{id}
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, AppDbContext dbContext) =>
        {
            Game game = dbContext.Games.Find(id)!;
            if (game is null)
            {
                return Results.NotFound();
            }

            game.Name = updatedGame.Name;
            game.Genre = dbContext.Genres.Find(updatedGame.GenreId);
            game.Price = updatedGame.Price;
            game.ReleaseDate = updatedGame.ReleaseDate;
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        // DELETE /games/{id}
        group.MapDelete("/{id}", (int id, AppDbContext dbContext) =>
        {
            Game game = dbContext.Games.Find(id)!;
            if (game is null)
            {
                return Results.NotFound();
            }

            dbContext.Games.Remove(game);
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        return group;
    }
}
