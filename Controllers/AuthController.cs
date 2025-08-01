using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Authentication;
using TrilhaApiDesafio.Services;

namespace TrilhaApiDesafio.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação de usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Realiza o login do usuário
        /// </summary>
        /// <param name="loginRequest">Dados de login</param>
        /// <returns>Token JWT se o login for bem-sucedido</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Tentativa de login com dados inválidos");
                    return BadRequest(ModelState);
                }

                var result = await _userService.AuthenticateAsync(loginRequest);

                if (result == null)
                {
                    _logger.LogWarning("Tentativa de login falhada para usuário: {Username}", loginRequest.Username);
                    return Unauthorized(new { message = "Credenciais inválidas" });
                }

                _logger.LogInformation("Login bem-sucedido para usuário: {Username}", loginRequest.Username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o processo de login");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="registerRequest">Dados de registro</param>
        /// <returns>Resultado do registro</returns>
        /// <response code="200">Usuário registrado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos ou usuário já existe</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Tentativa de registro com dados inválidos");
                    return BadRequest(ModelState);
                }

                var result = await _userService.RegisterAsync(registerRequest);

                if (!result.Success)
                {
                    _logger.LogWarning("Falha no registro para usuário: {Username} - {Message}", registerRequest.Username, result.Message);
                    return BadRequest(new { message = result.Message });
                }

                _logger.LogInformation("Usuário registrado com sucesso: {Username}", registerRequest.Username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o processo de registro");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém informações do usuário atual autenticado
        /// </summary>
        /// <returns>Informações do usuário atual</returns>
        /// <response code="200">Informações do usuário</response>
        /// <response code="401">Não autenticado</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult<object> GetCurrentUser()
        {
            try
            {
                var username = User.Identity?.Name;
                var roles = User.Claims
                    .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                var userInfo = new
                {
                    Username = username,
                    Roles = roles,
                    IsAuthenticated = User.Identity?.IsAuthenticated ?? false
                };

                _logger.LogInformation("Informações do usuário recuperadas para: {Username}", username);
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar informações do usuário atual");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Endpoint para testar se o token JWT é válido
        /// </summary>
        /// <returns>Status da validação</returns>
        /// <response code="200">Token válido</response>
        /// <response code="401">Token inválido ou expirado</response>
        [HttpGet("validate")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult ValidateToken()
        {
            return Ok(new { message = "Token válido", valid = true });
        }
    }
}
