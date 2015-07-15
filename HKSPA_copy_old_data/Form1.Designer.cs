namespace HKSPA_copy_old_data
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
            this.txt_result = new System.Windows.Forms.TextBox();
            this.lbl_limitFrom = new System.Windows.Forms.Label();
            this.lbl_limitTo = new System.Windows.Forms.Label();
            this.btn_mysql_record = new System.Windows.Forms.Button();
            this.btn_copy_photo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_result
            // 
            this.txt_result.Location = new System.Drawing.Point(13, 25);
            this.txt_result.Multiline = true;
            this.txt_result.Name = "txt_result";
            this.txt_result.ReadOnly = true;
            this.txt_result.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_result.Size = new System.Drawing.Size(459, 425);
            this.txt_result.TabIndex = 0;
            // 
            // lbl_limitFrom
            // 
            this.lbl_limitFrom.AutoSize = true;
            this.lbl_limitFrom.Location = new System.Drawing.Point(394, 9);
            this.lbl_limitFrom.Name = "lbl_limitFrom";
            this.lbl_limitFrom.Size = new System.Drawing.Size(13, 13);
            this.lbl_limitFrom.TabIndex = 1;
            this.lbl_limitFrom.Text = "0";
            this.lbl_limitFrom.Visible = false;
            // 
            // lbl_limitTo
            // 
            this.lbl_limitTo.AutoSize = true;
            this.lbl_limitTo.Location = new System.Drawing.Point(435, 9);
            this.lbl_limitTo.Name = "lbl_limitTo";
            this.lbl_limitTo.Size = new System.Drawing.Size(37, 13);
            this.lbl_limitTo.TabIndex = 2;
            this.lbl_limitTo.Text = "10000";
            this.lbl_limitTo.Visible = false;
            // 
            // btn_mysql_record
            // 
            this.btn_mysql_record.Location = new System.Drawing.Point(94, -1);
            this.btn_mysql_record.Name = "btn_mysql_record";
            this.btn_mysql_record.Size = new System.Drawing.Size(107, 23);
            this.btn_mysql_record.TabIndex = 3;
            this.btn_mysql_record.Text = "Copy Mysql Record";
            this.btn_mysql_record.UseVisualStyleBackColor = true;
            this.btn_mysql_record.Visible = false;
            this.btn_mysql_record.Click += new System.EventHandler(this.btn_mysql_record_Click);
            // 
            // btn_copy_photo
            // 
            this.btn_copy_photo.Location = new System.Drawing.Point(13, -1);
            this.btn_copy_photo.Name = "btn_copy_photo";
            this.btn_copy_photo.Size = new System.Drawing.Size(75, 23);
            this.btn_copy_photo.TabIndex = 4;
            this.btn_copy_photo.Text = "Copy Photo";
            this.btn_copy_photo.UseVisualStyleBackColor = true;
            this.btn_copy_photo.Click += new System.EventHandler(this.btn_copy_photo_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 462);
            this.Controls.Add(this.btn_copy_photo);
            this.Controls.Add(this.btn_mysql_record);
            this.Controls.Add(this.lbl_limitTo);
            this.Controls.Add(this.lbl_limitFrom);
            this.Controls.Add(this.txt_result);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_result;
        private System.Windows.Forms.Label lbl_limitFrom;
        private System.Windows.Forms.Label lbl_limitTo;
        private System.Windows.Forms.Button btn_mysql_record;
        private System.Windows.Forms.Button btn_copy_photo;
    }
}

