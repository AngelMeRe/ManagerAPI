using ManagerAPI.Data;
using ManagerAPI.DTOs;
using ManagerAPI.Models;
using ManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static ManagerAPI.DTOs.TaskResponseDto;

namespace ManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITaskService _service;

        public TasksController(ITaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _service.GetById(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            return Ok(await _service.GetMine(userId));
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var created = await _service.Create(dto, userId);
            return Ok(created);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "user";

            var updated = await _service.Update(id, dto, userId, role);

            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted) return NotFound();
            return Ok(new { message = "Tarea eliminada" });
        }
    }
}
