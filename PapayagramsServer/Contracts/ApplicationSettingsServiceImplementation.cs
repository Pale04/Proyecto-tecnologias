﻿using BussinessLogic;
using DataAccess;
using DomainClasses;
using LanguageExt;
using System;
using System.Data.Entity.Core;

namespace Contracts
{
    public partial class ServiceImplementation : IApplicationSettingsService
    {
        public (int returnCode, ApplicationSettingsDC configuration) GetApplicationSettings(string username)
        {
            Option<ApplicationSettings> wrappedConfiguration;
            try
            {
                wrappedConfiguration = UserDB.GetApplicationSettings(username);
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed. Get application settings", error);
                return (102, null);
            }

            if (wrappedConfiguration.IsSome)
            {
                return (0, ApplicationSettingsDC.ConvertToApplicationSettingsDC((ApplicationSettings)wrappedConfiguration.Case));
            }
            else
            {
                _logger.WarnFormat("Application settings retrieval failed (Username: {0})", username);
                return (205, null);
            }
        }

        public int UpdateAplicationSettings(string username, ApplicationSettingsDC updatedConfiguration)
        {
            if (string.IsNullOrEmpty(username) || updatedConfiguration == null)
            {
                return 101;
            }

            int operationResult;
            try
            {
                operationResult = UserDB.UpdateApplicationSettings(username, new ApplicationSettings()
                {
                    PieceColor = updatedConfiguration.PieceColor,
                    SelectedLanguage = (ApplicationLanguage)Enum.Parse(typeof(ApplicationLanguage), updatedConfiguration.SelectedLanguage.ToString()),
                    Cursor = updatedConfiguration.Cursor
                });
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed. Update application settings attempt", error);
                return 102;
            }

            if (operationResult == 0)
            {
                _logger.WarnFormat("Application settings update failed (Username: {0})", username);
                return 501;
            }

            return 0;
        }

        public int UpdatePassword(string username, string currentPassword, string newPasswordord)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPasswordord))
            {
                return 101;
            }

            int operationResult;
            try
            {
                operationResult = UserDB.UpdatePassword(username, currentPassword, newPasswordord);
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed. Update password attempt", error);
                return 102;
            }

            if (operationResult == -1)
            {
                _logger.InfoFormat("Password update failed, current password wrong (Username: {0})", username);
                return 503;
            }
            return 0;
        }

        public int UpdateProfileIcon(string username, int profileIcon)
        {
            if (string.IsNullOrEmpty(username) || profileIcon < 1)
            {
                return 101;
            }

            int operationResult;
            try
            {
                operationResult = UserDB.UpdateProfileIcon(username, profileIcon);
            }
            catch (EntityException error)
            {
                _logger.Fatal("Database connection failed. Update profile icon attempt", error);
                return 102;
            }

            if (operationResult == -1)
            {
                _logger.WarnFormat("Profile icon update failed (Username: {0}, ProfileIcon: {1})", username, profileIcon);
                return 502;
            }

            PlayersOnlinePool.GetPlayer(username).ProfileIcon = profileIcon;
            return 0;
        }
    }
}
