using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Web.DeviceControl.Models;

namespace Web.DeviceControl.Controllers
{
    public class HomeController : Controller
    {

        private RegistryManager registryManager;

        private readonly string ConnectionString = "HostName=StackOverFlow-Demo01.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=k42hgvrGuyAYzn/xuMaKRZk58nFY9zZZKuJPxAQmk/A=";
        private readonly string HostName = "StackOverFlow-Demo01.azure-devices.net";
        static ServiceClient serviceClient;
        string deviceId = "1001";


        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> PostMyName(string myName)
        {
            var azureIoTCommand = new AzureIoTCommand
            {
                MethodName = "CommandFromCloud",
                ResponseTimeoutInSeconds = 200,
                PlayLoad = new PlayLoad
                {
                    Command = IoTCommandTypeEnum.ShowName,
                    Name = myName,
                    Status = "OK"
                }
            };

            var playLoad = await SendCommandToIotDevice(azureIoTCommand);

            return View(playLoad);
        }

        [HttpGet]
        public async Task<ActionResult> ChangeColor()
        {
            var azureIoTCommand = new AzureIoTCommand
            {
                MethodName = "CommandFromCloud",
                ResponseTimeoutInSeconds = 200,
                PlayLoad = new PlayLoad
                {
                    Command = IoTCommandTypeEnum.ChangeColor,
                    Name = "",
                    Status = "OK"
                }
            };

            var playLoad = await SendCommandToIotDevice(azureIoTCommand);

            return View(playLoad);
        }

        public async Task<PlayLoad> SendCommandToIotDevice(AzureIoTCommand azureIoTCommand)
        {
            serviceClient = ServiceClient.CreateFromConnectionString(ConnectionString);

            var message = JsonConvert.SerializeObject(azureIoTCommand);
            var methodInvocation = new CloudToDeviceMethod(azureIoTCommand.MethodName) { ResponseTimeout = TimeSpan.FromSeconds(30) };
            methodInvocation.SetPayloadJson(message);

            var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);
            string responseMessage = response.GetPayloadAsJson();
            var playLoad = JsonConvert.DeserializeObject<PlayLoad>(responseMessage);
            return playLoad;
        }
    }
}