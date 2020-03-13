using System;

namespace VehicleClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        // IoTHub
        // private static readonly string IoTHubName = Environment.GetEnvironmentVariable("IoTHubName");

        // DeviceId
        // private static readonly string VehicleId = Environment.GetEnvironmentVariable("DeviceId");

        // VideoPlayer
        private static readonly VideoPlayer vPlayer = new VideoPlayer();

        static void Main()
        {
            var cts = new CancellationTokenSource();

            var tasks = new List<Task>();
            tasks.Add(VideoLoop(cts.Token));
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
                Console.Clear();

                vPlayer.ShowNextFrame(j);

                await Task.Delay(1000).ConfigureAwait(false);
            }
        }
    }
}
