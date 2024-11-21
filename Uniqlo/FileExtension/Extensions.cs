using Uniqlo.ViewModels.Sliders;

namespace Uniqlo.FileExtension
{
    public static class Extensions
    {
        public static bool IsValidType(this string contentType)
        {
            if (contentType.StartsWith("image"))
            {
                return true;
            }
            return false;
        }
        public static bool IsValidSize(this long kb)
        {
            if (kb > 2 * 1024 * 1024)
            {
                return true;
            }
            return false;
        }
        public static string Upload(string path, SliderCreateVM vm)
        {
            return Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);
        }
    }
}
/*
string Upload(string path) methodları olsun. 
IsValidType və IsValidSize methodları geriyə bool qaytarır. Upload isə geriyə file-ın save olunduğu adı qaytarır.
*/