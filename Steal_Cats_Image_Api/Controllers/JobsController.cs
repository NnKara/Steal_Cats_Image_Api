using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Steal_Cats_Image_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetJobStatus(string id)
        {
            try
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var jobDetails = monitoringApi.JobDetails(id);

                if (jobDetails == null)
                    return NotFound(new { message = $"Job with id {id} was not found." });

                var state = jobDetails.History?.FirstOrDefault()?.StateName ?? "Unknown";
                var createdAt = jobDetails.CreatedAt;


                return Ok(new
                {
                    jobId = id,
                    state,
                    createdAt,
                    message = GetStatusMessage(state)
                });
            }
            catch (Exception)
            {
                return StatusCode(503, new { message = "Job status service is temporarily unavailable." });
            }


        }
        private string GetStatusMessage(string state) => state switch
        {
            "Enqueued" => "Job is waiting to be processed.",
            "Processing" => "Job is currently running.",
            "Succeeded" => "Job completed successfully.",
            "Failed" => "Job failed. Check Hangfire dashboard for details.",
            "Scheduled" => "Job is scheduled for later execution.",
            _ => "Job status unknown."
        };
    }
}

