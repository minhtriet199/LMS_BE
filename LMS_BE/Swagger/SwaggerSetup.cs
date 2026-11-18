using System.Text.Json.Nodes;
using LMS_BE.Data;
using LMS_BE.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace LMS_BE.Swagger;

// Swagger UI = trang web liệt kê mọi API và cho bấm gọi thử ngay trên trình duyệt: http://localhost:5000/swagger
// Chỉ bật khi chạy Development (máy dev) — xem Program.cs.
public static class SwaggerSetup
{
    private const string BearerScheme = "Bearer";

    // Tạo tài liệu OpenAPI (/openapi/v1.json). Swagger UI đọc file này để vẽ giao diện.
    public static IServiceCollection AddLmsSwagger(this IServiceCollection services) =>
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = "LMS_BE API";
                document.Info.Description = """
                    Gọi thử API ngay trên trang này:

                    1. Mở **POST /api/auth/login** → bấm **Execute** (đã điền sẵn tài khoản sinh viên demo).
                    2. Token tự được gắn vào nút **Authorize** → các API có hình ổ khóa giờ gọi được.
                    3. Token hết hạn sau 15 phút → bấm Execute ở login lần nữa.
                    """;

                // Khai báo kiểu đăng nhập JWT → Swagger hiện nút "Authorize" để dán token.
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[BearerScheme] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Dán token lấy từ POST /api/auth/login (chỉ dán token, không gõ chữ Bearer).",
                };
                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, _) =>
            {
                // API có [Authorize] → Swagger hiện ổ khóa và tự gửi kèm token đã nhập ở nút Authorize.
                var metadata = context.Description.ActionDescriptor.EndpointMetadata;
                var authorize = metadata.OfType<IAuthorizeData>().ToList();
                if (authorize.Count > 0 && !metadata.OfType<IAllowAnonymous>().Any())
                {
                    operation.Security ??= [];
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(BearerScheme, context.Document)] = [],
                    });

                    operation.Responses ??= [];
                    operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Chưa đăng nhập, hoặc token đã hết hạn" });
                    if (authorize.Any(a => !string.IsNullOrEmpty(a.Roles) || !string.IsNullOrEmpty(a.Policy)))
                        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Đã đăng nhập nhưng không đúng vai trò (role)" });
                }

                // Điền sẵn tài khoản demo vào ô body của API đăng nhập → chỉ cần bấm Execute.
                if (context.Description.ParameterDescriptions.Any(p => p.Type == typeof(LoginRequest))
                    && operation.RequestBody?.Content is { } content)
                {
                    foreach (var mediaType in content.Values)
                        mediaType.Example = new JsonObject
                        {
                            ["email"] = "student@lms.dev",
                            ["password"] = DbSeeder.DefaultPassword,
                        };
                }
                return Task.CompletedTask;
            });
        });

    public static WebApplication UseLmsSwagger(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "LMS_BE v1");
            options.DocumentTitle = "LMS_BE API";
            options.EnableTryItOutByDefault();    // không cần bấm "Try it out" trước khi Execute
            options.EnablePersistAuthorization(); // tải lại trang (F5) vẫn giữ token
            options.DisplayRequestDuration();
            options.UseRequestInterceptor(AddCsrfHeader);
            options.UseResponseInterceptor(AutoAuthorize);
        });
        return app;
    }

    // Các API /api/auth bắt buộc header chống CSRF (xem AuthController) → Swagger tự gắn vào mọi request.
    private const string AddCsrfHeader = """
        function (request) {
            request.headers = request.headers || {};
            request.headers['X-Requested-With'] = 'XMLHttpRequest';
            return request;
        }
        """;

    // Đăng nhập / refresh thành công → tự gắn token vào nút Authorize. Đăng xuất → gỡ token.
    private const string AutoAuthorize = """
        function (response) {
            try {
                var url = (response.url || '').split('?')[0];
                if (response.ok && response.body && response.body.token && /\/api\/auth\/(login|refresh)$/.test(url)) {
                    window.ui.preauthorizeApiKey('Bearer', response.body.token);
                    window.ui.authActions.persistAuthorizationIfNeeded();
                }
                if (response.ok && /\/api\/auth\/logout$/.test(url)) {
                    window.ui.authActions.logoutWithPersistOption(['Bearer']);
                }
            } catch (e) {
                console.warn('Swagger: không tự gắn được token, hãy dán tay vào nút Authorize', e);
            }
            return response;
        }
        """;
}
