namespace BIPhone.Services;

/// <summary>Modal Error Handler</summary>
public class ModalErrorHandler : IErrorHandler
{
    SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>Handle error in UI</summary>
    /// <param name="ex">Exception.</param>
    public void HandleError(Exception ex)
    {
        DisplayAlert(ex).FireAndForgetSafeAsync();
    }

    async Task DisplayAlert(Exception ex)
    {
        try
        {
            await _semaphore.WaitAsync();
            if (Shell.Current is Shell shell)
                await shell.DisplayAlert("Lỗi hệ thống", ex.Message, "Đóng");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
public static class TaskExtensions
{
    /// <summary>
    /// Thực thi Task bất đồng bộ một cách an toàn từ ngữ cảnh đồng bộ (Fire-and-Forget)
    /// </summary>
    public static async void FireAndForgetSafeAsync(this Task task, Action<Exception>? handler = null)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            //LogWriter.WriteLine($"[FireAndForget Error]: {ex.Message}");
            handler?.Invoke(ex);
        }
    }
}