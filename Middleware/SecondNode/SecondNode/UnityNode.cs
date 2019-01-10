﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Services;

namespace Middleware
{
    class UnityNode
    {
        public static void Start(string[] args)
        {
            // initiate variables 
            string repository;
            string UserName;
            string ProjectName;
            string filepath;
            string page;
            string filetype = ".json";
            string IP = "192.168.43.56";
            int Port = 23456;

            // Checks if the we succesfully got the correct amount of arguments
            // otherwise we set it to a pre-know project
            if (args.Length >= 3)
            {
                UserName = args[0];
                ProjectName = args[1];
                page = args[2];

            }
            else if (args.Length == 2)
            {
                UserName = args[0];
                ProjectName = args[1];
                page = "t";
            }
            else
            {
                // this should be changed as it reference a none java project
                UserName = "dit341";
                ProjectName = "express-template";
                page = "t";
            }

            //This could already be done before we give the parameters 
            //but doing it this way makes it easier to name the json file based on the 
            // project that you are getting
            repository = UserName + "/" + ProjectName + " " + page;

            //This will set up the channel we will use to communicate with the "Server" process
            // the first value refers to the IP of the PC that is running the Server process, the second is what port to send it to
            // and the third is ChannelCredentials that have to match the one of the Server
            Channel channel = new Channel(IP, Port, ChannelCredentials.Insecure);

            // This makes a new client variable referencing the Trial and TrialGrpc files
            var client = new Services.GameLog.GameLogClient(channel);

            //This makes a new json variable and provides a request message, the return message will then be saved in the json var
            var json = client.MainInteraction(new ParsingRequest { Address = repository });

            //sets the variable so we always make the file in the same place no matter the PC that runs it
            //the filetype variable at the end decides what kind of file we will make.
            filepath = System.AppDomain.CurrentDomain.BaseDirectory + "../Project" + filetype;

            //This will write the file on the computer
            System.IO.File.WriteAllText(filepath, json.File);
            channel.ShutdownAsync().Wait();
        }
    }
}