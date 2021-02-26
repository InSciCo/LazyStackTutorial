using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LazyStackAuth;
using PetStoreMobileApp.ViewModels;
using PetStoreClientSDK;
using System.Runtime.CompilerServices;

namespace PetStoreMobileApp.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            aboutViewModel = (BindingContext as AboutViewModel);
            authProcess = aboutViewModel.AuthProcess;

            aboutViewModel.PropertyChanged += AuthProcessForm_PropertyChanged;
            authProcess.PropertyChanged += AuthProcess_PropertyChanged;
        }

        IAuthProcess authProcess;
        AboutViewModel aboutViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            aboutViewModel.PropertyChanged -= AuthProcessForm_PropertyChanged;
            authProcess.PropertyChanged -= AuthProcess_PropertyChanged;
            base.OnDisappearing();
        }

        /// <summary>
        /// Animate the transition among AuthProcess challenges. We only need to
        /// animate the AuthProcessForm StackLayout. Nothing special here, just
        /// something to make the screen transitions less herky-jerky.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthProcessForm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(aboutViewModel.IsCallingAuthProcess):

                    if (aboutViewModel.IsCallingAuthProcess)
                    {
                        AuthProcessForm.FadeTo(0, aboutViewModel.FadeTime);
                        AuthProcessForm.IsVisible = false;
                    }
                    else
                    {
                        AuthProcessForm.Opacity = 0;
                        AuthProcessForm.IsVisible = true;
                        AuthProcessForm.FadeTo(1, aboutViewModel.FadeTime);
                    }
                    break;
            }
        }

        /// <summary>
        ///  Focus in entry field on challenge change. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthProcess_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(authProcess.CurrentChallenge):
                    switch (authProcess.CurrentChallenge)
                    {
                        case AuthChallengeEnum.Login:
                            LoginEntry.Focus();
                            break;
                        case AuthChallengeEnum.NewLogin:
                            NewLoginEntry.Focus();
                            break;
                        case AuthChallengeEnum.Password:
                            PasswordEntry.Focus();
                            break;
                        case AuthChallengeEnum.NewPassword:
                            NewPasswordEntry.Focus();
                            break;
                        case AuthChallengeEnum.Email:
                            EmailEntry.Focus();
                            break;
                        case AuthChallengeEnum.NewEmail:
                            NewEmailEntry.Focus();
                            break;
                        case AuthChallengeEnum.Phone:
                            PhoneEntry.Focus();
                            break;
                        case AuthChallengeEnum.NewPhone:
                            NewPhoneEntry.Focus();
                            break;
                        case AuthChallengeEnum.Code:
                            CodeEntry.Focus();
                            break;
                    }
                    break;
            }
        }
    }
}