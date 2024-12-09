﻿using PapayagramsClient.Game;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PapayagramsClient.PapayagramsService;
using PapayagramsClient.Menu;
using PapayagramsClient.WPFControls;
using PapayagramsClient.ClientData;
using System.Threading;
using System.Windows.Input;
using System.IO;

namespace PapayagramsClient
{
    public partial class MainMenu : Page, IMainMenuServiceCallback
    {
        private MainMenuServiceClient _host;
        private string _invitationGameCode;
        private MainWindow _mainWindow;

        public MainMenu()
        {
            _mainWindow = (MainWindow)Application.Current.MainWindow;
            RetrieveConfiguration();

            if (CurrentPlayer.Configuration != null)
            {
                ApplyLanguageConfiguration();
                ApplyCursorConfiguration();
            }

            InitializeComponent();

            InstanceContext context = new InstanceContext(this);

            _host = new MainMenuServiceClient(context);

            try
            {
                _host.Open();
            }
            catch (EndpointNotFoundException)
            {
                new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorServerConnection, 3).ShowDialog();
                return;
            }

            int returnCode = _host.ReportToServer(CurrentPlayer.Player.Username);

            switch (returnCode)
            {
                case 0:
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    return;
            }

            RetrieveRelationships();
        }

        ~MainMenu()
        {
            try
            {
                _host.Close();
            }
            catch (CommunicationObjectFaultedException)
            { 
                
            }
        }

        private void RetrieveConfiguration()
        {
            ApplicationSettingsServiceClient settingsHost = new ApplicationSettingsServiceClient();
            try
            {
                settingsHost.Open();
            }
            catch (EndpointNotFoundException)
            {
                new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorServerConnection, 3).ShowDialog();
                return;
            }

            (int returnCode1, ApplicationSettingsDC userSettings) = settingsHost.GetApplicationSettings(CurrentPlayer.Player.Username);
            settingsHost.Close();
            CurrentPlayer.Configuration = userSettings;

            switch (returnCode1)
            {
                case 0:
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    return;

                case 205:
                    break;
            }
        }

        private void ApplyLanguageConfiguration()
        {
            if (CurrentPlayer.Configuration == null)
            {
                Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
                return;
            }

            switch (CurrentPlayer.Configuration.SelectedLanguage)
            {
                case ApplicationLanguageDC.auto:
                    Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
                    break;

                case ApplicationLanguageDC.english:
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                    break;

                case ApplicationLanguageDC.spanish:
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-MX");
                    break;
            }
        }

        private void ApplyCursorConfiguration()
        {
            switch (CurrentPlayer.Configuration.Cursor)
            {
                case 0:
                    Cursor = null;
                    break;

                case 1:
                    Cursor = new Cursor(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Resources\\Cursors\\papaya.cur");
                    break;
            }

            Mouse.OverrideCursor = Cursor;
        }

        private void RetrieveRelationships()
        {
            (int returnCode, FriendDC[] relationships) = _host.GetAllRelationships(CurrentPlayer.Player.Username);

            switch (returnCode)
            {
                case 0:
                    UserRelationships.FillLists(relationships);
                    break;

                case 102:
                    return;
            }
        }

        private void CreateNewGame(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GameCreation());
        }

        private void JoinGame(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new JoinGame());
        }

        private void GoToProfile(object sender, RoutedEventArgs e)
        {
            PlayerStatsDC userStats;
            int returnCode;

            try
            {
                (returnCode, userStats) = _host.GetPlayerProfile(CurrentPlayer.Player.Username);
            }
            catch (CommunicationObjectFaultedException)
            {
                NavigationService.Navigate(new Login.Login());
                return;
            }

            switch (returnCode)
            {
                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    return;
                case 205:
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    return;
            }

            NavigationService.Navigate(new Profile(userStats));
        }

        public void ReceiveFriendRequest(PlayerDC player)
        {
            throw new NotImplementedException();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            LoginServiceClient host = new LoginServiceClient();

            try
            {
                host.Open();
            }
            catch (EndpointNotFoundException)
            {
                new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorServerConnection, 3).ShowDialog();
                return;
            }

            int returnCode = host.Logout(CurrentPlayer.Player.Username);
            host.Close();

            switch (returnCode)
            {
                case 0:
                    CurrentPlayer.Player = null;
                    NavigationService.Navigate(new Login.Login());
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    break;

                case 205:
                    break;
            }
        }

        private void GoToAchievements(object sender, RoutedEventArgs e)
        {
            (int returnCode, AchievementDC[] achievements) = _host.GetAchievements(CurrentPlayer.Player.Username);

            switch (returnCode)
            {
                case 0:
                    NavigationService.Navigate(new Achievements(achievements));
                    break;
            }
        }

        private void GoToConfiguration(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Menu.Configuration());
        }

        public void ReceiveGameInvitation(GameInvitationDC invitation)
        {
            GameInvitationPanel.Visibility = Visibility.Visible;
            GameInvitationPanel.IsEnabled = true;

            string friend = invitation.SenderUsername;

            GameInvitationPanel.MessageLabel.Text = friend + " " + Properties.Resources.gameInvitationMessage;
            _invitationGameCode = invitation.GameRoomCode;
        }

        private void JoinInvitationGame(object sender, RoutedEventArgs e)
        {
            GameInvitationPanel.Visibility = Visibility.Hidden;
            GameInvitationPanel.IsEnabled = false;

            if (!string.IsNullOrWhiteSpace(_invitationGameCode))
            {
                bool roomAvailable = new GameCodeVerificationServiceClient().VerifyGameRoom(_invitationGameCode);

                if (roomAvailable)
                {
                    NavigationService.Navigate(new Lobby(_invitationGameCode));
                }
                else
                {
                    new SelectionPopUpWindow(Properties.Resources.joinGameCantJoinTitle, Properties.Resources.joinGameCantJoin, 2).ShowDialog();
                }
            }

            _invitationGameCode = "";
        }

        private void RejectInvitationGame(object sender, RoutedEventArgs e)
        {
            GameInvitationPanel.Visibility = Visibility.Hidden;
            GameInvitationPanel.IsEnabled = false;

            _invitationGameCode = "";
        }

        private void OpenFriendsOverlay(object sender, RoutedEventArgs e)
        {
            FriendsMenuPanel.ClearLists();
            FriendsMenuPanel.Visibility = Visibility.Visible;
            FriendsMenuPanel.IsEnabled = true;
            (int returnCode, FriendDC[] relationships) = _host.GetAllRelationships(CurrentPlayer.Player.Username);

            switch (returnCode)
            {
                case 0:
                    UserRelationships.FillLists(relationships);
                    FriendsMenuPanel.FillLists();
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    FriendsMenuPanel.Visibility = Visibility.Hidden;
                    FriendsMenuPanel.IsEnabled = false;
                    return;
            }
        }

        private void CloseFriendsOverlay(object sender, RoutedEventArgs e)
        {
            FriendsMenuPanel.Visibility = Visibility.Hidden;
            FriendsMenuPanel.IsEnabled = false;
        }

        private void AddNewFriend(object sender, RoutedEventArgs e)
        {
            string friendUsername = FriendsMenuPanel.NewFriendUsernameTextBox.Text;
            int returnCode = 1;

            if (!string.IsNullOrWhiteSpace(friendUsername))
            {
                returnCode = _host.SendFriendRequest(CurrentPlayer.Player.Username, friendUsername);
            }

            switch (returnCode)
            {
                case 0:
                    new SelectionPopUpWindow(Properties.Resources.friendsRequestSentTitle, Properties.Resources.friendsRequestSent, 0).ShowDialog();
                    FriendsMenuPanel.NewFriendUsernameTextBox.Clear();
                    break;

                case 101:
                    // one of the usernames passed to the function was null
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    break;

                case 301:
                    new SelectionPopUpWindow(Properties.Resources.friendsAlreadyRequestedUser, Properties.Resources.friendsAlreadyRequestedUser, 3).ShowDialog();
                    break;

                case 302:
                    new SelectionPopUpWindow(Properties.Resources.friendsPendingRequestFromUser, Properties.Resources.friendsPendingRequestFromUser, 3).ShowDialog();
                    break;

                case 303:
                    new SelectionPopUpWindow(Properties.Resources.friendsAlreadyFriends, Properties.Resources.friendsAlreadyFriends, 3).ShowDialog();
                    break;

                case 304:
                    new SelectionPopUpWindow(Properties.Resources.friendsUserIsBlocked, Properties.Resources.friendsUserIsBlocked, 3).ShowDialog();
                    break;

                default:
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;
            }
        }

        private void AcceptFriendRequest(object sender, RoutedEventArgs e)
        {
            FriendInfoPanel friendPanel = (FriendInfoPanel)sender;
            int returnCode = _host.RespondFriendRequest(CurrentPlayer.Player.Username, friendPanel.UsernameLabel.Text, true);

            switch (returnCode)
            {
                case 0:
                    UserRelationships.FriendRequestsList.Remove(friendPanel.UsernameLabel.Text);
                    UserRelationships.FriendsList.Add(friendPanel.UsernameLabel.Text, 1);
                    FriendsMenuPanel.ClearLists();
                    FriendsMenuPanel.FillLists();
                    break;

                case 101:
                    // one of the usernames passed to the function was null
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    break;

                case 305:
                    // No request exists for the specified users
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 306:
                    // Wrong username for respondent
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 307:
                    // Wrong username for requester
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;
            }
        }

        private void RejectFriendRequest(object sender, RoutedEventArgs e)
        {
            FriendInfoPanel friendPanel = (FriendInfoPanel)sender;
            int returnCode = _host.RespondFriendRequest(CurrentPlayer.Player.Username, friendPanel.UsernameLabel.Text, false);

            switch (returnCode)
            {
                case 0:
                    UserRelationships.FriendRequestsList.Remove(friendPanel.UsernameLabel.Text);
                    FriendsMenuPanel.ClearLists();
                    FriendsMenuPanel.FillLists();
                    break;

                case 101:
                    // one of the usernames passed to the function was null
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    break;

                case 305:
                    // No request exists for the specified users
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 306:
                    // Wrong username for respondent
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 307:
                    // Wrong username for requester
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;
            }
        }

        private void BlockUser(object sender, RoutedEventArgs e)
        {
            FriendInfoPanel friendPanel = (FriendInfoPanel)sender;
            int returnCode = _host.BlockPlayer(CurrentPlayer.Player.Username, friendPanel.UsernameLabel.Text);

            switch (returnCode)
            {
                case 0:
                    UserRelationships.BloquedUsersList.Add(friendPanel.UsernameLabel.Text, 1);
                    UserRelationships.FriendsList.Remove(friendPanel.UsernameLabel.Text);
                    new SelectionPopUpWindow(Properties.Resources.friendsPlayerBlockedSuccessfully, Properties.Resources.friendsPlayerBlockedSuccessfully, 0).ShowDialog();
                    FriendsMenuPanel.ClearLists();
                    FriendsMenuPanel.FillLists();
                    break;

                case 101:
                    // one of the usernames passed to the function was null
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    break;

                case 308:
                    new SelectionPopUpWindow(Properties.Resources.errorCouldntBlockPlayer, Properties.Resources.errorCouldntBlockPlayer, 3).ShowDialog();
                    break;
            }
        }

        private void UnblockUser(object sender, RoutedEventArgs e)
        {
            FriendInfoPanel friendPanel = (FriendInfoPanel)sender;
            int returnCode = _host.UnblockPlayer(CurrentPlayer.Player.Username, friendPanel.UsernameLabel.Text);
             
            switch (returnCode)
            {
                case 0:
                    new SelectionPopUpWindow(Properties.Resources.friendsPlayerUnblockedSuccessfully, Properties.Resources.friendsPlayerBlockedSuccessfully, 0).ShowDialog();
                    UserRelationships.BloquedUsersList.Remove(friendPanel.UsernameLabel.Text);
                    UserRelationships.FriendsList.Add(friendPanel.UsernameLabel.Text, 1);
                    FriendsMenuPanel.ClearLists();
                    FriendsMenuPanel.FillLists();
                    break;

                case 101:
                    // one of the usernames passed to the function was null
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    break;

                case 308:
                    new SelectionPopUpWindow(Properties.Resources.errorCouldntBlockPlayer, Properties.Resources.errorCouldntBlockPlayer, 3).ShowDialog();
                    break;
            }
        }

        private void RemoveFriend(object sender, RoutedEventArgs e)
        {
            FriendInfoPanel friendPanel = (FriendInfoPanel)sender;
            int returnCode = _host.RemoveFriend(CurrentPlayer.Player.Username, friendPanel.UsernameLabel.Text);

            switch (returnCode)
            {
                case 0:
                    new SelectionPopUpWindow(Properties.Resources.friendsPlayerUnblockedSuccessfully, Properties.Resources.friendsPlayerBlockedSuccessfully, 0).ShowDialog();
                    UserRelationships.FriendsList.Remove(friendPanel.UsernameLabel.Text);
                    FriendsMenuPanel.ClearLists();
                    FriendsMenuPanel.FillLists();
                    break;

                case 101:
                    // one of the usernames passed to the function was null
                    new SelectionPopUpWindow(Properties.Resources.errorOccurredTitle, Properties.Resources.errorUnexpectedError, 3).ShowDialog();
                    break;

                case 102:
                    new SelectionPopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                    break;

                case 309:
                    new SelectionPopUpWindow(Properties.Resources.errorCouldntUnfriendPlayer, Properties.Resources.errorCouldntUnfriendPlayer, 3).ShowDialog();
                    break;
            }
        }

        private void GoToLeaderboard(object sender, RoutedEventArgs e)
        {
            LeaderboardStatsDC[] stats = _host.GetGlobalLeaderboard();
            NavigationService.Navigate(new Leaderboards(stats));
        }
    }
}
