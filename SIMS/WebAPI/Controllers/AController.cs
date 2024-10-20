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
        public virtual async Task<ActionResult<Entity>> ReadAsync([Required] int id)
        {
            Entity? e = await _repository.GetAsync(id);

            if (e == null)
                return UnprocessableEntity();
            else 
                return Ok(e);
        }


        [HttpGet]
        public async Task<ActionResult<List<Entity>>> ReadAllAsync()
        {
            List<Entity>? e = await _repository.GetAllAsync();

            if (e is null)
                return UnprocessableEntity();
            else
                return Ok(e);
        } 

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
                return UnprocessableEntity();

            await _repository.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([Required] int id)
        {
            var e = await _repository.GetAsync(id);
            if (e is null)
                return UnprocessableEntity();
            await _repository.DeleteAsync(e);
            return NoContent();
        }
    }
}
