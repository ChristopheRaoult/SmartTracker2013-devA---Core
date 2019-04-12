namespace SDK_SC_Fingerprint
{
    partial class EnrollmentProcess
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
            this.CloseButton = new System.Windows.Forms.Button();
            this.EnrollmentControl = new DPFP.Gui.Enrollment.EnrollmentControl();
            this.groupEvents = new System.Windows.Forms.GroupBox();
            this.ListEvents = new System.Windows.Forms.ListBox();
            this.groupEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(401, 466);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(73, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // EnrollmentControl
            // 
            this.EnrollmentControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.EnrollmentControl.EnrolledFingerMask = 0;
            this.EnrollmentControl.Location = new System.Drawing.Point(-3, 1);
            this.EnrollmentControl.MaxEnrollFingerCount = 10;
            this.EnrollmentControl.Name = "EnrollmentControl";
            this.EnrollmentControl.ReaderSerialNumber = "00000000-0000-0000-0000-000000000000";
            this.EnrollmentControl.Size = new System.Drawing.Size(492, 314);
            this.EnrollmentControl.TabIndex = 1;
            this.EnrollmentControl.OnDelete += new DPFP.Gui.Enrollment.EnrollmentControl._OnDelete(this.EnrollmentControl_OnDelete);
            this.EnrollmentControl.OnSampleQuality += new DPFP.Gui.Enrollment.EnrollmentControl._OnSampleQuality(this.EnrollmentControl_OnSampleQuality);
            this.EnrollmentControl.OnComplete += new DPFP.Gui.Enrollment.EnrollmentControl._OnComplete(this.EnrollmentControl_OnComplete);
            this.EnrollmentControl.OnFingerRemove += new DPFP.Gui.Enrollment.EnrollmentControl._OnFingerRemove(this.EnrollmentControl_OnFingerRemove);
            this.EnrollmentControl.OnReaderConnect += new DPFP.Gui.Enrollment.EnrollmentControl._OnReaderConnect(this.EnrollmentControl_OnReaderConnect);
            this.EnrollmentControl.OnStartEnroll += new DPFP.Gui.Enrollment.EnrollmentControl._OnStartEnroll(this.EnrollmentControl_OnStartEnroll);
            this.EnrollmentControl.OnEnroll += new DPFP.Gui.Enrollment.EnrollmentControl._OnEnroll(this.EnrollmentControl_OnEnroll);
            this.EnrollmentControl.OnFingerTouch += new DPFP.Gui.Enrollment.EnrollmentControl._OnFingerTouch(this.EnrollmentControl_OnFingerTouch);
            this.EnrollmentControl.OnReaderDisconnect += new DPFP.Gui.Enrollment.EnrollmentControl._OnReaderDisconnect(this.EnrollmentControl_OnReaderDisconnect);
            this.EnrollmentControl.OnCancelEnroll += new DPFP.Gui.Enrollment.EnrollmentControl._OnCancelEnroll(this.EnrollmentControl_OnCancelEnroll);
            // 
            // groupEvents
            // 
            this.groupEvents.Controls.Add(this.ListEvents);
            this.groupEvents.Location = new System.Drawing.Point(11, 321);
            this.groupEvents.Name = "groupEvents";
            this.groupEvents.Size = new System.Drawing.Size(462, 135);
            this.groupEvents.TabIndex = 2;
            this.groupEvents.TabStop = false;
            this.groupEvents.Text = "Events";
            // 
            // ListEvents
            // 
            this.ListEvents.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ListEvents.FormattingEnabled = true;
            this.ListEvents.Location = new System.Drawing.Point(8, 25);
            this.ListEvents.Name = "ListEvents";
            this.ListEvents.Size = new System.Drawing.Size(445, 95);
            this.ListEvents.TabIndex = 0;
            // 
            // EnrollmentForm
            // 
            this.AcceptButton = this.CloseButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(487, 499);
            this.Controls.Add(this.groupEvents);
            this.Controls.Add(this.EnrollmentControl);
            this.Controls.Add(this.CloseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EnrollmentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fingerprint Enrollment";
            this.Load += new System.EventHandler(this.EnrollmentForm_Load);
            this.groupEvents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private DPFP.Gui.Enrollment.EnrollmentControl EnrollmentControl;
        private System.Windows.Forms.GroupBox groupEvents;
        private System.Windows.Forms.ListBox ListEvents;
    }
}