﻿using BussinessLogic;
using DomainClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;

namespace Contracts
{
    public partial class ServiceImplementation : IPregameService
    {
        public void CreateGame(string username)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.state = GameRoomState.Waiting;
            gameRoom.Players.Add(username, OperationContext.Current.GetCallbackChannel<IPregameServiceCallback>());
            gameRoom.RoomCode = GameData.AddGameRoom(gameRoom);

            Console.WriteLine("sala de juego creada: " + gameRoom.RoomCode);

            OperationContext.Current.GetCallbackChannel<IPregameServiceCallback>().JoinGameResponse(gameRoom.RoomCode);
        }

        public void JoinGame(string username, string roomCode)
        {
            Console.WriteLine("hello");
        }

        public int LeaveGame(string username, string code)
        {
            GameData.RemovePlayerFromGameRoom(username, code);
            Console.WriteLine(username + " leaved the game");
            return 0;
        }

        public void SendMessage(Message message)
        {
            Console.Write("sending: " + message.Content + " from: " + message.AuthorUsername);
            GameRoom room = GameData.GetGameRoom(message.GameRoomCode);

            foreach (DictionaryEntry player in room.Players)
            {
                Console.WriteLine("Sending message to " + player.Key);
                (player.Value as IPregameServiceCallback).ReceiveMessage(message);
            }
        }
    }
}
