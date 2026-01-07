# AsyncTaskList

## 專案目的
這個專案是一個示範應用程式，展示如何在 ASP.NET Core 中高效執行並行任務，並利用 `IAsyncEnumerable` 的強大功能，以即時串流的方式將結果傳遞給前端。它同時也展示了如何在任務執行過程中，結合資料庫交易管理，確保資料一致性，並在任何任務失敗時執行交易回溯。

## 技術棧
*   **框架**: .NET 8.0 ASP.NET Core MVC
*   **語言**: C# 12+ (偏好 Primary Constructors, Collection Expressions)
*   **資料庫**: Oracle Free 23c (透過 `Oracle.ManagedDataAccess.Core` 驅動)
*   **ORM**: Dapper (用於輕量級 SQL 操作)
*   **前端**: Razor Views, Bootstrap 5, jQuery 搭配 Fetch API 進行串流消費

## 核心架構模式

### 多專案解決方案
專案分為兩個主要部分：
*   **AsyncTaskList**: 主 MVC 應用程式 (`AsyncEnumerable_TEST_MVC` 命名空間)。
*   **DbService**: 獨立的類別庫，用於資料存取抽象化。

### 每請求交易模式 (Transaction-Per-Request Pattern)
`SqlRepository` 在其建構子中會自動開啟資料庫連線並啟動交易。服務層應透過方法參數接收 `ISqlRepository` 實例，而非透過建構子注入，這允許控制器根據聚合的任務結果來決定提交 (`CommitAsync()`) 或回溯 (`RollbackAsync()`) 交易。

### 非同步串流模式 (核心功能)
參見 `HomeController.ExecuteTasks()` 方法。這個模式使用 `Task.WhenAny` 同時啟動多個任務，並在任何任務完成時立即透過 `yield return` 將結果串流回前端。前端則利用 Fetch API 即時消費這些串流的 JSON 片段。

### 前端健壯的串流 JSON 解析器
前端的 `MultiTask.cshtml` 頁面實作了一個健壯的 JavaScript 串流解析器，能夠從傳入的數據塊中，基於大括號計數器 (`{` 和 `}`) 識別並解析完整的 JSON 物件。這確保了即使伺服器以非標準方式（例如連續的 JSON 物件或 JSON 陣列片段）發送數據，UI 也能夠即時更新，實現真正的逐步進度顯示。

## 依賴注入規則
*   **`ISqlRepository`**: 註冊為 `Transient` (每個 HTTP 請求一個新實例)。
*   **`IJobExecutionService`**: 註冊為 `Scoped`。
*   服務應透過方法參數接收 `ISqlRepository`，而非建構子注入，以精細控制交易範圍。

## 資料庫管理

### 本機開發環境
建議透過 Docker Compose 啟動 Oracle 資料庫：
```bash
docker-compose up -d
```
連線字串配置於 `appsettings.json` 中。

### Schema 初始化
應用程式啟動時，`Program.cs` 會執行 PL/SQL 匿名區塊來初始化資料庫 Schema。在開發環境中，此處包含對 `ORA-00955` 錯誤的處理，以避免表已存在時的重複建立問題。

## 編碼標準
*   **Primary Constructors**: 優先用於控制器和服務中的依賴注入 (例如 `public class HomeController(ILogger logger)` )。
*   **Async 命名**: 所有非同步方法應以 `Async` 結尾。
*   **避免阻塞**: 一律使用 `await`，避免使用 `.Result` 或 `.Wait()` (除了 `Program.cs` 啟動等同步上下文)。
*   **命名空間**: 主專案使用 `AsyncEnumerable_TEST_MVC`，資料庫服務使用 `DbService`。

## 關鍵檔案
*   `AsyncTaskList/Program.cs`: 依賴注入設定、資料庫初始化。
*   `AsyncTaskList/Controllers/HomeController.cs`: 非同步串流示範的核心邏輯。
*   `DbService/Services/SqlRepository.cs`: 資料庫底層操作與交易管理。
*   `AsyncTaskList/Views/Home/MultiTask.cshtml`: 實現即時進度更新的前端使用者介面。
*   `.github/copilot-instructions.md`: 本專案更詳細的開發者指南與 Copilot 指令。