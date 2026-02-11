using Logi.PluginCore;
using Logi.PluginCore.Actions;
using PersonaKeys.Services;

namespace PersonaKeys.Actions;

/// <summary>
/// Base class for all PersonaKeys actions
/// </summary>
public abstract class BasePersonaAction : PluginAction
{
    protected Plugin PluginInstance => (Plugin)Plugin;
    
    protected LLMService LLMService => PluginInstance.GetLLMService();
    protected ClipboardService ClipboardService => PluginInstance.GetClipboardService();
    protected SettingsService SettingsService => PluginInstance.GetSettingsService();

    /// <summary>
    /// Shows a loading indicator on the button
    /// </summary>
    protected void ShowLoading()
    {
        // Update button state to show processing
        SetState(0);
    }

    /// <summary>
    /// Shows success feedback
    /// </summary>
    protected void ShowSuccess()
    {
        SetState(1);
        
        // Send haptic feedback if enabled
        var settings = SettingsService.GetSettings();
        if (settings.EnableHaptics)
        {
            SendHapticFeedback(HapticPattern.Success);
        }
        
        // Reset state after delay
        Task.Delay(1500).ContinueWith(_ => SetState(0));
    }

    /// <summary>
    /// Shows error feedback
    /// </summary>
    protected void ShowError(string? message = null)
    {
        SetState(2);
        
        if (!string.IsNullOrEmpty(message))
        {
            Log.Error(message);
            ShowAlert(message);
        }
        
        // Send haptic feedback if enabled
        var settings = SettingsService.GetSettings();
        if (settings.EnableHaptics)
        {
            SendHapticFeedback(HapticPattern.Error);
        }
        
        // Reset state after delay
        Task.Delay(2000).ContinueWith(_ => SetState(0));
    }

    /// <summary>
    /// Sends haptic feedback to the device
    /// </summary>
    protected void SendHapticFeedback(HapticPattern pattern)
    {
        try
        {
            // Implementation depends on SDK version
            // This is a placeholder for the actual SDK haptics API
            // Example: HapticEngine.Play(pattern);
        }
        catch (Exception ex)
        {
            Log.Warning($"Failed to send haptic feedback: {ex.Message}");
        }
    }
}

/// <summary>
/// Haptic feedback patterns
/// Ring change → light tick
/// Command invoked → soft pulse
/// Success → short confirmation buzz
/// Error → distinct vibration pattern
/// </summary>
public enum HapticPattern
{
    Success,      // Short confirmation buzz
    Error,        // Distinct error vibration
    Click,        // Soft pulse on invocation
    Adjustment    // Light tick on ring change
}
