using System;
using System.Drawing;
using System.Windows.Forms;
using MediaToolkit;
using MediaToolkit.Options;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VideoResizerCutter
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
        bool isEditingEnabled;
        int width;
        int height;
        bool iscustomResolutionEnabled = false;

        ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
        private ConversionOptions conversionOptions;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = 540;
            contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuStrip1_ItemClicked);
            progressBar1.Visible = false;
            this.textBox1.ScrollBars = ScrollBars.Both;
            this.textBox2.ScrollBars = ScrollBars.Both;
            label6.Visible = false;
            label7.Visible = false;
            label9.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            button10.Visible = true;
            label8.Visible = false;
            label11.Visible = false;
            fromMinutesTextBox.Visible = false;
            FromSecondsTextBox.Visible = false;
            ToMinutesTextBox.Visible = false;
            ToSecondsTextBox.Visible = false;
            label12.Visible = false;
            label10.Visible = false;
            axWindowsMediaPlayer1.settings.autoStart = false;
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
                    button7.Visible = true;
                    button6.Visible = true;
                    foreach (string list in openFileDialog.FileNames)
                    {
                        files.Add(list);
                        fileNames.Add(Path.GetFileNameWithoutExtension(list));
                        InputFilePathWithoutFile.Add(Path.GetDirectoryName(list));
                        textBox1.AppendText(list + "\n" + Environment.NewLine);
                    }
                    amountOfFiles = openFileDialog.FileNames.Length;
                    textBox3.Text = "Currently editing file: " + files[0];
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(!fileNames.Any())
            {
                MessageBox.Show("Select input file ffirst.");
                return;
            }

            button6.Visible = true;

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                OutputFilePath = fbd.SelectedPath;
                
                if(String.IsNullOrEmpty(fbd.SelectedPath))
                {
                    return;
                }

                for (int b2 = 0; b2 < amountOfFiles; b2++)
                {
                    textBox2.AppendText(OutputFilePath + fileNames[b2] + " - ConvertedByCziterGaming.mp4\n" + Environment.NewLine);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ToMinutesTextBox.BackColor = Color.White;
            ToMinutesTextBox.BackColor = Color.White;

            if (!fileNames.Any() || String.IsNullOrEmpty(OutputFilePath))
            {
                MessageBox.Show("Not input file or output folder selected.");
                return;
            }

            if (!Regex.IsMatch(fromMinutesTextBox.Text, "^[0-9]{0,999}$"))
            {
                MessageBox.Show("Invalid number.");
                fromMinutesTextBox.Text = "";
                fromMinutesTextBox.BackColor = Color.LightCoral;
                return;
            }

            if (!Regex.IsMatch(FromSecondsTextBox.Text, "^[0-9]{0,999}$"))
            {
                MessageBox.Show("Invalid number.");
                FromSecondsTextBox.Text = "";
                FromSecondsTextBox.BackColor = Color.LightCoral;
                return;
            }

            if (!Regex.IsMatch(ToMinutesTextBox.Text, "^[0-9]{0,999}$"))
            {
                MessageBox.Show("Invalid number.");
                ToMinutesTextBox.Text = "";
                ToMinutesTextBox.BackColor = Color.LightCoral;
                return;
            }

            if (!Regex.IsMatch(ToSecondsTextBox.Text, "^[0-9]{0,999}$"))
            {
                MessageBox.Show("Invalid number.");
                ToSecondsTextBox.Text = "";
                ToSecondsTextBox.BackColor = Color.LightCoral;
                return;
            }

            MessageBox.Show("Please wait. You will be informed after succesfull conversion. Don't click the window while converting otherwise it will crashed.");
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

                if (iscustomResolutionEnabled)
                {
                    if (width == 0 || height == 0)
                    {
                        MessageBox.Show("One of resolution variables is 0 or null.");
                        iscustomResolutionEnabled = false;
                        progressBar1.Value = 0;
                        label6.Visible = false;
                        label7.Visible = false;
                        label9.Visible = false;
                        progressBar1.Visible = false;
                        return;
                    }
                }

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
            button6.Visible = true;
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
            label9.Text = (int)e.ProcessedDuration.TotalSeconds + " / " + (int)e.TotalDuration.TotalSeconds;
        }

        public void ConOptions()
        {
            double firstCut;
            double secondCut;

            if (isEditingEnabled)
            {
                if (String.IsNullOrEmpty(fromMinutesTextBox.Text))
                {
                    firstCut = Convert.ToDouble(FromSecondsTextBox.Text);
                }
                else
                {
                    if (String.IsNullOrEmpty(FromSecondsTextBox.Text))
                    {
                        firstCut = Convert.ToDouble(fromMinutesTextBox.Text) * 60;
                    }
                    else
                    {
                        firstCut = Convert.ToDouble(fromMinutesTextBox.Text) * 60 + Convert.ToDouble(FromSecondsTextBox.Text);
                    }
                }

                if (String.IsNullOrEmpty(ToMinutesTextBox.Text))
                {
                    secondCut = Convert.ToDouble(ToSecondsTextBox.Text) - firstCut;
                }
                else
                {
                    if (String.IsNullOrEmpty(ToSecondsTextBox.Text))
                    {
                        secondCut = Convert.ToDouble(ToMinutesTextBox.Text) * 60 - firstCut;
                    }
                    else
                    {
                        secondCut = (Convert.ToDouble(ToMinutesTextBox.Text) * 60 + Convert.ToDouble(ToSecondsTextBox.Text)) - firstCut;
                    }
                }
                Console.WriteLine(firstCut + "   " + secondCut);
            }
            else
            {
                firstCut = 0;
                secondCut = 0;
            }


            if (radioButton1.Checked)
            {
                if (isEditingEnabled)
                {
                    def720pConv();
                    conversionOptions.CutMedia(TimeSpan.FromSeconds(firstCut), TimeSpan.FromSeconds(secondCut));
                }
                else if (!isEditingEnabled)
                {
                    def720pConv();
                }
                else
                {
                    MessageBox.Show("error.");
                    Environment.Exit(0);
                }
            }
            else if (radioButton2.Checked)
            {
                if (isEditingEnabled)
                {
                    def480pConv();
                    conversionOptions.CutMedia(TimeSpan.FromSeconds(firstCut), TimeSpan.FromSeconds(secondCut));
                }
                else if (!isEditingEnabled)
                {
                    def480pConv();
                }
                else
                {
                    MessageBox.Show("error.");
                    Environment.Exit(0);
                }
            }
            else if (radioButton3.Checked)
            {
                iscustomResolutionEnabled = true;

                CustomRes customRes = new CustomRes(this);
                customRes.ShowDialog();

                if (isEditingEnabled)
                {
                    conversionOptions = new ConversionOptions
                    {
                        CustomHeight = height,
                        CustomWidth = width,
                        VideoSize = VideoSize.Custom,
                        AudioSampleRate = AudioSampleRate.Hz44100
                    };

                    conversionOptions.CutMedia(TimeSpan.FromSeconds(firstCut), TimeSpan.FromSeconds(secondCut));
                }
                else if (!isEditingEnabled)
                {
                    conversionOptions = new ConversionOptions
                    {
                        CustomHeight = height,
                        CustomWidth = width,
                        VideoSize = VideoSize.Custom,
                        AudioSampleRate = AudioSampleRate.Hz44100
                    };
                }
                else
                {
                    MessageBox.Show("error.");
                    Environment.Exit(0);
                }
            }
        }

        public void def720pConv()
        {
            conversionOptions = new ConversionOptions
            {
                VideoAspectRatio = VideoAspectRatio.R16_9,
                VideoSize = VideoSize.Hd720,
                AudioSampleRate = AudioSampleRate.Hz44100
            };
        }
        public void def480pConv()
        {
            conversionOptions = new ConversionOptions
            {
                VideoAspectRatio = VideoAspectRatio.R16_9,
                VideoSize = VideoSize.Hd480,
                AudioSampleRate = AudioSampleRate.Hz44100
            };
        }

        private void button6_Click(object sender, EventArgs e)
        {
            clearLists();
            button6.Visible = false;
            button7.Visible = false;
        }

        public void clearLists()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            files.Clear();
            fileNames.Clear();
            InputFilePathWithoutFile.Clear();
            fileSizes.Clear();
            OutputFilePath = "";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(files[0]))
            {
                MessageBox.Show("No file selected.");
                return;
            }
            MessageBox.Show("Video editing is beta option. Editing is currently possible only to one video.");

            axWindowsMediaPlayer1.URL = files[0];
            this.Width = 1202;
            button7.Visible = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = "";
            this.Width = 540;
            button7.Visible = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            button10.Visible = false;
            label8.Visible = true;
            label11.Visible = true;
            fromMinutesTextBox.Visible = true;
            FromSecondsTextBox.Visible = true;
            ToMinutesTextBox.Visible = true;
            ToSecondsTextBox.Visible = true;
            label12.Visible = true;
            label10.Visible = true;
            isEditingEnabled = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            button10.Visible = true;
            label8.Visible = false;
            label11.Visible = false;
            fromMinutesTextBox.Visible = false;
            FromSecondsTextBox.Visible = false;
            ToMinutesTextBox.Visible = false;
            ToSecondsTextBox.Visible = false;
            label12.Visible = false;
            label10.Visible = false;
            isEditingEnabled = false;
        }

        public void setResolution(string width, string height)
        {
            if(String.IsNullOrEmpty(width) || String.IsNullOrEmpty(height))
            {
                this.width = 0;
                this.height = 0;
            }
            this.width = Convert.ToInt32(width);
            this.height = Convert.ToInt32(height);
        }
    }
}