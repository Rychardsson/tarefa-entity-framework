using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using TrilhaApiDesafio.Authentication;
using TrilhaApiDesafio.Context;
using Microsoft.EntityFrameworkCore;

namespace TrilhaApiDesafio.Services
{
    public interface IUserService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto loginRequest);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string username);
    }

    public class UserService : IUserService
    {
        private readonly OrganizadorContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserService> _logger;

        public UserService(OrganizadorContext context, IJwtService jwtService, ILogger<UserService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto loginRequest)
        {
            try
            {
                var user = await GetUserByUsernameAsync(loginRequest.Username);
                
                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Tentativa de login com usuário inexistente ou inativo: {Username}", loginRequest.Username);
                    return null;
                }

                if (!VerifyPassword(loginRequest.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Tentativa de login com senha incorreta para o usuário: {Username}", loginRequest.Username);
                    return null;
                }

                // Buscar roles do usuário
                var roles = _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.Role.Name)
                    .ToList();

                // Se não tem roles, adicionar role padrão
                if (!roles.Any())
                {
                    roles.Add("User");
                }

                var token = _jwtService.GenerateToken(user.Username, roles);
                var expiresAt = DateTime.UtcNow.AddMinutes(60); // Default 60 minutes

                _logger.LogInformation("Login realizado com sucesso para o usuário: {Username}", user.Username);

                return new LoginResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    ExpiresAt = expiresAt,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante autenticação do usuário: {Username}", loginRequest.Username);
                return null;
            }
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                // Verificar se o usuário já existe
                if (await UserExistsAsync(registerRequest.Username))
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = "Nome de usuário já existe"
                    };
                }

                // Verificar se o email já existe (se fornecido)
                if (!string.IsNullOrEmpty(registerRequest.Email))
                {
                    var emailExists = await _context.Users
                        .AnyAsync(u => u.Email == registerRequest.Email);
                    
                    if (emailExists)
                    {
                        return new RegisterResponseDto
                        {
                            Success = false,
                            Message = "Email já está em uso"
                        };
                    }
                }

                // Criar o hash da senha
                var passwordHash = HashPassword(registerRequest.Password);

                // Criar o usuário
                var user = new User
                {
                    Username = registerRequest.Username,
                    PasswordHash = passwordHash,
                    Email = registerRequest.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Atribuir role padrão "User"
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (userRole != null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = userRole.Id,
                        AssignedAt = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Usuário registrado com sucesso: {Username}", user.Username);

                return new RegisterResponseDto
                {
                    Success = true,
                    Username = user.Username,
                    Message = "Usuário registrado com sucesso"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante registro do usuário: {Username}", registerRequest.Username);
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Erro interno do servidor"
                };
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        private static string HashPassword(string password)
        {
            // Gerar salt aleatório
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash da senha com salt
            var hashedPassword = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            // Combinar salt e hash
            var combined = new byte[48];
            Array.Copy(salt, 0, combined, 0, 16);
            Array.Copy(hashedPassword, 0, combined, 16, 32);

            return Convert.ToBase64String(combined);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                var combined = Convert.FromBase64String(hashedPassword);
                
                // Extrair salt e hash
                var salt = new byte[16];
                var hash = new byte[32];
                Array.Copy(combined, 0, salt, 0, 16);
                Array.Copy(combined, 16, hash, 0, 32);

                // Hash da senha fornecida com o mesmo salt
                var verificationHash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 32);

                // Comparar hashes
                return verificationHash.SequenceEqual(hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
