namespace Sales.Droid
{
    using Android.App;
    using Android.OS;

    [Activity(Theme ="@style/Theme.Splash", MainLauncher =true,NoHistory =true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Threading.Thread.Sleep(800); //Let's wait awhile...
            this.StartActivity(typeof(MainActivity));
        }
    }
}