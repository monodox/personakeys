using System.Runtime.InteropServices;
using System.Text;
using Logi.PluginCore;

namespace PersonaKeys.Services;

/// <summary>
/// Handles clipboard operations for reading and writing text
/// </summary>
public class ClipboardService : IDisposable
{
    [DllImport("user32.dll")]
    private static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    private static extern IntPtr GetClipboardData(uint uFormat);

    [DllImport("user32.dll")]
    private static extern bool SetClipboardData(uint uFormat, IntPtr hMem);

    [DllImport("user32.dll")]
    private static extern bool EmptyClipboard();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll")]
    private static extern bool GlobalUnlock(IntPtr hMem);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

    private const uint CF_UNICODETEXT = 13;
    private const uint GMEM_MOVEABLE = 0x0002;

    /// <summary>
    /// Gets text from the clipboard
    /// </summary>
    public string GetText()
    {
        try
        {
            if (!OpenClipboard(IntPtr.Zero))
            {
                Log.Warning("Failed to open clipboard");
                return string.Empty;
            }

            try
            {
                IntPtr handle = GetClipboardData(CF_UNICODETEXT);
                if (handle == IntPtr.Zero)
                {
                    return string.Empty;
                }

                IntPtr pointer = GlobalLock(handle);
                if (pointer == IntPtr.Zero)
                {
                    return string.Empty;
                }

                try
                {
                    return Marshal.PtrToStringUni(pointer) ?? string.Empty;
                }
                finally
                {
                    GlobalUnlock(handle);
                }
            }
            finally
            {
                CloseClipboard();
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to get clipboard text: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Sets text to the clipboard
    /// </summary>
    public bool SetText(string text)
    {
        try
        {
            if (!OpenClipboard(IntPtr.Zero))
            {
                Log.Warning("Failed to open clipboard");
                return false;
            }

            try
            {
                EmptyClipboard();

                byte[] bytes = Encoding.Unicode.GetBytes(text + '\0');
                IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)bytes.Length);
                
                if (hGlobal == IntPtr.Zero)
                {
                    return false;
                }

                IntPtr pointer = GlobalLock(hGlobal);
                if (pointer == IntPtr.Zero)
                {
                    return false;
                }

                try
                {
                    Marshal.Copy(bytes, 0, pointer, bytes.Length);
                }
                finally
                {
                    GlobalUnlock(hGlobal);
                }

                if (SetClipboardData(CF_UNICODETEXT, hGlobal) == IntPtr.Zero)
                {
                    return false;
                }

                Log.Info("Text copied to clipboard");
                return true;
            }
            finally
            {
                CloseClipboard();
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to set clipboard text: {ex.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
