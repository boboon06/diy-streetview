using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AutoGenPano
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string imagepath = @"D:\Joshua\Waikato University\COMP 241\DIY Street View\Images";
        private string libpanopath = @"D:\Joshua\Waikato University\COMP 241\DIY Street View\PanoTools\PanoTools";
        private void button1_Click(object sender, EventArgs e)
        {
            int panoid = 0;
            string[] cameraids = new string[8];
            // Insert Each Camera ID here.
            cameraids[0] = "Camera ID 1";
            cameraids[1] = "Camera ID 2";
            cameraids[2] = "Camera ID 3";
            cameraids[3] = "Camera ID 4";
            cameraids[4] = "Camera ID 5";
            cameraids[5] = "Camera ID 6";
            cameraids[6] = "Camera ID 7";
            cameraids[7] = "Camera ID 8";
            if (!File.Exists(libpanopath + "\\PTStitcher.exe"))
            {
                MessageBox.Show("ERROR: PanoTools NOT FOUND");
                this.Close();
            }
            else if (!File.Exists(libpanopath + "\\diystreetview.txt"))
            {
                MessageBox.Show("ERROR: PanoTools Stitching Script NOT FOUND");
                this.Close();
            }
            else
            {
                StreamReader csvinput = new StreamReader(imagepath + "\\imagelog.csv");
                while (csvinput.Peek() != -1)
                {
                    string line = csvinput.ReadLine();
                    string[] values = line.Split(',');
                    if (values[5] != "8")
                    {
                        MessageBox.Show("Error: GUID " + values[0] + " doesn't have all 8 cameras taking pictures... Skipping.", "Panorama Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (!File.Exists(imagepath + "\\Panoramas\\" + values[0] + ".jpg"))
                        {
                            panoid++;
                            sitrepStatusLabel.Text = "Stitching Panorama No: " + panoid;
                            int count_images = 0;
                            while (count_images < 8)
                            {
                                File.Copy(imagepath + "\\RAW\\" + values[0] + cameraids[count_images] + ".bmp", libpanopath + "DIY" + (count_images + 1) + ".bmp", true);
                                count_images++;
                            }
                            // ALSO I need to set up the PT Script with said photos and TEST THEM.
                            // AFTER CHRIS MAKES THE RIG.
                            System.Diagnostics.Process ptsticher = new System.Diagnostics.Process();
                            ptsticher.StartInfo.FileName = libpanopath + "\\PTStitcher.exe";
                            ptsticher.StartInfo.Arguments = "-o " + values[0] + " diystreetview.txt";
                            ptsticher.Start();
                            ptsticher.WaitForExit();
                            File.Move(libpanopath + "\\" + values[0] + ".jpg", imagepath + "\\Panoramas\\" + values[0] + ".jpg");
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Because Chris hasn't done the RIG yet, I can waste time like this!
            this.Text = "AutoPano Creator";
            if (Environment.MachineName == "RAINBOWDASH" && Environment.UserName == "Boboon")
            {
                this.Text += " - rainbowdash.theboboon.com";
            }
        }
    }
}
