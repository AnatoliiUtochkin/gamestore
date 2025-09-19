using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record GameDto(
    int Id,
    [Required][StringLength(50)] string Name,
    [Required][StringLength(20)] string Genre,
    [Range(1, 250)] decimal Price,
    DateOnly ReleaseDate);
