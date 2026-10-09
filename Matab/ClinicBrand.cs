using System;

namespace Matab
{
    /// <summary>Centralized bilingual branding; does not alter patient or clinic database values.</summary>
    internal static class ClinicBrand
    {
        internal const string ProductName = "Ashkan Matab";
        internal const string DoctorNameFa = "دکتر اشکان مطاعی";
        internal const string DoctorNameEn = "dr.ashkan motaei";
        internal static string DoctorName { get { return ClinicLanguage.IsEnglish ? DoctorNameEn : DoctorNameFa; } }
        internal static string AppTitle { get { return ClinicLanguage.IsEnglish ? "Ashkan Matab | Clinic Management" : "اشکان مطب | مدیریت مطب"; } }
        internal static string DashboardTitle { get { return ClinicLanguage.IsEnglish ? "Ashkan Matab | Dashboard" : "اشکان مطب | داشبورد"; } }
    }
}
