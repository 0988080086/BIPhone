using Foundation;

namespace BIPhone
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        //protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp().Result;
        }
    }
}
