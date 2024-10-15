using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public abstract class AController<Entity> : ControllerBase where Entity : class
    {
        protected readonly IRepository<Entity> _repository;

        protected AController(IRepository<Entity> repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Entity>> ReadAsync([Required] int id)
            => Ok(await _repository.GetAsync(id));

        [HttpGet]
        public async Task<ActionResult<List<Entity>>> ReadAllAsync()  
            => Ok(await _repository.GetAllAsync());

        [HttpPost]
        public async Task<ActionResult> CreateAsync([Required] Entity entity)
        {
            await _repository.CreateAsync(entity);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync([Required] int id, Entity entity)
        {
            var e = await _repository.GetAsync(id);
            if (e is null)
                return NotFound();

            await _repository.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([Required] int id)
        {
            var e = await _repository.GetAsync(id);
            if (e is null)
                return NotFound();
            await _repository.DeleteAsync(e);
            return NoContent();
        }
    }
}
