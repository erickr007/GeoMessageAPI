using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using System.Net.Http;
using System.Net;

namespace GeoMessageAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Register")]
    public class RegisterController : Controller
    {

        #region Private Properties

        private NotificationHubClient _hub;

        #endregion


        #region Private Classes

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        #endregion


        #region Constructor

        public RegisterController()
        {
            _hub = Notifications.Instance.Hub;
        }

        #endregion


        #region Actions

        // POST api/register
        // This creates a registration id
        public async Task<string> Post(string handle = null)
        {
            string newRegistrationId = null;

            if(handle != null)
            {
                var registrations = await _hub.GetRegistrationsByChannelAsync(handle, 100);
                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await _hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
                newRegistrationId = await _hub.CreateRegistrationIdAsync();

            return newRegistrationId;
        }

        // PUT api/register/5
        // This creates or updates a registration (with provided channelURI) at the specified id
        public async Task<HttpResponseMessage> Put(string id, string username, DeviceRegistration deviceUpdate)
        {
            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "gcm":
                    registration = new GcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    throw new Exception(HttpStatusCode.BadRequest.ToString());
            }

            registration.RegistrationId = id;

            // add check if user is allowed to add these tags
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            registration.Tags.Add("username:" + username);

            try
            {
                await _hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                ReturnGoneIfHubResponseIsGone(e);
            }
            
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE api/register/5
        public async Task<HttpResponseMessage> Delete(string id)
        {
            await _hub.DeleteRegistrationAsync(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse)webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }

        #endregion

    }
}