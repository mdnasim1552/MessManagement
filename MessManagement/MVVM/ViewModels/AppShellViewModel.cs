using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.MVVM.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly UserSessionService _userSession;
        private readonly AuthService _authService;
        [ObservableProperty]
        private ImageSource profileImage;
        [ObservableProperty]
        private string fullName;
        [ObservableProperty]
        private bool isBusy;
        public AppShellViewModel(UserSessionService userSessionService, AuthService authService)
        {
            _userSession = userSessionService;
            _authService = authService;
            LoadProfilePicture();
        }
        private void LoadProfilePicture()
        {
            try
            {
                var profileImageBytes = _userSession.CurrentUser.ProfilePicture;

                if (profileImageBytes != null && profileImageBytes.Length > 0)
                {
                    // Convert byte[] to ImageSource
                    ProfileImage = ImageSource.FromStream(() => new MemoryStream(profileImageBytes));
                }
                else
                {
                    ProfileImage = "dotnet_bot.png";
                }
                FullName = _userSession.CurrentUser.FullName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profile image: {ex.Message}");
                ProfileImage = "dotnet_bot.png";
            }
        }
        [RelayCommand]
        private async Task UploadProfilePictureAsync()
        {
            try
            {
                IsBusy = true;
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a profile picture",
                    FileTypes = FilePickerFileType.Images
                });

                if (result == null)
                    return;

                //// Open stream twice: one for Image preview, one for HTTP upload
                //var streamForUpload = await result.OpenReadAsync();
                //var streamForImage = await result.OpenReadAsync();
                //ProfileImage = ImageSource.FromStream(() => streamForImage);

                byte[] fileBytes;
                using (var stream = await result.OpenReadAsync())
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                // Set preview
                ProfileImage = ImageSource.FromStream(() => new MemoryStream(fileBytes));


                var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(fileBytes);

                //var imageContent = new StreamContent(streamForUpload);
                var mimeType = result.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg";
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                content.Add(imageContent, "profileImage", result.FileName);

                // Add userId or other data
                content.Add(new StringContent(_userSession.CurrentUser.Id.ToString()), "userId"); // Replace with actual user ID

                var response = await _authService.UploadProfilePictureAsync(content);

                if (response.Success)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "Profile picture uploaded!", "OK");
                    _userSession.CurrentUser.ProfilePicture = ProfileImage is FileImageSource fileImageSource
                        ? System.IO.File.ReadAllBytes(fileImageSource.File)
                        : null;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
