namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Newtonsoft.Json;
    using Sales.Common.Models;
    using Sales.Views;
    using Services;
    using System;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class LoginViewModel : BaseViewModels
    {
        #region Atributes
        private ApiService apiService;
        private bool isRunning;
        private bool isEnable;
        #endregion

        #region Properties
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRemembered { get; set; }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.SetValue(ref this.isRunning, value); }
        }
        public bool IsEnable
        {
            get { return this.isEnable; }
            set { this.SetValue(ref this.isEnable, value); }
        }

        #endregion

        #region Constructor
        public LoginViewModel()
        {
            this.isEnable = true;
            this.apiService = new ApiService();
            this.IsEnable = true;
            this.IsRemembered = true;
        }
        #endregion

        #region Commands
        public ICommand RegisterCommand {
            get
            {
                return new RelayCommand(Register);
            }
        }

        private async void Register()
        {
            MainViewModel.GetInstance().Register = new RegisterViewModel();
            Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }

        public ICommand  LoginCommand {
            get {
                return new RelayCommand(Login);
            }
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email)) {
                await Application.Current.MainPage.DisplayAlert(Languages.Error,Languages.EmailValidation,Languages.Accept);
                return;
            }
            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.PasswordValidation, Languages.Accept);
                return;
            }

            this.isRunning = true;
            this.isEnable = false;
            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.isRunning =false ;
                this.isEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var Token = await this.apiService.GetToken(url, this.Email, this.Password);

            if (Token==null || string.IsNullOrEmpty(Token.AccessToken))
            {
                this.isRunning = false;
                this.isEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.SomethingWrong, Languages.Accept);
                return;
            }
            Settings.TokenType = Token.TokenType;
            Settings.AccessToken = Token.AccessToken;
            Settings.IsRemembered = this.IsRemembered;

            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlUsersController"].ToString();
            var response = await this.apiService.GetUser(url, prefix, $"{controller}/GetUser", this.Email, Token.TokenType, Token.AccessToken);
            if (response.IsSuccess)
            {
                var userASP = (MyUserASP)response.Result;
                MainViewModel.GetInstance().UserASP = userASP;
                Settings.UserASP = JsonConvert.SerializeObject(userASP);
            }

            MainViewModel.GetInstance().Products = new ProductsViewModel();
            Application.Current.MainPage = new MasterPage();

            this.isRunning = false;
            this.isEnable = true;
        }

        public ICommand LoginFacebookComand {
            get { return new RelayCommand(LoginFacebook); }
        }
        private async void LoginFacebook()
        {
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            await Application.Current.MainPage.Navigation.PushAsync(new LoginFacebookPage());
        }

        public ICommand LoginInstagramComand {
            get { return new RelayCommand(LoginInstagram); }
        }
        private async void LoginInstagram()
        {
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            await Application.Current.MainPage.Navigation.PushAsync(new LoginInstagramPage());
        }

        public ICommand LoginTwitterComand {
            get { return new RelayCommand(LoginTwitter); }
        }
        private async void LoginTwitter()
        {
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnable = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            await Application.Current.MainPage.Navigation.PushAsync(new LoginTwitterPage());
        }
        #endregion
    }
}
