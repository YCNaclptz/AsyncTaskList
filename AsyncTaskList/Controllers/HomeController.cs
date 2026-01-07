using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AsyncEnumerable_TEST_MVC.Models;
using AsyncEnumerable_TEST_MVC.Interface;
using DbService.Interface;

namespace AsyncEnumerable_TEST_MVC.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    IJobExecutionService jobService) : Controller
{

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async IAsyncEnumerable<BaseModel> ExecuteTasks(
        [FromServices] ISqlRepository sqlRepository,
        [FromQuery] bool fail = true)
    {
        var tasks = new List<Task<BaseModel>>
        {
            jobService.InitializeJobAsync(sqlRepository),
            jobService.ProcessBusinessLogicAsync(sqlRepository),
            jobService.CallExternalApiAsync(sqlRepository, fail),
            jobService.GenerateReportAsync(sqlRepository)
        };

        var results = new List<BaseModel>();
        while (tasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(tasks);

            var finishedTasks = tasks.Where(t => t.IsCompleted).ToList();

            foreach (var task in finishedTasks)
            {
                tasks.Remove(task);
                await Task.Delay(10);
                var result = await task;
                results.Add(result);
                yield return result;
            }
        }

        if (results.Any(r => r.IsSuccess == false))
        {
            await sqlRepository.RollbackAsync();
        }
        else
        {
            await sqlRepository.CommitAsync();
        }

    }

    public IActionResult MultiTask()
    {
        return View();
    }
}