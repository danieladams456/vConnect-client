﻿namespace vConnect
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
            this.server_label = new System.Windows.Forms.Label();
            this.server_port = new System.Windows.Forms.Label();
            this.port_number = new System.Windows.Forms.Label();
            this.server_IP = new System.Windows.Forms.Label();
            this.edit_port_button = new System.Windows.Forms.Button();
            this.edit_IP_button = new System.Windows.Forms.Button();
            this.BT_ID = new System.Windows.Forms.Label();
            this.DeviceStatue_label = new System.Windows.Forms.Label();
            this.device_Status_Label = new System.Windows.Forms.Label();
            this.browse_button = new System.Windows.Forms.Button();
            this.disconnect_BT = new System.Windows.Forms.Button();
            this.start_button = new System.Windows.Forms.Button();
            this.stop_polling_button = new System.Windows.Forms.Button();
            this.BTID_label = new System.Windows.Forms.Label();
            this.update_schema_button = new System.Windows.Forms.Button();
            this.view_error_log_button = new System.Windows.Forms.Button();
            this.title_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "vConnect";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
            // 
            // help_button
            // 
            this.help_button.Location = new System.Drawing.Point(548, 211);
            this.help_button.Name = "help_button";
            this.help_button.Size = new System.Drawing.Size(92, 23);
            this.help_button.TabIndex = 0;
            this.help_button.Text = "Help";
            this.help_button.UseVisualStyleBackColor = true;
            this.help_button.Click += new System.EventHandler(this.help_button_Click);
            // 
            // close_button
            // 
            this.close_button.Location = new System.Drawing.Point(450, 211);
            this.close_button.Name = "close_button";
            this.close_button.Size = new System.Drawing.Size(92, 23);
            this.close_button.TabIndex = 3;
            this.close_button.Text = "Close";
            this.close_button.UseVisualStyleBackColor = true;
            this.close_button.Click += new System.EventHandler(this.close_button_Click);
            // 
            // server_label
            // 
            this.server_label.AutoSize = true;
            this.server_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.server_label.Location = new System.Drawing.Point(275, 9);
            this.server_label.Name = "server_label";
            this.server_label.Size = new System.Drawing.Size(126, 20);
            this.server_label.TabIndex = 7;
            this.server_label.Text = "Server Address: ";
            // 
            // server_port
            // 
            this.server_port.AutoSize = true;
            this.server_port.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.server_port.Location = new System.Drawing.Point(275, 29);
            this.server_port.Name = "server_port";
            this.server_port.Size = new System.Drawing.Size(46, 20);
            this.server_port.TabIndex = 9;
            this.server_port.Text = "Port: ";
            // 
            // port_number
            // 
            this.port_number.AutoSize = true;
            this.port_number.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.port_number.Location = new System.Drawing.Point(446, 29);
            this.port_number.Name = "port_number";
            this.port_number.Size = new System.Drawing.Size(45, 20);
            this.port_number.TabIndex = 12;
            this.port_number.Text = "5000";
            // 
            // server_IP
            // 
            this.server_IP.AutoEllipsis = true;
            this.server_IP.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.server_IP.Location = new System.Drawing.Point(446, 9);
            this.server_IP.Name = "server_IP";
            this.server_IP.Size = new System.Drawing.Size(190, 20);
            this.server_IP.TabIndex = 13;
            this.server_IP.Text = "www.google.com";
            // 
            // edit_port_button
            // 
            this.edit_port_button.Location = new System.Drawing.Point(450, 52);
            this.edit_port_button.Name = "edit_port_button";
            this.edit_port_button.Size = new System.Drawing.Size(190, 23);
            this.edit_port_button.TabIndex = 14;
            this.edit_port_button.Text = "Configure Port";
            this.edit_port_button.UseVisualStyleBackColor = true;
            this.edit_port_button.Click += new System.EventHandler(this.edit_port_button_Click);
            // 
            // edit_IP_button
            // 
            this.edit_IP_button.Location = new System.Drawing.Point(254, 52);
            this.edit_IP_button.Name = "edit_IP_button";
            this.edit_IP_button.Size = new System.Drawing.Size(190, 23);
            this.edit_IP_button.TabIndex = 15;
            this.edit_IP_button.Text = "Configure Server Address";
            this.edit_IP_button.UseVisualStyleBackColor = true;
            this.edit_IP_button.Click += new System.EventHandler(this.edit_IP_button_Click);
            // 
            // BT_ID
            // 
            this.BT_ID.AutoSize = true;
            this.BT_ID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_ID.Location = new System.Drawing.Point(446, 96);
            this.BT_ID.Name = "BT_ID";
            this.BT_ID.Size = new System.Drawing.Size(70, 20);
            this.BT_ID.TabIndex = 19;
            this.BT_ID.Text = "BT-1459";
            // 
            // DeviceStatue_label
            // 
            this.DeviceStatue_label.AutoSize = true;
            this.DeviceStatue_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeviceStatue_label.Location = new System.Drawing.Point(275, 116);
            this.DeviceStatue_label.Name = "DeviceStatue_label";
            this.DeviceStatue_label.Size = new System.Drawing.Size(116, 20);
            this.DeviceStatue_label.TabIndex = 20;
            this.DeviceStatue_label.Text = "Device Status: ";
            // 
            // device_Status_Label
            // 
            this.device_Status_Label.AutoSize = true;
            this.device_Status_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.device_Status_Label.Location = new System.Drawing.Point(446, 116);
            this.device_Status_Label.Name = "device_Status_Label";
            this.device_Status_Label.Size = new System.Drawing.Size(87, 20);
            this.device_Status_Label.TabIndex = 21;
            this.device_Status_Label.Text = "Connected";
            // 
            // browse_button
            // 
            this.browse_button.Location = new System.Drawing.Point(254, 139);
            this.browse_button.Name = "browse_button";
            this.browse_button.Size = new System.Drawing.Size(190, 23);
            this.browse_button.TabIndex = 23;
            this.browse_button.Text = "Select OBDII Device";
            this.browse_button.UseVisualStyleBackColor = true;
            this.browse_button.Click += new System.EventHandler(this.browse_button_Click);
            // 
            // disconnect_BT
            // 
            this.disconnect_BT.Location = new System.Drawing.Point(450, 139);
            this.disconnect_BT.Name = "disconnect_BT";
            this.disconnect_BT.Size = new System.Drawing.Size(190, 23);
            this.disconnect_BT.TabIndex = 30;
            this.disconnect_BT.Text = "Disconnect BT Device";
            this.disconnect_BT.UseVisualStyleBackColor = true;
            this.disconnect_BT.Click += new System.EventHandler(this.disconnect_BT_button_Click);
            // 
            // start_button
            // 
            this.start_button.BackColor = System.Drawing.Color.Lime;
            this.start_button.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.start_button.FlatAppearance.BorderSize = 2;
            this.start_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.start_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_button.Location = new System.Drawing.Point(12, 156);
            this.start_button.Name = "start_button";
            this.start_button.Size = new System.Drawing.Size(111, 78);
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
            this.stop_polling_button.Location = new System.Drawing.Point(129, 156);
            this.stop_polling_button.Name = "stop_polling_button";
            this.stop_polling_button.Size = new System.Drawing.Size(111, 78);
            this.stop_polling_button.TabIndex = 31;
            this.stop_polling_button.Text = "Stop";
            this.stop_polling_button.UseVisualStyleBackColor = false;
            this.stop_polling_button.Click += new System.EventHandler(this.stop_polling_button_Click);
            // 
            // BTID_label
            // 
            this.BTID_label.AutoEllipsis = true;
            this.BTID_label.AutoSize = true;
            this.BTID_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTID_label.Location = new System.Drawing.Point(275, 96);
            this.BTID_label.Name = "BTID_label";
            this.BTID_label.Size = new System.Drawing.Size(159, 20);
            this.BTID_label.TabIndex = 16;
            this.BTID_label.Text = "Bluetooth Device ID: ";
            // 
            // update_schema_button
            // 
            this.update_schema_button.Location = new System.Drawing.Point(254, 211);
            this.update_schema_button.Name = "update_schema_button";
            this.update_schema_button.Size = new System.Drawing.Size(92, 23);
            this.update_schema_button.TabIndex = 32;
            this.update_schema_button.Text = "Update Schema";
            this.update_schema_button.UseVisualStyleBackColor = true;
            // 
            // view_error_log_button
            // 
            this.view_error_log_button.Location = new System.Drawing.Point(352, 211);
            this.view_error_log_button.Name = "view_error_log_button";
            this.view_error_log_button.Size = new System.Drawing.Size(92, 23);
            this.view_error_log_button.TabIndex = 33;
            this.view_error_log_button.Text = "View Error Log";
            this.view_error_log_button.UseVisualStyleBackColor = true;
            this.view_error_log_button.Click += new System.EventHandler(this.view_error_log_button_Click);
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 38F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_label.ForeColor = System.Drawing.Color.Blue;
            this.title_label.Location = new System.Drawing.Point(12, 52);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(240, 59);
            this.title_label.TabIndex = 34;
            this.title_label.Text = "vConnect";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 244);
            this.Controls.Add(this.title_label);
            this.Controls.Add(this.view_error_log_button);
            this.Controls.Add(this.update_schema_button);
            this.Controls.Add(this.stop_polling_button);
            this.Controls.Add(this.disconnect_BT);
            this.Controls.Add(this.start_button);
            this.Controls.Add(this.browse_button);
            this.Controls.Add(this.device_Status_Label);
            this.Controls.Add(this.DeviceStatue_label);
            this.Controls.Add(this.BT_ID);
            this.Controls.Add(this.BTID_label);
            this.Controls.Add(this.edit_IP_button);
            this.Controls.Add(this.edit_port_button);
            this.Controls.Add(this.server_IP);
            this.Controls.Add(this.port_number);
            this.Controls.Add(this.server_port);
            this.Controls.Add(this.server_label);
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
        private System.Windows.Forms.Label server_label;
        private System.Windows.Forms.Label server_port;
        private System.Windows.Forms.Label port_number;
        private System.Windows.Forms.Label server_IP;
        private System.Windows.Forms.Button edit_port_button;
        private System.Windows.Forms.Button edit_IP_button;
        private System.Windows.Forms.Label BT_ID;
        private System.Windows.Forms.Label DeviceStatue_label;
        private System.Windows.Forms.Label device_Status_Label;
        private System.Windows.Forms.Button browse_button;
        private System.Windows.Forms.Button disconnect_BT;
        private System.Windows.Forms.Button start_button;
        private System.Windows.Forms.Button stop_polling_button;
        private System.Windows.Forms.Label BTID_label;
        private System.Windows.Forms.Button update_schema_button;
        private System.Windows.Forms.Button view_error_log_button;
        private System.Windows.Forms.Label title_label;

    }
}

