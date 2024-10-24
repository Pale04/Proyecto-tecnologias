﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainClasses;
using LanguageExt;
using System;
using System.Data.Entity.Core;
using Tests;

namespace DataAccess.Tests
{
    [TestClass()]
    public class UserDBTests
    {
        private readonly Player _registeredPlayer1 = new Player()
        {
            Id = 1,
            Username = "Pale04",
            Email = "epalemolina@hotmail.com",
            Password = "040704"
        };
        private readonly Player _registeredPlayer2 = new Player()
        {
            Id = 2,
            Username = "David04",
            Email = "david@gmail.com",
            Password = "040704"
        };

        [TestInitialize()]
        public void SetUp()
        {
            UserDB.RegisterUser(_registeredPlayer1);
            UserDB.RegisterUser(_registeredPlayer2);
        }

        [TestCleanup()]
        public void CleanUp()
        {
            DataBaseOperation.RebootDataBase();
        }

        [TestMethod()]
        public void RegisterUserSuccesfulTest()
        {
            Player newPlayer = new Player()
            {
                Username = "Pale47",
                Email = "epalemolina@gmail.com",
                Password = "asdfl_´.468*-"
            };

            int expected = 6;
            int result = UserDB.RegisterUser(newPlayer);

            Assert.AreEqual(expected, result, "RegisterUserSuccesfulTest");
        }

        [TestMethod()]
        public void RegisterUserEmptyTest()
        {
            try
            {
                UserDB.RegisterUser(new Player());
                Assert.Fail("RegisterUserEmptyTest");
            }
            catch (Exception error)
            {
                Assert.IsInstanceOfType(error, typeof(EntityCommandExecutionException), "RegisterUserEmptyTest");
            }
        }

        [TestMethod()]
        public void LogInSuccessfulTest()
        {
            int expected = 0;
            int result = UserDB.LogIn(_registeredPlayer1.Username, _registeredPlayer1.Password);
            Assert.AreEqual(expected, result, "LogInSuccessfulTest");
        }

        //It is the same case when the username is null
        [TestMethod()]
        public void LogInNonExistentAccountTest()
        {
            int expected = -1;
            int result = UserDB.LogIn("Pale", "1");
            Assert.AreEqual(expected, result, "LogInNonExistentAccountTest");
        }

        //It is the same case when the password is null
        [TestMethod()]
        public void LogInPasswordIncorrectTest()
        {
            int expected = -2;
            int result = UserDB.LogIn(_registeredPlayer1.Username, "1");
            Assert.AreEqual(expected, result, "LogInPasswordIncorrectTest");
        }

        [TestMethod()]
        public void GetPlayerByUsernameSuccessfulTest()
        {
            Option<Player> result = UserDB.GetPlayerByUsername(_registeredPlayer1.Username);
            Assert.AreEqual(_registeredPlayer1, result.Case, "GetPlayerByUsernameSuccessfulTest");
        }

        //It it the same case when the username is null
        [TestMethod()]
        public void GetPlayerByUsernameNonExistentTest()
        {
            Option<Player> result = UserDB.GetPlayerByUsername("Pale");
            Assert.IsTrue(result.IsNone, "GetPlayerByUsernameNonExistentTest");
        }

        [TestMethod()]
        public void GetPlayerByEmailSuccessfulTest()
        {
            Option<Player> result = UserDB.GetPlayerByEmail(_registeredPlayer1.Email);
            Assert.AreEqual(_registeredPlayer1, result.Case, "GetPlayerByEmailSuccessfulTest");
        }

        //It is the same case when the email is null
        [TestMethod()]
        public void GetPlayerByEmailInexistentTest()
        {
            Option<Player> result = UserDB.GetPlayerByEmail("pale@gmail.com");
            Assert.IsTrue(result.IsNone, "GetPlayerByEmailInexistentTest");
        }

        [TestMethod()]
        public void LogOutSuccessfulTest()
        {
            int expected = 1;
            int result = UserDB.LogOut(_registeredPlayer1.Username);
            Assert.AreEqual(expected, result, "LogOutSuccessfulTest");
        }

        [TestMethod()]
        public void LogOutNonExistentAccountTest()
        {
            int expected = 0;
            int result = UserDB.LogOut("Pale");
            Assert.AreEqual(expected, result, "LogOutNonExistentAccountTest");
        }

        [TestMethod()]
        public void LogOutNullUsernameTest()
        {
            int expected = 0;
            int result = UserDB.LogOut(null);
            Assert.AreEqual(expected, result, "LogOutNullUsernameTest");
        }

        [TestMethod()]
        public void SearchNoFriendPlayerSuccessfulTest()
        {
            Option<Player> result = UserDB.SearchNoFriendPlayer(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Assert.AreEqual(_registeredPlayer2, result.Case, "SearchNoFriendPlayerSuccessfulTest");
        }

        [TestMethod()]
        public void SearchNoFriendPlayerPendingRequestTest()
        {
            UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Option<Player> result = UserDB.SearchNoFriendPlayer(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Assert.AreEqual(_registeredPlayer2, result.Case, "SearchNoFriendPlayerPendingRequestTest");
        }

        [TestMethod()]
        public void SearchNoFriendPlayerBlockedTest()
        {
            UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);

            //TODO: hacer método para bloquear a un jugador

            Option<Player> result = UserDB.SearchNoFriendPlayer(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Assert.AreEqual(_registeredPlayer2, result.Case, "SearchNoFriendPlayerBlockedTest");
        }

        [TestMethod()]
        public void SearchNoFriendPlayerHimSelfTest()
        {
            Option<Player> result = UserDB.SearchNoFriendPlayer(_registeredPlayer1.Username, _registeredPlayer1.Username);
            Assert.IsTrue(result.IsNone, "SearchNoFriendPlayerHimSelfTest");
        }

        [TestMethod()]
        public void SearchNoFriendPlayerAlreadyFriendsTest()
        {
            UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);
            
            //TODO Add the method to accept the friend request

            Option<Player> result = UserDB.SearchNoFriendPlayer(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Assert.IsTrue(result.IsNone, "SearchNo FriendPlayerAlreadyFriendsTest");
        }

        [TestMethod()]
        public void SearchNoFriendPlayerNonExistentTest()
        {
            Option<Player> result = UserDB.SearchNoFriendPlayer(_registeredPlayer1.Username, "Pale");
            Assert.IsTrue(result.IsNone, "SearchNoFriendPlayerNonExistentTest");
        }

        [TestMethod()]
        public void SendFriendRequestSuccessfulTest()
        {
            int expected = 0;
            int result = UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Assert.AreEqual(expected, result, "SendFriendRequestSuccessfulTest");
        }

        [TestMethod()]
        public void SendFriendRequestSenderRequestedBefore()
        {
            int expected = -1;
            UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);
            int result = UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Assert.AreEqual(expected, result, "SendFriendRequestSenderRequestedBefore");
        }

        [TestMethod()]
        public void SendFriendRequestReceiverRequestedBefore()
        {
            int expected = -2;
            UserDB.SendFriendRequest(_registeredPlayer2.Username, _registeredPlayer1.Username);
            int result = UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Assert.AreEqual(expected, result, "SendFriendRequestReceiverRequestedBefore");
        }

        [TestMethod()]
        public void SendFriendRequestAlreadyFriendsTest()
        {
            int expected = -3;
            UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);

            //TODO Add the method to accept the friend request

            int result = UserDB.SendFriendRequest(_registeredPlayer1.Username, _registeredPlayer2.Username);
            Assert.AreEqual(expected, result, "SendFriendRequestAlreadyFriendsTest");
        }
    }
}