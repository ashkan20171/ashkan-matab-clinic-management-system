# راهنمای رفع خطاهای Matab در Visual Studio 2022

## خطای MSBuild در فایل‌های resx
این خطا به Mark of the Web مربوط است و لزوماً به محتوای RESX مربوط نیست.

1. قبل از استخراج، در Properties فایل ZIP و در صورت وجود گزینه Unblock را انتخاب کنید.
2. فقط اگر به سورس اعتماد دارید، PowerShell را در ریشه پروژه باز کنید و دستور زیر را اجرا کنید:
   `powershell -ExecutionPolicy Bypass -File .\Unblock-Matab.ps1`
3. Visual Studio را باز کنید و Clean Solution و سپس Rebuild Solution را اجرا کنید.

## هشدار Missing Stimulsoft
در فایل ZIP اولیه هیچ‌کدام از شش اسمبلی Stimulsoft وجود ندارد. فایل csproj به نسخه 2010.1.700.0 اشاره می‌کند و DLLها را در `Matab\lib` انتظار دارد. باید اسمبلی‌های قانونی و سازگار با کد پروژه در آن پوشه قرار بگیرند. نسخه‌های جدیدتر لزوماً سازگار نیستند.

این بسته مشکل Mark of the Web را با اسکریپت محلی رفع می‌کند اما ادعای رفع هشدار Stimulsoft یا Build موفق ندارد.
