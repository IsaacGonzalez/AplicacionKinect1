using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace kinect1
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor sensor_anterior = (KinectSensor) e.OldValue;
            stopSensor(sensor_anterior);
            KinectSensor sensor_nuevo = (KinectSensor)e.NewValue;
            if(sensor_nuevo==null){
                return;
            }

            sensor_nuevo.SkeletonStream.Enable();
            sensor_nuevo.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_nuevo_AllFramesReady);

            try
            {
                sensor_nuevo.Start();


            }catch(System.IO.IOException ex){
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        void sensor_nuevo_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeleton_frame = e.OpenSkeletonFrame())
            {

                if (skeleton_frame == null) return;

                Skeleton[] esqueletos = new Skeleton[skeleton_frame.SkeletonArrayLength];
                skeleton_frame.CopySkeletonDataTo(esqueletos);
                
                Skeleton first = (
                    from s in esqueletos
                    where s.TrackingState == SkeletonTrackingState.Tracked
                    select s
                ).FirstOrDefault();

                Joint mano_derecha = first.Joints[JointType.HandRight];
                Joint mano_izquierda = first.Joints[JointType.HandLeft];

                setEllipsePosition(elipse_1, mano_derecha.Position.X, mano_derecha.Position.Y);
                setEllipsePosition(elipse_1, mano_derecha.Position.X, mano_izquierda.Position.Y);
                
            }
            
        }

        private void stopSensor(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                if (sensor.AudioSource != null)
                {
                    sensor.AudioSource.Stop();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stopSensor(kinectSensorChooser1.Kinect);
        }

        private void setEllipsePosition(Ellipse elipse, double X, double Y)
        {
            Canvas.SetLeft(elipse, X);
            Canvas.SetTop(elipse, Y);
        }
      


    }
}
