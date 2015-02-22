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
            this.cancel_button = new System.Windows.Forms.Button();
            this.apply_button = new System.Windows.Forms.Button();
            this.ok_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.port_number = new System.Windows.Forms.Label();
            this.server_IP = new System.Windows.Forms.Label();
            this.edit_port = new System.Windows.Forms.Button();
            this.edit_IP = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.server_status_label = new System.Windows.Forms.Label();
            this.server_test = new System.Windows.Forms.Button();
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
            this.Disconnect_BT = new System.Windows.Forms.Button();
            this.getSchema = new System.Windows.Forms.Button();
            this.start = new System.Windows.Forms.Button();
            this.Stop_Polling = new System.Windows.Forms.Button();
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
            this.help_button.Location = new System.Drawing.Point(456, 299);
            this.help_button.Name = "help_button";
            this.help_button.Size = new System.Drawing.Size(75, 23);
            this.help_button.TabIndex = 0;
            this.help_button.Text = "Help";
            this.help_button.UseVisualStyleBackColor = true;
            this.help_button.Click += new System.EventHandler(this.help_button_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(375, 299);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(75, 23);
            this.cancel_button.TabIndex = 1;
            this.cancel_button.Text = "Cancel";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // apply_button
            // 
            this.apply_button.Location = new System.Drawing.Point(294, 299);
            this.apply_button.Name = "apply_button";
            this.apply_button.Size = new System.Drawing.Size(75, 23);
            this.apply_button.TabIndex = 2;
            this.apply_button.Text = "Apply";
            this.apply_button.UseVisualStyleBackColor = true;
            this.apply_button.Click += new System.EventHandler(this.apply_button_Click);
            // 
            // ok_button
            // 
            this.ok_button.Location = new System.Drawing.Point(213, 299);
            this.ok_button.Name = "ok_button";
            this.ok_button.Size = new System.Drawing.Size(75, 23);
            this.ok_button.TabIndex = 3;
            this.ok_button.Text = "Ok";
            this.ok_button.UseVisualStyleBackColor = true;
            this.ok_button.Click += new System.EventHandler(this.ok_button_Click);
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
            this.label3.Location = new System.Drawing.Point(375, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Server Port Number:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(213, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Server Status:";
            // 
            // port_number
            // 
            this.port_number.AutoSize = true;
            this.port_number.Location = new System.Drawing.Point(375, 45);
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
            // edit_port
            // 
            this.edit_port.Location = new System.Drawing.Point(378, 61);
            this.edit_port.Name = "edit_port";
            this.edit_port.Size = new System.Drawing.Size(75, 23);
            this.edit_port.TabIndex = 14;
            this.edit_port.Text = "Edit ";
            this.edit_port.UseVisualStyleBackColor = true;
            this.edit_port.Click += new System.EventHandler(this.edit_port_Click);
            // 
            // edit_IP
            // 
            this.edit_IP.Location = new System.Drawing.Point(213, 61);
            this.edit_IP.Name = "edit_IP";
            this.edit_IP.Size = new System.Drawing.Size(75, 23);
            this.edit_IP.TabIndex = 15;
            this.edit_IP.Text = "Edit";
            this.edit_IP.UseVisualStyleBackColor = true;
            this.edit_IP.Click += new System.EventHandler(this.edit_IP_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "BT Device ID:";
            // 
            // server_status_label
            // 
            this.server_status_label.AutoSize = true;
            this.server_status_label.Location = new System.Drawing.Point(213, 123);
            this.server_status_label.Name = "server_status_label";
            this.server_status_label.Size = new System.Drawing.Size(130, 13);
            this.server_status_label.TabIndex = 17;
            this.server_status_label.Text = "Connected/Disconnected";
            // 
            // server_test
            // 
            this.server_test.Location = new System.Drawing.Point(213, 139);
            this.server_test.Name = "server_test";
            this.server_test.Size = new System.Drawing.Size(75, 23);
            this.server_test.TabIndex = 18;
            this.server_test.Text = "Test";
            this.server_test.UseVisualStyleBackColor = true;
            this.server_test.Click += new System.EventHandler(this.server_test_Click);
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
            this.label10.Location = new System.Drawing.Point(15, 110);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Device Status:";
            // 
            // device_Status_Label
            // 
            this.device_Status_Label.AutoSize = true;
            this.device_Status_Label.Location = new System.Drawing.Point(12, 123);
            this.device_Status_Label.Name = "device_Status_Label";
            this.device_Status_Label.Size = new System.Drawing.Size(130, 13);
            this.device_Status_Label.TabIndex = 21;
            this.device_Status_Label.Text = "Connected/Disconnected";
            // 
            // browse_button
            // 
            this.browse_button.Location = new System.Drawing.Point(12, 165);
            this.browse_button.Name = "browse_button";
            this.browse_button.Size = new System.Drawing.Size(75, 23);
            this.browse_button.TabIndex = 23;
            this.browse_button.Text = "Browse";
            this.browse_button.UseVisualStyleBackColor = true;
            this.browse_button.Click += new System.EventHandler(this.browse_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Connect to ODBII Device:";
            // 
            // OBDIITest
            // 
            this.OBDIITest.Location = new System.Drawing.Point(456, 248);
            this.OBDIITest.Name = "OBDIITest";
            this.OBDIITest.Size = new System.Drawing.Size(75, 45);
            this.OBDIITest.TabIndex = 25;
            this.OBDIITest.Text = "Test OBDII codes";
            this.OBDIITest.UseVisualStyleBackColor = true;
            this.OBDIITest.Click += new System.EventHandler(this.OBDIITest_Click);
            // 
            // DataTest
            // 
            this.DataTest.Location = new System.Drawing.Point(378, 248);
            this.DataTest.Name = "DataTest";
            this.DataTest.Size = new System.Drawing.Size(75, 45);
            this.DataTest.TabIndex = 26;
            this.DataTest.Text = "Test Vehicle Data Elements";
            this.DataTest.UseVisualStyleBackColor = true;
            this.DataTest.Click += new System.EventHandler(this.DataTest_Click);
            // 
            // CacheTest
            // 
            this.CacheTest.Location = new System.Drawing.Point(294, 248);
            this.CacheTest.Name = "CacheTest";
            this.CacheTest.Size = new System.Drawing.Size(75, 45);
            this.CacheTest.TabIndex = 27;
            this.CacheTest.Text = "Test Data Cache";
            this.CacheTest.UseVisualStyleBackColor = true;
            this.CacheTest.Click += new System.EventHandler(this.button3_Click);
            // 
            // ServerTest
            // 
            this.ServerTest.Location = new System.Drawing.Point(213, 248);
            this.ServerTest.Name = "ServerTest";
            this.ServerTest.Size = new System.Drawing.Size(75, 45);
            this.ServerTest.TabIndex = 28;
            this.ServerTest.Text = "Test Data to Server";
            this.ServerTest.UseVisualStyleBackColor = true;
            this.ServerTest.Click += new System.EventHandler(this.ServerTest_Click);
            // 
            // RemoveTest
            // 
            this.RemoveTest.Location = new System.Drawing.Point(110, 248);
            this.RemoveTest.Name = "RemoveTest";
            this.RemoveTest.Size = new System.Drawing.Size(97, 45);
            this.RemoveTest.TabIndex = 29;
            this.RemoveTest.Text = "Test Removing data from cache";
            this.RemoveTest.UseVisualStyleBackColor = true;
            this.RemoveTest.Click += new System.EventHandler(this.RemoveTest_Click);
            // 
            // Disconnect_BT
            // 
            this.Disconnect_BT.Location = new System.Drawing.Point(12, 194);
            this.Disconnect_BT.Name = "Disconnect_BT";
            this.Disconnect_BT.Size = new System.Drawing.Size(75, 45);
            this.Disconnect_BT.TabIndex = 30;
            this.Disconnect_BT.Text = "Disconnect BT Device";
            this.Disconnect_BT.UseVisualStyleBackColor = true;
            this.Disconnect_BT.Click += new System.EventHandler(this.Disconnect_BT_Click);
            // 
            // getSchema
            // 
            this.getSchema.Location = new System.Drawing.Point(378, 139);
            this.getSchema.Name = "getSchema";
            this.getSchema.Size = new System.Drawing.Size(100, 23);
            this.getSchema.TabIndex = 25;
            this.getSchema.Text = "Update Schema";
            this.getSchema.UseVisualStyleBackColor = true;
            this.getSchema.Click += new System.EventHandler(this.getSchema_Click);
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(378, 164);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(75, 23);
            this.start.TabIndex = 26;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // Stop_Polling
            // 
            this.Stop_Polling.Location = new System.Drawing.Point(378, 194);
            this.Stop_Polling.Name = "Stop_Polling";
            this.Stop_Polling.Size = new System.Drawing.Size(75, 23);
            this.Stop_Polling.TabIndex = 31;
            this.Stop_Polling.Text = "Stop";
            this.Stop_Polling.UseVisualStyleBackColor = true;
            this.Stop_Polling.Click += new System.EventHandler(this.Stop_Polling_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 334);
            this.Controls.Add(this.Stop_Polling);
            this.Controls.Add(this.Disconnect_BT);
            this.Controls.Add(this.RemoveTest);
            this.Controls.Add(this.ServerTest);
            this.Controls.Add(this.CacheTest);
            this.Controls.Add(this.DataTest);
            this.Controls.Add(this.OBDIITest);
            this.Controls.Add(this.start);
            this.Controls.Add(this.getSchema);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.browse_button);
            this.Controls.Add(this.device_Status_Label);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.BT_ID);
            this.Controls.Add(this.server_test);
            this.Controls.Add(this.server_status_label);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.edit_IP);
            this.Controls.Add(this.edit_port);
            this.Controls.Add(this.server_IP);
            this.Controls.Add(this.port_number);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ok_button);
            this.Controls.Add(this.apply_button);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.help_button);
            this.Name = "Form1";
            this.Text = "vConnect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.Button help_button;
        private System.Windows.Forms.Button cancel_button;
        private System.Windows.Forms.Button apply_button;
        private System.Windows.Forms.Button ok_button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label port_number;
        private System.Windows.Forms.Label server_IP;
        private System.Windows.Forms.Button edit_port;
        private System.Windows.Forms.Button edit_IP;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label server_status_label;
        private System.Windows.Forms.Button server_test;
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
        private System.Windows.Forms.Button Disconnect_BT;
        private System.Windows.Forms.Button getSchema;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.Button Stop_Polling;

    }
}

