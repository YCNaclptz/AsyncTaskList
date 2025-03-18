using AsyncEnumerable_TEST_MVC.Models;

namespace AsyncEnumerable_TEST_MVC.Interface
{
    public interface IHomeService
    {
        Task<BaseModel> Task1();
        Task<BaseModel> Task2();
        Task<BaseModel> Task3();
        Task<BaseModel> Task4();
    }
}