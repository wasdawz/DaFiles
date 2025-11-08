using DaFiles.Services;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security.Credentials;

namespace DaFiles.Desktop.Services;

/// <param name="identifier">App name, website URL, or any other string to be used as the identifier of
/// the origin of the secrets.</param>
[SupportedOSPlatform("windows5.1.2600")]
public class WindowsCredentialManagerSecretStore(string identifier) : ISecretStore
{
    private readonly string _identifier = identifier;

    public unsafe SecureString? Read(string key, SecureString? target)
    {
        string targetName = GetTargetName(key);

        if (!PInvoke.CredRead(targetName, CRED_TYPE.CRED_TYPE_GENERIC, out CREDENTIALW* credential))
        {
            int error = Marshal.GetLastWin32Error();

            if ((WIN32_ERROR)error == WIN32_ERROR.ERROR_NOT_FOUND)
                return null;

            throw new Win32Exception(error);
        }

        try
        {
            if (target is not null)
            {
                fixed (char* ch = "")
                {
                    for (uint j = 0; j < credential->CredentialBlobSize; j += 2)
                    {
                        Encoding.Unicode.GetChars(credential->CredentialBlob + j, 2, ch, 1);
                        target.AppendChar(*ch);
                    }
                }
                return target;
            }
            else
            {
                return new SecureString((char*)credential->CredentialBlob, (int)credential->CredentialBlobSize);
            }
        }
        finally
        {
            PInvoke.CredFree(credential);
        }
    }

    public unsafe void Write(string key, SecureString secret)
    {
        string targetName = GetTargetName(key);
        nint secretPtr = Marshal.SecureStringToGlobalAllocUnicode(secret);

        try
        {
            fixed (char* targetNamePtr = targetName)
            {
                CREDENTIALW credential = new()
                {
                    Type = CRED_TYPE.CRED_TYPE_GENERIC,
                    TargetName = new(targetNamePtr),
                    CredentialBlobSize = (uint)(secret.Length * UnicodeEncoding.CharSize),
                    CredentialBlob = (byte*)secretPtr,
                    Persist = CRED_PERSIST.CRED_PERSIST_LOCAL_MACHINE,
                };

                if (!PInvoke.CredWrite(in credential, 0))
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error);
                }
            }
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(secretPtr);
        }
    }

    public bool Remove(string key)
    {
        string targetName = GetTargetName(key);
        
        if (!PInvoke.CredDelete(targetName, CRED_TYPE.CRED_TYPE_GENERIC))
        {
            int error = Marshal.GetLastWin32Error();

            if ((WIN32_ERROR)error == WIN32_ERROR.ERROR_NOT_FOUND)
                return false;

            throw new Win32Exception(error);
        }

        return true;
    }

    private string GetTargetName(string key) => $"{_identifier}|{key}";
}
