using IPCamera.ODEV;
using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.InfoCamera
{
    public partial class CapCamera : Form
    {
        Structures structures;
        Capabilities capabilities;
        public CapCamera(uint Selected)
        {
            InitializeComponent();
            structures = Structures.Load()[Selected];
            capabilities = ONVIF.DeviceONVIF.GetCapabilities(structures);
        }

        private void CapCamera_Load(object sender, EventArgs e)
        {
            UpdateTree();
        }
        private void UpdateTree()
        {
            treeView1.Nodes.Clear();

            TreeNode tn = new TreeNode(structures.IP);
            tn.Nodes.Add(GetAnalyz());
            tn.Nodes.Add(GetMedia());
            tn.Nodes.Add(GetDevice());
            tn.Nodes.Add(GetEvents());
            tn.Nodes.Add(GetPTZ());

            treeView1.Nodes.Add(tn);
            tn.Expand();
        }
        private TreeNode GetAnalyz()
        {
            TreeNode tn = new TreeNode("Возможности аналитики");
            tn.Nodes.Add("Поддержка модуля: " + (capabilities.Analytics.AnalyticsModuleSupport ? " есть" : " нет"));
            tn.Nodes.Add("Поддержка правил: " + (capabilities.Analytics.RuleSupport ? " да" : " нет"));
            return tn;
        }
        private TreeNode GetMedia()
        {
            TreeNode tn = new TreeNode("Возможности медии");
            tn.Nodes.Add("[Эксперементально] RTP широковещательный: " + (capabilities.Media.StreamingCapabilities.RTPMulticast ? " есть" : " нет"));
            tn.Nodes.Add("[Эксперементально] RTP широковещательный спецификация: " + (capabilities.Media.StreamingCapabilities.RTPMulticastSpecified ? " есть" : " нет"));
            tn.Nodes.Add("[Эксперементально] RTSP через TCP: " + (capabilities.Media.StreamingCapabilities.RTP_RTSP_TCP ? " есть" : " нет"));
            tn.Nodes.Add("[Эксперементально] RTSP через TCP спецификация: " + (capabilities.Media.StreamingCapabilities.RTP_RTSP_TCPSpecified ? " есть" : " нет"));
            tn.Nodes.Add("[Эксперементально] RTP через TCP: " + (capabilities.Media.StreamingCapabilities.RTP_TCP ? " есть" : " нет"));
            tn.Nodes.Add("[Эксперементально] RTP через TCP спецификация: " + (capabilities.Media.StreamingCapabilities.RTP_TCPSpecified ? " есть" : " нет"));
            return tn;
        }
        private TreeNode GetDevice()
        {
            TreeNode tn = new TreeNode("Возможности устройства");
            tn.Nodes.Add(GetDeviceIO());
            tn.Nodes.Add(GetDeviceNetwork());
            tn.Nodes.Add(GetDeviceSecurity());
            tn.Nodes.Add(GetDeviceSystem());                      
            return tn;
        }
        private TreeNode GetEvents()
        {
            TreeNode tn = new TreeNode("Поддержка событие");
            tn.Nodes.Add("Событие паузы: " + (capabilities.Events.WSPausableSubscriptionManagerInterfaceSupport ? " есть" : " нет"));
            tn.Nodes.Add("Событие по точке: " + (capabilities.Events.WSPullPointSupport ? " есть" : " нет"));
            tn.Nodes.Add("Событие по политике: " + (capabilities.Events.WSSubscriptionPolicySupport ? " есть" : " нет"));
            return tn;
        }
        private TreeNode GetPTZ()
        {
            TreeNode tn = new TreeNode("Возможности PTZ");
            tn.Nodes.Add("Возможность управления: " + (structures.GetPTZController().IsSuported ? " есть" : " нет"));
            return tn;
        }
        private TreeNode GetDeviceIO()
        {
            TreeNode tn = new TreeNode("Логические вводы/выводы");
            tn.Nodes.Add("Логический вход: " + (capabilities.Device.IO.InputConnectorsSpecified ? " есть" : " нет"));
            tn.Nodes.Add("Релейный выход: " + (capabilities.Device.IO.RelayOutputsSpecified ? " есть" : " нет"));
            return tn;
        }
        private TreeNode GetDeviceNetwork()
        {
            TreeNode tn = new TreeNode("Сетевые возможности");
            tn.Nodes.Add("Динамический DNS (DDNS): " + (capabilities.Device.Network.DynDNSSpecified ? " есть" : " нет"));
            tn.Nodes.Add("Фильтр по IP: " + (capabilities.Device.Network.IPFilterSpecified ? " есть" : " нет"));
            tn.Nodes.Add("IPv6: " + (capabilities.Device.Network.IPVersion6Specified ? " есть" : " нет"));
            tn.Nodes.Add("Нулевая конфигурация: " + (capabilities.Device.Network.ZeroConfigurationSpecified ? " есть" : " нет"));
            return tn;
        }
        private TreeNode GetDeviceSecurity()
        {
            TreeNode tn = new TreeNode("Возможности безопасности");
            tn.Nodes.Add("Политика доступа: " + (capabilities.Device.Security.AccessPolicyConfig ? " есть" : " нет"));
            tn.Nodes.Add("Kerberos авторизация: " + (capabilities.Device.Security.KerberosToken ? " есть" : " нет"));
            tn.Nodes.Add("Внутриплатный ключ: " + (capabilities.Device.Security.OnboardKeyGeneration ? " есть" : " нет"));
            tn.Nodes.Add("REL авторизация: " + (capabilities.Device.Security.RELToken ? " есть" : " нет"));
            tn.Nodes.Add("SAML авторизация: " + (capabilities.Device.Security.SAMLToken ? " есть" : " нет"));
            tn.Nodes.Add("Поддержка TLS1.1: " + (capabilities.Device.Security.TLS11 ? " есть" : " нет"));
            tn.Nodes.Add("Поддержка TLS1.2: " + (capabilities.Device.Security.TLS12 ? " есть" : " нет"));
            tn.Nodes.Add("Поддержка X509: " + (capabilities.Device.Security.X509Token ? " есть" : " нет"));
            return tn;
        }
        private TreeNode GetDeviceSystem()
        {
            TreeNode tn = new TreeNode("Возможности системы");
            tn.Nodes.Add("Обновления системы по ONVIF: " + (capabilities.Device.System.FirmwareUpgrade ? " есть" : " нет"));
            tn.Nodes.Add("Удаленное управление: " + (capabilities.Device.System.RemoteDiscovery ? " есть" : " нет"));
            tn.Nodes.Add(GetDeviceSystemSO());
            tn.Nodes.Add("Создание резервных копий: " + (capabilities.Device.System.SystemBackup ? " есть" : " нет"));
            tn.Nodes.Add("Системный журнал: " + (capabilities.Device.System.SystemLogging ? " есть" : " нет"));
            return tn;
        }
        private TreeNode GetDeviceSystemSO()
        {
            TreeNode tn = new TreeNode("Поддерживаемые версии ONVIF");
            foreach (var item in capabilities.Device.System.SupportedVersions)
            {
                tn.Nodes.Add(item.Major + "." + item.Minor);
            }
            return tn;
        }
    }
}
