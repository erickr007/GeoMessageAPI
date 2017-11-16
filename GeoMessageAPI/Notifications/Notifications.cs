using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMessageAPI
{
    public class Notifications
    {

        #region Private Properties

        private static Notifications _instance;
        private NotificationHubClient _hub;

        #endregion


        #region Public Properties

        public static Notifications Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Notifications();

                return _instance;
            }
        }

        public NotificationHubClient Hub
        {
            get { return _hub; }
            set { _hub = value; }
        }

        #endregion


        #region Constructor

        private Notifications()
        {
            _hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://geomessageapi.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=GHtT6WOVhs2Eh2qYkWmLw00kotbibgE2n/eNH/FCx6Y=",
                "GeoMessageHub");
        }

        #endregion

    }
}
