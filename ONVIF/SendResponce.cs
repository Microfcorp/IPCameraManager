using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel;
using IPCamera.ServiceReference1;
using IPCamera.OPTZ;
using IPCamera.Settings;
using System.Drawing;
using System.Threading;
using IPCamera.OREP;
using IPCamera.OREC;
using IPCamera.OEVE;

namespace IPCamera.ONVIF
{
    class SendResponce
    {
        public static string GetSnapsotURL(Structures set, int profile = 1)
        {
            try
            {
                var tok = set.GetMediaTokens().FirstOrDefault();
                if (tok == "" || !set.IsActive) return "";

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Media");
                MediaClient mediaClient = new MediaClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                MediaUri mediaUri = mediaClient.GetSnapshotUri(set.GetMediaTokens()[profile]);
                return mediaUri.Uri;
            }
            catch
            {
                return "";
            }
        }

        public static MediaClient GetMediaBase(Structures set)
        {
            try
            {
                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Media");
                MediaClient mediaClient = new MediaClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;
                return mediaClient;
            }
            catch
            {
                return null;
            }
        }
        public static MediaClient GetMediaBase(string url, string login, string pass)
        {
            try
            {
                if (!Network.Ping.IsOKServer(url)) return null;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(url + "/onvif/Media");
                MediaClient mediaClient = new MediaClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = login;
                mediaClient.ClientCredentials.UserName.Password = pass;
                return mediaClient;
            }
            catch
            {
                return null;
            }
        }

        [Obsolete]
        public static string GetMediaToken(Structures set, int profile = 0)
        {
            try
            {
                return GetMediaBase(set).GetProfiles()[profile].token;
            }
            catch
            {
                return "";
            }
        }

        public static Size GetVideoResolution(Structures set, int profiles = 0)
        {
            try
            {
                var c = GetProfiles(set)[profiles].VideoEncoderConfiguration.Resolution;
                return new Size(c.Width, c.Height);
            }
            catch
            {
                return new Size(1, 1);
            }
        }
        public static Profile[] GetProfiles(Structures set)
        {
            try
            {
                if (!Network.Ping.IsOnline(set.IP, (int)set.ONVIFPort)) return new Profile[] { };

                var mediaClient = GetMediaBase(set).GetProfiles() ?? new Profile[] { };
                return mediaClient != null ? mediaClient : new Profile[] { };
            }
            catch
            {
                return new Profile[] {  };
            }
        }

        public static Profile[] GetProfiles(string url, string login, string pass)
        {
            try
            {
                MediaClient mediaClient = GetMediaBase(url, login, pass);
                return mediaClient.GetProfiles();
            }
            catch
            {
                return new Profile[] { new Profile() };
            }
        }

        public static string GetStreamURL(Structures set, int profile = 0)
        {
            try
            {
                if (set.GetMediaTokens().FirstOrDefault() == "" || !set.IsActive) return "";
                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Media");
                MediaClient mediaClient = new MediaClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                ServiceReference1.StreamSetup ss = new ServiceReference1.StreamSetup();
                ss.Stream = ServiceReference1.StreamType.RTPUnicast;
                ss.Transport = new ServiceReference1.Transport();
                ss.Transport.Protocol = ServiceReference1.TransportProtocol.RTSP;
                MediaUri mediaUri = mediaClient.GetStreamUri(ss, set.GetMediaTokens()[profile]);
                return mediaUri.Uri;
            }
            catch
            {
                return "";
            }
        }
        public static string GetPTZToken(Structures set, int profile = 0)
        {
            try
            {
                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Media");
                MediaClient mediaClient = new MediaClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;
                Profile[] profiles = mediaClient.GetProfiles();
                return profiles[profile].PTZConfiguration.token;
            }
            catch
            {
                return "";
            }
        }

        public static void RotatePTZ(Structures set, PTZVector vector, OPTZ.PTZSpeed speed)
        {
            try
            {
                if (set.GetPTZTokens() == "" || !set.IsActive) return;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                var t = mediaClient.GetConfigurations()[0].DefaultPTZTimeout;

                mediaClient.RelativeMove(set.GetMediaTokens()[0], vector, speed); //set.GetMediaTokens()[0]
                //mediaClient.ContinuousMove(set.GetMediaTokens()[0], speed)
            }
            catch
            {
                return;
            }
        }
        class CRPTZ
        {
            public Structures set; 
            public OPTZ.PTZSpeed speed;

            public CRPTZ(Structures set, OPTZ.PTZSpeed speed)
            {
                this.set = set;
                this.speed = speed;
            }
        }
        public static void AsyncCountinuousRotatePTZ(Structures set, OPTZ.PTZSpeed speed)
        {
            Thread th = new Thread(new ParameterizedThreadStart(_AsyncCountinuousRotatePTZ));
            th.Start(new CRPTZ(set, speed));
        }
        private static void _AsyncCountinuousRotatePTZ(object ob)
        {
            var c = (ob as CRPTZ);
            CountinuousRotatePTZ(c.set, c.speed);
        }
        public static void CountinuousRotatePTZ(Structures set, OPTZ.PTZSpeed speed, int timeout = -1)
        {
            try
            {
                if (set.GetPTZTokens() == "" || !set.IsActive) return;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                mediaClient.ContinuousMove(set.GetMediaTokens()[0], speed, "0");
                System.Threading.Thread.Sleep(timeout <= 0 ? (int)Settings.StaticMembers.PTZSettings.Timeout : timeout);//1000
                mediaClient.Stop(set.GetPTZTokens(), true, true);
            }
            catch
            {
                return;
            }
        }
        public static string PTZStatus(Structures set)
        {
            try
            {
               if (set.GetPTZTokens() == "" || !set.IsActive) return MoveStatus.UNKNOWN.ToString();

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                try
                {
                    var t = mediaClient.GetStatus(set.GetPTZTokens()).MoveStatus;
                    return t.PanTilt.ToString();
                }
                catch
                {
                    return MoveStatus.UNKNOWN.ToString();
                }
            }
            catch
            {
                return MoveStatus.UNKNOWN.ToString();
            }
        }
        public static void PTZHome(Structures set, OPTZ.PTZSpeed speed)
        {
            try
            {
                if (set.GetPTZTokens() == "" || !set.IsActive || !set.GetPTZController().IsSuported) return;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                mediaClient.GotoHomePosition(set.GetPTZTokens(), speed);
            }
            catch
            {
                return;
            }
        }
        public static void PTZStop(Structures set)
        {
            try
            {
                if (set.GetPTZTokens() == "" || !set.IsActive || !set.GetPTZController().IsSuported) return;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                mediaClient.Stop(set.GetPTZTokens(), true, true);
            }
            catch
            {
                return;
            }
        }
        public static void PTZSetHome(Structures set)
        {
            try
            {
                if (set.GetPTZTokens() == "" || !set.IsActive) return;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                mediaClient.SetHomePosition(set.GetPTZTokens());
            }
            catch
            {
                return;
            }
        }
        public static PTZVector PTZPosition(Structures set)
        {
            try
            {
                //if (set.GetPTZTokens() == "" || !set.IsActive) return new PTZVector();

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                return mediaClient.GetStatus(set.GetMediaTokens()[0]).Position;
            }
            catch
            {
                return new PTZVector();
            }
        }

        public static bool PTZSupport(Structures set)
        {
            try
            {
                if (set.GetPTZTokens() == "" || !set.IsActive) return false;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                var nodes = mediaClient.GetNodes();

                return nodes[0].HomeSupported;
            }
            catch
            {
                return false;
            }
        }

        public static string GetReplayUri(Structures set)
        {
            try
            { 
                //if (set.GetPTZTokens() == "" || !set.IsActive) return false;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Replay");
                ReplayPortClient mediaClient = new ReplayPortClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                var ss = new OREP.StreamSetup
                {
                    Stream = OREP.StreamType.RTPUnicast,
                    Transport = new OREP.Transport()
                    {
                        Protocol = OREP.TransportProtocol.RTSP,
                    }
                };

                var nodes = mediaClient.GetReplayUri(ss, set.GetMediaTokens()[0]);

                return nodes;
            }
            catch
            {
                return "";
            }
        }

        public static string GetRecordingJobs(Structures set)
        {
            try
            { 
                //if (set.GetPTZTokens() == "" || !set.IsActive) return false;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Digest;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/recording");
                RecordingPortClient mediaClient = new RecordingPortClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                var nodes = mediaClient.GetServiceCapabilities();

                return "";
            }
            catch
            {
                return "ERROR";
            }
        }
        public static NotificationMessageHolderType[] GetEvents(Structures set)
        {
            try
            {
                //if (set.GetPTZTokens() == "" || !set.IsActive) return false;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/event");
                PullPointSubscriptionClient mediaClient = new PullPointSubscriptionClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                NotificationMessageHolderType[] t;
                DateTime term;
                var nodes = mediaClient.PullMessages("1", 1, null, out term, out t);

                return t;
            }
            catch
            {
                return new NotificationMessageHolderType[] { };
            }
        }
    }
}
