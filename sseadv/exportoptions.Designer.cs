namespace sseadv
{
    partial class exportoptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(exportoptions));
            this.originalChk = new System.Windows.Forms.CheckBox();
            this.enableBordersChk = new System.Windows.Forms.CheckBox();
            this.consistentSizeChk = new System.Windows.Forms.CheckBox();
            this.spriteNameChk = new System.Windows.Forms.CheckBox();
            this.spriteSheetChk = new System.Windows.Forms.CheckBox();
            this.autoDetectSizeChk = new System.Windows.Forms.CheckBox();
            this.spriteSheetColumnsTxt = new System.Windows.Forms.TextBox();
            this.spriteSheetRowsTxt = new System.Windows.Forms.TextBox();
            this.exportSeparateChk = new System.Windows.Forms.CheckBox();
            this.exportAmtTxt = new System.Windows.Forms.TextBox();
            this.animFullChk = new System.Windows.Forms.CheckBox();
            this.exportByAnimChk = new System.Windows.Forms.CheckBox();
            this.exportSimpleJsonChk = new System.Windows.Forms.CheckBox();
            this.exportBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // originalChk
            // 
            this.originalChk.AutoSize = true;
            this.originalChk.Checked = false;
            this.originalChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.originalChk.Location = new System.Drawing.Point(15, 19);
            this.originalChk.Name = "originalChk";
            this.originalChk.Size = new System.Drawing.Size(183, 18);
            this.originalChk.TabIndex = 1;
            this.originalChk.Text = "export individual sprites";
            this.originalChk.UseVisualStyleBackColor = true;
            this.originalChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            // 
            // enableBordersChk
            // 
            this.enableBordersChk.AutoSize = true;
            this.enableBordersChk.Checked = true;
            this.enableBordersChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableBordersChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.enableBordersChk.Location = new System.Drawing.Point(53, 65);
            this.enableBordersChk.Name = "enableBordersChk";
            this.enableBordersChk.Size = new System.Drawing.Size(120, 18);
            this.enableBordersChk.TabIndex = 1;
            this.enableBordersChk.Text = "enable red borders";
            this.enableBordersChk.UseVisualStyleBackColor = true;
            this.enableBordersChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            this.enableBordersChk.Enabled = false;
            // 
            // consistentSizeChk
            // 
            this.consistentSizeChk.AutoSize = true;
            this.consistentSizeChk.Checked = true;
            this.consistentSizeChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.consistentSizeChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.consistentSizeChk.Location = new System.Drawing.Point(34, 42);
            this.consistentSizeChk.Name = "consistentSizeChk";
            this.consistentSizeChk.Size = new System.Drawing.Size(120, 18);
            this.consistentSizeChk.TabIndex = 1;
            this.consistentSizeChk.Text = "keep sprite size consistent";
            this.consistentSizeChk.UseVisualStyleBackColor = true;
            this.consistentSizeChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            this.consistentSizeChk.Enabled = false;
            // 
            // spriteNameChk
            // 
            this.spriteNameChk.AutoSize = true;
            this.spriteNameChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.spriteNameChk.Location = new System.Drawing.Point(34, 88);
            this.spriteNameChk.Name = "spriteNameChk";
            this.spriteNameChk.Size = new System.Drawing.Size(120, 18);
            this.spriteNameChk.TabIndex = 1;
            this.spriteNameChk.Text = "use sprite name instead of index";
            this.spriteNameChk.UseVisualStyleBackColor = true;
            this.spriteNameChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            this.spriteNameChk.Enabled = false;
            // 
            // spriteSheetChk
            // 
            this.spriteSheetChk.AutoSize = true;
            this.spriteSheetChk.Checked = true;
            this.spriteSheetChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.spriteSheetChk.Location = new System.Drawing.Point(15, 111);
            this.spriteSheetChk.Name = "spriteSheetChk";
            this.spriteSheetChk.Size = new System.Drawing.Size(183, 18);
            this.spriteSheetChk.TabIndex = 1;
            this.spriteSheetChk.Text = "export to custom SpriteSheet";
            this.spriteSheetChk.UseVisualStyleBackColor = true;
            this.spriteSheetChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            // 
            // autoDetectSizeChk
            // 
            this.autoDetectSizeChk.AutoSize = true;
            this.autoDetectSizeChk.Checked = true;
            this.autoDetectSizeChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoDetectSizeChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.autoDetectSizeChk.Location = new System.Drawing.Point(34, 134);
            this.autoDetectSizeChk.Name = "autoDetectSizeChk";
            this.autoDetectSizeChk.Size = new System.Drawing.Size(120, 18);
            this.autoDetectSizeChk.TabIndex = 1;
            this.autoDetectSizeChk.Text = "auto detect dimentions";
            this.autoDetectSizeChk.UseVisualStyleBackColor = true;
            this.autoDetectSizeChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            this.autoDetectSizeChk.Enabled = true;
            // 
            // spriteSheetColumnsTxt
            // 
            this.spriteSheetColumnsTxt.AutoSize = true;
            this.spriteSheetColumnsTxt.Location = new System.Drawing.Point(53, 157);
            this.spriteSheetColumnsTxt.Name = "spriteSheetColumnsTxt";
            this.spriteSheetColumnsTxt.Size = new System.Drawing.Size(20, 18);
            this.spriteSheetColumnsTxt.TabIndex = 1;
            this.spriteSheetColumnsTxt.Text = "-1";
            this.spriteSheetColumnsTxt.Enabled = false;
            // 
            // spriteSheetRowsTxt
            // 
            this.spriteSheetRowsTxt.AutoSize = true;
            this.spriteSheetRowsTxt.Location = new System.Drawing.Point(83, 157);
            this.spriteSheetRowsTxt.Name = "spriteSheetRowsTxt";
            this.spriteSheetRowsTxt.Size = new System.Drawing.Size(20, 18);
            this.spriteSheetRowsTxt.TabIndex = 1;
            this.spriteSheetRowsTxt.Text = "-1";
            this.spriteSheetRowsTxt.Enabled = false;
            // 
            // exportSeparateChk
            // 
            this.exportSeparateChk.AutoSize = true;
            this.exportSeparateChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.exportSeparateChk.Location = new System.Drawing.Point(34, 180);
            this.exportSeparateChk.Name = "exportSeparateChk";
            this.exportSeparateChk.Size = new System.Drawing.Size(120, 18);
            this.exportSeparateChk.TabIndex = 1;
            this.exportSeparateChk.Text = "split multiple sheets by sprites";
            this.exportSeparateChk.UseVisualStyleBackColor = true;
            this.exportSeparateChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            this.exportSeparateChk.Enabled = true;
            // 
            // exportAmtTxt
            // 
            this.exportAmtTxt.AutoSize = true;
            this.exportAmtTxt.Location = new System.Drawing.Point(53, 203);
            this.exportAmtTxt.Name = "exportAmtTxt";
            this.exportAmtTxt.Size = new System.Drawing.Size(20, 18);
            this.exportAmtTxt.TabIndex = 1;
            this.exportAmtTxt.Text = "-1";
            this.exportAmtTxt.Enabled = false;
            // 
            // animFullChk
            // 
            this.animFullChk.AutoSize = true;
            this.animFullChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.animFullChk.Location = new System.Drawing.Point(34, 226);
            this.animFullChk.Name = "animFullChk";
            this.animFullChk.Size = new System.Drawing.Size(120, 18);
            this.animFullChk.TabIndex = 1;
            this.animFullChk.Text = "export all animations";
            this.animFullChk.UseVisualStyleBackColor = true;
            this.animFullChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            this.animFullChk.Enabled = true;
            // 
            // exportByAnimChk
            // 
            this.exportByAnimChk.AutoSize = true;
            this.exportByAnimChk.Checked = true;
            this.exportByAnimChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.exportByAnimChk.Location = new System.Drawing.Point(53, 249);
            this.exportByAnimChk.Name = "exportByAnimChk";
            this.exportByAnimChk.Size = new System.Drawing.Size(120, 18);
            this.exportByAnimChk.TabIndex = 1;
            this.exportByAnimChk.Text = "split by animations";
            this.exportByAnimChk.UseVisualStyleBackColor = true;
            this.exportByAnimChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            this.exportByAnimChk.Enabled = false;
            // 
            // exportSimpleJsonChk
            // 
            this.exportSimpleJsonChk.AutoSize = true;
            this.exportSimpleJsonChk.Checked = true;
            this.exportSimpleJsonChk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.exportSimpleJsonChk.Location = new System.Drawing.Point(34, 272);
            this.exportSimpleJsonChk.Name = "exportSimpleJsonChk";
            this.exportSimpleJsonChk.Size = new System.Drawing.Size(120, 18);
            this.exportSimpleJsonChk.TabIndex = 1;
            this.exportSimpleJsonChk.Text = "export json (CKSAA)";
            this.exportSimpleJsonChk.UseVisualStyleBackColor = true;
            this.exportSimpleJsonChk.CheckStateChanged += new System.EventHandler(this.StateChanged);
            this.exportSimpleJsonChk.Enabled = true;
            // 
            // exportBtn
            // 
            this.exportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exportBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.exportBtn.Location = new System.Drawing.Point(15, 290);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(245, 23);
            this.exportBtn.TabIndex = 2;
            this.exportBtn.Text = "export";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.exportBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelBtn.Location = new System.Drawing.Point(15, 319);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(245, 23);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // exportoptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 346);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.originalChk);
            this.Controls.Add(this.spriteNameChk);
            this.Controls.Add(this.consistentSizeChk);
            this.Controls.Add(this.enableBordersChk);
            this.Controls.Add(this.spriteSheetChk);
            this.Controls.Add(this.autoDetectSizeChk);
            this.Controls.Add(this.spriteSheetColumnsTxt);
            this.Controls.Add(this.spriteSheetRowsTxt);
            this.Controls.Add(this.exportSeparateChk);
            this.Controls.Add(this.exportAmtTxt);
            this.Controls.Add(this.animFullChk);
            this.Controls.Add(this.exportByAnimChk);
            this.Controls.Add(this.exportSimpleJsonChk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "exportoptions";
            this.Text = "export-options";
            this.Load += new System.EventHandler(this.exportoptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox originalChk;
        private System.Windows.Forms.CheckBox enableBordersChk;
        private System.Windows.Forms.CheckBox consistentSizeChk;
        private System.Windows.Forms.CheckBox spriteNameChk;
        private System.Windows.Forms.CheckBox spriteSheetChk;
        private System.Windows.Forms.CheckBox autoDetectSizeChk;
        private System.Windows.Forms.TextBox spriteSheetColumnsTxt;
        private System.Windows.Forms.TextBox spriteSheetRowsTxt;
        private System.Windows.Forms.CheckBox exportSeparateChk;
        private System.Windows.Forms.TextBox exportAmtTxt;
        private System.Windows.Forms.CheckBox animFullChk;
        private System.Windows.Forms.CheckBox exportByAnimChk;
        private System.Windows.Forms.CheckBox exportSimpleJsonChk;
        private System.Windows.Forms.Button exportBtn;
        private System.Windows.Forms.Button cancelBtn;
    }
}