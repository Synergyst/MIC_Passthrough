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
using System.Security.AccessControl;
using MicPassthroughAndRemoteMic.Properties;

namespace MicPassthroughAndRemoteMic {
    public partial class Form1 : Form {
        public static bool actuallyClose = false;
        Thread micThr, micThrNet;
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
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough.dll")]
        unsafe static extern float getDecibel();
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough.dll")]
        unsafe static extern bool transmitState();
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough.dll")]
        unsafe static extern void setVolume(int volume);
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough_Net.dll")]
        unsafe static extern int startMicPassthrough_net(int captureDev, int playbackDev);
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough_Net.dll")]
        unsafe static extern float getVadProbability_net();
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough_Net.dll")]
        unsafe static extern float getDecibel_net();
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough_Net.dll")]
        unsafe static extern bool transmitState_net();
        [System.Runtime.InteropServices.DllImport("MIC_Passthrough_Net.dll")]
        unsafe static extern void setVolume_net(int volume);
        private static bool firstRun1 = true;
        private static bool firstRun2 = true;
        private void Form1_Load(object sender, EventArgs e) {
            pictureBox1.Image = Resources.micpassthroughmuted;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = Resources.micpassthroughmuted;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            //
            var playMsg = new StringBuilder(65535);
            var capMsg = new StringBuilder(65535);
            var playMsgList = new StringBuilder(65535);
            var capMsgList = new StringBuilder(65535);
            retDevNameList(playMsg, capMsg, playMsgList, capMsgList, capMsg.Capacity);
            string[] playbackList = playMsgList.ToString().Split('\n');
            string[] captureList = capMsgList.ToString().Split('\n');
            for (int i = 0; i < Convert.ToInt32(playMsg.ToString()); i++) checkedListBoxVirtMic.Items.Add("[" + i + "]: " + playbackList[i]);
            for (int i = 0; i < Convert.ToInt32(capMsg.ToString()); i++) checkedListBoxRealMic.Items.Add("[" + i + "]: " + captureList[i]);
            for (int i = 0; i < Convert.ToInt32(playMsg.ToString()); i++) checkedListBoxAuxMic.Items.Add("[" + i + "]: " + playbackList[i]);
            bool resetSettings = false;
            if (firstRun1 == true) {
                if (resetSettings) {
                    Properties.Settings.Default.PlaybackDeviceID = 0;
                    Properties.Settings.Default.AuxDeviceID = 0;
                    Properties.Settings.Default.Save();
                }
                checkedListBoxVirtMic.SetItemChecked(Properties.Settings.Default.PlaybackDeviceID, true);
                this.checkedListBoxVirtMic.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxVirtMic_ItemCheck);
                checkedListBoxAuxMic.SetItemChecked(Properties.Settings.Default.AuxDeviceID, true);
                this.checkedListBoxAuxMic.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxAuxMic_ItemCheck);
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
            micThr.DisableComObjectEagerCleanup();
            micThr.Start();
            //
            micThrNet = new Thread(micFunc_net);
            micThrNet.IsBackground = true;
            micThrNet.DisableComObjectEagerCleanup();
            micThrNet.Start();
            knobControl2.Value = Properties.Settings.Default.RemoteDeviceVolume;
            knobControl1.Value = Properties.Settings.Default.LocalDeviceVolume;
            //
        }
        private static void micFunc() {
            while (true) {
                Console.WriteLine("Launching network thread with [capture ID : playback ID : Aux ID]: " + Properties.Settings.Default.CaptureDeviceID.ToString() + " : " + Properties.Settings.Default.PlaybackDeviceID.ToString() + " : " + Properties.Settings.Default.AuxDeviceID.ToString());
                startMicPassthrough(Properties.Settings.Default.CaptureDeviceID, Properties.Settings.Default.PlaybackDeviceID);
                //startMicPassthrough(1, 0);
            }
        }
        private static void micFunc_net() {
            System.Threading.Thread.Sleep(1500);
            //
            while (true) {
                Console.WriteLine("Launching network thread with [capture ID : playback ID : Aux ID]: " + Properties.Settings.Default.CaptureDeviceID.ToString() + " : " + Properties.Settings.Default.PlaybackDeviceID.ToString() + " : " + Properties.Settings.Default.AuxDeviceID.ToString());
                startMicPassthrough_net(Properties.Settings.Default.CaptureDeviceID, Properties.Settings.Default.AuxDeviceID);
                //startMicPassthrough_net(3, 5);
            }
        }
        private void button3_Click(object sender, EventArgs e) {
            System.Threading.Thread.Sleep(1250);
            Environment.Exit(0);
        }
        private void checkedListBoxAuxMic_ItemCheck(object sender, ItemCheckEventArgs e) {
            for (int ix = 0; ix < checkedListBoxAuxMic.Items.Count; ++ix)
                if (ix != e.Index) checkedListBoxAuxMic.SetItemChecked(ix, false);
            Properties.Settings.Default.AuxDeviceID = checkedListBoxAuxMic.Items.IndexOf(checkedListBoxAuxMic.SelectedItem.ToString());
            Console.WriteLine(Properties.Settings.Default.AuxDeviceID);
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
        private void checkedListBoxAuxMic_SelectedIndexChanged(object sender, EventArgs e) {
            //
        }
        private void button2_Click_1(object sender, EventArgs e) {
            Properties.Settings.Default.Save();
            MessageBox.Show("Saving [capture ID : playback ID : Aux ID]: " + Properties.Settings.Default.CaptureDeviceID.ToString() + " : " + Properties.Settings.Default.PlaybackDeviceID.ToString() + " : " + Properties.Settings.Default.AuxDeviceID.ToString() + "\nRefreshing device list now..");
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            Show();
            this.WindowState = FormWindowState.Normal;
        }
        private void timer1_Tick(object sender, EventArgs e) {
            int vadProb = (int)(getVadProbability() * 1000.0F);
            progressBar1.Value = vadProb;
            progressBar1.Refresh();
        }
        private void timer3_Tick(object sender, EventArgs e) {
            int vadProb = (int)(getVadProbability_net() * 1000.0F);
            progressBar3.Value = vadProb;
            progressBar3.Refresh();
        }
        private void timer4_Tick(object sender, EventArgs e) {
            if (!transmitState_net()) {
                int decibelLevel = (int)((getDecibel_net() + 60.0F) * 1.6667F);
                var decibelLevelStr = decibelLevel.ToString();
                if (decibelLevel > 0) {
                    progressBar4.Value = decibelLevel;
                } else {
                    if (progressBar4.Value > 0) {
                        progressBar4.Value = progressBar4.Value - 1;
                    }
                }
                progressBar4.Refresh();
            } else {
                progressBar4.Value = 0;
            }
        }
        private void timer5_Tick(object sender, EventArgs e) {
            if (transmitState()) {
                pictureBox1.Image = Resources.micpassthrough;
            } else {
                pictureBox1.Image = Resources.micpassthroughmuted;
            }
            if (!transmitState_net()) {
                pictureBox2.Image = Resources.micpassthrough;
            } else {
                pictureBox2.Image = Resources.micpassthroughmuted;
            }
        }
        private void knobControl1_Load(object sender, EventArgs e) {
            //
        }
        private void knobControl2_Load(object sender, EventArgs e) {
            //
        }
        private void knobControl1_ValueChanged(object sender) {
            Properties.Settings.Default.LocalDeviceVolume = knobControl1.Value;
            Properties.Settings.Default.Save();
        }
        private void knobControl2_ValueChanged(object sender) {
            Properties.Settings.Default.RemoteDeviceVolume = knobControl2.Value;
            Properties.Settings.Default.Save();
        }
        private void timer6_Tick(object sender, EventArgs e) {
            setVolume(knobControl1.Value);
            setVolume_net(knobControl2.Value);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            //setAmplified(checkBox1.Checked);
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            //setAmplified_net(checkBox2.Checked);
        }
        private void timer2_Tick(object sender, EventArgs e) {
            if (transmitState()) {
                int decibelLevel = (int)((getDecibel() + 60.0F) * 1.6667F);
                var decibelLevelStr = decibelLevel.ToString();
                if (decibelLevel > 0) {
                    progressBar2.Value = decibelLevel;
                } else {
                    if (progressBar2.Value > 0) {
                        progressBar2.Value = progressBar2.Value - 1;
                    }
                }
                progressBar2.Refresh();
            } else {
                progressBar2.Value = 0;
            }
            //Console.WriteLine(decibelLevel.ToString());
        }
    }
}