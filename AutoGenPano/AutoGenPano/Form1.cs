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
            StreamReader csvinput = new StreamReader(imagepath + "\\imagelog.csv");
            while (csvinput.Peek() != -1)
            {
                string line = csvinput.ReadLine();
                string[] values = line.Split(',');
                if (values[5] != "8")
                {
                    MessageBox.Show("Error: GUID " + values[0] + " doesn't have all 8 cameras taking pictures... Skipping.","Panorama Generation Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
                else
                {
                    //TODO I need to set up the Constants for each of the cameras
                    // and make C# Move them to the Pano Tools folder, and change their names to constents.
                    // AFTER CHRIS MAKES THE RIG.
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
