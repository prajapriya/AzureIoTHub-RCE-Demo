using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;



namespace Console.CreateNewDevice
{
    class Program
    {
        private static readonly string ConnectionString = "HostName=StackOverFlow-Demo01.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=k42hgvrGuyAYzn/xuMaKRZk58nFY9zZZKuJPxAQmk/A=";
        private static readonly string HostName = "StackOverFlow-Demo01.azure-devices.net";
        private static RegistryManager _registryManager;

        static void Main(string[] args)
        {
            RegisterNewDevice();
            System.Console.ReadLine();
        }

        public static async Task RegisterNewDevice()
        {
            var deviceId = "1001";
            Device device;
            try
            {
                _registryManager = RegistryManager.CreateFromConnectionString(ConnectionString);
                device = await _registryManager.AddDeviceAsync(new Device(deviceId));
                System.Console.WriteLine(device.Authentication.SymmetricKey.PrimaryKey);
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(deviceId);
                System.Console.WriteLine(device.Authentication.SymmetricKey.PrimaryKey);
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.Message);
            }
        }
    }
}
