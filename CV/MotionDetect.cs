//----------------------------------------------------------------------------
//  Copyright (C) 2004-2018 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.Util;
using PedestrianDetection;
using Emgu.CV.Cuda;

namespace IPCamera.CV
{
    public class MDEV
    {
        public MDEV(long s, Mat image, long m) { count = s; Bitmap = image; Motion = m; }
        public long count { get; } // readonly
        public Mat Bitmap { get; } // readonly
        public long Motion { get; } // readonly
    }
    public partial class MootionDetect : IDisposable
    {
        private VideoCapture _capture;
        private MotionHistory _motionHistory;
        private IBackgroundSubtractor _forgroundDetector;

        public delegate void MDEV(long s, Mat image, long m);
        public event MDEV OnMD;
        public Boolean DetectPed = false;
        public Rectangle Zone;
        public const byte allfpsc = 2;
        public const byte skipfpsc = 60;

        public MootionDetect(string path)
        {

            //try to create the capture
            if (_capture == null)
            {
                try
                {
                    _capture = new VideoCapture(path);
                    _capture.SetCaptureProperty(CapProp.FrameWidth, 640);
                    _capture.SetCaptureProperty(CapProp.FrameHeight, 360);
                    _capture.SetCaptureProperty(CapProp.Fps, 30);
                    _capture.SetCaptureProperty(CapProp.ConvertRgb, 0);
                    CvInvoke.UseOptimized = true;
                    CvInvoke.NumThreads = 4;
                }
                catch (NullReferenceException excpt)
                {   //show errors if there is any
                    MessageBox.Show(excpt.Message);
                }
            }

            if (_capture != null) //if camera capture has been successfully created
            {
                _motionHistory = new MotionHistory(
                    1.0, //in second, the duration of motion history you wants to keep
                    0.15, //in second, maxDelta for cvCalcMotionGradient
                    0.9); //in second, minDelta for cvCalcMotionGradient
                _capture.ImageGrabbed += ProcessFrame;
                _capture.Start();
            }
        }

        int fpsc = 0;
        int maxfpsc = 0;

        private Mat _segMask = new Mat();
        private Mat _forgroundMask = new Mat();
        private void ProcessFrame(object sender, EventArgs e)
        {
            List<Rectangle> rt = new List<Rectangle>();
            double overallAngle = default(double), overallMotionPixelCount = default(double);
            Mat image = new Mat();

            _capture.Retrieve(image);

            if (fpsc++ >= allfpsc)
            {
                fpsc = 0;

                if (_forgroundDetector == null)
                {
                    _forgroundDetector = new BackgroundSubtractorMOG2();
                }

                _forgroundDetector.Apply(image, _forgroundMask);

                //update the motion history
                _motionHistory.Update(_forgroundMask);

                #region get a copy of the motion mask and enhance its color
                double[] minValues, maxValues;
                Point[] minLoc, maxLoc;
                _motionHistory.Mask.MinMax(out minValues, out maxValues, out minLoc, out maxLoc);
                Mat motionMask = new Mat();
                using (ScalarArray sa = new ScalarArray(255.0 / maxValues[0]))
                    CvInvoke.Multiply(_motionHistory.Mask, sa, motionMask, 1, DepthType.Cv8U);
                //Image<Gray, Byte> motionMask = _motionHistory.Mask.Mul(255.0 / maxValues[0]);
                #endregion

                //create the motion image 
                Mat motionImage = new Mat(motionMask.Size.Height, motionMask.Size.Width, DepthType.Cv8U, 3);
                motionImage.SetTo(new MCvScalar(0));
                //display the motion pixels in blue (first channel)
                //motionImage[0] = motionMask;
                CvInvoke.InsertChannel(motionMask, motionImage, 0);

                //Threshold to define a motion area, reduce the value to detect smaller motion
                double minArea = 100;

                //storage.Clear(); //clear the storage
                Rectangle[] rects;
                using (VectorOfRect boundingRect = new VectorOfRect())
                {
                    _motionHistory.GetMotionComponents(_segMask, boundingRect);
                    rects = boundingRect.ToArray();
                }                

                //iterate through each of the motion component
                foreach (Rectangle comp in rects)
                {
                    if (Zone.Contains(comp))
                        rt.Add(comp);

                    int area = comp.Width * comp.Height;
                    //reject the components that have small area;
                    if (area < minArea) continue;

                    // find the angle and motion pixel count of the specific area
                    double angle, motionPixelCount;
                    _motionHistory.MotionInfo(_forgroundMask, comp, out angle, out motionPixelCount);

                    //reject the area that contains too few motion
                    if (motionPixelCount < area * 0.05) continue;

                    //Draw each individual motion in red
                    //DrawMotion(motionImage, comp, angle, new Bgr(Color.Blue));               
                }

                //rects = rt.ToArray();

                // find and draw the overall motion angle
                

                if (Zone.Width <= 0 & Zone.Height <= 0 & Zone.X < 0 & Zone.Y < 0) Zone = new Rectangle(0, 0, 10, 10);

                _motionHistory.MotionInfo(_forgroundMask, Zone /*new Rectangle(Point.Empty, motionMask.Size)*/, out overallAngle, out overallMotionPixelCount);
                //DrawMotion(motionImage, new Rectangle(Point.Empty, motionMask.Size), overallAngle, new Bgr(Color.Green));

                Image<Bgr, Byte> grayImage = image.ToImage<Bgr, Byte>();

                if (DetectPed)
                {
                    long processingTime;
                    Rectangle[] results;

                    if (CudaInvoke.HasCuda)
                    {
                        using (GpuMat gpuMat = new GpuMat(image))
                            results = FindPedestrian.Find(gpuMat, out processingTime);
                    }
                    else
                    {
                        using (UMat uImage = image.GetUMat(AccessType.ReadWrite))
                            results = FindPedestrian.Find(uImage, out processingTime);
                    }

                    foreach (Rectangle rect in results)
                    {
                        CvInvoke.Rectangle(image, rect, new Bgr(Color.Red).MCvScalar);
                    }
                }
            }

            if(maxfpsc++ > skipfpsc) OnMD?.Invoke((long)overallMotionPixelCount, image, rt.Count);
            else OnMD?.Invoke(default(long), image, default(int));
        }

        private static void DrawMotion(IInputOutputArray image, Rectangle motionRegion, double angle, Bgr color)
        {
            //CvInvoke.Rectangle(image, motionRegion, new MCvScalar(255, 255, 0));
            float circleRadius = (motionRegion.Width + motionRegion.Height) >> 2;
            Point center = new Point(motionRegion.X + (motionRegion.Width >> 1), motionRegion.Y + (motionRegion.Height >> 1));

            CircleF circle = new CircleF(
               center,
               circleRadius);

            int xDirection = (int)(Math.Cos(angle * (Math.PI / 180.0)) * circleRadius);
            int yDirection = (int)(Math.Sin(angle * (Math.PI / 180.0)) * circleRadius);
            Point pointOnCircle = new Point(
                center.X + xDirection,
                center.Y - yDirection);
            LineSegment2D line = new LineSegment2D(center, pointOnCircle);
            CvInvoke.Circle(image, Point.Round(circle.Center), (int)circle.Radius, color.MCvScalar);
            CvInvoke.Line(image, line.P1, line.P2, color.MCvScalar);

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _capture.Stop();
        }

        public void Dispose()
        {
            _capture.Stop();
            //((IDisposable)_capture).Dispose();
        }
    }
}