using System.Text;

namespace BIPhone;

public static class LogWriter
{
    private static string _FileName = "";
    private static string GetFileName()
    {
        _FileName = "";
        return _FileName;
    }
    public static void WriteLine(string text)
    {
        try
        {
            if (string.IsNullOrEmpty(text)) { return; }
            if (string.IsNullOrEmpty(_FileName)) 
            {
                _FileName = GetFileName();
            }
            if (File.Exists(_FileName) == false)
            {
                return;
            }
            string lineStr = DateTime.Now.ToString("HH:mm:ss") + "  " + text;
            File.AppendAllText(_FileName, lineStr + Environment.NewLine, Encoding.UTF8);
        }
        catch { }
    }
    public static string ReadAll()
    {
        if (string.IsNullOrEmpty(_FileName))
        {
            _FileName = GetFileName();
        }
        if (File.Exists(_FileName) == false)
        {
            return "";
        }
        string mText;
        try
        {
            mText = File.ReadAllText(_FileName);
        }
        catch
        {
            mText = "";
        }
        return mText;
    }
    public static void Clear()
    {
        if (string.IsNullOrEmpty(_FileName))
        {
            _FileName = GetFileName();
        }
        if (File.Exists(_FileName) == false)
        {
            return;
        }

        if (File.Exists(_FileName))
        {
            File.Delete(_FileName);
        }
    }
}
