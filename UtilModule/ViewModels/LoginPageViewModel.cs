using LSlicer.BL.Interaction;
using LSlicer.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows;
using System.Windows.Media;

namespace UtilModule.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;

        private IUserIdentity _user;

        public LoginPageViewModel(IEventAggregator ea)
        {
            _user = new UserIdentityModel();
            _ea = ea;
            _ea.GetEvent<UserIdentitySentEvent>().Subscribe(GetUser);
            _ea.GetEvent<UserSentAutorizedStateEvent>().Subscribe(SetAutorizationStatus);
            _ea.GetEvent<UserSentAutorizedUserEvent>().Subscribe(SetAutorizationUser);
        }
        private Boolean _isAutorized;

        private Boolean _isSignUp;
        public Boolean IsSignUp
        {
            get 
            {
                return _isSignUp; 
            }
            set 
            { 
                SetProperty(ref _isSignUp, value);
                ConfirmVisibility = _isSignUp == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _confirmVisibility = Visibility.Collapsed;
        public Visibility ConfirmVisibility
        {
            get { return _confirmVisibility; }
            set { SetProperty(ref _confirmVisibility, value); }
        }

        private Visibility _loginVisibility = Visibility.Visible;
        public Visibility LoginVisibility
        {
            get { return _loginVisibility; }
            set { SetProperty(ref _loginVisibility, value); }
        }

        private Visibility _currentUserVisibility = Visibility.Collapsed;
        public Visibility CurrentUserVisibility
        {
            get { return _currentUserVisibility; }
            set { SetProperty(ref _currentUserVisibility, value); }
        }

        private string _currentUserName;
        public string CurrentUserName
        {
            get
            {
                return _currentUserName;
            }
            set
            {
                SetProperty(ref _currentUserName, value);
            }
        }

        private string _name;
        public string Name
        {
            get 
            {
                _name = _user.Name;
                return _name;             
            }
            set 
            {
                _user.Name = value;
                SetProperty(ref _name, value); 
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { SetProperty(ref _confirmPassword, value); }
        }

        private Brush _indicatorColor;
        public Brush IndicatorColor
        {
            get { return _indicatorColor; }
            set { SetProperty(ref _indicatorColor, value); }
        }

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand, CanExecuteLoginCommand)
            .ObservesProperty(() => Password)
            .ObservesProperty(() => Name)
            .ObservesProperty(() => ConfirmPassword));
        
        private DelegateCommand _logoutCommand;
        public DelegateCommand LogoutCommand =>
            _logoutCommand ?? (_logoutCommand = new DelegateCommand(ExecuteLogoutCommand, CanExecuteLogoutCommand)
            .ObservesProperty(() => CurrentUserName));

        private bool CanExecuteLogoutCommand() => _isAutorized;

        private void ExecuteLogoutCommand()
        {
            var user = new UserIdentityModel();
            _ea.GetEvent<UserIdentityGetEvent>().Publish(user);
            _ea.GetEvent<UserGetAutorizedStateEvent>().Publish();
        }

        void ExecuteLoginCommand()
        {
            if (IsSignUp)
            {
                _user.PasswordHash = Password.GetHashCode().ToString();
                _ea.GetEvent<UserIdentityCreateEvent>().Publish(_user);
                return;
            }
            _user.PasswordHash = Password.GetHashCode().ToString();
            _ea.GetEvent<UserIdentityGetEvent>().Publish(_user);
            _ea.GetEvent<UserGetAutorizedStateEvent>().Publish();
        }

        bool CanExecuteLoginCommand()
        {
            Boolean isValid = !String.IsNullOrEmpty(_name) 
                && !String.IsNullOrEmpty(_password) 
                && _password.Length > 5;
            
            if (IsSignUp)
            {
                isValid &= (ConfirmPassword == Password);               
            }

            if (isValid)
                IndicatorColor = Brushes.Blue;
            else
                IndicatorColor = Brushes.DarkRed;
            return isValid;
        }

        private void GetUser(IUserIdentity user) 
        {
            if (user!= null && !String.IsNullOrEmpty(user.Name) && !String.IsNullOrEmpty(user.PasswordHash))
            {
                Name = user.Name;
                IndicatorColor = Brushes.Green;
                return;
            }
            IndicatorColor = Brushes.Red;
        }

        private void SetAutorizationStatus(Boolean isAutorized) 
        {
            _isAutorized = isAutorized;
            if (_isAutorized)
            {
                CurrentUserVisibility = Visibility.Visible;
                LoginVisibility = Visibility.Collapsed;
                IndicatorColor = Brushes.LightBlue;
                CurrentUserName = _user.Name;
            }
            else
            {
                CurrentUserVisibility = Visibility.Collapsed;
                LoginVisibility = Visibility.Visible;
                IndicatorColor = Brushes.LightGray;
            }
        }
        private void SetAutorizationUser(IUserIdentity user)
        {
            if (_isAutorized)
            {
                _user.Name = user.Name;
                CurrentUserName = _user.Name;
            }
        }
    }
}
