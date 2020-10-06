using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using IPCamera.OIMG;
using IPCamera.Settings;

namespace IPCamera.ONVIF
{
    public class ImagingResponce
    {
        public static ImagingSettings20 GetImageSetting(Structures set, uint tokenid)
        {
            try
            {
                var messageElement = new TextMessageEncodingBindingElement();
                messageElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
                HttpTransportBindingElement httpBinding = new HttpTransportBindingElement();
                httpBinding.AuthenticationScheme = AuthenticationSchemes.Basic;
                CustomBinding bind = new CustomBinding(messageElement, httpBinding);
                EndpointAddress mediaAddress = new EndpointAddress(set.GetONVIF + "/onvif/imaging");
                ImagingPortClient client = new ImagingPortClient(bind, mediaAddress);
                client.ClientCredentials.UserName.UserName = set.Login;
                client.ClientCredentials.UserName.Password = set.Password;

                return client.GetImagingSettings(set.GetMediaTokens()[tokenid]);
            }
            catch
            {
                return null;
            }
        }
    }
}
