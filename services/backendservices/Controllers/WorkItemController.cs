using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workhelpers.Models;
using WorkHelpers.Context;

namespace backendservices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkItemController : ControllerBase
    {
        // private static List<WorkItem> workItemsList = new List<WorkItem>();
        private readonly WorkDbContext _dbContext;

        public WorkItemController(WorkDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkItem>>> GetTasks()
        {
            var workItems = await _dbContext.WorkItems.ToListAsync();
            return Ok(workItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkItem>> GetTask(int id)
        {
            WorkItem? workItem = await _dbContext.WorkItems.FindAsync(id);
            if (workItem == null)
            {
                return NotFound();
            }
            return Ok(workItem);
        }

        [HttpPost]
        public async Task<ActionResult<WorkItem>> CreateTask(WorkItem workItem)
        {
            // workItem.Id = workItemsList.Count > 0 ? workItemsList.Max(t => t.Id) + 1 : 1;
            await _dbContext.WorkItems.AddAsync(workItem);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTask), new { id = workItem.Id }, workItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTask(int id, WorkItem updatedTask)
        {
            var task = await _dbContext.WorkItems.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            task.Title = updatedTask.Title;
            task.Completed = updatedTask.Completed;
            await _dbContext.SaveChangesAsync();
            return NoContent();

            // var task = workItemsList.FirstOrDefault(t => t.Id == id);
            // if (task == null)
            // {
            //     return NotFound();
            // }
            // task.Title = updatedTask.Title;
            // task.Completed = updatedTask.Completed;
            // return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            using (var httpClient = new HttpClient())
            {
                // http://localhost:5235
                //https://localhost:7046

                var response = await httpClient.DeleteAsync($"https://localhost:7046/api/SecondService/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                }
            }
        }
    }
}