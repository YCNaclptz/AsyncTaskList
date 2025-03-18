using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AsyncEnumerable_TEST_MVC.Models;
using AsyncEnumerable_TEST_MVC.Interface;

namespace AsyncEnumerable_TEST_MVC.Controllers;

public class HomeController(
    ILogger<HomeController> logger, 
    IHomeService homeService) : Controller
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

    public async IAsyncEnumerable<BaseModel> ExecuteTasks()
    {
        var tasks = new List<Task<BaseModel>>
        {
            homeService.Task1(),
            homeService.Task2(),
            homeService.Task3(),
            homeService.Task4()
        };

        while (tasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(tasks);

            var finishedTasks = tasks.Where(t => t.IsCompleted).ToList();

            foreach (var task in finishedTasks)
            {
                tasks.Remove(task);
                await Task.Delay(10);
                yield return await task;
            }
        }
            Debug.WriteLine("End Task");
    }

    public IActionResult MultiTask()
    {
        return View();
    }
}
