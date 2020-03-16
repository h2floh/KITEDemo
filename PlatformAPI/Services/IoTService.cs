using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Text;
using System.Resources;
using System.Globalization;

namespace PlatformAPI.Services
{
    public class IoTService
    {
        private readonly IConfiguration _configuration;
        private readonly ServiceClient _serviceClient;

        public IoTService(IConfiguration configuration)
        {
            ResourceManager rm = new ResourceManager("Messages",
                               typeof(IoTService).Assembly);

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration), rm.GetString("ConfigurationNull", CultureInfo.CurrentCulture));
            }
            _configuration = configuration;
            _serviceClient = ServiceClient.CreateFromConnectionString(configuration["IoTHubConnectionString"]);             
        }

        public Boolean SendMessage(BackgroundImage content)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(content?.Number.ToString(CultureInfo.InvariantCulture)));

            _serviceClient.SendAsync(content.VehicleId.ToString(CultureInfo.InvariantCulture), commandMessage)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            
            commandMessage.Dispose();

            return true;
        }
    }
}
