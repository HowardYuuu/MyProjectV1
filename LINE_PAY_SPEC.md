# LINE Pay 購物車結帳功能規格書

## 1. 功能概述

本規格書詳細說明購物車結帳流程，包含 LINE Pay 支付整合、付款方式選擇以及優惠券選擇器的功能設計與實作細節。

---

## 2. 使用者流程

### 2.1 整體流程圖

```
購物車頁面 → 填寫結帳資訊 → 選擇付款方式 → 確認訂單 → LINE Pay 付款 → 付款結果頁面
```

### 2.2 詳細步驟

1. **步驟一：購物車頁面**
   - 使用者在購物車檢視所選商品
   - 確認商品數量、尺寸、價格
   - 點擊「前往結帳」按鈕

2. **步驟二：結帳確認頁面**
   - 填寫收件人資訊（姓名、地址、電話、Email）
   - 選擇取貨門市（可選）
   - 檢視訂單明細與總金額
   - 選擇付款方式
   - 選擇優惠券（如有可用）

3. **步驟三：確認付款**
   - 點擊「確認付款」按鈕
   - 系統建立訂單資料
   - 根據選擇的付款方式進行處理

4. **步驟四：LINE Pay 付款流程**
   - 系統發送 LINE Pay 付款請求
   - 使用者被導向 LINE Pay 付款頁面
   - 使用者在 LINE 應用程式中完成付款
   - LINE Pay 回調確認網址

5. **步驟五：付款結果頁面**
   - 顯示付款成功或失敗訊息
   - 顯示訂單編號與相關資訊
   - 提供返回首頁或查看訂單按鈕

---

## 3. 頁面規格

### 3.1 購物車頁面 (ShoppingCart.cshtml)

**路徑：** `/Home/ShoppingCart`

**功能：**
- 顯示購物車內所有商品
- 允許修改商品數量（+/- 按鈕）
- 允許刪除商品
- 顯示每項商品的小計
- 顯示總金額
- 提供「前往結帳」按鈕

**UI 元素：**
- 商品列表（圖片、名稱、尺寸、單價、數量、小計）
- 數量調整按鈕
- 刪除按鈕
- 總金額顯示區
- 前往結帳按鈕

**現有狀態：** ✅ 已實作

---

### 3.2 結帳確認頁面 (CheckOut.cshtml)

**路徑：** `/Home/CheckOut`

**功能：**
- 收集收件人資訊
- 選擇取貨門市（整合 ECPay 物流地圖）
- 顯示訂單明細
- 選擇付款方式
- 選擇優惠券
- 確認並提交訂單

**UI 區塊：**

#### 3.2.1 結帳資訊表單
```
- 真實姓名（必填）
- 聯絡地址（必填）
- 取貨地址（選填，可透過按鈕選擇門市）
- Email（必填）
- 電話號碼（必填，格式：09-XXXXXXXX）
```

#### 3.2.2 訂單明細區
```
- 商品列表（名稱、尺寸、數量、金額）
- 運費：$0.00
- 折扣：$0.00（如使用優惠券）
- 訂單總金額
```

#### 3.2.3 付款方式選擇器
```html
<select id="selectPayment">
  <option value="Linepay" selected>LINE Pay</option>
  <option value="Amount">取貨付款</option>
</select>
```

**當前版本：** 目前僅支援 LINE Pay

#### 3.2.4 優惠券選擇器（未來功能）
```
暫時顯示：「目前無可用優惠券」
未來實作：
- 顯示可用優惠券列表
- 支援選擇與套用優惠券
- 即時更新折扣後金額
```

**驗證規則：**
- 姓名：不可為空
- Email：符合 Email 格式
- 電話：格式驗證（09 開頭，共 10 碼）
- 地址：不可為空

**現有狀態：** ⚠️ 需要增強

---

### 3.3 付款結果頁面 (PaymentResult.cshtml) - 新增

**路徑：** `/Home/PaymentResult`

**功能：**
- 顯示付款狀態（成功/失敗）
- 顯示訂單編號
- 顯示訂單金額
- 顯示交易時間
- 提供導航按鈕

**成功時顯示：**
```
✅ 付款成功！
訂單編號：20231215-00001
付款金額：NT$ 1,580
付款時間：2023-12-15 14:30:25
交易代碼：[LINE Pay Transaction ID]

[查看訂單詳情] [返回首頁]
```

**失敗時顯示：**
```
❌ 付款失敗
失敗原因：[錯誤訊息]

[重新付款] [返回購物車]
```

**現有狀態：** ❌ 需要新增

---

## 4. 後端 API 規格

### 4.1 訂單建立 API

**端點：** `POST /Home/ConfirmPayment`

**Request Body：**
```json
{
  "Name": "王小明",
  "Address": "台北市信義區信義路五段7號",
  "Address2": "7-11 信義門市",
  "Email": "user@example.com",
  "Phone": "0912345678",
  "Payment": "Linepay",
  "TotalAmount": 1580.00
}
```

**Response (成功)：**
```json
{
  "success": true,
  "orderId": "20231215-00001",
  "message": "訂單建立成功"
}
```

**功能：**
1. 建立訂單主檔 (TOrder)
2. 建立訂單明細 (TOrderDetail)
3. 清空購物車
4. 如選擇 LINE Pay，初始化支付流程

**現有狀態：** ✅ 已實作（需整合 LINE Pay 流程）

---

### 4.2 LINE Pay 付款請求 API

**端點：** `POST /api/LinePay/Create`

**Request Body：**
```json
{
  "Amount": 1580,
  "Currency": "TWD",
  "OrderId": "20231215-00001",
  "Packages": [
    {
      "Id": "PKG001",
      "Amount": 1580,
      "Name": "商品包裝",
      "Products": [
        {
          "Name": "Nike Air Max 270",
          "Quantity": 1,
          "Price": 1580
        }
      ]
    }
  ],
  "RedirectUrls": {
    "ConfirmUrl": "https://yourdomain.com/Home/LinePayConfirm",
    "CancelUrl": "https://yourdomain.com/Home/LinePayCancel"
  }
}
```

**Response (成功)：**
```json
{
  "returnCode": "0000",
  "returnMessage": "Success",
  "info": {
    "paymentUrl": {
      "web": "https://sandbox-web-pay.line.me/web/payment/...",
      "app": "line://pay/payment/..."
    },
    "transactionId": 2021123112345678900,
    "paymentAccessToken": "187568751124"
  }
}
```

**現有狀態：** ✅ 已實作

---

### 4.3 LINE Pay 付款確認 API

**端點：** `POST /api/LinePay/Confirm?transactionId={transactionId}&orderId={orderId}`

**Request Body：**
```json
{
  "Amount": 1580,
  "Currency": "TWD"
}
```

**Response (成功)：**
```json
{
  "returnCode": "0000",
  "returnMessage": "Success",
  "info": {
    "orderId": "20231215-00001",
    "transactionId": 2021123112345678900,
    "payInfo": [
      {
        "method": "CREDIT_CARD",
        "amount": 1580
      }
    ]
  }
}
```

**現有狀態：** ✅ 已實作

---

### 4.4 LINE Pay 取消交易 API

**端點：** `GET /api/LinePay/Cancel?transactionId={transactionId}`

**功能：**
- 當使用者取消 LINE Pay 交易時呼叫
- 紀錄取消事件
- 導向取消頁面

**現有狀態：** ✅ 已實作

---

## 5. 資料庫設計

### 5.1 訂單主檔 (TOrder)

| 欄位名稱 | 資料型別 | 說明 | 範例 |
|---------|---------|------|------|
| FId | int | 主鍵 (Identity) | 1 |
| FOrderId | string | 訂單編號 | 20231215-00001 |
| FCustomerId | int | 客戶編號 (FK) | 101 |
| FCusName | string | 收件人姓名 | 王小明 |
| FCusEmail | string | 收件人 Email | user@example.com |
| FCusPhone | string | 收件人電話 | 0912345678 |
| FCusAddress | string | 收件地址 | 台北市信義區... |
| FOrderDate | DateTime | 訂單日期 | 2023-12-15 14:30 |
| FEndDate | DateTime | 預計到貨日期 | 2023-12-18 14:30 |
| FTotalAmount | decimal | 訂單總金額 | 1580.00 |
| FPaymentMethod | string | 付款方式 | Linepay |
| FStatus | string | 訂單狀態 | 未付款/已付款/已出貨 |

**現有狀態：** ✅ 已存在

---

### 5.2 訂單明細 (TOrderDetail)

| 欄位名稱 | 資料型別 | 說明 |
|---------|---------|------|
| FOrderDetailId | int | 主鍵 (Identity) |
| FOrderId | string | 訂單編號 (FK) |
| FProductId | int | 商品編號 (FK) |
| FSize | decimal | 尺寸 |
| FQuantity | int | 數量 |
| FUnitPrice | decimal | 單價 |
| FSubTotal | decimal | 小計 |

**現有狀態：** ✅ 已存在

---

### 5.3 優惠券資料表 (未來新增)

**建議欄位：**
```sql
CREATE TABLE TCoupon (
    FCouponId INT PRIMARY KEY IDENTITY,
    FCouponCode NVARCHAR(50) NOT NULL UNIQUE,
    FDiscountType NVARCHAR(20), -- 'Percentage' 或 'Fixed'
    FDiscountValue DECIMAL(10,2),
    FMinPurchase DECIMAL(10,2),
    FStartDate DATETIME,
    FEndDate DATETIME,
    FIsActive BIT DEFAULT 1,
    FUsageLimit INT,
    FUsedCount INT DEFAULT 0
)
```

**現有狀態：** ❌ 未實作

---

## 6. LINE Pay 整合流程

### 6.1 付款流程時序圖

```
使用者端                 系統後端              LINE Pay API
   |                        |                      |
   |--點擊確認付款--------->|                      |
   |                        |--建立訂單----------->|
   |                        |                      |
   |                        |--請求付款 API------->|
   |                        |<-返回付款網址--------|
   |<--導向 LINE Pay 頁面---|                      |
   |                        |                      |
   |--在 LINE 中完成付款--->|                      |
   |                        |                      |
   |--回調 ConfirmUrl------>|                      |
   |                        |--確認付款 API------->|
   |                        |<-返回付款結果--------|
   |                        |--更新訂單狀態------->|
   |<--顯示付款結果頁面-----|                      |
```

### 6.2 設定參數

**Channel ID：** `2004484565` (Sandbox 環境)
**Channel Secret Key：** `02b88ff93c73397a9162d09c0414317e` (需妥善保管)
**API Base URL：** `https://sandbox-api-pay.line.me` (測試環境)

**正式環境：**
- API Base URL：`https://api-pay.line.me`
- 需向 LINE Pay 申請正式帳號

### 6.3 簽章機制

LINE Pay 使用 HMAC-SHA256 進行請求簽章：

```csharp
string signature = HMACSHA256(
    channelSecretKey,
    channelSecretKey + requestUrl + jsonBody + nonce
)
```

**Headers：**
```
X-LINE-ChannelId: {channelId}
X-LINE-Authorization-Nonce: {nonce}
X-LINE-Authorization: {signature}
Content-Type: application/json
```

---

## 7. 錯誤處理

### 7.1 前端驗證錯誤

| 錯誤情境 | 錯誤訊息 | 處理方式 |
|---------|---------|---------|
| 姓名空白 | 請輸入真實姓名 | 阻止提交，顯示錯誤訊息 |
| Email 格式錯誤 | 請輸入有效的 Email | 阻止提交，顯示錯誤訊息 |
| 電話格式錯誤 | 請輸入正確的手機號碼 | 阻止提交，顯示錯誤訊息 |
| 地址空白 | 請輸入聯絡地址 | 阻止提交，顯示錯誤訊息 |
| 購物車為空 | 購物車目前為空 | 導向首頁 |

### 7.2 LINE Pay API 錯誤碼

| 錯誤碼 | 訊息 | 處理方式 |
|-------|------|---------|
| 0000 | Success | 正常流程 |
| 1104 | 商店不存在 | 顯示系統錯誤訊息 |
| 1105 | 該商店無法使用 LINE Pay | 顯示系統錯誤訊息 |
| 1106 | Header 資訊錯誤 | 系統記錄，重試 |
| 1124 | 金額錯誤 | 顯示金額錯誤訊息 |
| 1145 | 交易已處理 | 顯示交易已完成 |
| 1150 | 交易不存在 | 顯示交易不存在 |
| 1198 | API 呼叫錯誤 | 系統記錄，重試 |
| 2101 | 參數錯誤 | 系統記錄，檢查參數 |
| 9000 | 內部請求錯誤 | 顯示系統暫時無法服務 |

### 7.3 取消交易處理

當使用者在 LINE Pay 頁面取消付款：
1. 導向取消頁面 (CancelUrl)
2. 顯示「付款已取消」訊息
3. 提供「重新付款」與「返回購物車」按鈕
4. 訂單狀態保持「未付款」

---

## 8. 安全性考量

### 8.1 敏感資訊保護

- ✅ Channel Secret Key 存放於環境變數或 Azure Key Vault
- ✅ 不在前端暴露 API 金鑰
- ✅ 使用 HTTPS 加密傳輸
- ✅ 付款確認使用伺服器端驗證

### 8.2 防範攻擊

- **CSRF 保護：** 使用 ASP.NET Core 內建的防偽標記
- **SQL Injection：** 使用 Entity Framework 參數化查詢
- **XSS 防護：** 前端輸入進行編碼處理
- **重放攻擊：** 使用 Nonce 機制防止重複請求

### 8.3 資料驗證

- 伺服器端驗證所有使用者輸入
- 驗證金額是否匹配購物車總額
- 驗證訂單狀態，防止重複付款
- 驗證 LINE Pay 回傳的簽章

---

## 9. 測試計畫

### 9.1 單元測試

- [x] 訂單建立邏輯測試
- [ ] LINE Pay 請求建立測試
- [ ] 金額計算正確性測試
- [ ] 優惠券折扣計算測試（未來）

### 9.2 整合測試

- [ ] 完整購物流程測試
- [ ] LINE Pay Sandbox 環境測試
- [ ] 付款成功流程測試
- [ ] 付款失敗流程測試
- [ ] 取消付款流程測試

### 9.3 使用者驗收測試 (UAT)

**測試案例：**

1. **正常購物流程**
   - 新增商品至購物車
   - 前往結帳填寫資訊
   - 選擇 LINE Pay 付款
   - 完成付款
   - 驗證訂單狀態更新

2. **取消付款流程**
   - 進入 LINE Pay 付款頁面
   - 點擊取消
   - 驗證導向取消頁面
   - 驗證訂單狀態未變更

3. **表單驗證測試**
   - 測試必填欄位驗證
   - 測試格式驗證
   - 測試空購物車防呆

4. **金額驗證測試**
   - 驗證小計計算正確
   - 驗證總金額計算正確
   - 驗證優惠券折扣計算（未來）

---

## 10. 部署注意事項

### 10.1 環境設定

**appsettings.json 設定：**
```json
{
  "LinePay": {
    "ChannelId": "YOUR_CHANNEL_ID",
    "ChannelSecretKey": "YOUR_SECRET_KEY",
    "BaseUrl": "https://sandbox-api-pay.line.me",
    "ConfirmUrl": "https://yourdomain.com/Home/LinePayConfirm",
    "CancelUrl": "https://yourdomain.com/Home/LinePayCancel"
  }
}
```

### 10.2 Sandbox 測試帳號

- 測試時需使用 LINE Pay Sandbox 測試帳號
- 測試信用卡號可在 LINE Pay 開發者文件中取得

### 10.3 正式環境部署檢查清單

- [ ] 更新為正式環境 API URL
- [ ] 更新為正式環境 Channel ID 與 Secret
- [ ] 設定正確的 Confirm URL 與 Cancel URL
- [ ] 啟用 HTTPS
- [ ] 設定伺服器防火牆規則
- [ ] 備份資料庫
- [ ] 進行壓力測試
- [ ] 準備監控與日誌系統

---

## 11. 未來擴充功能

### 11.1 優惠券系統（第二階段）

**功能規劃：**
- 優惠券碼輸入介面
- 優惠券驗證機制
- 自動套用最優惠的優惠券
- 優惠券使用記錄
- 優惠券管理後台

### 11.2 其他付款方式（第三階段）

- 信用卡直接支付
- Apple Pay / Google Pay
- 貨到付款
- ATM 轉帳
- 超商代碼繳費

### 11.3 會員點數系統（第四階段）

- 消費累積點數
- 點數折抵金額
- 點數兌換優惠券

---

## 12. 附錄

### 12.1 相關文件連結

- [LINE Pay API 文件](https://pay.line.me/documents/online_v3.html)
- [LINE Pay Sandbox 測試環境](https://sandbox-web-pay.line.me/)
- [ASP.NET Core 文件](https://docs.microsoft.com/aspnet/core/)

### 12.2 聯絡資訊

**技術支援：**
- LINE Pay 技術支援：[LINE Pay 開發者中心](https://pay.line.me/portal/tw/main)

### 12.3 變更歷史

| 版本 | 日期 | 變更內容 | 作者 |
|-----|------|---------|------|
| 1.0 | 2023-12-15 | 初版規格書建立 | GitHub Copilot |

---

## 13. 結語

本規格書涵蓋 LINE Pay 結帳功能的完整設計與實作細節。開發團隊應依照本規格進行開發，並在開發過程中隨時更新此文件以反映實際實作情況。

**重要提醒：**
1. 所有金流相關功能必須經過完整測試
2. 敏感資訊（如 API 金鑰）不可提交至版控系統
3. 正式上線前必須完成 UAT 測試
4. 建議設置監控系統追蹤交易成功率

**下一步行動：**
1. ✅ 審核本規格書
2. 開發付款結果頁面
3. 整合 LINE Pay 完整流程
4. 實作優惠券功能（未來）
5. 進行測試與部署
