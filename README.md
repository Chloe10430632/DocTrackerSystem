# DocTracker：文件閱讀追蹤系統

本專案為應徵元大期貨系統維護開發職位所開發之測試專案。

## 系統架構概要
本系統採用**三層式架構 (3-Tier Architecture)** 設計，確保資料存取層、商業邏輯層與表現層之高耦合度分離。
* **表現層 (ASP.NET Core MVC)**：處理使用者介面、權限驗證與會話管理。
* **商業邏輯層 (ASP.NET Core Web API)**：提供前後端數據交換與軌跡處理邏輯。
* **資料持久層 (EF Core)**：基於 SQL Server 的數據存取與 ORM 對應。



## 核心功能實作
1. **身分驗證與權限管理**：
   - 採用 Cookie 基礎驗證，整合防偽 Token (AntiForgeryToken) 機制。
   - 實作角色權限控制 (RBAC)，確保僅授權使用者可存取特定頁面。
2. **閱讀軌跡追蹤 (Tracking Mechanism)**：
   - 使用者瀏覽文件時，透過 AJAX 非同步請求 Web API，精準紀錄完整閱讀歷程。
   - 資料結構：`UserId`, `DocId`, `StartTime`, `EndTime`, `ClientIP`。
3. **LOG 查詢儀表板**：
   - 提供簡易搜尋功能，便於管理者快速回溯使用者行為軌跡。

## 資安防護策略 (Security Highlights)
針對金融產業對資料安全性之高要求，本專案實作以下資安強化機制：
* **密碼安全**：採用 `BCrypt` 強度雜湊演算法 (Hashing) 儲存使用者密碼，有效防止資料庫外洩風險。
* **防禦常見攻擊**：
    - **CSRF 防禦**：全域啟用 `ValidateAntiForgeryToken`。
    - **XSS 防禦**：Cookie 設定 `HttpOnly`，阻絕跨站腳本攻擊。
* **雲端架構適配**：針對 Cloudflare CDN 架構，系統具備 `CF-Connecting-IP` 真實 IP 解析能力，確保日誌紀錄之真實性與防禦價值。

## 技術環境
* **Framework**: .NET 8 / ASP.NET Core MVC & Web API
* **Database**: SQL Server
* **Deployment**: Docker 容器化部署，確保開發環境與生產環境之高度一致性。

---

## 聯絡與測試資訊
* **測試站點 URL**: [https://doctracker-mvc.salter-ocean.online](https://doctracker-mvc.salter-ocean.online)
* **帳號說明**: 
    - 管理員測試帳號：`admin@yuanta.test.com` / `Admin123456`
    - 一般使用者帳號：`chloe@yuanta.test.com` / `Chloe123456`
