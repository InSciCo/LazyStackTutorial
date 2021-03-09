using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using PetStoreMobileApp.ViewModels;
using LazyStackAuth;

namespace PetStoreMobileApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            mainPageViewModel = (BindingContext as MainPageViewModel);
            authProcess = mainPageViewModel.AuthProcess;

            mainPageViewModel.PropertyChanged += AuthProcessForm_PropertyChanged;
            authProcess.PropertyChanged += AuthProcess_PropertyChanged;
        }
        IAuthProcess authProcess;
        MainPageViewModel mainPageViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            mainPageViewModel.PropertyChanged -= AuthProcessForm_PropertyChanged;
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
                case nameof(mainPageViewModel.IsCallingAuthProcess):

                    if (mainPageViewModel.IsCallingAuthProcess)
                    {
                        AuthProcessForm.FadeTo(0, mainPageViewModel.FadeTime);
                        AuthProcessForm.IsVisible = false;
                    }
                    else
                    {
                        AuthProcessForm.Opacity = 0;
                        AuthProcessForm.IsVisible = true;
                        AuthProcessForm.FadeTo(1, mainPageViewModel.FadeTime);
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

        private void SeePets_Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ItemsPage());

        }
    }
}
