using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleGlicemia.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroGlicoseController : ControllerBase
    {
        private readonly IRegistroGlicoseService _registroGlicoseService;

        public RegistroGlicoseController(IRegistroGlicoseService registroGlicoseService)
        {
            _registroGlicoseService = registroGlicoseService;
        }

        private int GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("User ID not found or invalid in token.");

            return userId;
        }

        // Validação extra no controller (defesa adicional)
        private void ValidarRegrasRegistro(DateTime medidoEm, MomentoMedicao momentoMedicao, string? observacoes)
        {
            if (medidoEm > DateTime.UtcNow.AddMinutes(5))
                ModelState.AddModelError(nameof(CreateRegistroGlicoseDto.MedidoEm), "A data da medição não pode ser futura.");

            if (!Enum.IsDefined(typeof(MomentoMedicao), momentoMedicao))
                ModelState.AddModelError(nameof(CreateRegistroGlicoseDto.MomentoMedicao), "MomentoMedicao inválido.");

            if (!string.IsNullOrWhiteSpace(observacoes) && observacoes.Length > 300)
                ModelState.AddModelError(nameof(CreateRegistroGlicoseDto.Observacoes), "Observações devem ter no máximo 300 caracteres.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegistrosGlicose()
        {
            var userId = GetUserIdFromClaims();
            var registros = await _registroGlicoseService.GetAllRegistrosGlicoseByUserIdAsync(userId);
            return Ok(registros);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegistroGlicoseById(int id)
        {
            var userId = GetUserIdFromClaims();
            var registro = await _registroGlicoseService.GetRegistroGlicoseByIdAsync(id);

            if (registro == null || registro.UserId != userId)
                return NotFound("Registro de glicose não encontrado ou você não tem permissão para acessá-lo.");

            return Ok(registro);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegistroGlicose([FromBody] CreateRegistroGlicoseDto registroGlicoseDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            ValidarRegrasRegistro(
                registroGlicoseDto.MedidoEm,
                registroGlicoseDto.MomentoMedicao,
                registroGlicoseDto.Observacoes);

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = GetUserIdFromClaims();
            await _registroGlicoseService.AddRegistroGlicoseAsync(userId, registroGlicoseDto);

            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegistroGlicose(int id, [FromBody] UpdateRegistroGlicoseDto registroGlicoseDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (id != registroGlicoseDto.Id)
                return BadRequest("O ID da rota não corresponde ao ID do registro fornecido.");

            ValidarRegrasRegistro(
                registroGlicoseDto.MedidoEm,
                registroGlicoseDto.MomentoMedicao,
                registroGlicoseDto.Observacoes);

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = GetUserIdFromClaims();
            var updatedRegistro = await _registroGlicoseService.UpdateRegistroGlicoseAsync(id, userId, registroGlicoseDto);

            if (updatedRegistro == null)
                return NotFound("Registro de glicose não encontrado ou você não tem permissão para atualizá-lo.");

            return Ok(updatedRegistro);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistroGlicose(int id)
        {
            var userId = GetUserIdFromClaims();
            var registro = await _registroGlicoseService.GetRegistroGlicoseByIdAsync(id);

            if (registro == null || registro.UserId != userId)
                return NotFound("Registro de glicose não encontrado ou você não tem permissão para excluí-lo.");

            await _registroGlicoseService.DeleteRegistroGlicoseAsync(id);
            return NoContent();
        }
    }
}