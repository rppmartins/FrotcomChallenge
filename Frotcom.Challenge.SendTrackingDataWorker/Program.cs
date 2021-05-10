using Frotcom.Challenge.Data.Models;
using Frotcom.Challenge.Queue;
using Frotcom.Challenge.SendTrackingDataWorker.Cache;
using Frotcom.Challenge.SendTrackingDataWorker.ExternalService;
using Frotcom.Challenge.SendTrackingDataWorker.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Frotcom.Challenge.SendTrackingDataWorker
{
    class Program
    {
        /// <summary>
        /// FROTCOM CHALLENGE STARTS HERE
        /// </summary>
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ConsoleHostedService>();

                    services.AddSingleton<IQueueProcessorFactory, QueueProcessorFactory>();
                    services.AddSingleton<ICache<Packet>, PacketCache>();

                    services.AddScoped<IService<Packet>, ConsoleService>();
                })
                .RunConsoleAsync();
        }
    }

    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IQueueProcessorFactory processorFactory;
        private readonly ICache<Packet> cache;

        private bool isRunning = true;

        private QueueProcessorHost processorHost = null;
        private Timer timer = null;


        public ConsoleHostedService(
            IHostApplicationLifetime appLifetime,
            IQueueProcessorFactory processorFactory,
            ICache<Packet> cache
        )
        {
            _appLifetime = appLifetime;
            
            this.processorFactory = processorFactory;
            this.cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine("Frotcom Challenge \n");

                bool toPrint = true;

                while (isRunning)
                {
                    if (toPrint) PrintOptions();

                    string operation = Console.ReadLine();

                    if (ValidateOperation(operation))
                    {
                        toPrint = false;

                        ConsumeOperation(int.Parse(operation));
                    }
                    else
                    {
                        toPrint = true;
                    }
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void PrintOptions()
        {
            Console.WriteLine("1. Run");
            Console.WriteLine("2. Stop");
            Console.WriteLine("0. Exit");
        }

        private bool ValidateOperation(string operation)
        {
            try
            {
                if (!int.TryParse(operation, out int value))
                {
                    throw new Exception("Error: Not an int value.");
                }

                if (value < 0 || value > 2)
                {
                    throw new Exception("Error: Not an valid operation.");
                }

                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("\nInvalid option, please try again.");

                return false;
            }

        }

        private async void ConsumeOperation(int operation)
        {
            switch (operation)
            {
                case 0:
                    {
                        DisposeObjects();

                        isRunning = false;

                        _appLifetime.StopApplication();
                        break;
                    }
                case 1:
                    {
                        if (timer == null && processorHost == null)
                        {
                            timer = new Timer(new TimerCallback(cache.GetEntityValues), null, 0, 10000);

                            processorHost = new QueueProcessorHost(processorFactory, 100, 100);

                            await processorHost.Run();
                        }

                        break;
                    }
                case 2:
                    {
                        DisposeObjects();

                        Console.WriteLine("Stopped.");
                        break;
                    }
            }
        }

        private void DisposeObjects()
        {
            timer?.Dispose();
            timer = null;

            processorHost?.Stop();
            processorHost = null;
        }
    }
}
