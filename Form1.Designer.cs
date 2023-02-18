
using System;
using System.Windows.Forms;

namespace MicrophonePassthrough
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.checkedListBoxVirtMic = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkedListBoxRealMic = new System.Windows.Forms.CheckedListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.checkedListBoxAuxMic = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.button4 = new System.Windows.Forms.Button();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.progressBar4 = new System.Windows.Forms.ProgressBar();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.knobControl1 = new KnobControl.KnobControl();
            this.knobControl2 = new KnobControl.KnobControl();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(70, 533);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(38, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(377, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(39, 27);
            this.button3.TabIndex = 3;
            this.button3.Text = "Quit";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Double-click to unhide!";
            this.notifyIcon1.BalloonTipTitle = "Microphone Passthrough";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Microphone Passthrough";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Real Microphone";
            // 
            // checkedListBoxVirtMic
            // 
            this.checkedListBoxVirtMic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(248)))));
            this.checkedListBoxVirtMic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxVirtMic.FormattingEnabled = true;
            this.checkedListBoxVirtMic.Location = new System.Drawing.Point(3, 206);
            this.checkedListBoxVirtMic.Name = "checkedListBoxVirtMic";
            this.checkedListBoxVirtMic.Size = new System.Drawing.Size(413, 152);
            this.checkedListBoxVirtMic.TabIndex = 5;
            this.checkedListBoxVirtMic.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxVirtMic_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Virtual Microphone";
            // 
            // checkedListBoxRealMic
            // 
            this.checkedListBoxRealMic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(248)))));
            this.checkedListBoxRealMic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxRealMic.FormattingEnabled = true;
            this.checkedListBoxRealMic.Location = new System.Drawing.Point(3, 35);
            this.checkedListBoxRealMic.Name = "checkedListBoxRealMic";
            this.checkedListBoxRealMic.Size = new System.Drawing.Size(413, 152);
            this.checkedListBoxRealMic.TabIndex = 7;
            this.checkedListBoxRealMic.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxRealMic_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(242, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(129, 27);
            this.button2.TabIndex = 8;
            this.button2.Text = "Save Device Selections";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(145, 535);
            this.progressBar1.MarqueeAnimationSpeed = 1;
            this.progressBar1.Maximum = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 11);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F);
            this.label3.Location = new System.Drawing.Point(143, 550);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "Voice activity";
            // 
            // checkedListBoxAuxMic
            // 
            this.checkedListBoxAuxMic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(248)))));
            this.checkedListBoxAuxMic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxAuxMic.FormattingEnabled = true;
            this.checkedListBoxAuxMic.Location = new System.Drawing.Point(3, 377);
            this.checkedListBoxAuxMic.Name = "checkedListBoxAuxMic";
            this.checkedListBoxAuxMic.Size = new System.Drawing.Size(413, 152);
            this.checkedListBoxAuxMic.TabIndex = 13;
            this.checkedListBoxAuxMic.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxAuxMic_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 361);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "AUX Microphone";
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(316, 536);
            this.progressBar2.MarqueeAnimationSpeed = 1;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(100, 11);
            this.progressBar2.Step = 1;
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar2.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F);
            this.label5.Location = new System.Drawing.Point(362, 550);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "Loudness";
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 25;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // radioButton1
            // 
            this.radioButton1.Location = new System.Drawing.Point(244, 361);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(61, 16);
            this.radioButton1.TabIndex = 17;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "C";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.Location = new System.Drawing.Point(311, 361);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(49, 16);
            this.radioButton2.TabIndex = 18;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "A";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.Location = new System.Drawing.Point(366, 361);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(50, 16);
            this.radioButton3.TabIndex = 19;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Z";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.Location = new System.Drawing.Point(183, 361);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(55, 16);
            this.radioButton4.TabIndex = 20;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "no-net";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(53, 636);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(57, 23);
            this.button4.TabIndex = 21;
            this.button4.Text = "Start Net";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // progressBar3
            // 
            this.progressBar3.Location = new System.Drawing.Point(145, 648);
            this.progressBar3.MarqueeAnimationSpeed = 1;
            this.progressBar3.Maximum = 1000;
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.Size = new System.Drawing.Size(100, 11);
            this.progressBar3.Step = 1;
            this.progressBar3.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar3.TabIndex = 22;
            // 
            // progressBar4
            // 
            this.progressBar4.Location = new System.Drawing.Point(316, 648);
            this.progressBar4.MarqueeAnimationSpeed = 1;
            this.progressBar4.Name = "progressBar4";
            this.progressBar4.Size = new System.Drawing.Size(100, 11);
            this.progressBar4.Step = 1;
            this.progressBar4.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar4.TabIndex = 23;
            // 
            // timer3
            // 
            this.timer3.Enabled = true;
            this.timer3.Interval = 50;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // timer4
            // 
            this.timer4.Enabled = true;
            this.timer4.Interval = 25;
            this.timer4.Tick += new System.EventHandler(this.timer4_Tick);
            // 
            // timer5
            // 
            this.timer5.Enabled = true;
            this.timer5.Interval = 200;
            this.timer5.Tick += new System.EventHandler(this.timer5_Tick);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(116, 636);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(23, 23);
            this.pictureBox2.TabIndex = 25;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(114, 533);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(23, 23);
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // knobControl1
            // 
            this.knobControl1.EndAngle = 405F;
            this.knobControl1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.knobControl1.KnobBackColor = System.Drawing.Color.WhiteSmoke;
            this.knobControl1.KnobPointerStyle = KnobControl.KnobControl.KnobPointerStyles.line;
            this.knobControl1.LargeChange = 5;
            this.knobControl1.Location = new System.Drawing.Point(251, 535);
            this.knobControl1.Maximum = 100;
            this.knobControl1.Minimum = 0;
            this.knobControl1.Name = "knobControl1";
            this.knobControl1.PointerColor = System.Drawing.Color.SlateBlue;
            this.knobControl1.ScaleColor = System.Drawing.Color.Black;
            this.knobControl1.ScaleDivisions = 11;
            this.knobControl1.ScaleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.knobControl1.ScaleSubDivisions = 4;
            this.knobControl1.ShowLargeScale = true;
            this.knobControl1.ShowSmallScale = false;
            this.knobControl1.Size = new System.Drawing.Size(59, 59);
            this.knobControl1.SmallChange = 1;
            this.knobControl1.StartAngle = 135F;
            this.knobControl1.TabIndex = 26;
            this.knobControl1.Value = 0;
            // 
            // knobControl2
            // 
            this.knobControl2.EndAngle = 405F;
            this.knobControl2.ImeMode = System.Windows.Forms.ImeMode.On;
            this.knobControl2.KnobBackColor = System.Drawing.Color.WhiteSmoke;
            this.knobControl2.KnobPointerStyle = KnobControl.KnobControl.KnobPointerStyles.line;
            this.knobControl2.LargeChange = 5;
            this.knobControl2.Location = new System.Drawing.Point(251, 615);
            this.knobControl2.Maximum = 100;
            this.knobControl2.Minimum = 0;
            this.knobControl2.Name = "knobControl2";
            this.knobControl2.PointerColor = System.Drawing.Color.SlateBlue;
            this.knobControl2.ScaleColor = System.Drawing.Color.Black;
            this.knobControl2.ScaleDivisions = 11;
            this.knobControl2.ScaleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.knobControl2.ScaleSubDivisions = 4;
            this.knobControl2.ShowLargeScale = true;
            this.knobControl2.ShowSmallScale = false;
            this.knobControl2.Size = new System.Drawing.Size(59, 59);
            this.knobControl2.SmallChange = 1;
            this.knobControl2.StartAngle = 135F;
            this.knobControl2.TabIndex = 27;
            this.knobControl2.Value = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F);
            this.label6.Location = new System.Drawing.Point(143, 662);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 12);
            this.label6.TabIndex = 28;
            this.label6.Text = "Voice activity";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F);
            this.label7.Location = new System.Drawing.Point(362, 662);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 12);
            this.label7.TabIndex = 29;
            this.label7.Text = "Loudness";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(419, 688);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.knobControl2);
            this.Controls.Add(this.knobControl1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.progressBar4);
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.radioButton4);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkedListBoxAuxMic);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkedListBoxRealMic);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkedListBoxVirtMic);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Microphone Passthrough";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private NotifyIcon notifyIcon1;
        private Label label1;
        private CheckedListBox checkedListBoxVirtMic;
        private Label label2;
        private CheckedListBox checkedListBoxRealMic;
        private Button button2;
        private Timer timer1;
        private ProgressBar progressBar1;
        private Label label3;
        private CheckedListBox checkedListBoxAuxMic;
        private Label label4;
        private ProgressBar progressBar2;
        private Label label5;
        private Timer timer2;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;
        private Button button4;
        private ProgressBar progressBar3;
        private ProgressBar progressBar4;
        private Timer timer3;
        private Timer timer4;
        private Timer timer5;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private KnobControl.KnobControl knobControl1;
        private KnobControl.KnobControl knobControl2;
        private Label label6;
        private Label label7;
    }
}

