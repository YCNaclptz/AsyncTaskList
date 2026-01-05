# Project Context: AsyncTaskList

## 專案目的
示範如何在 ASP.NET Core 中執行並行任務，並結合 `IAsyncEnumerable` 即時串流結果，同時維持資料庫交易的一致性。

## 核心邏輯 (HomeController.ExecuteTasks)
1. **並行執行**: 同時啟動多個 `Task<BaseModel>`。
2. **即時回傳**: 使用 `while (tasks.Count > 0)` 搭配 `Task.WhenAny`，誰先做完就先 `yield return` 給前端。
3. **交易控制**: 在串流結束後，檢查所有結果。若有任何失敗則執行 `RollbackAsync()`，全成功則 `CommitAsync()`。

## 資料庫層級 (DbService)
- 使用 `Dapper` 進行 SQL 操作。
- `SqlRepository` 在建構時即開啟連線並啟動 `Transaction`。
- 必須確保在要求結束前呼叫 `Commit` 或 `Rollback` 以釋放連線。

## 關鍵檔案
- `AsyncTaskList/Controllers/HomeController.cs`: 核心並行邏輯與串流。
- `AsyncTaskList/Services/HomeService.cs`: 模擬商業邏輯。
- `DbService/Services/SqlRepository.cs`: 資料庫底層與交易封裝。
