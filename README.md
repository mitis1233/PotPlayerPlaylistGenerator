# PotPlayer 播放清單產生器

一個簡單實用的 WPF 工具，可以快速掃描指定資料夾內的影片檔案，並使用 PotPlayer 建立播放清單來播放。

<img width="523" height="453" alt="image" src="https://github.com/user-attachments/assets/cc8744ef-f459-4c23-997d-1f3ed899a22e" />


## ✨ 功能特色

-   **📁 資料夾掃描**：自動列出指定根目錄下的所有子資料夾。
-   **▶️ 一鍵播放**：選擇資料夾後，一鍵即可使用 PotPlayer 播放其中所有的影片檔案。
-   **🎨 現代化 UI**：採用半透明的 Aero 玻璃風格介面，美觀且易於使用。
-   **⚙️ 高度可自訂**：
    -   可隨時變更要掃描的影片根目錄。
    -   可自訂要搜尋的影片副檔名 (例如 `.mp4, .mkv, .avi`)。
-   **💾 自動儲存設定**：您的路徑和副檔名設定會自動儲存，下次啟動時無需重新設定。
-   **🗑️ 自動清理**：程式關閉時會自動刪除產生的暫時播放清單檔案，不殘留任何垃圾檔案。

## 📋 系統需求

-   Windows 作業系統
-   [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) 或更高版本。
-   已安裝 [PotPlayer](https://potplayer.daum.net/)，並且 `.dpl` 檔案已與其關聯。

## 🚀 如何使用

1.  下載並解壓縮最新的發行版 (Release)。
2.  執行 `PotPlayerPlaylistGenerator.exe`。
3.  **初次設定** (如果需要)：
    -   點擊右上角的「⚙️ 設定」按鈕。
    -   在「影片副檔名」欄位中，輸入您想搜尋的檔案類型，以逗號分隔。
    -   在「根資料夾路徑」欄位中，輸入或瀏覽至您存放影片的根目錄。
    -   點擊「儲存設定」。
4.  從主視窗的列表中選擇一個資料夾。
5.  點擊「使用 PotPlayer 播放」按鈕，PotPlayer 將會啟動並開始播放。

## 🛠️ 從原始碼建置

如果您想自行修改或建置此專案，請遵循以下步驟：

1.  確保您已安裝 [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)。
2.  Clone 此儲存庫：
    ```bash
    git clone https://github.com/your-username/PotPlayerPlaylistGenerator.git
    ```
3.  進入專案目錄：
    ```bash
    cd PotPlayerPlaylistGenerator
    ```
4.  使用以下指令進行建置：
    ```bash
    dotnet build --configuration Release
    ```
5.  編譯完成後，可執行檔將位於 `bin/Release/net10.0-windows/` 目錄下。
