﻿using BussinessLogic;
using DomainClasses;
using System;
using System.ServiceModel;

namespace Contracts
{
    public partial class ServiceImplementation : ILoginService
    {
        public int Login(string username, string password)
        {
            Console.WriteLine("Login attempt for user: " + username);
            Player player = new Player() { Email = "mail@example.com", UserName = username};
            PlayerData.AddPlayer(player, username);
            return 0;
        }

        public int Logout(string username)
        {
            PlayerData.RemovePlayer(username);
            Console.WriteLine("User " + username + " logged out");
            return 0;
        }
    }
}
