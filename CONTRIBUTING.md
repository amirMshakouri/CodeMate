# Contributing to CodeMate

**[English](#english) · [فارسی](#فارسی)**

---

## English

Thanks for considering a contribution. This document covers the workflow and conventions this codebase actually follows — please read it before opening a PR, it'll save both of us a review round-trip.

### Branching

- `main` — stable releases only. Nothing is pushed here directly.
- `develop` — integration branch. All feature branches merge here.
- `feature/<scope>-<description>` — one branch per unit of work, branched off `develop`.

Open your pull request against `develop`, not `main`.

### Before you start coding

1. Check the [Roadmap](README.md#roadmap) to see what's already done, in progress, or explicitly deferred.
2. If you're picking up an open item, comment on its issue (or open one) so two people don't build the same thing.
3. If your change touches the database schema, plan for a migration — see [Database changes](#database-changes) below.

### Architecture rules

CodeMate follows Clean Architecture (see `docs/architecture.md` for the full picture). Two rules are non-negotiable:

- **`CodeMate.Contracts` never references `CodeMate.Domain`.** DTOs use primitives (`int`, `string`, `Guid`), never domain enums directly.
- **Commands and queries stay separate.** Write logic and read logic live in different services (`IXCommandService` / `IXQueryService`), different repository methods, and different controller partials (`XController.Commands.cs` / `XController.Queries.cs`).

Beyond that, keep to the existing layering — a controller should never talk to a `DbContext` or a repository directly, only to a service interface.

### Database changes

1. Change the entity in `CodeMate.Domain.Entities`.
2. Update its `Configuration` class in `CodeMate.Infrastructure.Persistence.Configurations`.
3. Generate a migration:
   ```bash
   dotnet ef migrations add <DescriptiveName> --project CodeMate.Infrastructure --startup-project CodeMate.API
   ```
4. Apply it locally and confirm the app still runs before committing.
5. Commit the migration files alongside your code changes — never generate a migration in a separate, later PR unless the team has explicitly agreed to sequence it that way.

### Testing your change

There's no automated test suite yet (a deliberate, temporary trade-off for delivery speed — see the Roadmap). In its place:

- Run the actual endpoint(s) you changed through Swagger against a real local database — not just a successful `dotnet build`.
- Test the failure paths, not just the happy path: wrong owner, missing record, invalid input, duplicate data.
- If your change affects an existing, already-merged feature, re-test that feature's core flow too, not just your addition.

### Pull request checklist

- [ ] Builds with no warnings you introduced
- [ ] Ran against a real database, not just `dotnet build`
- [ ] No connection strings, secrets, or personal `appsettings.Development.json` values committed
- [ ] New Contracts don't reference Domain types
- [ ] New commands and queries are in separate files, matching the existing pattern
- [ ] Migration included if the schema changed
- [ ] PR description says what changed and how you tested it

### Reporting bugs or proposing features

Open an issue. Include what you expected, what happened instead, and, for bugs, the exact request/response from Swagger if it's API-related. For feature proposals, a short rationale is more useful than a full spec.

---

## فارسی

<div dir="rtl">

از این‌که به مشارکت فکر می‌کنید ممنونیم. این سند، جریان کاری و قراردادهایی را پوشش می‌دهد که این کدبیس واقعاً از آن‌ها پیروی می‌کند — لطفاً پیش از باز کردن PR آن را بخوانید، هم وقت شما هم وقت بازبین را کم می‌کند.

### Branch‌بندی

- `main` — فقط نسخه‌های پایدار. هیچ‌چیز مستقیماً اینجا Push نمی‌شود.
- `develop` — برنچ یکپارچه‌سازی. تمام برنچ‌های فیچر اینجا Merge می‌شوند.
- `feature/<scope>-<description>` — یک برنچ برای هر واحد کاری، از `develop` گرفته می‌شود.

Pull Request خود را روی `develop` باز کنید، نه `main`.

### پیش از شروع کدنویسی

۱. [نقشه راه](README.md#نقشه-راه) را چک کنید تا ببینید چه چیزی تمام شده، در حال انجام است، یا عمداً به تعویق افتاده.
۲. اگر یک مورد باز را برمی‌دارید، روی Issue مربوطه Comment بگذارید (یا یکی باز کنید) تا دو نفر یک چیز را دوبار نسازند.
۳. اگر تغییرتان روی Schema دیتابیس اثر می‌گذارد، برای یک Migration برنامه‌ریزی کنید — به بخش [تغییرات دیتابیس](#تغییرات-دیتابیس) مراجعه کنید.

### قوانین معماری

CodeMate از Clean Architecture پیروی می‌کند (تصویر کامل در `docs/architecture.md`). دو قانون غیرقابل مذاکره‌اند:

- **`CodeMate.Contracts` هرگز به `CodeMate.Domain` رفرنس نمی‌دهد.** DTOها از انواع پایه (`int`، `string`، `Guid`) استفاده می‌کنند، هرگز مستقیماً از Enumهای Domain.
- **Command و Query از هم جدا می‌مانند.** منطق نوشتن و خواندن در سرویس‌های جدا (`IXCommandService` / `IXQueryService`)، متدهای ریپازیتوری جدا، و Partialهای جداگانه‌ی کنترلر (`XController.Commands.cs` / `XController.Queries.cs`) قرار می‌گیرند.

فراتر از این، به لایه‌بندی موجود پایبند بمانید — یک کنترلر هرگز نباید مستقیماً با `DbContext` یا یک Repository صحبت کند، فقط با یک Interface سرویس.

### تغییرات دیتابیس

۱. Entity را در `CodeMate.Domain.Entities` تغییر دهید.
۲. کلاس `Configuration` مربوطه را در `CodeMate.Infrastructure.Persistence.Configurations` به‌روز کنید.
۳. یک Migration بسازید:

</div>

```bash
dotnet ef migrations add <DescriptiveName> --project CodeMate.Infrastructure --startup-project CodeMate.API
```

<div dir="rtl">

۴. آن را به‌صورت لوکال اعمال کنید و پیش از Commit مطمئن شوید برنامه هنوز اجرا می‌شود.
۵. فایل‌های Migration را همراه با تغییرات کدتان Commit کنید — هرگز یک Migration را در یک PR جدا و بعدی نسازید، مگر این‌که تیم صراحتاً روی این ترتیب توافق کرده باشد.

### تست تغییرتان

هنوز مجموعه‌ی تست خودکاری وجود ندارد (یک مصالحه‌ی عمدی و موقت برای سرعت تحویل — به نقشه راه مراجعه کنید). به‌جایش:

- Endpoint(های)ی که تغییر دادید را واقعاً از طریق Swagger روی یک دیتابیس لوکال واقعی اجرا کنید — نه فقط یک `dotnet build` موفق.
- مسیرهای شکست را هم تست کنید، نه فقط مسیر موفق: مالک اشتباه، رکورد ناموجود، ورودی نامعتبر، داده‌ی تکراری.
- اگر تغییرتان روی یک فیچر موجود و از قبل Merge‌شده اثر می‌گذارد، جریان اصلی آن فیچر را هم دوباره تست کنید، نه فقط افزوده‌ی خودتان را.

### چک‌لیست Pull Request

- [ ] بدون هیچ Warning جدیدی که خودتان اضافه کرده‌اید Build می‌شود
- [ ] روی یک دیتابیس واقعی اجرا شده، نه فقط `dotnet build`
- [ ] هیچ Connection String، اطلاعات محرمانه، یا مقدار شخصی `appsettings.Development.json` Commit نشده
- [ ] Contractهای جدید به Domain رفرنس نمی‌دهند
- [ ] Commandها و Queryهای جدید در فایل‌های جدا، مطابق الگوی موجود هستند
- [ ] در صورت تغییر Schema، Migration اضافه شده
- [ ] توضیح PR مشخص می‌کند چه چیزی تغییر کرده و چطور تست شده

### گزارش باگ یا پیشنهاد فیچر

یک Issue باز کنید. بنویسید چه انتظاری داشتید، چه اتفاقی افتاد، و برای باگ‌ها، دقیقاً Request/Response مربوطه از Swagger را (اگر مربوط به API است) اضافه کنید. برای پیشنهاد فیچر، یک توجیه کوتاه مفیدتر از یک Spec کامل است.

</div>