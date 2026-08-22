using Android.App;
using Android.Runtime;

namespace BIPhone
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        //protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp().Result;
        }
    }
}
