# CodeMate

**[English](#english) · [فارسی](#فارسی)**

---

## English

A collaboration platform where developers publish project ideas, define the skills those projects need, and build teams from people who actually have them.

Most side projects die because the right people never find each other. CodeMate fixes the matching part: you post a project, define its teams and the skills they need, and people whose profiles match can request to join. Once a team forms, the same platform handles the work — tasks, progress tracking, and oversight.

### Features

- **Authentication** — register, login (JWT), logout, change password, forgot/reset password
- **User profiles** — view and edit your profile
- **Skills** — a moderated skill catalog (managed by admins); users pick skills from it and set their proficiency level
- **Projects** — create, edit, delete, change status; search and list with filtering and pagination; ownership enforced on every write
- **Teams** — sub-teams within a project (e.g. Front-End, Back-End); required-skill definitions per project; join requests with skill-based eligibility checks; member management
- **Tasks** — create, assign to project members, move through `Todo → Doing → Done`
- **Dashboard** — project progress metrics for owners (task counts, overdue items, member performance), personal task overview for members
- **Admin panel** — user oversight (search, deactivate/reactivate, promote), project oversight (force status change, force delete), skill catalog management (add/edit/remove)

### Tech stack

- .NET / ASP.NET Core Web API
- Entity Framework Core + SQL Server
- JWT authentication, role-based authorization
- FluentValidation, AutoMapper
- Swagger / OpenAPI

### Architecture

CodeMate follows Clean Architecture. Dependencies point inward only — the domain layer knows nothing about the outside world.

```
CodeMate.Domain          Entities, enums, business rules — no dependencies
CodeMate.Application     Use cases, service interfaces, validators, mappings
CodeMate.Contracts       Request/response DTOs — the client-facing contract
CodeMate.Infrastructure  EF Core, repositories, JWT, password hashing, migrations
CodeMate.API             Controllers, middleware, DI, Swagger, CORS
CodeMate.Shared          Helpers, exceptions, pagination, result types
```

Two conventions worth knowing before you contribute:

- **Contracts never reference Domain.** DTOs use primitives (`int`, `string`, `Guid`) rather than domain enums, so a domain change can't silently break the public API.
- **Commands and queries are separated.** Write operations and read operations live in separate services, repositories, and controller partials (e.g. `ProjectsController.Commands.cs` / `ProjectsController.Queries.cs`). This keeps concerns apart and lets multiple people work on the same feature without stepping on each other.

### Getting started

**Prerequisites**

- .NET SDK
- SQL Server (LocalDB, Express, or a full instance)

**Setup**

```bash
git clone <repository-url>
cd CodeMate
dotnet restore
```

Set your connection string — use user secrets or `appsettings.Development.json`, never the shared `appsettings.json`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=CodeMateDb;Trusted_Connection=True;TrustServerCertificate=True"
```

If LocalDB isn't installed or `sqllocaldb info` isn't recognized, point `Server=` at any SQL Server instance you can reach — the database itself is created by the migrations.

Apply migrations and run:

```bash
dotnet ef database update
dotnet run --project CodeMate.API
```

Swagger UI is available at the root URL once the app is running. On first run, an admin account is seeded automatically (see the `SeedData` / `AdminSeed` configuration for credentials).

### API overview

The full, current reference — including request/response shapes — is in Swagger. Key endpoint groups:

| Area | Base route | Examples |
|---|---|---|
| Auth | `/api/auth` | register, login, logout, change-password, forgot-password, reset-password |
| Users | `/api/users` | `GET/PUT /me` |
| Skills | `/api/skills` | catalog search, `GET/POST/DELETE /me` |
| Projects | `/api/projects` | CRUD, search, `PATCH /{id}/status` |
| Teams | `/api/teams`, `/api/projects/{id}/teams` | CRUD, members, join requests (send/accept/reject) |
| Tasks | `/api/tasks` | CRUD, `PATCH /{id}/assign`, `PATCH /{id}/status`, `/mine` |
| Dashboard | `/api/dashboard` | `/projects/{id}`, `/me` |
| Admin | `/api/admin/users`, `/api/admin/projects`, `/api/admin/skills` | user oversight, forced project actions, skill catalog CRUD — all `Admin`-role only |

### Roadmap

- [x] Authentication (register, login, logout, password recovery)
- [x] User management (profile, skills)
- [x] Project management (CRUD, search, status)
- [x] Team management (teams, join requests, skill matching)
- [x] Task management (create, assign, status workflow)
- [x] Dashboard (project and personal metrics)
- [x] Admin panel (user/project oversight, skill catalog)
- [ ] Meaningful use of team member roles (Lead vs. Member permissions)
- [ ] Link a project to its GitHub repository
- [ ] Email notifications (join request outcomes, task assignments)
- [ ] Self-service leaving a team / project ownership transfer
- [ ] Rate limiting on authentication endpoints
- [ ] Web frontend

### Contributing

Contributions are welcome.

Branches come off `develop`, one branch per unit of work, named `feature/<scope>-<description>`. Open a pull request against `develop`; `main` holds stable releases only.

Before opening a PR:

- Match the existing layer structure — don't reach across architectural boundaries
- Keep commands and queries in separate files, following the partial-class pattern already in use
- Verify your changes run against a real database, not just that the build succeeds
- Don't commit connection strings or secrets

### License

TBD

---

## فارسی

<div dir="rtl">

پلتفرمی برای همکاری، که در آن توسعه‌دهنده‌ها ایده‌های پروژه‌شان را منتشر می‌کنند، تیم و مهارت‌های موردنیازشان را مشخص می‌کنند، و تیمشان را از میان کسانی می‌سازند که واقعاً آن مهارت‌ها را دارند.

بیشتر پروژه‌های جانبی به این دلیل شکست می‌خورند که آدم‌های مناسب هیچ‌وقت همدیگر را پیدا نمی‌کنند. CodeMate همین بخش را حل می‌کند: پروژه‌ات را ثبت می‌کنی، تیم‌ها و مهارت‌های لازمشان را مشخص می‌کنی، و کسانی که پروفایلشان می‌خواند می‌توانند درخواست عضویت بدهند. بعد از شکل‌گیری تیم، ادامه‌ی کار هم در همین پلتفرم انجام می‌شود — تسک‌ها، پیگیری پیشرفت، و نظارت.

### قابلیت‌ها

- **احراز هویت** — ثبت‌نام، ورود (JWT)، خروج، تغییر رمز عبور، بازیابی رمز عبور فراموش‌شده
- **پروفایل کاربری** — مشاهده و ویرایش پروفایل
- **مهارت‌ها** — فهرست مهارت‌های تحت نظارت ادمین؛ کاربران از همین فهرست انتخاب می‌کنند و سطح تسلط خود را مشخص می‌کنند
- **پروژه‌ها** — ایجاد، ویرایش، حذف، تغییر وضعیت؛ جست‌وجو و فهرست با فیلتر و صفحه‌بندی؛ کنترل مالکیت روی تمام عملیات نوشتن
- **تیم‌ها** — زیرتیم‌های درون هر پروژه (مثل Front-End و Back-End)؛ تعریف مهارت‌های موردنیاز هر پروژه؛ درخواست عضویت با بررسی صلاحیت بر اساس مهارت؛ مدیریت اعضا
- **تسک‌ها** — ایجاد، تخصیص به اعضای پروژه، گردش وضعیت `Todo → Doing → Done`
- **داشبورد** — آمار پیشرفت پروژه برای مالک (تعداد تسک‌ها، موارد عقب‌افتاده، عملکرد اعضا)، نمای کلی تسک‌های شخصی برای اعضا
- **پنل ادمین** — نظارت بر کاربران (جست‌وجو، غیرفعال/فعال‌سازی، ارتقا)، نظارت بر پروژه‌ها (تغییر اجباری وضعیت، حذف اجباری)، مدیریت فهرست مهارت‌ها

### تکنولوژی‌ها

- .NET / ASP.NET Core Web API
- Entity Framework Core + SQL Server
- احراز هویت با JWT و مجوزدهی بر اساس نقش
- FluentValidation و AutoMapper
- Swagger / OpenAPI

### معماری

CodeMate بر پایه‌ی Clean Architecture ساخته شده است. وابستگی‌ها فقط رو به داخل هستند و لایه‌ی Domain هیچ اطلاعی از دنیای بیرون ندارد.

</div>

```
CodeMate.Domain          موجودیت‌ها، Enumها و قواعد کسب‌وکار — بدون هیچ وابستگی
CodeMate.Application     Use Caseها، اینترفیس سرویس‌ها، Validatorها و Mappingها
CodeMate.Contracts       DTOهای Request/Response — قرارداد سمت کلاینت
CodeMate.Infrastructure  EF Core، ریپازیتوری‌ها، JWT، هش رمز عبور و Migrationها
CodeMate.API             کنترلرها، Middlewareها، DI، Swagger و CORS
CodeMate.Shared          Helperها، Exceptionها، صفحه‌بندی و Result Typeها
```

<div dir="rtl">

دو قرارداد مهم که پیش از مشارکت بهتر است بدانید:

- **لایه‌ی Contracts هیچ‌وقت به Domain رفرنس نمی‌دهد.** DTOها به‌جای Enumهای Domain از انواع پایه (`int`، `string`، `Guid`) استفاده می‌کنند تا تغییر در Domain باعث شکستن بی‌سروصدای API نشود.
- **Command و Query از هم جدا هستند.** عملیات نوشتن و خواندن در سرویس‌ها، ریپازیتوری‌ها و Partialهای جداگانه‌ی کنترلر قرار می‌گیرند (مثل `ProjectsController.Commands.cs` و `ProjectsController.Queries.cs`). این کار علاوه بر تفکیک مسئولیت‌ها، اجازه می‌دهد چند نفر هم‌زمان روی یک فیچر کار کنند بدون آنکه به کار یکدیگر بخورند.

### راه‌اندازی

**پیش‌نیازها**

- .NET SDK
- SQL Server (نسخه‌ی LocalDB، Express یا یک Instance کامل)

**مراحل**

</div>

```bash
git clone <repository-url>
cd CodeMate
dotnet restore
```

<div dir="rtl">

رشته‌ی اتصال (Connection String) را تنظیم کنید — از User Secrets یا `appsettings.Development.json` استفاده کنید، هرگز از فایل مشترک `appsettings.json`:

</div>

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=CodeMateDb;Trusted_Connection=True;TrustServerCertificate=True"
```

<div dir="rtl">

اگر LocalDB نصب نیست یا دستور `sqllocaldb info` شناخته نمی‌شود، مقدار `Server=` را به هر نمونه‌ی SQL Server در دسترس خود تغییر دهید — خود دیتابیس توسط Migrationها ساخته می‌شود.

سپس Migrationها را اعمال و پروژه را اجرا کنید:

</div>

```bash
dotnet ef database update
dotnet run --project CodeMate.API
```

<div dir="rtl">

پس از اجرا، رابط Swagger روی آدرس اصلی در دسترس است. در اولین اجرا، یک حساب Admin به‌صورت خودکار Seed می‌شود (برای اطلاعات ورود به بخش `SeedData` و تنظیمات `AdminSeed` مراجعه کنید).

### نمای کلی API

مرجع کامل و به‌روز، شامل ساختار درخواست‌ها و پاسخ‌ها، در Swagger موجود است. گروه‌بندی کلی Endpointها:

| بخش | مسیر پایه | نمونه‌ها |
|---|---|---|
| احراز هویت | `/api/auth` | register، login، logout، change-password، forgot-password، reset-password |
| کاربران | `/api/users` | `GET/PUT /me` |
| مهارت‌ها | `/api/skills` | جست‌وجوی Catalog، `GET/POST/DELETE /me` |
| پروژه‌ها | `/api/projects` | CRUD، جست‌وجو، `PATCH /{id}/status` |
| تیم‌ها | `/api/teams`، `/api/projects/{id}/teams` | CRUD، اعضا، درخواست عضویت (ارسال/قبول/رد) |
| تسک‌ها | `/api/tasks` | CRUD، `PATCH /{id}/assign`، `PATCH /{id}/status`، `/mine` |
| داشبورد | `/api/dashboard` | `/projects/{id}`، `/me` |
| ادمین | `/api/admin/users`، `/api/admin/projects`، `/api/admin/skills` | نظارت کاربران، عملیات اجباری روی پروژه‌ها، مدیریت فهرست مهارت‌ها — همه مخصوص نقش Admin |

### نقشه راه

- [x] احراز هویت (ثبت‌نام، ورود، خروج، بازیابی رمز عبور)
- [x] مدیریت کاربر (پروفایل و مهارت‌ها)
- [x] مدیریت پروژه (CRUD، جست‌وجو و وضعیت)
- [x] مدیریت تیم (تیم‌ها، درخواست عضویت و تطبیق مهارت)
- [x] مدیریت تسک (ایجاد، تخصیص و گردش وضعیت)
- [x] داشبورد (آمار پروژه و آمار شخصی)
- [x] پنل ادمین (نظارت بر کاربران/پروژه‌ها، فهرست مهارت‌ها)
- [ ] استفاده معنادار از نقش اعضای تیم (تفاوت Lead و Member)
- [ ] اتصال پروژه به ریپازیتوری GitHub
- [ ] اعلان ایمیلی (نتیجه‌ی درخواست عضویت، تخصیص تسک)
- [ ] خروج خودخواسته از تیم / انتقال مالکیت پروژه
- [ ] محدودسازی نرخ درخواست (Rate Limiting) روی Endpointهای احراز هویت
- [ ] رابط کاربری تحت وب

### مشارکت

از مشارکت استقبال می‌کنیم.

برنچ‌ها از `develop` گرفته می‌شوند، هر برنچ برای یک واحد کاری، با الگوی نام‌گذاری `feature/<scope>-<description>`. Pull Request روی `develop` باز شود؛ برنچ `main` فقط نسخه‌های پایدار را نگه می‌دارد.

پیش از باز کردن PR:

- ساختار لایه‌های موجود را رعایت کنید و از مرزهای معماری عبور نکنید
- Command و Query را در فایل‌های جدا و مطابق الگوی Partial Class موجود نگه دارید
- مطمئن شوید تغییراتتان روی یک دیتابیس واقعی اجرا می‌شود، نه صرفاً اینکه Build موفق است
- رشته‌ی اتصال یا هیچ اطلاعات محرمانه‌ای را Commit نکنید
وز

هنوز مشخص نشده است.

</div>
