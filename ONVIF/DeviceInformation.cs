using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using IPCamera.ODEV;
using IPCamera.Settings;

namespace IPCamera.ONVIF
{
    public class DeviceONVIF
    {
        /// <summary>
        /// Информация об устройстве
        /// </summary>
        public struct DeviceInformation
        {
            /// <summary>
            /// Модель устройтва
            /// </summary>
            public string Model;
            /// <summary>
            /// Версия ПО
            /// </summary>
            public string Version; 
            /// <summary>
            /// Серийный номер
            /// </summary>
            public string SerialNumber; 
            /// <summary>
            /// ID железа
            /// </summary>
            public string HardwareID;
            /// <summary>
            /// Другая информация
            /// </summary>
            public string Other;

            internal DeviceInformation(string model, string version, string serialNumber, string hardwareID, string other)
            {
                Model = model;
                Version = version;
                SerialNumber = serialNumber;
                HardwareID = hardwareID;
                Other = other;
            }
        }
        public static DeviceInformation GetDeviceInformation(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            DeviceInformation dv = new DeviceInformation();

            dv.Other = client.GetDeviceInformation(out dv.Model, out dv.Version, out dv.SerialNumber, out dv.HardwareID);
            return dv;
        }
        public static SystemDateTime GetCameraDate(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            return client.GetSystemDateAndTime();
        }
        public static void SetCameraDate(Structures set, SetDateTimeType t, string TZ, System.DateTime DT)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            if(t == SetDateTimeType.Manual)
                client.SetSystemDateAndTime(t, true, new ODEV.TimeZone() { TZ = TZ}, new ODEV.DateTime() { Date = new Date() { Day = DT.Day, Month = DT.Month, Year = DT.Year }, Time = new Time() { Hour = DT.Hour, Minute = DT.Minute, Second = DT.Second } });
            else
                client.SetSystemDateAndTime(t, true, null, null);
        }

        public static NetworkProtocol[] GetNetworkProtocols(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            var t = client.GetNetworkProtocols();

            return t;
        }

        public static string GetHostname(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            var t = client.GetHostname().Name;

            return t;
        }

        public static string SetHostname(Structures set, string newhostname)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            client.SetHostname(Uri.EscapeDataString(newhostname));

            return newhostname;
        }

        public static bool SetHostnameFromDHCP(Structures set, bool FromDHCP)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            client.SetHostnameFromDHCP(FromDHCP);

            return FromDHCP;
        }

        public static Capabilities GetCapabilities(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            var t = client.GetCapabilities(new CapabilityCategory[] { CapabilityCategory.All });

            return t;
        }
        public static User[] GetUsers(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            var t = new User[0];
            try
            {
                t = client.GetUsers();
            }
            catch
            {
            }

            return t;
        }
        public static OnvifVersion GetOnvifVersion(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            var t = client.GetServices(true)[0].Version;

            return t;
        }

        public static string Reboot(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            var t = client.SystemReboot();

            return t;
        }

        public static string Test(Structures set)
        {
            var messageElement = new TextMessageEncodingBindingElement();
            messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
            httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
            CustomBinding bind = new CustomBinding(messageElement, httpBinding);
            EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/Device");
            DeviceClient client = new DeviceClient(bind, mediaAddress);
            client.ClientCredentials.UserName.UserName = set.Login;
            client.ClientCredentials.UserName.Password = set.Password;

            var t = client.GetServices(true);

            return "";
        }        
    }
}
