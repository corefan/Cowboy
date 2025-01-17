﻿using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cowboy.Logging;
using Cowboy.Logging.NLogIntegration;

namespace Cowboy.Sockets.TestTcpSocketSaeaClient
{
    class Program
    {
        static TcpSocketSaeaClient _client;

        static void Main(string[] args)
        {
            NLogLogger.Use();

            try
            {
                var config = new TcpSocketSaeaClientConfiguration();

                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 22222);
                _client = new TcpSocketSaeaClient(remoteEP, new SimpleMessageDispatcher(), config);
                _client.Connect().Wait();

                Console.WriteLine("TCP client has connected to server [{0}].", remoteEP);
                Console.WriteLine("Type something to send to server...");
                while (true)
                {
                    try
                    {
                        string text = Console.ReadLine();
                        if (text == "quit")
                            break;
                        Task.Run(async () =>
                        {
                            await _client.SendAsync(Encoding.UTF8.GetBytes(text));
                            Console.WriteLine("Client [{0}] send text -> [{1}].", _client.LocalEndPoint, text);
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                _client.Close().Wait();
                Console.WriteLine("TCP client has disconnected from server [{0}].", remoteEP);
            }
            catch (Exception ex)
            {
                Logger.Get<Program>().Error(ex.Message, ex);
            }

            Console.ReadKey();
        }
    }
}
