﻿using System;
using Services;
using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client2
{
    class Program
    {
            public static void Main(string[] args)
            {
                Testing();
            }
            private static void Testing()
            {
                string repository = "";
                string filepath = "../../../json2.json";
                Channel channel = new Channel("127.0.0.1", 23456, ChannelCredentials.Insecure);
                var client = new Services.GameLog.GameLogClient(channel);
                //Console.WriteLine("Type in the Repository name, its key sensative");
                //repository = Console.ReadLine();

                var json = client.MainInteraction(new ParsingRequest { Address = repository });
                System.IO.File.WriteAllText(filepath, json.File);

                channel.ShutdownAsync().Wait();
               // Console.WriteLine("Press any key to exit...");
               // Console.ReadKey();
            }
        }

}
