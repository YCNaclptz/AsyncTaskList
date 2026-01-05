using AsyncEnumerable_TEST_MVC.Models;
using DbService.Interface;

namespace AsyncEnumerable_TEST_MVC.Interface;

public interface IJobExecutionService
{
    Task<BaseModel> InitializeJobAsync(ISqlRepository sqlRepository);
    Task<BaseModel> ProcessBusinessLogicAsync(ISqlRepository sqlRepository);
    Task<BaseModel> CallExternalApiAsync(ISqlRepository sqlRepository);
    Task<BaseModel> GenerateReportAsync(ISqlRepository sqlRepository);
}
