# LMS_BE — Backend ASP.NET Core (.NET 10)

> **Đây là hướng dẫn cài đặt đầy đủ cho cả dự án** (Database + Backend + Frontend).
> Làm lần lượt từ Bước 0 đến Bước 6, không bỏ bước. Mất khoảng 30–45 phút lần đầu.

Dự án gồm 3 repo:

| Repo | Là gì | Mở bằng | Nhánh |
|---|---|---|---|
| [LMS_DB](https://github.com/minhtriet199/LMS_DB) | Script SQL + sơ đồ ERD | pgAdmin | `main` |
| [LMS_BE](https://github.com/minhtriet199/LMS_BE) | API .NET (repo này) | Visual Studio 2026 | `develop` |
| [LMS_FE](https://github.com/minhtriet199/LMS_FE) | Web React | VS Code | `develop` |

## Các phần nối với nhau thế nào

```
Trình duyệt ──> http://localhost:5173  (React / Vite — LMS_FE)
                     │  mọi request bắt đầu bằng /api được Vite "proxy" chuyển tiếp
                     ▼
               http://localhost:5000  (ASP.NET Core — LMS_BE)
                     │  chuỗi kết nối (connection string) trong User Secrets
                     ▼
               localhost:5432         (PostgreSQL 18 — database LMS_DB)
```

| Cổng | Cấu hình ở đâu | Đổi cổng thì phải sửa thêm |
|---|---|---|
| **5173** (web) | `LMS_FE/LMS_FE/vite.config.ts` (`port`) | — |
| **5000** (API) | `LMS_BE/LMS_BE/Properties/launchSettings.json` (`applicationUrl`) | `VITE_BACKEND_URL` trong `LMS_FE/LMS_FE/.env` |
| **5432** (DB) | Lúc cài PostgreSQL | `Port=` trong connection string (Bước 3) |

Nhờ proxy của Vite, trình duyệt chỉ nói chuyện với `:5173`, nên **không cần cấu hình CORS** khi dev.

---

## Bước 0 — Cài phần mềm (1 lần)

| Phần mềm | Ghi chú |
|---|---|
| [Git](https://git-scm.com/downloads) | Cài mặc định, Next liên tục |
| [Visual Studio 2026](https://visualstudio.microsoft.com/) | Khi cài, tick workload **ASP.NET and web development** (đã kèm .NET 10 SDK) |
| [VS Code](https://code.visualstudio.com/) | Để chạy Frontend |
| [Node.js LTS](https://nodejs.org/) | Bản **22 trở lên**. Cài xong mở lại VS Code |
| [PostgreSQL 18](https://www.postgresql.org/download/windows/) | Kèm **pgAdmin 4**. **Ghi nhớ mật khẩu** đặt cho user `postgres`. Cổng để mặc định `5432` |

Kiểm tra: mở **Command Prompt** (phím Windows → gõ `cmd`) và chạy từng dòng — dòng nào báo lỗi thì cài lại phần đó:

```bat
git --version
dotnet --version     :: phải là 10.x
node --version       :: phải là v22 trở lên
```

## Bước 1 — Tải code về (1 lần)

Mở **Command Prompt**, chạy:

```bat
mkdir C:\Code\LMS
cd C:\Code\LMS
git clone https://github.com/minhtriet199/LMS_DB.git
git clone -b develop https://github.com/minhtriet199/LMS_BE.git
git clone -b develop https://github.com/minhtriet199/LMS_FE.git
```

Kết quả: thư mục `C:\Code\LMS` có 3 thư mục con `LMS_DB`, `LMS_BE`, `LMS_FE`.

## Bước 2 — Tạo database bằng pgAdmin (1 lần)

1. Mở **pgAdmin 4** → bên trái bấm **Servers** → **PostgreSQL 18** → nhập mật khẩu `postgres`.
2. Chuột phải **Databases** → **Create** → **Database...**
3. Ô **Database** gõ đúng: `LMS_DB` → bấm **Save**.

Chỉ cần tạo database rỗng. **Bảng sẽ do backend tự tạo** ở Bước 4 — không cần chạy SQL nào.

## Bước 3 — Khai báo mật khẩu DB cho backend (1 lần)

Mật khẩu **không** được ghi vào code (để không lộ lên GitHub). Mỗi người lưu trong **User Secrets** trên máy mình:

1. Mở Visual Studio → **Open a project or solution** → chọn `C:\Code\LMS\LMS_BE\LMS_BE.slnx`.
2. Trong **Solution Explorer** (bên phải), chuột phải project **LMS_BE** → **Manage User Secrets**.
3. Một file `secrets.json` mở ra. Xóa hết nội dung, dán đoạn sau và **thay `MAT_KHAU_CUA_BAN`** bằng mật khẩu postgres của bạn:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LMS_DB;Username=postgres;Password=MAT_KHAU_CUA_BAN"
  },
  "Jwt": {
    "Key": "doi-chuoi-nay-thanh-chuoi-ngau-nhien-dai-it-nhat-32-ky-tu"
  }
}
```

4. Bấm **Ctrl+S** để lưu.

> Không có Visual Studio? Dùng terminal trong thư mục `LMS_BE/LMS_BE`:
> ```bat
> dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=LMS_DB;Username=postgres;Password=MAT_KHAU_CUA_BAN"
> dotnet user-secrets set "Jwt:Key" "doi-chuoi-nay-thanh-chuoi-ngau-nhien-dai-it-nhat-32-ky-tu"
> ```

## Bước 4 — Chạy Backend

1. Trên thanh công cụ Visual Studio, ô cạnh nút ▶ xanh chọn profile **http**.
2. Bấm ▶ (hoặc **F5**). Cửa sổ Output/console hiện dòng `Now listening on: http://localhost:5000` là được.
3. Mở trình duyệt vào **http://localhost:5000/api/health** → phải thấy:
   ```json
   {"api":"ok","database":"ok"}
   ```

Lần chạy đầu tiên backend sẽ **tự tạo bảng** trong LMS_DB và **tự tạo 3 tài khoản demo**.
Vào pgAdmin → LMS_DB → chuột phải → **Refresh** → **Schemas › public › Tables** sẽ thấy `Users`, `Roles`, `UserRoles`, `RefreshTokens`...

> Lần đầu console có dòng đỏ `fail: ... __EFMigrationsHistory` — **bình thường**, do EF kiểm tra bảng lịch sử trước khi tạo nó.

Để backend **chạy nguyên đó** khi làm Bước 5.

## Bước 5 — Chạy Frontend

1. Mở **VS Code** → **File › Open Folder...** → chọn `C:\Code\LMS\LMS_FE\LMS_FE` (thư mục con có file `package.json`).
2. Menu **Terminal › New Terminal**, chạy:
   ```bat
   npm install
   npm run dev
   ```
3. Mở **http://localhost:5173**.

## Bước 6 — Đăng nhập thử

| Email | Mật khẩu | Vai trò |
|---|---|---|
| `student@lms.dev` | `Lms@12345` | Sinh viên |
| `teacher@lms.dev` | `Lms@12345` | Giảng viên |
| `admin@lms.dev` | `Lms@12345` | Quản trị |

Đăng nhập xong sẽ vào trang **Trang chủ** (dashboard đang để trống). Bấm F5 vẫn giữ đăng nhập; bấm **Đăng xuất** quay về trang login.

---

## Hằng ngày làm việc

1. Đảm bảo PostgreSQL đang chạy (cài xong nó tự chạy cùng Windows).
2. Visual Studio → mở `LMS_BE.slnx` → ▶.
3. VS Code → `LMS_FE/LMS_FE` → `npm run dev`.
4. Trước khi code: `git pull`. Code xong: commit & push lên nhánh `develop`.

## Gặp lỗi?

| Triệu chứng | Cách sửa |
|---|---|
| Backend tắt ngay, báo `Thiếu Jwt:Key` | Chưa làm Bước 3, hoặc chưa lưu `secrets.json` |
| `password authentication failed for user "postgres"` | Sai mật khẩu trong User Secrets (Bước 3) |
| `Failed to connect to 127.0.0.1:5432` / `Connection refused` | PostgreSQL chưa chạy: phím Windows → **Services** → `postgresql-x64-18` → **Start** |
| `database "LMS_DB" does not exist` | Chưa làm Bước 2 (tên phải đúng `LMS_DB`, phân biệt hoa thường) |
| `/api/health` trả `"database":"error"` | Như 3 dòng trên — kiểm tra mật khẩu, service, tên DB |
| Web báo "Email hoặc mật khẩu không đúng" dù nhập đúng | Backend chưa chạy → mở http://localhost:5000/api/health kiểm tra |
| `address already in use` / cổng 5000 bận | Đang chạy 1 backend khác. Tắt nó, hoặc xem ai giữ cổng: `netstat -ano \| findstr :5000` |
| `'npm' is not recognized` | Chưa cài Node.js, hoặc cài xong chưa mở lại VS Code |
| `running scripts is disabled on this system` khi chạy npm | Terminal PowerShell chặn script. Trong VS Code bấm mũi tên cạnh dấu **+** của terminal → chọn **Command Prompt**, chạy lại |
| `Port 5173 is already in use` | Đang có 1 terminal `npm run dev` khác — tắt nó (Ctrl+C) |
| Muốn làm lại DB từ đầu | pgAdmin: chạy `LMS_DB/scripts/99_reset_database.sql` trên LMS_DB → chạy lại backend |

---

## Cấu trúc code

```
LMS_BE/
├─ LMS_BE.slnx                 → mở file này bằng Visual Studio
└─ LMS_BE/
   ├─ Program.cs               → cấu hình DB, Identity, JWT; tự migrate + seed khi Development
   ├─ Controllers/             → AuthController (login/refresh/logout), HealthController
   ├─ Data/                    → ApplicationDbContext, DbSeeder (tài khoản demo)
   ├─ Models/                  → ApplicationUser, ApplicationRole, RefreshToken
   ├─ Dtos/                    → dữ liệu gửi/nhận qua API
   ├─ Services/                → JwtTokenService, RefreshTokenService, Roles
   ├─ Migrations/              → EF Core migrations (lịch sử thay đổi bảng)
   ├─ Properties/launchSettings.json → cổng 5000
   └─ LMS_BE.http              → gọi thử API ngay trong Visual Studio
```

## API hiện có

| Method | URL | Mô tả |
|---|---|---|
| POST | `/api/auth/login` | Body `{ email, password }` → `{ token, user: { id, name, email, role } }` + cookie refresh |
| POST | `/api/auth/refresh` | Đổi cookie refresh lấy access token mới (giữ đăng nhập khi F5) |
| POST | `/api/auth/logout` | Thu hồi refresh token |
| GET | `/api/health` | Kiểm tra API + kết nối DB |

Các API `auth` bắt buộc header `X-Requested-With: XMLHttpRequest` (chống CSRF). Frontend đã gắn sẵn.
Tài liệu OpenAPI (Development): http://localhost:5000/openapi/v1.json

## Thêm 1 API mới (ví dụ chạy được, theo CQRS)

Quy ước của dự án: **Controller chỉ nhận request → gọi Handler → trả kết quả.** Mọi xử lý nằm trong Handler.

- **Query** = chỉ đọc dữ liệu (GET). **Command** = thay đổi dữ liệu (POST / PUT / DELETE).
- Mỗi tính năng là 1 thư mục trong `Features/`, ví dụ `Features/Me/`.
- Handler đăng ký **Singleton** (tạo 1 lần, dùng chung). Vì database (`DbContext`, `UserManager`) là *Scoped*, handler **không được inject trực tiếp** chúng — thay vào đó inject `IServiceScopeFactory` và tạo scope bên trong hàm (xem code mẫu). Làm sai chỗ này backend sẽ báo lỗi `Cannot consume scoped service ...`.

### Làm 1 lần: tạo 2 file khung

Trong Visual Studio: chuột phải project **LMS_BE** → **Add** → **New Folder** đặt tên `Cqrs`, rồi chuột phải thư mục đó → **Add** → **Class...** đặt tên `Abstractions.cs`. Xóa hết nội dung, dán:

```csharp
namespace LMS_BE.Cqrs;

public interface IQuery<TResult>;
public interface ICommand<TResult>;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}

public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}
```

### Ví dụ 1 — API đọc dữ liệu: `GET /api/me` (Query)

**Bước 1.** Tạo thư mục `Features/Me`, thêm class `GetMe.cs`, dán:

```csharp
using LMS_BE.Cqrs;
using LMS_BE.Dtos;
using LMS_BE.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS_BE.Features.Me;

public record GetMeQuery(Guid UserId, string Role) : IQuery<UserDto?>;

public class GetMeHandler(IServiceScopeFactory scopeFactory) : IQueryHandler<GetMeQuery, UserDto?>
{
    public async Task<UserDto?> HandleAsync(GetMeQuery query, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        return user is null ? null : new UserDto(user.Id, user.FullName, user.Email!, query.Role);
    }
}
```

**Bước 2.** Trong `Controllers/`, thêm class `MeController.cs`, dán:

```csharp
using System.Security.Claims;
using LMS_BE.Cqrs;
using LMS_BE.Dtos;
using LMS_BE.Features.Me;
using LMS_BE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS_BE.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController(IQueryHandler<GetMeQuery, UserDto?> getMe) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserDto>> Get(CancellationToken ct)
    {
        var role = User.FindFirstValue(LmsClaims.Role) ?? string.Empty;
        var result = await getMe.HandleAsync(new GetMeQuery(User.GetUserId(), role), ct);
        return result is null ? NotFound() : result;
    }
}
```

**Bước 3.** Mở `Program.cs`. Thêm 3 dòng `using` lên đầu file:

```csharp
using LMS_BE.Cqrs;
using LMS_BE.Dtos;
using LMS_BE.Features.Me;
```

Rồi thêm dòng đăng ký ngay dưới dòng `builder.Services.AddScoped<RefreshTokenService>();`:

```csharp
builder.Services.AddSingleton<IQueryHandler<GetMeQuery, UserDto?>, GetMeHandler>();
```

**Bước 4.** Bấm ▶ (F5) chạy lại backend.

**Bước 5 — Thử.** Mở file `LMS_BE.http` trong Visual Studio:

1. Bấm **Send request** trên dòng `POST .../api/auth/login` → trong kết quả sao chép giá trị của `token`.
2. Thêm vào cuối file (dán token thật thay `DAN_TOKEN_VAO_DAY`):

```http
GET {{LMS_BE_HostAddress}}/api/me
Authorization: Bearer DAN_TOKEN_VAO_DAY

###
```

3. Bấm **Send request** trên dòng `GET .../api/me` → thấy `200 OK` cùng thông tin người dùng là xong. Không có dòng `Authorization` sẽ ra `401` (đúng như mong muốn).

### Ví dụ 2 — API thay đổi dữ liệu: `PUT /api/me/name` (Command)

Làm tương tự, 4 file/chỗ cần sửa:

**`Features/Me/UpdateMyName.cs`**

```csharp
using LMS_BE.Cqrs;
using LMS_BE.Models;
using Microsoft.AspNetCore.Identity;

namespace LMS_BE.Features.Me;

public record UpdateMyNameCommand(Guid UserId, string FullName) : ICommand<bool>;

public class UpdateMyNameHandler(IServiceScopeFactory scopeFactory) : ICommandHandler<UpdateMyNameCommand, bool>
{
    public async Task<bool> HandleAsync(UpdateMyNameCommand command, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null) return false;

        user.FullName = command.FullName;
        return (await userManager.UpdateAsync(user)).Succeeded;
    }
}
```

**`Dtos/Dtos.cs`** — thêm 1 dòng ở cuối file:

```csharp
public record UpdateNameRequest(string FullName);
```

**`Controllers/MeController.cs`** — thêm `ICommandHandler<UpdateMyNameCommand, bool> updateName` vào constructor và thêm hàm:

```csharp
public class MeController(
    IQueryHandler<GetMeQuery, UserDto?> getMe,
    ICommandHandler<UpdateMyNameCommand, bool> updateName) : ControllerBase
{
    // ... hàm Get ở trên giữ nguyên ...

    [HttpPut("name")]
    public async Task<IActionResult> UpdateName(UpdateNameRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.FullName)) return BadRequest();

        var ok = await updateName.HandleAsync(new UpdateMyNameCommand(User.GetUserId(), request.FullName.Trim()), ct);
        return ok ? NoContent() : NotFound();
    }
}
```

**`Program.cs`** — thêm dòng đăng ký:

```csharp
builder.Services.AddSingleton<ICommandHandler<UpdateMyNameCommand, bool>, UpdateMyNameHandler>();
```

Thử trong `LMS_BE.http`: `PUT {{LMS_BE_HostAddress}}/api/me/name` với header `Authorization`, `Content-Type: application/json` và body `{ "fullName": "Tên Mới" }` → `204 No Content`. Gọi lại `GET /api/me` sẽ thấy tên đã đổi.

### Giới hạn theo vai trò (role)

Thêm lên trên hàm hoặc class trong Controller:

```csharp
[Authorize(Roles = $"{Roles.Teacher},{Roles.Admin}")]
```

Sai role sẽ trả `403`. Dùng `Roles.Student` / `Roles.Teacher` / `Roles.Admin` từ `Services/Roles.cs`.

### Gọi API từ Frontend

Trong `LMS_FE/LMS_FE/src/services/`, tạo `meService.ts` (không gọi axios trực tiếp trong component):

```ts
import api from '../api/axios'
import type { User } from '../types'

async function getMe(): Promise<User> {
  const { data } = await api.get<User>('/api/me')
  return data
}

export const meService = { getMe }
```

Token và header chống CSRF đã được `src/api/axios.ts` tự gắn. Thêm trang mới xem README của LMS_FE, mục **Thêm 1 trang mới**.

### Checklist khi thêm 1 API

1. `Features/<Tên>/<HanhDong>.cs` — record Query/Command + Handler (Singleton, dùng `IServiceScopeFactory`).
2. `Controllers/<Tên>Controller.cs` — inject handler, gọi `HandleAsync`.
3. `Program.cs` — thêm `AddSingleton<IQueryHandler<...>, ...>` (hoặc `ICommandHandler`).
4. Cần bảng mới? Xem mục **Thêm bảng mới** bên dưới (EF migration).
5. Chạy lại ▶ → thử trong `LMS_BE.http`.
6. Cập nhật bảng **API hiện có** ở trên.

## Thêm bảng mới (cho dev)

Bảng **luôn** tạo bằng EF Core migration, không tạo tay trong pgAdmin:

1. Tạo class trong `Models/`, thêm `DbSet<...>` vào `Data/ApplicationDbContext.cs`.
2. Cài công cụ (1 lần): `dotnet tool install --global dotnet-ef`
3. Trong thư mục `LMS_BE/LMS_BE` (Visual Studio: **View › Terminal**):
   ```bat
   dotnet ef migrations add TenThayDoi
   ```
4. Chạy lại backend → bảng tự được tạo trong LMS_DB.
5. Cập nhật script tham khảo bên repo LMS_DB:
   ```bat
   dotnet ef migrations script --idempotent -o ..\..\LMS_DB\scripts\01_schema_from_migrations.sql
   ```

Khóa của user là **Guid (uuid)** — cột trỏ tới user ở bảng mới phải là `Guid`. Sơ đồ đích: [LMS_DB/docs/ERD.md](https://github.com/minhtriet199/LMS_DB/blob/main/docs/ERD.md).
