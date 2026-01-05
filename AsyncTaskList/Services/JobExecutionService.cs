using AsyncEnumerable_TEST_MVC.Interface;
using AsyncEnumerable_TEST_MVC.Models;
using DbService.Interface;

namespace AsyncEnumerable_TEST_MVC.Services;

public class JobExecutionService : IJobExecutionService
{
    public async Task<BaseModel> InitializeJobAsync(ISqlRepository sqlRepository)
    {
        try
        {
            // Insert into the new table JOB_LOGS
            await sqlRepository.ExecuteAsync("INSERT INTO JOB_LOGS (JOB_NAME, STATUS) VALUES ('Initialization', 'Started')");
            await Task.Delay(5000); // Simulate work
            return new BaseModel
            {
                IsSuccess = true,
                Message = "Job Initialization",
                Data = "Initialized successfully in DB."
            };
        }
        catch (Exception ex)
        {
            return new BaseModel
            {
                IsSuccess = false,
                Message = "Job Initialization",
                Data = ex.Message
            };
        }
    }

    public async Task<BaseModel> ProcessBusinessLogicAsync(ISqlRepository sqlRepository)
    {
        try
        {
            await Task.Delay(2000);
            return new BaseModel
            {
                IsSuccess = true,
                Message = "Business Logic",
                Data = "Processed 1000 records."
            };
        }
        catch (Exception ex)
        {
            return new BaseModel
            {
                IsSuccess = false,
                Message = "Business Logic",
                Data = ex.Message
            };
        }
    }

    public async Task<BaseModel> CallExternalApiAsync(ISqlRepository sqlRepository)
    {
        try
        {
            await Task.Delay(3000);
            // Simulate a failure
            return new BaseModel
            {
                IsSuccess = false,
                Message = "External API Call",
                Data = "Connection timeout detected."
            };
        }
        catch (Exception ex)
        {
            return new BaseModel
            {
                IsSuccess = false,
                Message = "External API Call",
                Data = ex.Message
            };
        }
    }

    public async Task<BaseModel> GenerateReportAsync(ISqlRepository sqlRepository)
    {
        try
        {
            await Task.Delay(2000);
            return new BaseModel
            {
                IsSuccess = true,
                Message = "Report Generation",
                Data = "PDF generated."
            };
        }
        catch (Exception ex)
        {
            return new BaseModel
            {
                IsSuccess = false,
                Message = "Report Generation",
                Data = ex.Message
            };
        }
    }
}
