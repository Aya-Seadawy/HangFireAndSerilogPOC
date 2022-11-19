using HangfirProject.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog.Context;
using System;

namespace Hangfire.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobTestController : ControllerBase
    {


        private readonly ILogger<JobTestController> _logger;
        private readonly IJobTestService _jobTestService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;



        public JobTestController(ILogger<JobTestController> logger, IBackgroundJobClient backgroundJobClient,
            IJobTestService jobTestService, IRecurringJobManager recurringJobManager)
        {
            _logger = logger;
            _jobTestService = jobTestService;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        [HttpGet("/FireAndForgetJob")]
        public ActionResult CreateFireAndForgetJob()
        {
            //var name = "aya";
            //LogContext.PushProperty("Developer Name", name);
            _logger.LogWarning("Inside CreateFireAndForgetJob endpoint");
            _backgroundJobClient.Enqueue(() => _jobTestService.FireAndForgetJob());
            _logger.LogCritical($"Creating task finished");
            //throw new Exception("Failed to retrieve data");
            return Ok();
        }

        [HttpGet("/DelayedJob")]
        public ActionResult CreateDelayedJob()
        {
            _backgroundJobClient.Schedule(() => _jobTestService.DelayedJob(), TimeSpan.FromSeconds(70));
            return Ok();
        }
        [HttpGet("/ReccuringJob")]
        public ActionResult CreateReccuringJob()
        {
            _recurringJobManager.AddOrUpdate("jobId", () => _jobTestService.ReccuringJob(), Cron.Minutely);
            return Ok();
        }
        [HttpGet("/ContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            var parentJobId = _backgroundJobClient.Enqueue(() => _jobTestService.FireAndForgetJob());
            _backgroundJobClient.ContinueJobWith(parentJobId, () => _jobTestService.ContinuationJob());

            return Ok();
        }
    }
}