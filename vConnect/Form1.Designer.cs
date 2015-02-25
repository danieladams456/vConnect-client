namespace vConnect
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

                // Make icon disappear on close.
                trayIcon.Icon = null;
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
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.help_button = new System.Windows.Forms.Button();
            this.close_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.port_number = new System.Windows.Forms.Label();
            this.server_IP = new System.Windows.Forms.Label();
            this.edit_port_button = new System.Windows.Forms.Button();
            this.edit_IP_button = new System.Windows.Forms.Button();
            this.BT_ID = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.device_Status_Label = new System.Windows.Forms.Label();
            this.browse_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.OBDIITest = new System.Windows.Forms.Button();
            this.DataTest = new System.Windows.Forms.Button();
            this.CacheTest = new System.Windows.Forms.Button();
            this.ServerTest = new System.Windows.Forms.Button();
            this.RemoveTest = new System.Windows.Forms.Button();
            this.disconnect_BT = new System.Windows.Forms.Button();
            this.update_schema_button = new System.Windows.Forms.Button();
            this.start_button = new System.Windows.Forms.Button();
            this.stop_polling_button = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "vConnect";
            this.trayIcon.Visible = true;
            // 
            // help_button
            // 
            this.help_button.Location = new System.Drawing.Point(331, 299);
            this.help_button.Name = "help_button";
            this.help_button.Size = new System.Drawing.Size(75, 23);
            this.help_button.TabIndex = 0;
            this.help_button.Text = "Help";
            this.help_button.UseVisualStyleBackColor = true;
            this.help_button.Click += new System.EventHandler(this.help_button_Click);
            // 
            // close_button
            // 
            this.close_button.Location = new System.Drawing.Point(250, 299);
            this.close_button.Name = "close_button";
            this.close_button.Size = new System.Drawing.Size(75, 23);
            this.close_button.TabIndex = 3;
            this.close_button.Text = "Close";
            this.close_button.UseVisualStyleBackColor = true;
            this.close_button.Click += new System.EventHandler(this.close_button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Server IP Address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(213, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Server Port Number:";
            // 
            // port_number
            // 
            this.port_number.AutoSize = true;
            this.port_number.Location = new System.Drawing.Point(213, 123);
            this.port_number.Name = "port_number";
            this.port_number.Size = new System.Drawing.Size(103, 13);
            this.port_number.TabIndex = 12;
            this.port_number.Text = "Current Port Number";
            // 
            // server_IP
            // 
            this.server_IP.AutoSize = true;
            this.server_IP.Location = new System.Drawing.Point(213, 45);
            this.server_IP.Name = "server_IP";
            this.server_IP.Size = new System.Drawing.Size(129, 13);
            this.server_IP.TabIndex = 13;
            this.server_IP.Text = "Current Server IP Address";
            // 
            // edit_port_button
            // 
            this.edit_port_button.Location = new System.Drawing.Point(216, 139);
            this.edit_port_button.Name = "edit_port_button";
            this.edit_port_button.Size = new System.Drawing.Size(75, 23);
            this.edit_port_button.TabIndex = 14;
            this.edit_port_button.Text = "Edit ";
            this.edit_port_button.UseVisualStyleBackColor = true;
            this.edit_port_button.Click += new System.EventHandler(this.edit_port_button_Click);
            // 
            // edit_IP_button
            // 
            this.edit_IP_button.Location = new System.Drawing.Point(216, 61);
            this.edit_IP_button.Name = "edit_IP_button";
            this.edit_IP_button.Size = new System.Drawing.Size(75, 23);
            this.edit_IP_button.TabIndex = 15;
            this.edit_IP_button.Text = "Edit";
            this.edit_IP_button.UseVisualStyleBackColor = true;
            this.edit_IP_button.Click += new System.EventHandler(this.edit_IP_button_Click);
            // 
            // BT_ID
            // 
            this.BT_ID.AutoSize = true;
            this.BT_ID.Location = new System.Drawing.Point(12, 45);
            this.BT_ID.Name = "BT_ID";
            this.BT_ID.Size = new System.Drawing.Size(18, 13);
            this.BT_ID.TabIndex = 19;
            this.BT_ID.Text = "ID";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 112);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Device Status:";
            // 
            // device_Status_Label
            // 
            this.device_Status_Label.AutoSize = true;
            this.device_Status_Label.Location = new System.Drawing.Point(12, 125);
            this.device_Status_Label.Name = "device_Status_Label";
            this.device_Status_Label.Size = new System.Drawing.Size(130, 13);
            this.device_Status_Label.TabIndex = 21;
            this.device_Status_Label.Text = "Connected/Disconnected";
            // 
            // browse_button
            // 
            this.browse_button.Location = new System.Drawing.Point(12, 167);
            this.browse_button.Name = "browse_button";
            this.browse_button.Size = new System.Drawing.Size(130, 23);
            this.browse_button.TabIndex = 23;
            this.browse_button.Text = "Browse";
            this.browse_button.UseVisualStyleBackColor = true;
            this.browse_button.Click += new System.EventHandler(this.browse_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Connect to ODBII Device:";
            // 
            // OBDIITest
            // 
            this.OBDIITest.Location = new System.Drawing.Point(425, 248);
            this.OBDIITest.Name = "OBDIITest";
            this.OBDIITest.Size = new System.Drawing.Size(97, 45);
            this.OBDIITest.TabIndex = 25;
            this.OBDIITest.Text = "Test OBDII codes";
            this.OBDIITest.UseVisualStyleBackColor = true;
            this.OBDIITest.Click += new System.EventHandler(this.OBDIITest_Click);
            // 
            // DataTest
            // 
            this.DataTest.Location = new System.Drawing.Point(425, 197);
            this.DataTest.Name = "DataTest";
            this.DataTest.Size = new System.Drawing.Size(97, 45);
            this.DataTest.TabIndex = 26;
            this.DataTest.Text = "Test Vehicle Data Element";
            this.DataTest.UseVisualStyleBackColor = true;
            this.DataTest.Click += new System.EventHandler(this.DataTest_Click);
            // 
            // CacheTest
            // 
            this.CacheTest.Location = new System.Drawing.Point(425, 147);
            this.CacheTest.Name = "CacheTest";
            this.CacheTest.Size = new System.Drawing.Size(97, 45);
            this.CacheTest.TabIndex = 27;
            this.CacheTest.Text = "Test Data Cache";
            this.CacheTest.UseVisualStyleBackColor = true;
            this.CacheTest.Click += new System.EventHandler(this.CacheTest_Click);
            // 
            // ServerTest
            // 
            this.ServerTest.Location = new System.Drawing.Point(425, 96);
            this.ServerTest.Name = "ServerTest";
            this.ServerTest.Size = new System.Drawing.Size(97, 45);
            this.ServerTest.TabIndex = 28;
            this.ServerTest.Text = "Test Data to Server";
            this.ServerTest.UseVisualStyleBackColor = true;
            this.ServerTest.Click += new System.EventHandler(this.ServerTest_Click);
            // 
            // RemoveTest
            // 
            this.RemoveTest.Location = new System.Drawing.Point(425, 45);
            this.RemoveTest.Name = "RemoveTest";
            this.RemoveTest.Size = new System.Drawing.Size(97, 45);
            this.RemoveTest.TabIndex = 29;
            this.RemoveTest.Text = "Test Removing data from cache";
            this.RemoveTest.UseVisualStyleBackColor = true;
            this.RemoveTest.Click += new System.EventHandler(this.RemoveTest_Click);
            // 
            // disconnect_BT
            // 
            this.disconnect_BT.Location = new System.Drawing.Point(12, 196);
            this.disconnect_BT.Name = "disconnect_BT";
            this.disconnect_BT.Size = new System.Drawing.Size(130, 22);
            this.disconnect_BT.TabIndex = 30;
            this.disconnect_BT.Text = "Disconnect BT Device";
            this.disconnect_BT.UseVisualStyleBackColor = true;
            this.disconnect_BT.Click += new System.EventHandler(this.disconnect_BT_button_Click);
            // 
            // update_schema_button
            // 
            this.update_schema_button.BackColor = System.Drawing.Color.Yellow;
            this.update_schema_button.FlatAppearance.BorderColor = System.Drawing.SystemColors.AppWorkspace;
            this.update_schema_button.FlatAppearance.BorderSize = 2;
            this.update_schema_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.update_schema_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.update_schema_button.Location = new System.Drawing.Point(171, 272);
            this.update_schema_button.Name = "update_schema_button";
            this.update_schema_button.Size = new System.Drawing.Size(73, 50);
            this.update_schema_button.TabIndex = 25;
            this.update_schema_button.Text = "Update Schema";
            this.update_schema_button.UseVisualStyleBackColor = false;
            this.update_schema_button.Click += new System.EventHandler(this.update_schema_button_Click);
            // 
            // start_button
            // 
            this.start_button.BackColor = System.Drawing.Color.Lime;
            this.start_button.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.start_button.FlatAppearance.BorderSize = 2;
            this.start_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.start_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_button.Location = new System.Drawing.Point(15, 272);
            this.start_button.Name = "start_button";
            this.start_button.Size = new System.Drawing.Size(72, 50);
            this.start_button.TabIndex = 26;
            this.start_button.Text = "Start";
            this.start_button.UseVisualStyleBackColor = false;
            this.start_button.Click += new System.EventHandler(this.start_button_Click);
            // 
            // stop_polling_button
            // 
            this.stop_polling_button.BackColor = System.Drawing.Color.Red;
            this.stop_polling_button.FlatAppearance.BorderColor = System.Drawing.SystemColors.AppWorkspace;
            this.stop_polling_button.FlatAppearance.BorderSize = 2;
            this.stop_polling_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.stop_polling_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stop_polling_button.Location = new System.Drawing.Point(95, 272);
            this.stop_polling_button.Name = "stop_polling_button";
            this.stop_polling_button.Size = new System.Drawing.Size(70, 50);
            this.stop_polling_button.TabIndex = 31;
            this.stop_polling_button.Text = "Stop";
            this.stop_polling_button.UseVisualStyleBackColor = false;
            this.stop_polling_button.Click += new System.EventHandler(this.stop_polling_button_Click);
            // 
            // label7
            // 
            this.label7.AutoEllipsis = true;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "BT Device ID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(431, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Testing Buttons";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 336);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stop_polling_button);
            this.Controls.Add(this.disconnect_BT);
            this.Controls.Add(this.RemoveTest);
            this.Controls.Add(this.ServerTest);
            this.Controls.Add(this.CacheTest);
            this.Controls.Add(this.DataTest);
            this.Controls.Add(this.OBDIITest);
            this.Controls.Add(this.start_button);
            this.Controls.Add(this.update_schema_button);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.browse_button);
            this.Controls.Add(this.device_Status_Label);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.BT_ID);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.edit_IP_button);
            this.Controls.Add(this.edit_port_button);
            this.Controls.Add(this.server_IP);
            this.Controls.Add(this.port_number);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.close_button);
            this.Controls.Add(this.help_button);
            this.Name = "Form1";
            this.Text = "vConnect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.Button help_button;
        private System.Windows.Forms.Button close_button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label port_number;
        private System.Windows.Forms.Label server_IP;
        private System.Windows.Forms.Button edit_port_button;
        private System.Windows.Forms.Button edit_IP_button;
        private System.Windows.Forms.Label BT_ID;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label device_Status_Label;
        private System.Windows.Forms.Button browse_button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button OBDIITest;
        private System.Windows.Forms.Button DataTest;
        private System.Windows.Forms.Button CacheTest;
        private System.Windows.Forms.Button ServerTest;
        private System.Windows.Forms.Button RemoveTest;
        private System.Windows.Forms.Button disconnect_BT;
        private System.Windows.Forms.Button update_schema_button;
        private System.Windows.Forms.Button start_button;
        private System.Windows.Forms.Button stop_polling_button;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;

    }
}

