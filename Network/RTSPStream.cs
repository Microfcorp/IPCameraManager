using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace IPCamera.Network
{
    /// <summary>
    /// ������ �������
    /// </summary>
    public enum RTSPStream
    {
        /// <summary>
        /// ��������� ����� 1920�1080
        /// </summary>
        First_Stream = 11,
        /// <summary>
        /// ��������� ����� (���������)
        /// </summary>
        Second_Stream = 12,
        /// <summary>
        /// ��������� �����
        /// </summary>
        Mobile_Stream = 13,
        /// <summary>
        /// ��������� ����� 1920�1080 sdp
        /// </summary>
        First_SDP = 0,
        /// <summary>
        /// ��������� ����� (���������) sdp
        /// </summary>
        Second_SDP = 1,
    }
    /// <summary>
    /// ����
    /// </summary>
    public class Network
    {
        /// <summary>
        /// ��� ������
        /// </summary>
        public enum TypeCamera : byte
        {
            /// <summary>
            /// �� ���� HI3510
            /// </summary>
            HI3510,
            /// <summary>
            /// �� HI3518
            /// </summary>
            HI3518,
            /// <summary>
            /// ONVIF ����������
            /// </summary>
            Other,
        }
        /// <summary>
        /// ��� ��������� ������
        /// </summary>
        public static TypeCamera TypeCurentCamera
        {
            get;
            set;
        }
        /// <summary>
        /// ����� RTSP ������ ��� HI3510
        /// </summary>
        public const string RTSPHI3510 = "rtsp://{0}:{1}@{2}:{3}/iphone/{4}"; // rtsp://admin:admin@192.168.1.34/iphone/11
        /// <summary>
        /// ����� RTSP ������ ��� Hi3518E
        /// </summary>
        public const string RTSPHi3518 = "rtsp://{2}:{3}/user={0}&password={1}&channel=1&stream={4}.sdp?real_stream"; // rtsp://192.168.1.10:554/user=admin&password=admin&channel=0&stream=0.sdp?real_stream

        //webcapture.jpg?command=snap&chanel=1
        /// <summary>
        /// ����� ���� ��� HI3510
        /// </summary>
        public const string PhotoHI3510 = "http://{0}:{1}/web/auto.jpg?-usr={2}&-pwd={3}&";
        /// <summary>
        /// ����� ���� ��� Hi3518
        /// </summary>
        public const string PhotoHi3518 = "http://{0}:{1}/webcapture.jpg?command=snap&chanel=1";

        /// <summary>
        /// �������� RTSP ����� ��� ��������� ������
        /// </summary>
        public static string RTSP
        {
            get
            {
                if (TypeCurentCamera == TypeCamera.HI3510) return RTSPHI3510;
                else return RTSPHi3518;
            }
        }
        /// <summary>
        /// �������� ���� ��� ��������� ������
        /// </summary>
        public static string Photo
        {
            get
            {
                if (TypeCurentCamera == TypeCamera.HI3510) return PhotoHI3510;
                else return PhotoHi3518;
            }
        }
        /// <summary>
        /// �������� RTSP ����� ��� ���� ����
        /// </summary>
        /// <param name="typeCamera">��� ����</param>
        /// <returns></returns>
        public static string GetRTSP(TypeCamera typeCamera)
        {
            if (typeCamera == TypeCamera.HI3510) return RTSPHI3510;
            else return RTSPHi3518;
        }
        /// <summary>
        /// �������� ���� ��� ���� ����
        /// </summary>
        /// <param name="typeCamera">��� ����</param>
        /// <returns></returns>
        public static string GetPhoto(TypeCamera typeCamera)
        {
            if (typeCamera == TypeCamera.HI3510) return PhotoHI3510;
            else return PhotoHi3518;
        }
    }
}