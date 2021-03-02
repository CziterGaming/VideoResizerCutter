using System;
using System.Drawing;
using System.Windows.Forms;
using MediaToolkit;
using MediaToolkit.Options;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace VideoResizer
{
    public partial class Form1 : Form
    {
        string OutputFilePath;
        string OutputFilePathWithDetails;
        int amountOfFiles;
        List<string> files = new List<string>();
        List<string> fileNames = new List<string>();
        List<string> InputFilePathWithoutFile = new List<string>();
        List<string> fileSizes = new List<string>();

        ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
        private ConversionOptions conversionOptions;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuStrip1_ItemClicked);
            progressBar1.Visible = false;
            this.textBox1.ScrollBars = ScrollBars.Both;
            this.textBox2.ScrollBars = ScrollBars.Both;
            label6.Visible = false;
            label7.Visible = false;
            label9.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clearLists();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Video files (*.mp4)|*.mp4|All files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach(string list in openFileDialog.FileNames)
                    {
                        files.Add(list);
                        fileNames.Add(Path.GetFileNameWithoutExtension(list));
                        InputFilePathWithoutFile.Add(Path.GetDirectoryName(list));
                        textBox1.AppendText(list + "\n" + Environment.NewLine);
                    }
                    amountOfFiles = openFileDialog.FileNames.Length;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                OutputFilePath = fbd.SelectedPath;
                
                for (int b2 = 0; b2 < amountOfFiles; b2++)
                {
                    textBox2.AppendText(OutputFilePath + fileNames[b2] + " - ConvertedByCziterGaming.mp4\n" + Environment.NewLine);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please wait. You will be informed after succesfull conversion");
            label6.Visible = true;
            label7.Visible = true;
            label9.Visible = true;
            int counter;
            for(int b3 = 0; b3 < amountOfFiles; b3++)
            {
                counter = b3 + 1;
                label6.Text = counter - 1  + "/" + amountOfFiles;
                progressBar1.Visible = true;
                OutputFilePathWithDetails = OutputFilePath + "/" + fileNames[b3] + " - ConvertedByCziterGaming.mp4";
                var inputFile = new MediaToolkit.Model.MediaFile { Filename = files[b3] };
                var outputFile = new MediaToolkit.Model.MediaFile { Filename = OutputFilePathWithDetails };

                ConOptions();

                using (var engine = new Engine())
                {
                    engine.ConvertProgressEvent += ConvertProgressEvent;
                    engine.Convert(inputFile, outputFile, conversionOptions);
                }
            }
            progressBar1.Value = 100;
            Thread.Sleep(200);
            MessageBox.Show("File converted.");
            Process.Start(OutputFilePath + "/");
            Thread.Sleep(200);
            progressBar1.Value = 0;
            label6.Visible = false;
            label7.Visible = false;
            label9.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            contextMenuStrip1.Items.Add("Settings");
            contextMenuStrip1.Items.Add("Exit");

            contextMenuStrip1.Show(button4, new Point(0, button4.Height - 1));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            contextMenuStrip1.Items.Add("Info");

            contextMenuStrip1.Show(button5, new Point(0, button1.Height - 13));
        }
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Settings")
            {
                MessageBox.Show("Not ready yet.");
            }
            else if (e.ClickedItem.Text == "Exit")
            {
                this.Close();
            }
            else if (e.ClickedItem.Text == "Info")
            {
                MessageBox.Show("Created by CziterGaming");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            for (int b8 = 0; b8 < amountOfFiles; b8++)
            {
                OutputFilePath = InputFilePathWithoutFile[0];
                textBox2.AppendText(InputFilePathWithoutFile[b8] + fileNames[b8] + " - ConvertedByCziterGaming.mp4\n" + Environment.NewLine);
            }
        }

        public void ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            float progressbar = (float)e.ProcessedDuration.TotalMilliseconds / (float)e.TotalDuration.TotalMilliseconds * 100;
            progressBar1.Value = (int)progressbar;
            label7.Text = "File size: " + e.SizeKb + "Kb";
            label9.Text = (int)e.ProcessedDuration.TotalSeconds + "s / " + (int)e.TotalDuration.TotalSeconds + "s";
        }

        public void ConOptions()
        {
            if (radioButton1.Checked)
            {
                conversionOptions = new ConversionOptions
                {
                    VideoAspectRatio = VideoAspectRatio.R16_9,
                    VideoSize = VideoSize.Hd720,
                    AudioSampleRate = AudioSampleRate.Hz44100
                };
            }
            else if (radioButton2.Checked)
            {
                conversionOptions = new ConversionOptions
                {
                    VideoAspectRatio = VideoAspectRatio.R16_9,
                    VideoSize = VideoSize.Hd480,
                    AudioSampleRate = AudioSampleRate.Hz44100
                };
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            clearLists();
        }

        public void clearLists()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            files.Clear();
            fileNames.Clear();
            InputFilePathWithoutFile.Clear();
            fileSizes.Clear();
        }
    }
}
