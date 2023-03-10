using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace MicrophonePassthrough {
    public partial class Form1 : Form {
        public static bool actuallyClose = false;
        Thread micThr;
        public Form1() {
            InitializeComponent();
            this.FormClosing += (s, e) => {
                if (Form1.actuallyClose == false) {
                    notifyIcon1.Visible = true;
                    notifyIcon1.ShowBalloonTip(1500);
                    this.ShowInTaskbar = false;
                    this.Hide();
                    e.Cancel = true;
                } else {
                    notifyIcon1.Visible = false;
                    System.Threading.Thread.Sleep(1250);
                    Environment.Exit(0);
                }
            };
        }
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough.dll")]
        unsafe static extern int startMicPassthrough(int captureDev, int playbackDev);
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough.dll")]
        unsafe static extern int retDevNameList(StringBuilder playbackCount, StringBuilder captureCount, StringBuilder playbackListGUI, StringBuilder captureListGUI, int len);
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough.dll")]
        unsafe static extern float getVadProbability();
        private static bool firstRun1 = true;
        private static bool firstRun2 = true;
        private void button1_Click_1(object sender, EventArgs e) {
            var playMsg = new StringBuilder(65535);
            var capMsg = new StringBuilder(65535);
            var playMsgList = new StringBuilder(65535);
            var capMsgList = new StringBuilder(65535);
            retDevNameList(playMsg, capMsg, playMsgList, capMsgList, capMsg.Capacity);
            string[] playbackList = playMsgList.ToString().Split('\n');
            string[] captureList = capMsgList.ToString().Split('\n');
            for (int i = 0; i < Convert.ToInt32(playMsg.ToString()); i++) checkedListBoxVirtMic.Items.Add("[" + i + "]: " + playbackList[i]);
            for (int i = 0; i < Convert.ToInt32(capMsg.ToString()); i++) checkedListBoxRealMic.Items.Add("[" + i + "]: " + captureList[i]);
            bool resetSettings = false;
            if (firstRun1 == true) {
                if (resetSettings) {
                    Properties.Settings.Default.PlaybackDeviceID = 0;
                    Properties.Settings.Default.Save();
                }
                checkedListBoxVirtMic.SetItemChecked(Properties.Settings.Default.PlaybackDeviceID, true);
                this.checkedListBoxVirtMic.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxVirtMic_ItemCheck);
            } else {
                firstRun1 = false;
            }
            if (firstRun2 == true) {
                if (resetSettings) {
                    Properties.Settings.Default.CaptureDeviceID = 0;
                    Properties.Settings.Default.Save();
                }
                checkedListBoxRealMic.SetItemChecked(Properties.Settings.Default.CaptureDeviceID, true);
                this.checkedListBoxRealMic.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxRealMic_ItemCheck);
            } else {
                firstRun2 = false;
            }
            micThr = new Thread(micFunc);
            micThr.IsBackground = true;
            micThr.Start();
        }
        private static void micFunc() {
            while (true) {
                Console.WriteLine("Launching with [capture ID : playback ID]: " + Properties.Settings.Default.CaptureDeviceID.ToString() + " : " + Properties.Settings.Default.PlaybackDeviceID.ToString());
                startMicPassthrough(Properties.Settings.Default.CaptureDeviceID, Properties.Settings.Default.PlaybackDeviceID);
                //startMicPassthrough(1, 0);
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            System.Threading.Thread.Sleep(1250);
            Environment.Exit(0);
            for (int x = 0; x < Application.OpenForms.Count; x++) {
                if (Application.OpenForms[x] != this)
                    Application.OpenForms[x].Close();
            }
            System.Threading.Thread.Sleep(500);
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e) {
            System.Threading.Thread.Sleep(1250);
            Environment.Exit(0);
        }
        private void checkedListBoxVirtMic_ItemCheck(object sender, ItemCheckEventArgs e) {
            for (int ix = 0; ix < checkedListBoxVirtMic.Items.Count; ++ix)
                if (ix != e.Index) checkedListBoxVirtMic.SetItemChecked(ix, false);
            Properties.Settings.Default.PlaybackDeviceID = checkedListBoxVirtMic.Items.IndexOf(checkedListBoxVirtMic.SelectedItem.ToString());
            Console.WriteLine(Properties.Settings.Default.PlaybackDeviceID);
        }
        private void checkedListBoxRealMic_ItemCheck(object sender, ItemCheckEventArgs e) {
            for (int ix = 0; ix < checkedListBoxRealMic.Items.Count; ++ix)
                if (ix != e.Index) checkedListBoxRealMic.SetItemChecked(ix, false);
            Properties.Settings.Default.CaptureDeviceID = checkedListBoxRealMic.Items.IndexOf(checkedListBoxRealMic.SelectedItem.ToString());
            Console.WriteLine(Properties.Settings.Default.CaptureDeviceID);
        }
        private void checkedListBoxVirtMic_SelectedIndexChanged(object sender, EventArgs e) {
            //
        }
        private void checkedListBoxRealMic_SelectedIndexChanged(object sender, EventArgs e) {
            //
        }
        private void button2_Click_1(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            MessageBox.Show("Saving [capture ID : playback ID]: " + Properties.Settings.Default.CaptureDeviceID.ToString() + " : " + Properties.Settings.Default.PlaybackDeviceID.ToString() + "\nRefreshing device list now..");
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            Show();
            this.WindowState = FormWindowState.Normal;
        }
        private void Form1_Load(object sender, EventArgs e) {
            //
        }

        private void timer1_Tick(object sender, EventArgs e) {
            int vadProb = (int)(getVadProbability() * 1000.0F);
            progressBar1.Value = vadProb;
            progressBar1.Refresh();
        }
    }
}