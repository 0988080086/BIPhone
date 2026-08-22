namespace BIPhone
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Bắt lỗi trên AppDomain (Luồng chung)
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                System.Diagnostics.Debug.WriteLine($"[CRASH AppDomain] {ex?.Message}\n{ex?.StackTrace}");
            };
            // Bắt lỗi trong các Task async/await
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"[CRASH Task] {args.Exception?.Message}\n{args.Exception?.StackTrace}");
                args.SetObserved(); // Ngăn ứng dụng bị crash
            };

            //Khởi động MainPage
            MainPage = new NavigationPage(new BIPhone.Views.MainPage());
        }

        protected override Window CreateWindow(
            IActivationState? activationState)
        {
            //return new Window(new AppShell());
            return new Window(MainPage);
        }
    }
}