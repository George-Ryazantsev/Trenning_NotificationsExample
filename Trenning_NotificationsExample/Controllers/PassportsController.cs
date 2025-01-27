using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Trenning_NotificationsExample.Models;
using Trenning_NotificationsExample.Services;


namespace Trenning_NotificationsExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassportsController : ControllerBase
    {
        private readonly PassportChangesService _passportsService;        

        public PassportsController(PassportChangesService passportsService)
        {
            _passportsService = passportsService;
        }

        [HttpGet("{series}/{number}")]
        public async Task<ActionResult<InactivePassports>> GetInactivePassport(string series, string number)
        {
            var stopwatch = Stopwatch.StartNew();
            var lastChange = await _passportsService.GetInactivePassportAsync(series, number);

            if (lastChange != null) 
            {
                stopwatch.Stop();
                Console.WriteLine( $"{stopwatch.ElapsedMilliseconds} ms");

                return lastChange; 
            }        
            else return NotFound();            
        }

        [HttpGet("changes/{date}")]
        public async Task<ActionResult<IEnumerable<PassportChanges>>> GetChangesByDate(DateTime date)
        {
            var stopwatch = Stopwatch.StartNew();
            var changes = await _passportsService.GetChangesByDateAsync(date);

            if (!changes.Any())
            {               
                return NotFound();
            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

            return Ok(changes);
        }

        [HttpGet("history/{series}/{number}")]
        public async Task<ActionResult<IEnumerable<PassportChanges>>> GetPassportHistory(string series, string number)
        {
            var stopwatch = Stopwatch.StartNew();
            var history = await _passportsService.GetHistoryAsync(series, number);

            if (!history.Any())
            {               
                return NotFound();
            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");

            return Ok(history);
        }
    }

}
