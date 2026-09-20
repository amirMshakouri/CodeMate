# Architecture

**[English](#english) · [فارسی](#فارسی)**

---

## English

### Overview

CodeMate is built as a .NET solution following Clean Architecture. The dependency rule is strict: **source code dependencies point only inward.** The Domain layer knows nothing about Application, Infrastructure, or the API — it has no project references to any of them.

```
                    ┌─────────────────────────────┐
                    │   CodeMate.API              │  Presentation
                    │   Controllers, Middleware,  │
                    │   DI wiring, Swagger, CORS  │
                    └──────────────┬──────────────┘
                                   │ depends on
                    ┌──────────────▼──────────────┐
                    │   CodeMate.Application      │  Use cases
                    │   Services, interfaces,     │
                    │   validators, mappings      │
                    └──────────────┬──────────────┘
                                   │ depends on
                    ┌──────────────▼──────────────┐
                    │   CodeMate.Domain           │  Core
                    │   Entities, enums,          │
                    │   business rules            │
                    │   (no outward dependencies) │
                    └─────────────────────────────┘

    CodeMate.Infrastructure  →  implements Application's interfaces
    (EF Core, JWT, password hashing, repositories, migrations)

    CodeMate.Contracts       →  request/response DTOs, referenced by
    API and Application, never by Domain

    CodeMate.Shared          →  cross-cutting helpers (exceptions,
    pagination, result types), referenced by any layer
```

`Infrastructure` implements interfaces defined in `Application` (`IUserRepository`, `IJwtService`, `IPasswordHasher`, `IEmailService`, ...) and is wired up via dependency injection in `Program.cs`. `Application` never references `Infrastructure` directly — it only knows the interfaces.

### Two conventions specific to this codebase

These aren't standard Clean Architecture requirements — they're decisions made for this project, and PRs are expected to follow them.

**1. Contracts never reference Domain**

A DTO in `CodeMate.Contracts` uses primitives — `int`, `string`, `Guid` — never a Domain enum or entity type directly. For example, `AddUserSkillRequest.Level` is an `int`, not a `SkillLevel`; the conversion and validation (`Enum.IsDefined`) happens in the Application layer (validator and mapping profile), not in the contract itself.

This means a Domain change can't silently break the public API contract — the break would show up explicitly in a mapping profile instead.

**2. Commands and queries are separated**

Every feature that has both writes and reads splits them:

- Two service interfaces: `IXCommandService` (create/update/delete/state changes) and `IXQueryService` (search/get)
- Two controller files sharing one class via `partial`: `XController.Commands.cs` and `XController.Queries.cs`
- Repository methods for reads and writes, sometimes split the same way

This was originally adopted so multiple people could build the same feature in parallel without editing the same files — but it's kept because it also makes each service's job harder to accidentally conflate.

### Data model

```
User            Id, UserName, Email, PasswordHash, FullName, PhoneNumber, Bio,
                Role, IsActive, PasswordResetToken, PasswordResetTokenExpiresAt

Skill           Id, Name                              (admin-managed catalog)
UserSkill       Id, UserId, SkillId, Level, YearsOfExperience

Project         Id, OwnerId, Title, Description, Status
ProjectSkill    Id, ProjectId, SkillId, RequiredLevel, IsMandatory

Team            Id, ProjectId, Name, Description       (sub-team within a project)
TeamMember      Id, TeamId, UserId, Role, JoinedAt, IsActive
JoinRequest     Id, TeamId, UserId, Message, Status

TaskItem        Id, ProjectId, AssignedUserId, Title, Description,
                Status, Priority, DueDate
```

Key relationships:

- `User 1—* Project` (as owner)
- `Project 1—* Team 1—* TeamMember *—1 User` — a project's membership is entirely mediated by its teams; there's no separate "project member" table.
- `Team 1—* JoinRequest` — join requests target a specific team, not the project directly.
- `Project *—* Skill` via `ProjectSkill` (required skills) and `User *—* Skill` via `UserSkill` — join-request eligibility checks compare these two.
- `Project 1—* TaskItem`, and a task's `AssignedUserId` must be a project member.

Every entity inherits from `BaseEntity`, which provides `CreatedAt`, `UpdatedAt`, and `IsDeleted` — deletions across the codebase are soft deletes.

### Status enums

```
ProjectStatus       Draft, Open, InProgress, Completed, Archived
TaskStatus          Todo, Doing, Done          (moves forward only, no skipping)
TaskPriority        Low, Medium, High
JoinRequestStatus   Pending, Accepted, Rejected
SkillLevel          Beginner, Intermediate, Advanced, Expert
TeamMemberRole      Member, Lead               (currently assigned but not yet
                                                 enforced anywhere — see Roadmap)
UserRole            User, Admin
```

### Authorization model

Authorization is role-based, using ASP.NET Core's built-in `[Authorize(Roles = "Admin")]` — the JWT already carries a `ClaimTypes.Role` claim. `ICurrentUserService` exposes the current user's `Id` and `Role` to the Application layer without it ever touching `HttpContext` directly.

Ownership checks (e.g. "only the project owner can edit this project") live in the relevant command service — see `ProjectCommandService.EnsureOwnership`, which also contains the Admin bypass.

### Known architectural debt

- `AuthService` currently handles registration, login, logout, change password, and password reset in one class — a candidate for splitting the way Project/Team/Task services already are.
- `TeamMemberRole` (Member/Lead) is set on every accepted join request but never read anywhere.
- No automated tests exist yet; verification is manual by design, to prioritize delivery speed early on.

---

## فارسی

<div dir="rtl">

### نمای کلی

CodeMate یک Solution دات‌نتی است که بر پایه‌ی Clean Architecture ساخته شده. قانون وابستگی سخت‌گیرانه است: **وابستگی‌های کد فقط رو به داخل هستند.** لایه‌ی Domain هیچ اطلاعی از Application، Infrastructure یا API ندارد — هیچ رفرنس پروژه‌ای به هیچ‌کدام از آن‌ها ندارد.

</div>

```
                    ┌─────────────────────────────┐
                    │   CodeMate.API              │  لایه نمایش
                    │   کنترلرها، Middlewareها،   │
                    │   DI، Swagger، CORS         │
                    └──────────────┬──────────────┘
                                   │ وابسته به
                    ┌──────────────▼──────────────┐
                    │   CodeMate.Application      │  Use Caseها
                    │   سرویس‌ها، اینترفیس‌ها،     │
                    │   Validatorها، Mappingها    │
                    └──────────────┬──────────────┘
                                   │ وابسته به
                    ┌──────────────▼──────────────┐
                    │   CodeMate.Domain           │  هسته
                    │   موجودیت‌ها، Enumها،        │
                    │   قواعد کسب‌وکار             │
                    │   (بدون وابستگی رو به بیرون) │
                    └─────────────────────────────┘

    CodeMate.Infrastructure  ←  پیاده‌سازی اینترفیس‌های Application
    (EF Core، JWT، هش رمز عبور، ریپازیتوری‌ها، Migrationها)

    CodeMate.Contracts       ←  DTOهای Request/Response، مورد استفاده‌ی
    API و Application، هرگز Domain

    CodeMate.Shared          ←  Helperهای مشترک (Exceptionها،
    صفحه‌بندی، Result Typeها)، مورد استفاده‌ی هر لایه
```

<div dir="rtl">

`Infrastructure` اینترفیس‌های تعریف‌شده در `Application` (`IUserRepository`, `IJwtService`, `IPasswordHasher`, `IEmailService` و ...) را پیاده‌سازی می‌کند و از طریق Dependency Injection در `Program.cs` متصل می‌شود. `Application` هرگز مستقیماً به `Infrastructure` رفرنس نمی‌دهد — فقط اینترفیس‌ها را می‌شناسد.

### دو قرارداد مخصوص این کدبیس

این‌ها الزامات استاندارد Clean Architecture نیستند — تصمیماتی هستند که برای این پروژه گرفته شده‌اند، و از PRها انتظار می‌رود از آن‌ها پیروی کنند.

**۱. Contracts هرگز به Domain رفرنس نمی‌دهد**

یک DTO در `CodeMate.Contracts` از انواع پایه — `int`، `string`، `Guid` — استفاده می‌کند، هرگز مستقیماً از یک Enum یا Entity Domain. برای مثال، `AddUserSkillRequest.Level` یک `int` است، نه `SkillLevel`؛ تبدیل و اعتبارسنجی (`Enum.IsDefined`) در لایه‌ی Application (Validator و Mapping Profile) انجام می‌شود، نه در خود Contract.

این یعنی یک تغییر در Domain نمی‌تواند به‌طور بی‌سروصدا قرارداد عمومی API را بشکند — شکست به‌جایش به‌طور صریح در یک Mapping Profile ظاهر می‌شود.

**۲. Command و Query از هم جدا هستند**

هر فیچری که هم نوشتن دارد هم خواندن، این دو را جدا می‌کند:

- دو اینترفیس سرویس: `IXCommandService` (ایجاد/ویرایش/حذف/تغییر وضعیت) و `IXQueryService` (جست‌وجو/دریافت)
- دو فایل کنترلر که با `partial` یک کلاس را مشترک هستند: `XController.Commands.cs` و `XController.Queries.cs`
- متدهای Repository برای خواندن و نوشتن، گاهی به همین شکل جدا شده

این الگو در ابتدا برای این اتخاذ شد که چند نفر بتوانند هم‌زمان روی یک فیچر کار کنند بدون این‌که فایل مشترکی را ویرایش کنند — ولی نگه داشته شده چون قاطی‌شدن اشتباهی مسئولیت‌های هر سرویس را هم سخت‌تر می‌کند.

### مدل داده

</div>

```
User            Id, UserName, Email, PasswordHash, FullName, PhoneNumber, Bio,
                Role, IsActive, PasswordResetToken, PasswordResetTokenExpiresAt

Skill           Id, Name                              (فهرست تحت نظارت Admin)
UserSkill       Id, UserId, SkillId, Level, YearsOfExperience

Project         Id, OwnerId, Title, Description, Status
ProjectSkill    Id, ProjectId, SkillId, RequiredLevel, IsMandatory

Team            Id, ProjectId, Name, Description       (زیرتیم درون یک پروژه)
TeamMember      Id, TeamId, UserId, Role, JoinedAt, IsActive
JoinRequest     Id, TeamId, UserId, Message, Status

TaskItem        Id, ProjectId, AssignedUserId, Title, Description,
                Status, Priority, DueDate
```

<div dir="rtl">

روابط کلیدی:

- `User 1—* Project` (به‌عنوان مالک)
- `Project 1—* Team 1—* TeamMember *—1 User` — عضویت در یک پروژه کاملاً از طریق تیم‌هایش واسطه‌گری می‌شود؛ جدول جداگانه‌ای برای «عضو پروژه» وجود ندارد.
- `Team 1—* JoinRequest` — درخواست‌های عضویت یک تیم مشخص را هدف می‌گیرند، نه مستقیماً خودِ پروژه را.
- `Project *—* Skill` از طریق `ProjectSkill` (مهارت‌های موردنیاز) و `User *—* Skill` از طریق `UserSkill` — بررسی صلاحیت درخواست عضویت این دو را با هم مقایسه می‌کند.
- `Project 1—* TaskItem`، و `AssignedUserId` یک Task باید عضو پروژه باشد.

تمام Entityها از `BaseEntity` ارث می‌برند که `CreatedAt`، `UpdatedAt` و `IsDeleted` را فراهم می‌کند — حذف‌ها در سراسر کدبیس Soft Delete هستند.

### Enumهای وضعیت

</div>

```
ProjectStatus       Draft, Open, InProgress, Completed, Archived
TaskStatus          Todo, Doing, Done          (فقط رو به جلو، بدون پرش)
TaskPriority        Low, Medium, High
JoinRequestStatus   Pending, Accepted, Rejected
SkillLevel          Beginner, Intermediate, Advanced, Expert
TeamMemberRole      Member, Lead               (فعلاً مقداردهی می‌شود ولی
                                                 هیچ‌جا اجرا نمی‌شود — نقشه راه)
UserRole            User, Admin
```

<div dir="rtl">

### مدل مجوزدهی

مجوزدهی بر اساس نقش است، با استفاده از `[Authorize(Roles = "Admin")]` داخلی ASP.NET Core — JWT از قبل یک Claim از نوع `ClaimTypes.Role` حمل می‌کند. `ICurrentUserService`، شناسه و نقش کاربر فعلی را بدون این‌که هرگز مستقیماً با `HttpContext` کار کند، در اختیار لایه‌ی Application قرار می‌دهد.

چک‌های مالکیت (مثلاً «فقط مالک پروژه می‌تواند آن را ویرایش کند») در سرویس Command مربوطه قرار دارند — به `ProjectCommandService.EnsureOwnership` مراجعه کنید که شامل عبور Admin هم هست.

### بدهی معماری شناخته‌شده

- `AuthService` در حال حاضر ثبت‌نام، ورود، خروج، تغییر رمز عبور و بازیابی رمز عبور را در یک کلاس واحد مدیریت می‌کند — کاندیدی برای تفکیک، به همان شکلی که سرویس‌های Project/Team/Task قبلاً تفکیک شده‌اند.
- `TeamMemberRole` (Member/Lead) روی هر درخواست عضویت پذیرفته‌شده مقداردهی می‌شود، ولی هیچ‌جا خوانده نمی‌شود.
- هنوز مجموعه‌ی تست خودکاری وجود ندارد؛ اعتبارسنجی عمداً دستی انجام می‌شود، برای اولویت‌دادن به سرعت تحویل در مراحل اولیه.

</div>