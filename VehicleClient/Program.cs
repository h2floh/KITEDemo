using System;

namespace VehicleClient
{
    using Microsoft.Azure.Devices.Client;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        // IoTHub
        // private static readonly string IoTHubName = Environment.GetEnvironmentVariable("IoTHubName");

        // DeviceId
        // private static readonly string VehicleId = Environment.GetEnvironmentVariable("DeviceId");

        // DeviceClient
        private static DeviceClient _deviceClient = DeviceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("ConnectionString"), TransportType.Mqtt);

        // VideoPlayer
        private static readonly VideoPlayer vPlayer = new VideoPlayer();

        // ImageNumber
        private static int imageNumber = -1;
        private static int imageNumberNext = 0;

        static void Main()
        {
            var cts = new CancellationTokenSource();

            var tasks = new List<Task>();
            tasks.Add(VideoLoop(cts.Token));
            tasks.Add(MessageReceiveLoop(cts.Token));
            Task.WaitAll(tasks.ToArray());

            cts.Dispose();
        }

        async static Task VideoLoop(CancellationToken ct)
        {
            long j = 0;
            // Main Loop
            while (!ct.IsCancellationRequested)
            {
                j++;
                if (imageNumberNext != imageNumber)
                {
                    Console.Clear();
                    vPlayer.ShowNextFrame((imageNumberNext % 6) + 1);
                    imageNumber = imageNumberNext;
                }
                

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        async static Task MessageReceiveLoop(CancellationToken ct)
        {
            // Main Loop
            while (!ct.IsCancellationRequested)
            {
                Message receivedMessage;
                string messageData;

                receivedMessage = await _deviceClient.ReceiveAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false);

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    // Console.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);
                    try
                    {
                        imageNumberNext = Convert.ToInt32(messageData, CultureInfo.InvariantCulture);
                    } catch (FormatException)
                    {
                        Console.WriteLine("Error while converting to Int\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);
                    }
                    

                    await _deviceClient.CompleteAsync(receivedMessage).ConfigureAwait(false);
                }
            }
        }
    }
}
