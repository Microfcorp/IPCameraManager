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

namespace IPCamera.ONVIF
{
    class SendResponce
    {
        public static string GetSnapsotURL(Structures set, int profile = 1)
        {
            try
            {
                if (set.MediaTokens.FirstOrDefault() == "") return "";

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Media");
                MediaClient mediaClient = new MediaClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                MediaUri mediaUri = mediaClient.GetSnapshotUri(set.MediaTokens[profile]);
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
                MediaClient mediaClient = GetMediaBase(set);
                return mediaClient.GetProfiles();
            }
            catch
            {
                return new Profile[] { new Profile() };
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
                if (set.MediaTokens.FirstOrDefault() == "") return "";
                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Media");
                MediaClient mediaClient = new MediaClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                StreamSetup ss = new StreamSetup();
                ss.Stream = StreamType.RTPUnicast;
                ss.Transport = new Transport();
                ss.Transport.Protocol = TransportProtocol.RTSP;
                MediaUri mediaUri = mediaClient.GetStreamUri(ss, set.MediaTokens[profile]);
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
                if (set.PTZTokens == "") return;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                mediaClient.RelativeMove(set.PTZTokens, vector, speed);
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
                if (set.PTZTokens == "") return MoveStatus.UNKNOWN.ToString();

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
                    return mediaClient.GetStatus(set.PTZTokens).MoveStatus.PanTilt.ToString();
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
                if (set.PTZTokens == "") return;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                mediaClient.GotoHomePosition(set.PTZTokens, speed);
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
                if (set.PTZTokens == "") return;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                mediaClient.SetHomePosition(set.PTZTokens);
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
                if (set.PTZTokens == "") return new PTZVector();

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                return mediaClient.GetStatus(set.PTZTokens).Position;
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
                if (set.PTZTokens == "") return false;

                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/PTZ");
                PTZClient mediaClient = new PTZClient(bind, mediaAddress);
                mediaClient.ClientCredentials.UserName.UserName = set.Login;
                mediaClient.ClientCredentials.UserName.Password = set.Password;

                return mediaClient.GetNodes()[0].HomeSupported;
            }
            catch
            {
                return false;
            }
        }
    }
}
