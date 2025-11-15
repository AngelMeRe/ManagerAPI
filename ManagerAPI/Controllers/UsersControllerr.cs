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

        public UsersControllerr(UsersService svc)
        {
            _svc = svc;
        }

        //Obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll()
        {
            return Ok(await _svc.GetAll());
        }

        
        // Obtener un usuario por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _svc.GetById(id);
            return user is not null ? Ok(user) : NotFound();
        }

        // Crear un nuevo usuario 
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create(UserCreateDto dto)
        {
            return Ok(await _svc.Create(dto));
        }

        //Actualizar un usuario existente
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> Update(int id, UserUpdateDto dto)
        {
            var updated = await _svc.Update(id, dto);
            return updated is not null ? Ok(updated) : NotFound();
        }

        //Eliminar un usuario por ID
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _svc.Delete(id);
            return deleted ? NoContent() : NotFound();
        }
    }

}
