using AsyncEnumerable_TEST_MVC.Models;
using DbService.Interface;

namespace AsyncEnumerable_TEST_MVC.Interface
{
    public interface IHomeService
    {
        Task<BaseModel> Task1(ISqlRepository sqlRepository);
        Task<BaseModel> Task2(ISqlRepository sqlRepository);
        Task<BaseModel> Task3(ISqlRepository sqlRepository);
        Task<BaseModel> Task4(ISqlRepository sqlRepository);
    }
}