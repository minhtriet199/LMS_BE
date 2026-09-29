namespace LMS_BE.Dtos;

public record UserDto(Guid Id, string Name, string Email, string Role);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, UserDto User);
