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
        private static string imagepath = @"D:\Joshua\Waikato University\COMP 241\DIY Street View\Images";
        private string libpanopath = @"D:\Joshua\Waikato University\COMP 241\DIY Street View\PanoTools\PanoTools";
        private StreamWriter panolog = new StreamWriter(imagepath + "\\panogen.log", true);
        private void button1_Click(object sender, EventArgs e)
        {
            int panoid = 0;
            string[] cameraids = new string[8];
            // Insert Each Camera ID here.
            cameraids[0] = "82fd8da3a00000";
            cameraids[1] = "86f7c0d800000";
            cameraids[2] = "8fdbb17100000";
            cameraids[3] = "82295621600000";
            cameraids[4] = "838bccad300000";
            cameraids[5] = "826031a2e00000";
            cameraids[6] = "81d1f299500000";
            cameraids[7] = "8a6578f000000";
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
                StreamWriter csvoutput = new StreamWriter(imagepath + "\\panolog.csv",true);
                csvoutput.AutoFlush = true;
                while (csvinput.Peek() != -1)
                {
                    string line = csvinput.ReadLine();
                    string[] values = line.Split(',');
                    if (values[5] != "8")
                    {
                        log("Error: GUID " + values[0] + " doesn't have all 8 cameras taking pictures... Skipping.");
                    }
                    else
                    {
                        if (!File.Exists(imagepath + "\\Panoramas\\" + values[0] + ".bmp"))
                        {
                            if (File.Exists(imagepath + "\\RAW\\" + values[0] + "." + cameraids[0] + ".bmp") && File.Exists(imagepath + "\\RAW\\" + values[0] + "." + cameraids[1] + ".bmp") && File.Exists(imagepath + "\\RAW\\" + values[0] + "." + cameraids[2] + ".bmp") && File.Exists(imagepath + "\\RAW\\" + values[0] + "." + cameraids[3] + ".bmp") && File.Exists(imagepath + "\\RAW\\" + values[0] + "." + cameraids[4] + ".bmp") && File.Exists(imagepath + "\\RAW\\" + values[0] + "." + cameraids[5] + ".bmp") && File.Exists(imagepath + "\\RAW\\" + values[0] + "." + cameraids[6] + ".bmp") && File.Exists(imagepath + "\\RAW\\" + values[0] + "." + cameraids[7] + ".bmp"))
                            {
                                panoid++;
                                log("All 8 Camera Files exist. Attempting to stitch Panorama " + values[0]);
                                sitrepStatusLabel.Text = "Attempting to stitch Panorama No: " + panoid;
                                int count_images = 0;
                                while (count_images < 8)
                                {
                                    File.Copy(imagepath + "\\RAW\\" + values[0] + "." + cameraids[count_images] + ".bmp", libpanopath + "\\" + (count_images + 1) + ".bmp", true);
                                    count_images++;
                                }
                                // ALSO I need to set up the PT Script with said photos and TEST THEM.
                                // AFTER CHRIS MAKES THE RIG.
                                System.Diagnostics.Process ptsticher = new System.Diagnostics.Process();
                                ptsticher.StartInfo.WorkingDirectory = libpanopath;
                                ptsticher.StartInfo.FileName = libpanopath + "\\PTStitcher.exe";
                                ptsticher.StartInfo.Arguments = "-o " + values[0] + " diystreetview.txt";
                                ptsticher.Start();
                                ptsticher.WaitForExit();
                                if (File.Exists(libpanopath + "\\" + values[0] + ".BMP"))
                                {
                                    File.Move(libpanopath + "\\" + values[0] + ".BMP", imagepath + "\\Panoramas\\" + values[0] + ".bmp");
                                    csvoutput.WriteLine(values[0] + "," + values[1] + "," + values[2]);
                                    log("Panorama " + values[0] + "generated Successfuly and all values have been written, and files moved");
                                }
                                else
                                {
                                    log("Critical Error: Panorama " + values[0] + " wasn't generated!");
                                }
                            }
                            else
                            {
                                log("Panorama " + values[0] + " Image Files don't exist. Skipping.");
                            }
                        }
                        else
                        {
                            log("Panorama " + values[0] + " already exists. Skipping.");
                        }
                    }
                }
                MessageBox.Show("Completed");
                csvinput.Close();
                csvoutput.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panolog.AutoFlush = true;
            //Because Chris hasn't done the RIG yet, I can waste time like this!
            this.Text = "AutoPano Creator";
            if (Environment.MachineName == "RAINBOWDASH" && Environment.UserName == "Boboon")
            {
                this.Text += " - rainbowdash.theboboon.com";
            }

        }
        private void log(string logtxt)
        {
            panolog.WriteLine(logtxt);
            Console.WriteLine(logtxt);
        }
    }
}
