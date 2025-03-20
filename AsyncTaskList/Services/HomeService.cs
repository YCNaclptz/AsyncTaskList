using AsyncEnumerable_TEST_MVC.Interface;
using AsyncEnumerable_TEST_MVC.Models;
using DbService.Interface;

namespace AsyncEnumerable_TEST_MVC.Services;

public class HomeService : IHomeService
{
    public async Task<BaseModel> Task1(ISqlRepository sqlRepository)
    {
        await sqlRepository.ExecuteAsync("INSERT INTO Test (Name) VALUES ('Task1')");
        await Task.Delay(5000);
        return new BaseModel
        {
            IsSuccess = true,
            Message = "Task1",
            Data = null
        };
    }

    public async Task<BaseModel> Task2(ISqlRepository sqlRepository)
    {
        await Task.Delay(2000);
        return new BaseModel
        {
            IsSuccess = true,
            Message = "Task2",
            Data = null
        };
    }

    public async Task<BaseModel> Task3(ISqlRepository sqlRepository)
    {
        await Task.Delay(3000);
        return new BaseModel
        {
            IsSuccess = false,
            Message = "Task3",
            Data = null
        };
    }

    public async Task<BaseModel> Task4(ISqlRepository sqlRepository)
    {
        await Task.Delay(2000);
        return new BaseModel
        {
            IsSuccess = true,
            Message = "Task4",
            Data = null
        };
    }


}
