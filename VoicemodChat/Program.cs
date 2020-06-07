using Fleck;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoicemodChat.Services;
using VoicemodChat.Services.Interfaces;

namespace VoicemodChat
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Port required as input parameter.\nPress any key to exit.");
                Console.ReadKey();
            }
            else
            {
                bool isNumeric = Int32.TryParse(args[0], out int port);
                if (!isNumeric)
                {
                    Console.WriteLine("The input parameter must be numeric.\nPress any key to exit.");
                    Console.ReadKey();
                }
                else
                {
                    var serviceProvider = new ServiceCollection()
                        .AddScoped<IChatService, ChatService>()
                        .BuildServiceProvider();

                    IChatService chatService = serviceProvider.GetService<IChatService>();
                    chatService.Start(port);
                }
            }
        }
    }
}
