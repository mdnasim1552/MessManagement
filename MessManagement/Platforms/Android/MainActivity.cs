using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api.SignIn;
using Android.Runtime;
using MessManagement.Platforms.Android;

namespace MessManagement
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static event EventHandler<GoogleSignInEventArgs> GoogleSignInCompleted;
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 9999)
            {
                try
                {
                    var task = GoogleSignIn.GetSignedInAccountFromIntent(data);

                    // CAST to GoogleSignInAccount
                    var account = task.Result.JavaCast<GoogleSignInAccount>();

                    var idToken = account.IdToken;

                    GoogleSignInCompleted?.Invoke(this, new GoogleSignInEventArgs
                    {
                        Success = true,
                        IdToken = idToken
                    });
                }
                catch (Exception ex)
                {
                    GoogleSignInCompleted?.Invoke(this, new GoogleSignInEventArgs
                    {
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                }
            }
        }

    }
}
