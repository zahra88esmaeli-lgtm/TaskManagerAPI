# TaskManagerAPI

یک Web API برای مدیریت تسک‌ها با ASP.NET Core (.NET 10). هر کاربر ثبت‌نام می‌کند، وارد می‌شود و فقط تسک‌های **خودش** را می‌بیند.

[English README](README.md)

## امکانات

- ثبت‌نام و ورود کاربر با احراز هویت **JWT**
- هش کردن رمز عبور
- تغییر رمز عبور و حذف حساب کاربری
- ساخت، مشاهده، ویرایش و حذف تسک (CRUD)
- هر کاربر فقط به تسک‌های خودش دسترسی دارد
- ذخیره‌سازی در **SQL Server (LocalDB)** با Entity Framework Core
- الگوی Repository و لایه‌ی Service

## تکنولوژی‌ها

ASP.NET Core Web API، C#، Entity Framework Core، SQL Server LocalDB، JWT Bearer

## نحوه‌ی اجرا

**پیش‌نیازها:** .NET 10 SDK و SQL Server LocalDB (همراه Visual Studio نصب می‌شود)

```
git clone https://github.com/zahra88esmaeli-lgtm/TaskManagerAPI.git
cd TaskManagerAPI/TaskManagerAPI
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "یک-متن-تصادفی-حداقل-۳۲-کاراکتر"
dotnet tool install --global dotnet-ef
dotnet ef database update
dotnet run
```

نکته‌ها:

- کلید `Jwt:Key` باید حداقل **۳۲ کاراکتر** باشد، وگرنه Login خطای ۵۰۰ می‌دهد.
- کلید واقعی را هرگز در GitHub نگذار. از User Secrets استفاده کن.
- رشته‌ی اتصال دیتابیس در `appsettings.json` است. در صورت نیاز آن را برای سیستم خودت تغییر بده.
- آدرس API معمولاً `http://localhost:5280` است (در خروجی `dotnet run` ببین).
- مشخصات endpointها در `http://localhost:5280/openapi/v1.json` هست.

## Endpointهای اصلی

| متد | مسیر | توضیح |
|-----|------|-------|
| POST | `/api/User/register` | ثبت‌نام |
| POST | `/api/User/login` | ورود و دریافت توکن JWT |
| GET / POST | `/api/TaskItem` | لیست تسک‌ها و ساخت تسک (نیاز به توکن) |

برای endpointهای محافظت‌شده، توکن را در هدر بفرست:

```
Authorization: Bearer <token>
```

## رابط وب

این API یک رابط وب دارد: [TaskManagerWeb](https://github.com/zahra88esmaeli-lgtm/TaskManagerWeb). اول API و بعد Web را اجرا کن.