using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;


namespace Win.DeviceProgram
{
    public partial class Form1 : Form
    {
        private string name = "";
        private bool changeColor = false;
        private Color backColor;

        public Form1()
        {
            InitializeComponent();
            Thread listenIoTHub = new Thread(ListenToCommandFromCloud)
            {
                IsBackground = true,
                Name = "IoT Hub Listen"
            };

            listenIoTHub.Start();
        }

        public void ListenToCommandFromCloud()
        {
            try
            {
                string deviceId = "1001";
                string deviceKey = "IPZUPRZCR4o629vC3qv5Xi/FfSO1cDxIco17E4lZngI=";
                string iotHubUri = "StackOverFlow-Demo01.azure-devices.net";
                DeviceClient deviceClient = null;

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
                    lstCommands.Invoke((MethodInvoker)(() => lstCommands.Items.Add(azureIoTCommand.PlayLoad.Name)));
                    azureIoTCommand.PlayLoad.Status = "Name Updated";
                    azureIoTCommand.PlayLoad.Command = IoTCommandTypeEnum.ShowName;
                    return azureIoTCommand.PlayLoad;

                case IoTCommandTypeEnum.ChangeColor:
                    changeColor = true;
                    Random randomizer = new Random();
                    int r = randomizer.Next(0, 255), g = randomizer.Next(0, 255), b = randomizer.Next(0, 255);
                    button1.Invoke((MethodInvoker)(() => button1.BackColor = Color.FromArgb(r, g, b)));
                    azureIoTCommand.PlayLoad.Name = Color.FromArgb(r, g, b).ToString();
                    azureIoTCommand.PlayLoad.Status = "Color Changed";
                    azureIoTCommand.PlayLoad.Command = IoTCommandTypeEnum.ChangeColor;
                    return azureIoTCommand.PlayLoad;

                default:

                    azureIoTCommand.PlayLoad.Status = "NOT FOUND";
                    return azureIoTCommand.PlayLoad;
            }
        }

       

        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }
    }
}
