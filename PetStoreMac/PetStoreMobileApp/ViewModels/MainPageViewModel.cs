using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using LazyStackAuth;
using PetStoreMobileApp;

namespace PetStoreMobileApp.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel()
        {
            ClearCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.ClearAsync));
            CancelCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.CancelAsync));
            SignOutCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.SignOutAsync));

            StartSignInCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.StartSignInAsync));
            StartSignUpCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.StartSignUpAsync));
            StartResetPasswordCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.StartResetPasswordAsync));
            StartUpdateLoginCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.StartUpdateLoginAsync));
            StartUpdateEmailCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.StartUpdateEmailAsync));
            StartUpdatePhoneCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.StartUpdatePhoneAsync));
            StartUpdatePasswordCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.StartUpdatePasswordAsync));

            VerifyLoginCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyLoginAsync));
            VerifyNewLoginCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyNewLoginAsync));
            VerifyPasswordCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyPasswordAsync));
            VerifyNewPasswordCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyNewPasswordAsync));
            VerifyEmailCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyEmailAsync));
            VerifyNewEmailCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyNewEmailAsync));
            VerifyPhoneCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyPhoneAsync));
            VerifyNewPhoneCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyNewPhoneAsync));
            VerifyCodeCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.VerifyCodeAsync));

            ResendCodeCommand = new Command(async () => await MakeAuthProcessCallAsync(_authProcess.ResendCodeAsync));

        }
        #region Locals
        #endregion

        #region Commands
        public ICommand TestCommand { get; }
        public ICommand OpenWebCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand StartSignInCommand { get; }
        public ICommand StartSignUpCommand { get; }
        public ICommand StartResetPasswordCommand { get; }
        public ICommand StartUpdateLoginCommand { get; }
        public ICommand StartUpdateEmailCommand { get; }
        public ICommand StartUpdatePhoneCommand { get; }
        public ICommand StartUpdatePasswordCommand { get; }

        public ICommand VerifyLoginCommand { get; }
        public ICommand VerifyNewLoginCommand { get; }

        public ICommand VerifyPasswordCommand { get; }
        public ICommand VerifyNewPasswordCommand { get; }

        public ICommand VerifyEmailCommand { get; }
        public ICommand VerifyNewEmailCommand { get; }

        public ICommand VerifyPhoneCommand { get; }
        public ICommand VerifyNewPhoneCommand { get; }

        public ICommand VerifyCodeCommand { get; }

        public ICommand ResendCodeCommand { get; }

        public ICommand ViewPetsCommand { get; }

        #endregion

        #region Properties
        public uint FadeTime { get; set; } = 250;
        private IAuthProcess _authProcess => DependencyService.Get<IAuthProcess>();
        public IAuthProcess AuthProcess { get { return _authProcess; } }
        private bool _IsCallingAuthProcess = false;
        public bool IsCallingAuthProcess
        {
            get { return _IsCallingAuthProcess; }
            set { SetProperty(ref _IsCallingAuthProcess, value); }
        }
        #endregion

        #region Methods

        // Wrap the call to AuthProcess to allow Xamarin specific animation to be performed
        public async Task MakeAuthProcessCallAsync(Func<Task> call)
        {
            IsCallingAuthProcess = true; // see code behind in view
            await call();
            IsCallingAuthProcess = false;
        }
        #endregion
    }
}
