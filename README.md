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
