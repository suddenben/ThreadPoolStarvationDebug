using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using ThreadPoolStarvationDebug.Data;

namespace ThreadPoolStarvationDebug.Controllers
{
    [ApiController]
    [Route("api/demo")]
    public class ThreadPoolStarvationController : ControllerBase
    {
        private readonly SqlDelayService _sqlDelayService;
        private readonly ILogger<ThreadPoolStarvationController> _logger;

        public ThreadPoolStarvationController(SqlDelayService sqlDelayService, ILogger<ThreadPoolStarvationController> logger)
        {
            _sqlDelayService = sqlDelayService;
            _logger = logger;
        }

        [HttpGet("sync")]
        public ActionResult DelaySync()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            _sqlDelayService.Execute();
            LogThreadPoolStats();
            stopwatch.Stop();
            return Ok(stopwatch.ElapsedMilliseconds);
        }

        [HttpGet("async")]
        public async Task<ActionResult> DelayAsync()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            await _sqlDelayService.ExecuteAsync();
            LogThreadPoolStats();
            stopwatch.Stop();
            return Ok(stopwatch.ElapsedMilliseconds);
        }

        [HttpGet("wait")]
        public ActionResult DelayAsyncWait()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            _sqlDelayService.ExecuteAsync().Wait();
            LogThreadPoolStats();
            stopwatch.Stop();
            return Ok(stopwatch.ElapsedMilliseconds);
        }

        [HttpGet("configureawait")]
        public ActionResult DelaySyncConfigureAwait()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            _sqlDelayService.ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            LogThreadPoolStats();
            stopwatch.Stop();
            return Ok(stopwatch.ElapsedMilliseconds);
        }

        [HttpGet("taskrun")]
        public async Task<ActionResult> DelayTaskRunOnSync()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            await Task.Run(() => _sqlDelayService.Execute());
            LogThreadPoolStats();
            stopwatch.Stop();
            return Ok(stopwatch.ElapsedMilliseconds);
        }

        [HttpGet("taskfactory")]
        public async Task<ActionResult> DelayTaskFactoryOnSync()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            await Task.Factory.StartNew(() => _sqlDelayService.Execute(), TaskCreationOptions.LongRunning);
            LogThreadPoolStats();
            stopwatch.Stop();
            return Ok(stopwatch.ElapsedMilliseconds);
        }


        private void LogThreadPoolStats()
        {
            var threadCount = ThreadPool.ThreadCount;
            var pendingWorkItemCount = ThreadPool.PendingWorkItemCount;
            var completedWorkItemCount = ThreadPool.CompletedWorkItemCount;
            _logger.LogInformation("Thread Count: {ThreadCount}, Pending Work Item Count: {PendingWorkItemCount}, Completed Work Item Count: {CompletedWorkItemCount}", threadCount, pendingWorkItemCount, completedWorkItemCount);
        }

        [HttpGet("threadinfo")]
        public ActionResult GetThreadInfo()
        {
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int _);
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int _);
            var threadCount = ThreadPool.ThreadCount;
            var pendingWorkItemCount = ThreadPool.PendingWorkItemCount;
            var completedWorkItemCount = ThreadPool.CompletedWorkItemCount;
            return Ok(new
            {
                MinWorkerThreads = minWorkerThreads,
                MaxWorkerThreads = maxWorkerThreads,
                ThreadCount = threadCount,
                PendingWorkItemCount = pendingWorkItemCount,
                CompletedWorkItemCount = completedWorkItemCount
            });
        }
    }
}
