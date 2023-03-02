using LSlicer.BL.Interaction;
using LSlicer.Implementations;
using LSlicer.Infrastructure;
using Prism.Events;
using System;
using System.Windows;
using Unity;

namespace LSlicer.Model
{
    public class IdentityController
    {
        private readonly IEventAggregator _ea;
        private readonly ILoggerService _loggerService;
        private readonly UserIdentityController _controller;
        private readonly IAppSettings _appSettings;
        private Window _loginWindow;
        private readonly IUnityContainer _containerProvider;
        private Boolean _isAutorized;
        private IUserIdentity _autorizedUser;

        public IdentityController(
            IEventAggregator ea,
            UserIdentityController controller,
            IAppSettings appSettings,
            ILoggerService loggerService,
            IUnityContainer containerProvider)
        {
            _ea = ea;
            _controller = controller;
            _ea.GetEvent<UserIdentityCreateEvent>().Subscribe(AddUser);
            _ea.GetEvent<UserIdentityRemoveEvent>().Subscribe(RemoveUser);
            _ea.GetEvent<UserIdentityUpdateEvent>().Subscribe(UpdateUser);
            _ea.GetEvent<UserIdentityGetEvent>().Subscribe(GetUser);
            _ea.GetEvent<UserGetAutorizedStateEvent>().Subscribe(SendAutorizedStatus);
            _appSettings = appSettings;
            _loggerService = loggerService;
            _containerProvider = containerProvider;
        }

        private void AddUser(IUserIdentity user)
        {
            if (_controller.Create(user.Name, user.PasswordHash))
            {
                GetUser(user);
                _loggerService.Info($"[{nameof(IdentityController)}] Add user witn name \"{user.Name}\".");
                _isAutorized = true;
                _ea.GetEvent<UserSentAutorizedStateEvent>().Publish(_isAutorized);
            }
            else _loggerService.Info($"[{nameof(IdentityController)}] Cant add user witn name \"{user.Name}\".");
            _isAutorized = false;
            _ea.GetEvent<UserSentAutorizedStateEvent>().Publish(_isAutorized);
        }

        private void GetUser(IUserIdentity user) 
        {
            User existingUser = _controller.GetByName(user.Name);
            if (existingUser == null || existingUser.PasswordHash != user.PasswordHash)
            {
                UserWrapper empty = new UserWrapper(new User());
                _ea.GetEvent<UserIdentitySentEvent>().Publish(empty);
                _isAutorized = false;
                _ea.GetEvent<UserSentAutorizedStateEvent>().Publish(_isAutorized);
                _appSettings.SetForUser(1);
                _ea.GetEvent<AppSettingsSentToViewEvent>().Publish(_appSettings);
                _loggerService.Info($"[{nameof(IdentityController)}] User \"{user.Name}\" wasnt been autorized.");
                return;
            }
            _appSettings.SetForUser(existingUser.Id);
            _ea.GetEvent<AppSettingsSentToViewEvent>().Publish(_appSettings);
            UserWrapper wrapper = new UserWrapper(existingUser);
            _ea.GetEvent<UserIdentitySentEvent>().Publish(wrapper);
            _isAutorized = true;
            _ea.GetEvent<UserSentAutorizedStateEvent>().Publish(_isAutorized);
            _autorizedUser = wrapper;
            _loggerService.Info($"[{nameof(IdentityController)}] User \"{user.Name}\" was been autorized.");
        }

        private void RemoveUser(IUserIdentity user)
        {
            User existingUser = _controller.GetByName(user.Name);
            if (existingUser != null)
            {
                if (_controller.Delete(existingUser.Id))
                {
                    _loggerService.Info($"[{nameof(IdentityController)}] Remove user witn name \"{user.Name}\".");
                    _autorizedUser = new UserWrapper(new User());
                }
                else
                {
                    _loggerService.Info($"[{nameof(IdentityController)}] Cant remove user witn name \"{user.Name}\".");
                }
                _ea.GetEvent<UserSentAutorizedStateEvent>().Publish(_isAutorized);
            }
        }

        private void UpdateUser(IUserIdentity user)
        {
            User existingUser = _controller.GetByName(user.Name);
            if (existingUser != null)
            {
                existingUser.PasswordHash = user.PasswordHash;
                if(_controller.Update(existingUser))
                    _loggerService.Info($"[{nameof(IdentityController)}] Update user witn name \"{user.Name}\".");
                else
                    _loggerService.Info($"[{nameof(IdentityController)}] Cant update user witn name \"{user.Name}\".");
            }
        }

        internal void RaiseLoginView()
        {
            _loginWindow = _containerProvider.Resolve<LSlicer.Views.LoginWindow>();
            _loginWindow.Owner = Application.Current.MainWindow;
            _loginWindow.Show();
            SendAutorizedStatus();
        }

        private void SendAutorizedStatus() 
        {
            _ea.GetEvent<UserSentAutorizedStateEvent>().Publish(_isAutorized);
            if (_isAutorized)
            {
                _ea.GetEvent<UserSentAutorizedUserEvent>().Publish(_autorizedUser);                
            }
        }
    }
}
