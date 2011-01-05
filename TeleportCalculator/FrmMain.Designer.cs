namespace TeleportCalculator
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.picFlag = new System.Windows.Forms.PictureBox();
            this.cmbServer = new System.Windows.Forms.ComboBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colID = new System.Windows.Forms.ColumnHeader();
            this.colCost = new System.Windows.Forms.ColumnHeader();
            this.colTeleportTime = new System.Windows.Forms.ColumnHeader();
            this.colEfficiency = new System.Windows.Forms.ColumnHeader();
            this.colWalkTime = new System.Windows.Forms.ColumnHeader();
            this.colFlyTime = new System.Windows.Forms.ColumnHeader();
            this.colRideTime = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlag)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(846, 880);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.Resize += new System.EventHandler(this.pictureBox1_Resize);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // picFlag
            // 
            this.picFlag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picFlag.Location = new System.Drawing.Point(634, 895);
            this.picFlag.Name = "picFlag";
            this.picFlag.Size = new System.Drawing.Size(24, 24);
            this.picFlag.TabIndex = 24;
            this.picFlag.TabStop = false;
            // 
            // cmbServer
            // 
            this.cmbServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbServer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbServer.FormattingEnabled = true;
            this.cmbServer.Location = new System.Drawing.Point(664, 898);
            this.cmbServer.Name = "cmbServer";
            this.cmbServer.Size = new System.Drawing.Size(194, 21);
            this.cmbServer.TabIndex = 0;
            this.cmbServer.SelectedIndexChanged += new System.EventHandler(this.cmbServer_SelectedIndexChanged);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colCost,
            this.colTeleportTime,
            this.colEfficiency,
            this.colWalkTime,
            this.colFlyTime,
            this.colRideTime});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(12, 898);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(616, 124);
            this.listView1.TabIndex = 25;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // colID
            // 
            this.colID.Width = 0;
            // 
            // colCost
            // 
            this.colCost.Text = "Cost";
            this.colCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colCost.Width = 87;
            // 
            // colTeleportTime
            // 
            this.colTeleportTime.Text = "Teleport time";
            this.colTeleportTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colTeleportTime.Width = 87;
            // 
            // colEfficiency
            // 
            this.colEfficiency.Text = "Efficiency";
            this.colEfficiency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colEfficiency.Width = 100;
            // 
            // colWalkTime
            // 
            this.colWalkTime.Text = "Walk time";
            this.colWalkTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colWalkTime.Width = 103;
            // 
            // colFlyTime
            // 
            this.colFlyTime.Text = "Fly time";
            this.colFlyTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colFlyTime.Width = 106;
            // 
            // colRideTime
            // 
            this.colRideTime.Text = "Ride time";
            this.colRideTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colRideTime.Width = 125;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 1034);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.picFlag);
            this.Controls.Add(this.cmbServer);
            this.Controls.Add(this.pictureBox1);
            this.Name = "FrmMain";
            this.Text = "PW teleport calculator";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFlag)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cmbServer;
        private System.Windows.Forms.PictureBox picFlag;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colID;
        private System.Windows.Forms.ColumnHeader colCost;
        private System.Windows.Forms.ColumnHeader colTeleportTime;
        private System.Windows.Forms.ColumnHeader colEfficiency;
        private System.Windows.Forms.ColumnHeader colWalkTime;
        private System.Windows.Forms.ColumnHeader colFlyTime;
        private System.Windows.Forms.ColumnHeader colRideTime;
    }
}

