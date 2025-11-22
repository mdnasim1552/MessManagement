using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using MessManagement.Shared.DTOs;
using System.Threading.Tasks;

namespace MessManagement.Platforms.Android
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly string _webClientId;

        public GoogleAuthService(string webClientId)
        {
            _webClientId = webClientId;
        }

        public Task<string> SignInAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            var activity = Platform.CurrentActivity as Activity;
            if (activity == null)
            {
                tcs.SetException(new Exception("Current activity is null"));
                return tcs.Task;
            }
            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                        .RequestEmail()
                        .RequestIdToken(_webClientId)
                        .Build();

            //var googleSignInClient = GoogleSignIn.GetClient(activity, gso);

            //var signInIntent = googleSignInClient.SignInIntent;
            //activity.StartActivityForResult(signInIntent, 9999);

            //// Handle result in MainActivity
            //MainActivity.GoogleSignInCompleted += (s, e) =>
            //{
            //    if (e.Success)
            //        tcs.TrySetResult(e.IdToken);
            //    else
            //        tcs.TrySetException(new Exception(e.ErrorMessage));
            //};

            //return tcs.Task;
            var googleSignInClient = GoogleSignIn.GetClient(activity, gso);

            var signInIntent = googleSignInClient.SignInIntent;

            EventHandler<GoogleSignInEventArgs> handler = null;
            handler = (s, e) =>
            {
                MainActivity.GoogleSignInCompleted -= handler; // unsubscribe immediately

                if (e.Success)
                    tcs.TrySetResult(e.IdToken);
                else
                    tcs.TrySetException(new Exception(e.ErrorMessage));
            };

            MainActivity.GoogleSignInCompleted += handler;

            activity.StartActivityForResult(signInIntent, 9999);

            return tcs.Task;
        }
        public async System.Threading.Tasks.Task SignOutAsync()
        {
            var activity = Platform.CurrentActivity as Activity;
            if (activity == null) return;

            var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                        .RequestEmail()
                        .RequestIdToken(_webClientId)
                        .Build();

            var googleSignInClient = GoogleSignIn.GetClient(activity, gso);
            await googleSignInClient.SignOut();
            await googleSignInClient.RevokeAccess();
        }
    }

    public class GoogleSignInEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string IdToken { get; set; }
        public string ErrorMessage { get; set; }
    }
}
