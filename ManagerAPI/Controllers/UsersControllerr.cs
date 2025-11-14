using ManagerAPI.DTOs;
using ManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "admin")]
    public class UsersControllerr : Controller
    {
        private readonly UsersService _svc;
        public UsersControllerr(UsersService svc) { _svc = svc; }

        [HttpGet] public async Task<ActionResult<List<UserResponseDto>>> GetAll() => Ok(await _svc.GetAll());
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
            => (await _svc.GetById(id)) is { } u ? Ok(u) : NotFound();

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create(UserCreateDto dto)
            => Ok(await _svc.Create(dto));

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> Update(int id, UserUpdateDto dto)
            => (await _svc.Update(id, dto)) is { } u ? Ok(u) : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
            => await _svc.Delete(id) ? NoContent() : NotFound();
    }

}
