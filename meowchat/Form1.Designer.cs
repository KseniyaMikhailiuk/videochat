﻿namespace meowchat
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxMine = new System.Windows.Forms.PictureBox();
            this.camToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.labelPortListen = new System.Windows.Forms.Label();
            this.labelMyPort = new System.Windows.Forms.Label();
            this.labelServerIP = new System.Windows.Forms.Label();
            this.textBoxFriendPort = new System.Windows.Forms.TextBox();
            this.textBoxMyPort = new System.Windows.Forms.TextBox();
            this.textBoxServerIP = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.pictureBoxFriend = new System.Windows.Forms.PictureBox();
            this.mylocalhostlabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.audioMyPort = new System.Windows.Forms.TextBox();
            this.audioFriendPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMine)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFriend)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxMine
            // 
            this.pictureBoxMine.BackColor = System.Drawing.Color.White;
            this.pictureBoxMine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxMine.Location = new System.Drawing.Point(12, 423);
            this.pictureBoxMine.Name = "pictureBoxMine";
            this.pictureBoxMine.Size = new System.Drawing.Size(774, 500);
            this.pictureBoxMine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMine.TabIndex = 0;
            this.pictureBoxMine.TabStop = false;
            // 
            // camToolStripMenuItem
            // 
            this.camToolStripMenuItem.Name = "camToolStripMenuItem";
            this.camToolStripMenuItem.Size = new System.Drawing.Size(123, 36);
            this.camToolStripMenuItem.Text = "WebCam";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(75, 36);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(75, 36);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.camToolStripMenuItem,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2439, 40);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // labelPortListen
            // 
            this.labelPortListen.AutoSize = true;
            this.labelPortListen.Location = new System.Drawing.Point(45, 172);
            this.labelPortListen.Name = "labelPortListen";
            this.labelPortListen.Size = new System.Drawing.Size(108, 25);
            this.labelPortListen.TabIndex = 4;
            this.labelPortListen.Text = "Port listen";
            // 
            // labelMyPort
            // 
            this.labelMyPort.AutoSize = true;
            this.labelMyPort.Location = new System.Drawing.Point(45, 242);
            this.labelMyPort.Name = "labelMyPort";
            this.labelMyPort.Size = new System.Drawing.Size(86, 25);
            this.labelMyPort.TabIndex = 5;
            this.labelMyPort.Text = "My Port";
            // 
            // labelServerIP
            // 
            this.labelServerIP.AutoSize = true;
            this.labelServerIP.Location = new System.Drawing.Point(588, 271);
            this.labelServerIP.Name = "labelServerIP";
            this.labelServerIP.Size = new System.Drawing.Size(100, 25);
            this.labelServerIP.TabIndex = 6;
            this.labelServerIP.Text = "Server IP";
            // 
            // textBoxFriendPort
            // 
            this.textBoxFriendPort.Location = new System.Drawing.Point(50, 200);
            this.textBoxFriendPort.Name = "textBoxFriendPort";
            this.textBoxFriendPort.Size = new System.Drawing.Size(145, 31);
            this.textBoxFriendPort.TabIndex = 7;
            this.textBoxFriendPort.Text = "8008";
            // 
            // textBoxMyPort
            // 
            this.textBoxMyPort.Location = new System.Drawing.Point(50, 270);
            this.textBoxMyPort.Name = "textBoxMyPort";
            this.textBoxMyPort.Size = new System.Drawing.Size(145, 31);
            this.textBoxMyPort.TabIndex = 8;
            this.textBoxMyPort.Text = "8009";
            // 
            // textBoxServerIP
            // 
            this.textBoxServerIP.Location = new System.Drawing.Point(593, 271);
            this.textBoxServerIP.Name = "textBoxServerIP";
            this.textBoxServerIP.Size = new System.Drawing.Size(145, 31);
            this.textBoxServerIP.TabIndex = 9;
            this.textBoxServerIP.Text = "127.0.0.1";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(587, 192);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(143, 47);
            this.buttonConnect.TabIndex = 10;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // pictureBoxFriend
            // 
            this.pictureBoxFriend.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBoxFriend.Location = new System.Drawing.Point(809, 43);
            this.pictureBoxFriend.Name = "pictureBoxFriend";
            this.pictureBoxFriend.Size = new System.Drawing.Size(1630, 1135);
            this.pictureBoxFriend.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxFriend.TabIndex = 11;
            this.pictureBoxFriend.TabStop = false;
            // 
            // mylocalhostlabel
            // 
            this.mylocalhostlabel.AutoSize = true;
            this.mylocalhostlabel.Location = new System.Drawing.Point(582, 122);
            this.mylocalhostlabel.Name = "mylocalhostlabel";
            this.mylocalhostlabel.Size = new System.Drawing.Size(0, 25);
            this.mylocalhostlabel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(588, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 25);
            this.label1.TabIndex = 12;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 25);
            this.label2.TabIndex = 13;
            this.label2.Text = "video";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(358, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 25);
            this.label3.TabIndex = 14;
            this.label3.Text = "audio";
            // 
            // audioMyPort
            // 
            this.audioMyPort.Location = new System.Drawing.Point(320, 271);
            this.audioMyPort.Name = "audioMyPort";
            this.audioMyPort.Size = new System.Drawing.Size(145, 31);
            this.audioMyPort.TabIndex = 18;
            this.audioMyPort.Text = "8011";
            // 
            // audioFriendPort
            // 
            this.audioFriendPort.Location = new System.Drawing.Point(320, 201);
            this.audioFriendPort.Name = "audioFriendPort";
            this.audioFriendPort.Size = new System.Drawing.Size(145, 31);
            this.audioFriendPort.TabIndex = 17;
            this.audioFriendPort.Text = "8010";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(315, 243);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 25);
            this.label4.TabIndex = 16;
            this.label4.Text = "My Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(315, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 25);
            this.label5.TabIndex = 15;
            this.label5.Text = "Port listen";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(2439, 1190);
            this.Controls.Add(this.audioMyPort);
            this.Controls.Add(this.audioFriendPort);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mylocalhostlabel);
            this.Controls.Add(this.pictureBoxFriend);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.textBoxServerIP);
            this.Controls.Add(this.textBoxMyPort);
            this.Controls.Add(this.textBoxFriendPort);
            this.Controls.Add(this.labelServerIP);
            this.Controls.Add(this.labelMyPort);
            this.Controls.Add(this.labelPortListen);
            this.Controls.Add(this.pictureBoxMine);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "MEOWCHAT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMine)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFriend)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxMine;
        private System.Windows.Forms.ToolStripMenuItem camToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label labelPortListen;
        private System.Windows.Forms.Label labelMyPort;
        private System.Windows.Forms.Label labelServerIP;
        private System.Windows.Forms.TextBox textBoxFriendPort;
        private System.Windows.Forms.TextBox textBoxMyPort;
        private System.Windows.Forms.TextBox textBoxServerIP;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Label mylocalhostlabel;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.PictureBox pictureBoxFriend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox audioMyPort;
        private System.Windows.Forms.TextBox audioFriendPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

