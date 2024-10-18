﻿using BussinessLogic;
using DataAccess;
using DomainClasses;
using LanguageExt;
using System;

namespace Contracts
{
    public partial class ServiceImplementation : ILoginService
    {
        public int Login(string username, string password)
        {
            int succesfulLogin;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password cannot be empty");
            }
            else
            {
                Option<Player> foundPlayer = UserDB.GetPlayerByUsername(username);
                if (foundPlayer.IsSome)
                {
                    succesfulLogin = UserDB.LogIn(username, password);
                }
                else
                {
                    throw new Exception("Player not found");
                }
            }

            return succesfulLogin;
        }

        public int Logout(string username)
        {
            PlayerData.RemovePlayer(username);
            Console.WriteLine("User " + username + " logged out");
            return 0;
        }

        private PlayerDC ConvertPlayerToDataContract(Player player)
        {
            return new PlayerDC()
            {
                Id = player.Id,
                Username = player.Username,
                Email = player.Email,
                Password = player.Password
            };
        }
    }
}
