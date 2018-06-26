using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;


namespace Windows.DeviceClientProgram
{
    public partial class Form1 : Form
    {

        //private readonly string ConnectionString = "HostName=StackOverFlow-Demo01.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=k42hgvrGuyAYzn/xuMaKRZk58nFY9zZZKuJPxAQmk/A=";
        //private readonly string HostName = "StackOverFlow-Demo01.azure-devices.net";
       

        public Form1()
        {
            InitializeComponent();

            //Thread listenIoTHub = new Thread(ListenToCommandFromCloud)
            //{
            //    IsBackground = true,
            //    Name = "IoT Hub Listen"
            //};

            //listenIoTHub.Start();
            ListenToCommandFromCloud();
        }


        public void ListenToCommandFromCloud()
        {
            try
            {
                string deviceId = "1001";
                string deviceKey = "IPZUPRZCR4o629vC3qv5Xi/FfSO1cDxIco17E4lZngI=";
                string iotHubUri = "StackOverFlow-Demo01.azure-devices.net";
                DeviceClient deviceClient =  null ;

                //var deviceConnectionString = "HostName=" + HostName + ";DeviceId=" + deviceId + ";SharedAccessKey=" + deviceKey;

                while (true)
                {
                    try
                    {
                        if (deviceClient == null)
                        {
                            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Mqtt);
                        }

                        deviceClient.SetMethodHandlerAsync("CommandFromCloud", GetCommandFromCloud, null).Wait();
                    }
                    catch (Exception exception)
                    {
                    }

                    Thread.Sleep(50);
                }
            }
            catch (Exception exception)
            {

            }
        }

        private Task<MethodResponse> GetCommandFromCloud(MethodRequest methodRequest, object userContext)
        {
            try
            {
                if (!string.IsNullOrEmpty(methodRequest.DataAsJson))
                {
                    AzureIoTCommand azureIoTCommand = JsonConvert.DeserializeObject<AzureIoTCommand>(methodRequest.DataAsJson);
                    var returnPlayLoad = azureIoTCommand.PlayLoad;
                    returnPlayLoad = DecryptAzureIoTCommand(azureIoTCommand);
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(returnPlayLoad)), 200));
                }

                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("ERROR"), 200));

            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public PlayLoad DecryptAzureIoTCommand(AzureIoTCommand azureIoTCommand)
        {
            switch (azureIoTCommand.PlayLoad.Command)
            {
                case IoTCommandTypeEnum.ShowName:

                    lstCommands.Items.Add(azureIoTCommand.PlayLoad.Name);
                    azureIoTCommand.PlayLoad.Status = "OK";
                    azureIoTCommand.PlayLoad.Command = IoTCommandTypeEnum.ShowName;
                    return azureIoTCommand.PlayLoad;

                case IoTCommandTypeEnum.ChangeColor:

                    Random randomizer = new Random();
                    int r = randomizer.Next(0, 255), g = randomizer.Next(0, 255), b = randomizer.Next(0, 255);
                    button1.BackColor = Color.FromArgb(r, g, b);

                    azureIoTCommand.PlayLoad.Status = "OK";
                    azureIoTCommand.PlayLoad.Command = IoTCommandTypeEnum.ChangeColor;
                    return azureIoTCommand.PlayLoad;

                default:

                    azureIoTCommand.PlayLoad.Status = "NOT FOUND";
                    return azureIoTCommand.PlayLoad;
            }
        }
    }
}
