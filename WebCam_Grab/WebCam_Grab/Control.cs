using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

/*
 * Ok, Just a few Notes:
 * 1. You will HAVE to get the AForge.Net Framework and Reference it. Otherwise it Can't build, and there will be errors.
 * 2. If you don't know what it does, DON'T TOUCH IT. Ask FIRST.
 * 3. If it breaks, DON'T PUSH IT TO GIT.
 * 4. I'm not responsible for any damage caused by this software. You are. Completely.
 * */

namespace WebCam_Grab
{
    public partial class Control : Form
    {
        private string imagepath = @"D:\Joshua\Waikato University\COMP 241\DIY Street View\Image Test"; // This WILL be put into a Config file on the Local filesystem, IN THE FUTURE.
        // The next 2 lines are Requred for AForge.net.
        private static FilterInfoCollection VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        private VideoCaptureDevice[] Cam = new VideoCaptureDevice[VideoDevices.Count];
        private Guid guid = new Guid(); //The simple GUID system.
        private CheckBox[] CheckedDevices = new CheckBox[VideoDevices.Count]; // To track what is being selected.
        private PictureBox[] images = new PictureBox[8]; // Where the Last Captured image Will be installed.
        private string[] tmpcid = new string[VideoDevices.Count]; // Required for Internal Camera Identification (Camera [Array] ID to DevID)
        struct Source
        {
            public string Name;
            public string Moniker;
            public string DevID;
        }
        private List<Source> CamSource = new List<Source>(); // A list of the Camera Sources with any asociated information that can have a need to be used. Designed in a Struct to enable Future expansion.
        // The Next Four Lines are used for the GPS data.
        private string GPSLAT = "0";
        private string GPSLONG = "0";
        private int GPSSTATUS = 0;
        private bool GPSPRESENT = false;
        private StreamWriter Dataout; // Used to create a Log of data, to be used in the Post Proccessing and Web Data (The file created contains GUIDs, GPS info, Date, Time, How many Cameras are used at this time)
        private bool[] TakePicture = new bool[VideoDevices.Count]; // This is used to tell the Camera "NewFrame" thread to take a picture.
        private Bitmap[] LastFrame = new Bitmap[VideoDevices.Count];
        private int[] camframe = new int[VideoDevices.Count];

        public Control()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(imagepath + "\\RAW\\"))
            {
                Directory.CreateDirectory(imagepath + "\\RAW\\"); // The Directory where RAW images will be stored.
            }
            Dataout = new StreamWriter(imagepath + "\\imagelog.csv",true,Encoding.UTF8); // Init the CSV that stores the data (Less Time Expensive, Less RAM intensive)
            Dataout.AutoFlush = true; // Make it automaticly Save the data.
            try
            {
                serialPort.Open(); // Open the port for the GPS.
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
            ImagePathTextbox.Text = imagepath; // Init the Directory Text Box.
            folderBrowser.SelectedPath = imagepath; // Set the Default Path for the folder Browser.
            int CountDevices = 0;
            /// This Gets the DIV ID, and removes Charictors that Are not needed.
            /// And Adds the Devices to the CamSource list.
            while (CountDevices < VideoDevices.Count)
            {
                string devID = VideoDevices[CountDevices].MonikerString;
                string[] tmpdevID = devID.Split('#','\\');
                if (tmpdevID.Length > 4)
                {
                    devID = tmpdevID[5];
                }
                else
                {
                    devID = tmpdevID[tmpdevID.Length - 1];
                }
                devID = devID.Replace("{", "");
                devID = devID.Replace("}", "");
                devID = devID.Replace("&", "");
                Source Cameras;
                Cameras.Name = VideoDevices[CountDevices].Name;
                Cameras.Moniker = VideoDevices[CountDevices].MonikerString;
                Cameras.DevID = devID;
                CamSource.Add(Cameras);
                CountDevices++;
            }
            /// This sets all the sources up.
            InitSources();
            CountDevices = 0;
            while (CountDevices < CamSource.Count)
            {
                /// This sets up all the Camera Checkboxes... At Runtime, Completely Dynamicly
                CheckedDevices[CountDevices] = new CheckBox();
                CheckedDevices[CountDevices].Location = new System.Drawing.Point(10, (17 * CountDevices) + 5);
                CheckedDevices[CountDevices].Text = CamSource[CountDevices].Name + " (ID: " + CamSource[CountDevices].DevID + ")";
                CheckedDevices[CountDevices].TextAlign = ContentAlignment.MiddleRight;
                CheckedDevices[CountDevices].CheckAlign = ContentAlignment.MiddleLeft;
                CheckedDevices[CountDevices].AutoSize = true;
                CheckedDevices[CountDevices].CheckedChanged += new EventHandler(UpdateCamStatus);
                Checkerpanel.Controls.Add(CheckedDevices[CountDevices]);
                CountDevices++;
            }
            /// And add Refferences of the Picture boxes to an Array, for simplicity and more effecent camera Management.
                images[0] = pictureBox1;
                images[1] = pictureBox2;
                images[2] = pictureBox3;
                images[3] = pictureBox4;
                images[4] = pictureBox5;
                images[5] = pictureBox6;
                images[6] = pictureBox7;
                images[7] = pictureBox8;
        }
        /// <summary>
        /// Used solely to Safely shutdown the Cameras and any other running Objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exiting(object sender, FormClosedEventArgs e)
        {
            int CountCameras = 0;
            while (CountCameras < Cam.Length)
            {
                if (Cam[CountCameras] != null)
                {
                    if (Cam[CountCameras].IsRunning)
                    {
                        Cam[CountCameras].SignalToStop();
                    }
                }
                CountCameras++;
            }
            Dataout.Close();
            Dataout.Dispose();
            serialPort.Close();
        }
        /// <summary>
        /// Does all the code when the "Browse" Button is Clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                imagepath = folderBrowser.SelectedPath;
                ImagePathTextbox.Text = imagepath;
            }
        }
        /// <summary>
        /// Fires all the Cameras to Get images, when the "Capture" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCapture_Click(object sender, EventArgs e)
        {
            {
            int CountClear = 0;
            while (CountClear < images.Length)
            {
                images[CountClear].ImageLocation = "";
                CountClear++;
            }

            }
            guid = Guid.NewGuid();
            int CountChecked = 0;
            int CounttmpCID = 0;
            while (CountChecked < CheckedDevices.Length && CountChecked < Cam.Length)
            {
                if (CheckedDevices[CountChecked].Checked == true)
                {
                    tmpcid[CounttmpCID] = CamSource[CountChecked].DevID;
                    TakePicture[CountChecked] = true;
                    CounttmpCID++;
                }
                CountChecked++;
            }
            if (CounttmpCID > 0)
            {
                Dataout.WriteLine(guid + "," + GPSLAT + "," + GPSLONG + "," + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "," + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "," + CounttmpCID);
            }
            LastGUIDLabel.Text = "Last GUID: " + guid;
            Cam[0].Start();
            Cam[1].Start();
        }
        /// <summary>
        /// This is fired when ever the camera has a New Frame Available, and only stores them when it should, otherwise it just disposes of them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="DevID">The Camera's Unique ID</param>
        /// <param name="CID">The Camera's Runtime ID (Unlikely to stay the same)</param>
        private void Cam_NewFrame(object sender, NewFrameEventArgs args, string DevID, int CID)
        {
            if (TakePicture[CID] == true)
            {
                if (camframe[CID] >= 90)
                {
                    TakePicture[CID] = false;
                    Bitmap b = (Bitmap)args.Frame.Clone();
                    Cam[CID].SignalToStop();
                    if ((CID + 2) < Cam.Length && Cam[CID + 2] != null)
                    {
                        Cam[CID + 2].Start();
                    }
                    string tmppath = imagepath + "\\RAW";
                    b.Save(tmppath + "\\" + guid + "." + DevID + ".bmp"); // Uncommpressed Storage of the Raw Data Stream, Better for Time and Quality.
                    int CounttmpCID = 0;
                    while (CounttmpCID < tmpcid.Length && tmpcid[CounttmpCID].ToString() != DevID)
                    {
                        CounttmpCID++; // If the Camera's DevID isn't the same as the current CID's Dev ID, then We need to try the next one.
                    }
                    if (CounttmpCID < tmpcid.Length)
                    {
                        images[CounttmpCID].ImageLocation = tmppath + "\\" + guid + "." + DevID + ".bmp"; // If it is, then update the Image Boxes with the Latest Picture.
                    }
                    LastFrame[CID] = b;
                    camframe[CID] = 0;
                }
                else
                {
                    camframe[CID]++;
                }
            }
            else
            {
                Cam[CID].SignalToStop();
                if ((CID + 2) < Cam.Length && Cam[CID + 2] != null)
                {
                    Cam[CID + 2].Start();
                }
            }
        }
        /// <summary>
        /// Initialises the Camera Sources
        /// </summary>
        private void InitSources()
        {
            Size FrameSize = new Size(1600, 1200); // 1600x1200 = 2 MP
            int CountCameraInit = 0;
            while (CountCameraInit < CamSource.Count)
            {
                int CID = CountCameraInit;
                string devid = CamSource[CID].DevID;
                Cam[CID] = new VideoCaptureDevice(CamSource[CID].Moniker);
                Cam[CID].NewFrame += new NewFrameEventHandler((sender, args) => Cam_NewFrame(sender, args, devid, CID)); // Set up the Event Handler
                Cam[CID].DesiredFrameSize = FrameSize; // Try to get the Max Res of 2 MP.
                CountCameraInit++; // NEXT!
            }
            ;
        }
        /// <summary>
        /// This is called Once every Tick of GPSPollTimer. (Currently Every Second)
        /// </summary>
        private void UpdateLoc()
        {
            if (GPSSTATUS == 1 && GPSPRESENT)
            {
                gpslabel.Text = "GPS: Latitude: " + GPSLAT + " Longitude: " + GPSLONG; // If there is a Valid Lock, update the Label.
            }
            else if (GPSSTATUS == 0 && GPSPRESENT)
            {
                gpslabel.Text = "GPS: No Lock. Storing " + GPSLAT + ", " + GPSLONG; // No Valid Lock, Stating the Last Known Location.
            }
            else
            {
                gpslabel.Text = "GPS: No GPS Present. Storing " + GPSLAT + "S," + GPSLONG + "E"; // No GPS, Stating the LKL.
            }
        }
        /// <summary>
        /// The Event Handler of the GPS Timer Tick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GPSPollTimer_Tick(object sender, EventArgs e)
        {
            GetLoc(); // Get the GPS Data.
            UpdateLoc(); // Update the Labels.
        }
        /// <summary>
        /// Update the Camera's Status, Run Every time the Checkboxes Change.
        /// Delays the Ability to Take a Photo for 1 Second to allow the Camera's to warm up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCamStatus(object sender, EventArgs e)
        {
            buttonCapture.Enabled = false;
            LabelCameraStatus.Text = "Camera's Warming";
            int CountChecked = 0;
            while (CountChecked < CheckedDevices.Length && CountChecked < Cam.Length)
            {
                if (CheckedDevices[CountChecked].Checked == false && Cam[CountChecked].IsRunning == true)
                {
                    Cam[CountChecked].SignalToStop();
                }
                CountChecked++;
            }
               LabelCameraStatus.Text = "Camera's Ready.";
               buttonCapture.Enabled = true;
        }
        /// <summary>
        /// Get the Current GPS Location data, Has to parse the NMEA Data from the Android Device.
        /// </summary>
        private void GetLoc()
        {            
            if (serialPort.IsOpen)
            {
                GPSPRESENT = true; // If we have Data flowing, Presume it's the GPS Device.
                string data = serialPort.ReadExisting(); // Read it.
                string[] strArr = data.Split('$'); // Split it.
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strTemp = strArr[i]; // Store it.
                    string[] lineArr = strTemp.Split(','); // Split it AGAIN.
                    if (lineArr[0] == "GPVTG") // If it's the line that talks about Heading...
                    {
                        if (lineArr[3] == "nan" || lineArr[3] == "") // If it's not Valid.
                        {
                            labelDirection.Text = "Direction: None supplied from GPS";
                        }
                        else // It must be VALID!
                        {
                            labelDirection.Text = "Direction: " + lineArr[3];
                        }
                    }
                    else if (lineArr[0] == "GPGGA") // This is the line that Talks about GPS Lat and Long
                    {
                        try
                        {
                            ///
                            /// Don't understand what this does?
                            /// Read up on the NMEA Standard.
                            /// 

                            //Latitude
                            Double dLat = Convert.ToDouble(lineArr[2]);
                            dLat = dLat / 100;
                            string[] lat = dLat.ToString().Split('.');
                            GPSLAT = lat[0].ToString() + "." + ((Convert.ToDouble(lat[1]) / 60)).ToString("#####") + lineArr[3].ToString();

                            //Longitude
                            Double dLon = Convert.ToDouble(lineArr[4]);
                            dLon = dLon / 100;
                            string[] lon = dLon.ToString().Split('.');
                            GPSLONG = lon[0].ToString() + "." + ((Convert.ToDouble(lon[1]) / 60)).ToString("#####") + lineArr[5].ToString();
                            GPSSTATUS = 1;
                        }
                        catch
                        {
                            GPSSTATUS = 0; // If that fails, the GPS is Broke.
                        }
                    }
                    else if (strTemp != "")
                    {
                        Console.Write(strTemp + Environment.NewLine); //DEBUG: Pipe the RAW lines to the Debug Console.
                    }
                }
                Console.WriteLine(); //DEBUG: Write the Console Line.
            }
            else
            {
                GPSSTATUS = 0; // If the COM Port is closed, there is no Lock
                GPSPRESENT = false; // And no GPS Device.
            }
        }
    }
}
