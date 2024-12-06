﻿using DataAccess;
using DomainClasses;
using System;
using System.Data.Entity.Core;
using LanguageExt;
using BussinessLogic;
using log4net;
using MailKit.Net.Smtp;

namespace Contracts
{
    public partial class ServiceImplementation : ILoginService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ServiceImplementation));

        /// <summary>
        /// Create an account for a new user and send an email with the verification code
        /// </summary>
        /// <param name="player">PlayerDC object with the user's data</param>
        /// <returns>0 if the registration was successful, an error code otherwise</returns>
        /// <remarks>Error codes that can be returned: 101, 102, 201, 202</remarks>
        public int RegisterUser(PlayerDC player)
        {
            int codeResult = 0;
            Player newPlayer = new Player();

            try
            {
                newPlayer.Username = player.Username;
                newPlayer.Email = player.Email;
                newPlayer.Password = player.Password;
            }
            catch (ArgumentException)
            {
                codeResult = 101;
            }

            if (codeResult == 0)
            {
                try
                {
                    if (UserDB.GetPlayerByUsername(newPlayer.Username).IsSome)
                    {
                        codeResult = 201;
                    }
                    else if (UserDB.GetPlayerByEmail(newPlayer.Email).IsSome)
                    {
                        codeResult = 202;
                    }
                    else
                    {
                        UserDB.RegisterUser(newPlayer);
                        SendAccountVerificationCode(newPlayer.Username);
                    }
                }
                catch (EntityException error)
                {
                    _logger.Fatal("Database connection failed", error);
                    return 102;
                }
            }

            return codeResult;
        }

        /// <summary>
        /// Log in the Papayagrams application
        /// </summary>
        /// <param name="username">Username of the account</param>
        /// <param name="password">Password of the account</param>
        /// <returns>(0,Player) if the log in was succesful, (errorCode, null) otherwise</returns>
        /// <remarks>Error codes that can be returned: 102, 203, 204, 205, 206, 207</remarks>
        public (int errorCode, PlayerDC loggedPlayer) Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return (203, null);
            }
            else if (string.IsNullOrEmpty(password))
            {
                return (204, null);
            }

            int loginResult;
            try
            {
                loginResult = UserDB.LogIn(username, password);
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed", error);
                return (102, null);
            }

            if (loginResult == -1)
            {
                return (205, null);
            }
            else if (loginResult == -2)
            {
                _logger.InfoFormat("Login attempt failed (username id: {0})",username);
                return (206, null);
            }

            Option<Player> playerLogged = UserDB.GetPlayerByUsername(username);
            int code = 0;
            if (loginResult == 1)
            {
                code = 207;
            }

            PlayersOnlinePool.AddPlayer((Player)playerLogged.Case);
            _logger.InfoFormat("Login successful (username id: {0})", username);

            return (code, PlayerDC.ConvertToPlayerDC((Player)playerLogged.Case));
        }

        /// <summary>
        /// Take out the player from the application
        /// </summary>
        /// <param name="username">Username of the player</param>
        /// <returns>0 if the logut was succesfull, an error code otherwise</returns>
        /// <remarks>Error codes that can be returned: 101, 102, 205</remarks>
        public int Logout(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return 101;
            }

            int logoutResult;
            try
            {
                logoutResult = UserDB.LogOut(username);
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed", error);
                return 102;
            }

            if (logoutResult == 0)
            {
                return 205;
            }

            CallbacksPool.RemoveAllCallbacksChannels(username);
            PlayersOnlinePool.RemovePlayer(username);

            return 0;
        }

        /// <summary>
        /// Change the status of an account to verified
        /// </summary>
        /// <param name="username">Username of the player veifyig his account</param>
        /// <param name="code">Verification code that was sent through email</param>
        /// <returns>0 if the operation was successful, an error code otherwise</returns>
        /// <remarks>Error codes that can be returned: 101, 102, 208, 209</remarks>
        public int VerifyAccount(string username, string code)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(username))
            {
                return 101;
            }

            if (!VerificationCodesPool.AccountVerificationCodeCorrect(username, code))
            {
                _logger.InfoFormat("Account verification attempt failed (username id: {0})", username);
                return 208;
            }

            int codeResult;
            try
            {
                codeResult = UserDB.VerifyAccount(username);
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed", error);
                return 102;
            }

            if (codeResult == 1)
            {
                VerificationCodesPool.RemoveAccountVerificationCode(username);
                _logger.InfoFormat("Account verification successful (username id: {0})",username);
                return 0;
            }
            else
            {
                return 209;
            }
        }

        /// <summary>
        /// Send an email with the account verification code
        /// </summary>
        /// <param name="username">Username of the player to send the code</param>
        /// <returns>0 if the operation was successful, an error code otherwise</returns>
        /// <remarks>Error codes that can be returned: 102, 205, 104</remarks>
        public int SendAccountVerificationCode(string username)
        {
            Option<Player> player;

            try
            {
                player = UserDB.GetPlayerByUsername(username);
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed",error);
                return 102;
            }

            if (player.IsNone)
            {
                return 205;
            }

            Player playerChecking = (Player)player.Case;
            string code = VerificationCodesPool.GenerateAccountVerificationCode(username);

            try
            {
                return MailService.MailService.SendMail(playerChecking.Email, "Account verification code", $"Your account verification code is: {code}");
            }
            catch (SmtpCommandException error)
            {
                _logger.Warn($"Verificaton email sending failed (username id: {username})", error);
                return 104;
            }
        }

        /// <summary>
        /// Create a temporary account for a guest
        /// </summary>
        /// <returns>A PlayerDC object with the temporary account</returns>
        public PlayerDC AccessAsGuest()
        {
            return PlayerDC.ConvertToPlayerDC(PlayersOnlinePool.CreateGuestProfile());
        }

        /// <summary>
        /// Send an email with a password recovery PIN for recovering the password
        /// </summary>
        /// <param name="email">Email of the account to send the PIN</param>
        /// <returns>0 if the operation was successful, an error code otherwise</returns>
        /// <remarks>Error codes that can be returned: 101, 104</remarks>
        public int SendPasswordRecoveryPIN(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return 101;
            }

            string pin = VerificationCodesPool.GeneratePasswordRecoveryPIN(email);
            try
            {
                return MailService.MailService.SendMail(email, "Password recovery PIN", $"Your password recovery PIN is: {pin}");
            }
            catch (SmtpCommandException error)
            {
                _logger.WarnFormat("Password recovery email sending failed (Account email: {0})", email);
                return 104;
            }
        }

        /// <summary>
        /// Change the password of an account if the pin is correct
        /// </summary>
        /// <param name="pin">PIN that was sent to palyer's email</param>
        /// <param name="email">email of the account</param>
        /// <param name="newPassword">new password for account</param>
        /// <returns>0 if the operation was successful, an error code otherwise</returns>
        /// <remarks>Error codes that can be returned: 101, 102, 210, 211</remarks>
        public int RecoverPassword(string pin, string email, string newPassword)
        {
            if (string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                return 101;
            }

            if (!VerificationCodesPool.PasswordRecoveryPINCorrect(email, pin))
            {
                _logger.InfoFormat("Password recovery attempt failed (Account email: {0})", email);
                return 210;
            }

            int codeResult;
            try
            {
                codeResult = UserDB.UpdatePassword(email, newPassword);
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed", error);
                return 102;
            }

            if (codeResult == 0)
            {
                _logger.WarnFormat("Password recovery failed (Account email: {0})", email);
                return 211;
            }

            VerificationCodesPool.RemovePasswordRecoveryPIN(email);
            return 0;
        }
    }
}
