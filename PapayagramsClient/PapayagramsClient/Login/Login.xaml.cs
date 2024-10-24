﻿using System;
using System.IO;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PapayagramsClient.Login
{
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            ClearErrorLabels();

            if (string.IsNullOrEmpty(UsernameTextbox.Text))
            {
                UsernameErrorText.Content = Properties.Resources.globalEmptyUsername;
                return;
            }
            if (string.IsNullOrEmpty(PasswordTextbox.Text))
            {
                PasswordErrorText.Content = Properties.Resources.signInEmptyPassword;
                return;
            }

            string Username = UsernameTextbox.Text;
            string Password = PasswordTextbox.Text;

            PapayagramsService.LoginServiceClient host = new PapayagramsService.LoginServiceClient();
            try
            {
                host.Open();
            }
            catch (EndpointNotFoundException)
            {
                new PopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorServerConnection, 3).ShowDialog();
                return;
            }

            (int err, PapayagramsService.PlayerDC player) = host.Login(Username, Password);
            SigninButton.IsEnabled = false;
            CurrentPlayer.Player = player;
            host.Close();

            if (err != 0)
            {
                switch (err)
                {
                    case 102:
                        new PopUpWindow(Properties.Resources.errorConnectionTitle, Properties.Resources.errorDatabaseConnection, 3).ShowDialog();
                        break;
                    case 203:
                        UsernameErrorText.Content = Properties.Resources.globalEmptyUsername;
                        break;
                    case 204:
                        PasswordErrorText.Content = Properties.Resources.signInEmptyPassword;
                        break;
                    case 205:
                        PasswordErrorText.Content = Properties.Resources.signInWrongCredentials;
                        break;
                    case 206:
                        PasswordErrorText.Content = Properties.Resources.signInWrongCredentials;
                        break;
                }

                SigninButton.IsEnabled = true;
                return;
            }

            SigninButton.IsEnabled = true;

            this.NavigationService.Navigate(new MainMenu());
        }

        private void RegisterNewUser(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Register());
        }

        private void ClearErrorLabels()
        {
            PasswordErrorText.Content = "";
            UsernameErrorText.Content = "";
        }
    }
}
