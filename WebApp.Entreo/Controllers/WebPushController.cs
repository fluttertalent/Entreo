using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApp.Entreo.Shared.Models;
using WebPush;

namespace WebApp.Entreo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebPushController : Controller
    {
        private readonly IConfiguration _configuration;

        public WebPushController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendNotificationAsync(string title, string message, string email = "example@example.com")
        {
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    title,
                    message,
                    //url = "https://codelabs.developers.google.com/codelabs/push-notifications#0",
                });

                //var keys = VapidHelper.GenerateVapidKeys();
                //ViewBag.PublicKey = keys.PublicKey;
                //ViewBag.PrivateKey = keys.PrivateKey;

                var vapidPublicKey = _configuration.GetSection("VapidKeys")["PublicKey"];
                var vapidPrivateKey = _configuration.GetSection("VapidKeys")["PrivateKey"];

                string fileName = "subscription.json";
                var json = System.IO.File.ReadAllText(fileName);
                var subscription = JsonSerializer.Deserialize<NotificationSubscription>(json);

                var endpoint = subscription.Url;
                var p256dh = subscription.P256dh;
                var auth = subscription.Auth;

                var pushSubscription = new PushSubscription(endpoint, p256dh, auth);
                var vapidDetails = new VapidDetails("mailto:" + email, vapidPublicKey, vapidPrivateKey);

                var webPushClient = new WebPushClient();
                webPushClient.SendNotification(pushSubscription, payload, vapidDetails);

                return Ok();
            }
            catch (WebPushException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        //Use Firebase
        [Route("sendfb")]
        public async Task<IActionResult> SendNotificationUsingFirebaseAsync(string title, string message, string email = "example@example.com")
        {
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    title,
                    message,
                    //url = "https://codelabs.developers.google.com/codelabs/push-notifications#0",
                });

                var vapidPublicKey = _configuration.GetSection("VapidKeys")["PublicKey"];
                var vapidPrivateKey = _configuration.GetSection("VapidKeys")["PrivateKey"];

                string fileName = "subscription.json";
                var json = System.IO.File.ReadAllText(fileName);
                var subscription = JsonSerializer.Deserialize<NotificationSubscription>(json);

                var endpoint = subscription.Url;
                var p256dh = subscription.P256dh;
                var auth = subscription.Auth;

                var pushSubscription = new PushSubscription(endpoint, p256dh, auth);
                var vapidDetails = new VapidDetails("mailto:" + email, vapidPublicKey, vapidPrivateKey);

                var webPushClient = new WebPushClient();
                webPushClient.SendNotification(pushSubscription, payload, vapidDetails);

                return Ok();
            }
            catch (WebPushException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> ListReceiversAsync()
        {
            try
            {
                string fileName = "subscription.json";
                var json = System.IO.File.ReadAllText(fileName);
                var subscription = JsonSerializer.Deserialize<NotificationSubscription>(json);

                return Ok(subscription);
            }
            catch (WebPushException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("subscribe")]
        public async Task RegisterSubscription(NotificationSubscription subscription)
        {
            if (subscription == null) return;

            string fileName = "subscription.json";
            await using FileStream createStream = System.IO.File.Create(fileName);
            await JsonSerializer.SerializeAsync(createStream, subscription);
        }

        [HttpPost]
        [Route("unsubscribe")]
        public void Delete()
        {
        }
    }
}