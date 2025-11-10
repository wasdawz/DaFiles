using System.Runtime.InteropServices;
using System.Security;

namespace DaFiles.Helpers;

internal static class ExtensionMethods
{
    public static string? Read(this SecureString secureString)
    {
        nint stringPtr = Marshal.SecureStringToCoTaskMemUnicode(secureString);
        string? str = Marshal.PtrToStringUni(stringPtr);
        Marshal.ZeroFreeCoTaskMemUnicode(stringPtr);
        return str;
    }
}
