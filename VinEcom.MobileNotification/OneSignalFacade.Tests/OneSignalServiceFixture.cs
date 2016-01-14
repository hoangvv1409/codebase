using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace OneSignalFacade.Tests
{
    [TestClass]
    public class OneSignalServiceFixture
    {
        [TestMethod]
        public void PostItem()
        {
            OneSignalService oneSignalService = new OneSignalService("https://onesignal.com/api/v1/notifications", "cae20aef-06ac-463e-a1da-bedf7200b64d", "NzUyMTJhNjQtNGQzMS00NWUxLTkzNzktZWVhZjdkNjhhODRj");

            var data = new { Source = 1, Type = 2 };
            var tag = new { key = "UserID", relation = "=", value = "11253" };

            OneSignalModel model = new OneSignalModel()
            {
                app_id = "cae20aef-06ac-463e-a1da-bedf7200b64d",
                tags = new object[] { tag },
                contents = new { en = "English Message" },
                data = data
            };

            string content = JsonConvert.SerializeObject(model);

            oneSignalService.CreateNotification(content).Wait();
        }
    }
}
