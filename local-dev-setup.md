# Local Development Setup

**[English](#english) · [فارسی](#فارسی)**

---

## English

CodeMate uses SQL Server via Entity Framework Core. This guide covers getting a working local database, including the two problems teammates have actually hit while setting this up.

### 1. Get a SQL Server instance

You have two options — pick whichever is faster for you.

**Option A — Install SQL Server Express LocalDB** (recommended if you have decent internet)

1. Download and install [SQL Server Express LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb).
2. Create the instance:
   ```bash
   sqllocaldb create MSSQLLocalDB
   sqllocaldb start MSSQLLocalDB
   ```
3. Verify it's running:
   ```bash
   sqllocaldb info MSSQLLocalDB
   ```

If `sqllocaldb` is not recognized at all after installing, your terminal likely needs a restart, or LocalDB wasn't installed as part of your Visual Studio installation — reinstalling the "Data storage and processing" workload in the Visual Studio Installer fixes this in most cases.

**Option B — Point at any SQL Server instance you already have** (faster if internet is slow, or LocalDB won't cooperate)

If you have SQL Server Management Studio (SSMS) installed and it can already connect to *something* — a full SQL Server instance, a named instance, whatever — you don't need LocalDB at all. Just use that instance's name as your `Server=` value. The `CodeMateDb` database itself doesn't need to exist yet; EF Core migrations create it for you.

This is the option that actually got a teammate unblocked when LocalDB refused to install correctly.

### 2. Set your connection string — locally, never in source control

**Do not** put your real connection string in `appsettings.json` — that file is shared and committed. Use one of these instead:

**User secrets (recommended):**
```bash
cd CodeMate.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=CodeMateDb;Trusted_Connection=True;TrustServerCertificate=True"
```

**Or `appsettings.Development.json`** (must be in `.gitignore` — check before you commit anything):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CodeMateDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

If you went with Option B above, replace `(localdb)\MSSQLLocalDB` with your instance name, e.g. `Server=DESKTOP-XXXXXXX;Database=CodeMateDb;Trusted_Connection=True;TrustServerCertificate=True`.

### 3. Create the database

```bash
dotnet ef database update --project CodeMate.Infrastructure --startup-project CodeMate.API
```

This runs every migration in order and creates `CodeMateDb` (or whatever you named it) from scratch. An admin account is seeded automatically on first run — check `SeedData.cs` for the seeded credentials.

### 4. Run it

```bash
dotnet run --project CodeMate.API
```

Swagger opens at the root URL. If you can register a user and log in, your database is wired up correctly.

### After pulling changes from `develop`

Any time you pull a branch that added a migration, run step 3 again (`dotnet ef database update`) before running the app — otherwise you'll get errors about missing tables or columns for whatever feature just landed.

### Testing without a finished feature branch

If you're building against an entity that another branch introduced (its migration is merged but the feature's endpoints aren't done yet), you can insert test rows directly via SSMS to unblock yourself rather than waiting for the full API surface — this is how the Dashboard queries were validated before Task Management's write endpoints existed.

---

## فارسی

<div dir="rtl">

CodeMate از SQL Server از طریق Entity Framework Core استفاده می‌کند. این راهنما نحوه‌ی راه‌اندازی یک دیتابیس لوکال کارآمد را پوشش می‌دهد، شامل دو مشکل واقعی که همکاران هنگام راه‌اندازی با آن‌ها روبه‌رو شدند.

### ۱. تهیه‌ی یک نمونه SQL Server

دو گزینه دارید — هرکدام که سریع‌تر جواب داد را انتخاب کنید.

**گزینه‌ی الف — نصب SQL Server Express LocalDB** (توصیه‌شده اگر اینترنت مناسبی دارید)

۱. [SQL Server Express LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) را دانلود و نصب کنید.
۲. نمونه را بسازید:

</div>

```bash
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

<div dir="rtl">

۳. مطمئن شوید در حال اجراست:

</div>

```bash
sqllocaldb info MSSQLLocalDB
```

<div dir="rtl">

اگر بعد از نصب، دستور `sqllocaldb` اصلاً شناخته نشد، احتمالاً ترمینال نیاز به ری‌استارت دارد، یا LocalDB به‌عنوان بخشی از نصب Visual Studio نصب نشده — نصب دوباره‌ی Workload مربوط به «Data storage and processing» از Visual Studio Installer در بیشتر موارد مشکل را حل می‌کند.

**گزینه‌ی ب — وصل‌شدن به هر SQL Server موجود** (سریع‌تر اگر اینترنت کند است یا LocalDB همکاری نمی‌کند)

اگر SQL Server Management Studio (SSMS) نصب دارید و از قبل به *چیزی* وصل می‌شود — یک نمونه‌ی کامل SQL Server، یک Instance با نام خاص، هرچه که باشد — اصلاً نیازی به LocalDB ندارید. فقط نام همان Instance را به‌عنوان مقدار `Server=` استفاده کنید. خودِ دیتابیس `CodeMateDb` هم لازم نیست از قبل وجود داشته باشد؛ Migrationهای EF Core آن را می‌سازند.

این همان گزینه‌ای بود که یکی از همکاران را وقتی نصب LocalDB درست کار نمی‌کرد، از گیر انداخت.

### ۲. تنظیم Connection String — فقط به‌صورت لوکال، هرگز داخل کد مشترک

**هرگز** Connection String واقعی خود را داخل `appsettings.json` نگذارید — این فایل مشترک و Commit‌شده است. به‌جایش از یکی از این دو استفاده کنید:

**User Secrets (توصیه‌شده):**

</div>

```bash
cd CodeMate.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=CodeMateDb;Trusted_Connection=True;TrustServerCertificate=True"
```

<div dir="rtl">

**یا `appsettings.Development.json`** (حتماً باید در `.gitignore` باشد — قبل از هر Commit چک کنید):

</div>

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CodeMateDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

<div dir="rtl">

اگر گزینه‌ی ب را انتخاب کردید، `(localdb)\MSSQLLocalDB` را با نام Instance خودتان جایگزین کنید، مثلاً: `Server=DESKTOP-XXXXXXX;Database=CodeMateDb;Trusted_Connection=True;TrustServerCertificate=True`.

### ۳. ساخت دیتابیس

</div>

```bash
dotnet ef database update --project CodeMate.Infrastructure --startup-project CodeMate.API
```

<div dir="rtl">

این دستور تمام Migrationها را به‌ترتیب اجرا می‌کند و `CodeMateDb` (یا هر نامی که گذاشته‌اید) را از صفر می‌سازد. یک حساب Admin به‌صورت خودکار در اولین اجرا Seed می‌شود — برای اطلاعات ورود به `SeedData.cs` مراجعه کنید.

### ۴. اجرای پروژه

</div>

```bash
dotnet run --project CodeMate.API
```

<div dir="rtl">

Swagger روی آدرس اصلی باز می‌شود. اگر توانستید یک کاربر ثبت‌نام کرده و وارد شوید، یعنی دیتابیستان درست وصل شده است.

### بعد از Pull کردن تغییرات از `develop`

هر زمان برنچی را Pull کردید که یک Migration جدید اضافه کرده، قبل از اجرای برنامه دوباره مرحله‌ی ۳ (`dotnet ef database update`) را اجرا کنید — وگرنه با خطاهایی درباره‌ی جدول یا ستون‌های ناموجود برای فیچری که تازه اضافه شده مواجه می‌شوید.

### تست بدون تمام‌شدن یک برنچ فیچر

اگر دارید روی یک Entity کار می‌کنید که برنچ دیگری آن را معرفی کرده (Migrationش Merge شده ولی Endpointهای فیچرش هنوز آماده نیست)، می‌توانید مستقیماً از طریق SSMS چند رکورد تستی Insert کنید تا خودتان را از گیر بیندازید، به‌جای این‌که منتظر کامل‌شدن API بمانید — دقیقاً همین‌طور بود که Queryهای Dashboard قبل از آماده‌شدن Endpointهای نوشتنی Task Management تست شدند.

</div>