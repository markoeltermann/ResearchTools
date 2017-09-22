namespace SpectrumAnalyzer
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.LoadAsciiFileButton = new System.Windows.Forms.Button();
            this.DoFittingButton = new System.Windows.Forms.Button();
            this.RemoveFittedCurveButton = new System.Windows.Forms.Button();
            this.YAxisUpperBoundTextBox = new System.Windows.Forms.TextBox();
            this.YAxisLowerBoundTextBox = new System.Windows.Forms.TextBox();
            this.AnalyzeFolderButton = new System.Windows.Forms.Button();
            this.LoadCorrectionDataButton = new System.Windows.Forms.Button();
            this.CorrectDataButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.IntervalTextBox = new System.Windows.Forms.TextBox();
            this.AddBaselinePointsCheckBox = new System.Windows.Forms.CheckBox();
            this.RemoveBaselineButton = new System.Windows.Forms.Button();
            this.CalculateBaselineButton = new System.Windows.Forms.Button();
            this.BaseLineOrderTextBox = new System.Windows.Forms.TextBox();
            this.CalculatePeakValuesButton = new System.Windows.Forms.Button();
            this.AddPeakPointsCheckBox = new System.Windows.Forms.CheckBox();
            this.RemovePeakPointsButton = new System.Windows.Forms.Button();
            this.SeriesListBox = new System.Windows.Forms.CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.AllowDrop = true;
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea3.AxisX.MajorGrid.Interval = 25D;
            chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea3.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea3.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea3.AxisY.Minimum = 0D;
            chartArea3.CursorX.IsUserEnabled = true;
            chartArea3.CursorX.IsUserSelectionEnabled = true;
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(167, 35);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(1085, 612);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            this.chart1.DragDrop += new System.Windows.Forms.DragEventHandler(this.chart1_DragDrop);
            this.chart1.DragEnter += new System.Windows.Forms.DragEventHandler(this.chart1_DragEnter);
            this.chart1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseClick);
            // 
            // LoadAsciiFileButton
            // 
            this.LoadAsciiFileButton.Location = new System.Drawing.Point(12, 38);
            this.LoadAsciiFileButton.Name = "LoadAsciiFileButton";
            this.LoadAsciiFileButton.Size = new System.Drawing.Size(123, 23);
            this.LoadAsciiFileButton.TabIndex = 1;
            this.LoadAsciiFileButton.Text = "Load file";
            this.LoadAsciiFileButton.UseVisualStyleBackColor = true;
            this.LoadAsciiFileButton.Click += new System.EventHandler(this.LoadAsciiFileButton_Click);
            // 
            // DoFittingButton
            // 
            this.DoFittingButton.Location = new System.Drawing.Point(12, 67);
            this.DoFittingButton.Name = "DoFittingButton";
            this.DoFittingButton.Size = new System.Drawing.Size(123, 23);
            this.DoFittingButton.TabIndex = 2;
            this.DoFittingButton.Text = "Do fitting";
            this.DoFittingButton.UseVisualStyleBackColor = true;
            this.DoFittingButton.Click += new System.EventHandler(this.DoFittingButton_Click);
            // 
            // RemoveFittedCurveButton
            // 
            this.RemoveFittedCurveButton.Location = new System.Drawing.Point(12, 96);
            this.RemoveFittedCurveButton.Name = "RemoveFittedCurveButton";
            this.RemoveFittedCurveButton.Size = new System.Drawing.Size(123, 23);
            this.RemoveFittedCurveButton.TabIndex = 3;
            this.RemoveFittedCurveButton.Text = "Remove fitted curve";
            this.RemoveFittedCurveButton.UseVisualStyleBackColor = true;
            this.RemoveFittedCurveButton.Click += new System.EventHandler(this.RemoveFittedCurveButton_Click);
            // 
            // YAxisUpperBoundTextBox
            // 
            this.YAxisUpperBoundTextBox.Location = new System.Drawing.Point(205, 11);
            this.YAxisUpperBoundTextBox.Name = "YAxisUpperBoundTextBox";
            this.YAxisUpperBoundTextBox.Size = new System.Drawing.Size(149, 20);
            this.YAxisUpperBoundTextBox.TabIndex = 4;
            this.YAxisUpperBoundTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.YAxisUpperBoundTextBox_KeyUp);
            // 
            // YAxisLowerBoundTextBox
            // 
            this.YAxisLowerBoundTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.YAxisLowerBoundTextBox.Location = new System.Drawing.Point(205, 654);
            this.YAxisLowerBoundTextBox.Name = "YAxisLowerBoundTextBox";
            this.YAxisLowerBoundTextBox.Size = new System.Drawing.Size(149, 20);
            this.YAxisLowerBoundTextBox.TabIndex = 5;
            this.YAxisLowerBoundTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.YAxisLowerBoundTextBox_KeyUp);
            // 
            // AnalyzeFolderButton
            // 
            this.AnalyzeFolderButton.Location = new System.Drawing.Point(12, 125);
            this.AnalyzeFolderButton.Name = "AnalyzeFolderButton";
            this.AnalyzeFolderButton.Size = new System.Drawing.Size(123, 23);
            this.AnalyzeFolderButton.TabIndex = 6;
            this.AnalyzeFolderButton.Text = "Analyze folder";
            this.AnalyzeFolderButton.UseVisualStyleBackColor = true;
            this.AnalyzeFolderButton.Click += new System.EventHandler(this.AnalyzeFolderButton_Click);
            // 
            // LoadCorrectionDataButton
            // 
            this.LoadCorrectionDataButton.Location = new System.Drawing.Point(12, 154);
            this.LoadCorrectionDataButton.Name = "LoadCorrectionDataButton";
            this.LoadCorrectionDataButton.Size = new System.Drawing.Size(123, 23);
            this.LoadCorrectionDataButton.TabIndex = 7;
            this.LoadCorrectionDataButton.Text = "Load correction data";
            this.LoadCorrectionDataButton.UseVisualStyleBackColor = true;
            this.LoadCorrectionDataButton.Click += new System.EventHandler(this.LoadCorrectionDataButton_Click);
            // 
            // CorrectDataButton
            // 
            this.CorrectDataButton.Location = new System.Drawing.Point(12, 183);
            this.CorrectDataButton.Name = "CorrectDataButton";
            this.CorrectDataButton.Size = new System.Drawing.Size(123, 23);
            this.CorrectDataButton.TabIndex = 8;
            this.CorrectDataButton.Text = "Correct data";
            this.CorrectDataButton.UseVisualStyleBackColor = true;
            this.CorrectDataButton.Click += new System.EventHandler(this.CorrectDataButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(165, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Y max";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(165, 656);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Y min";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(423, 656);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Interval";
            // 
            // IntervalTextBox
            // 
            this.IntervalTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IntervalTextBox.Location = new System.Drawing.Point(468, 654);
            this.IntervalTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.IntervalTextBox.Name = "IntervalTextBox";
            this.IntervalTextBox.Size = new System.Drawing.Size(114, 20);
            this.IntervalTextBox.TabIndex = 12;
            this.IntervalTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.IntervalTextBox_KeyUp);
            // 
            // AddBaselinePointsCheckBox
            // 
            this.AddBaselinePointsCheckBox.AutoSize = true;
            this.AddBaselinePointsCheckBox.Location = new System.Drawing.Point(12, 237);
            this.AddBaselinePointsCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.AddBaselinePointsCheckBox.Name = "AddBaselinePointsCheckBox";
            this.AddBaselinePointsCheckBox.Size = new System.Drawing.Size(120, 17);
            this.AddBaselinePointsCheckBox.TabIndex = 13;
            this.AddBaselinePointsCheckBox.Text = "Add Baseline Points";
            this.AddBaselinePointsCheckBox.UseVisualStyleBackColor = true;
            // 
            // RemoveBaselineButton
            // 
            this.RemoveBaselineButton.Location = new System.Drawing.Point(12, 287);
            this.RemoveBaselineButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RemoveBaselineButton.Name = "RemoveBaselineButton";
            this.RemoveBaselineButton.Size = new System.Drawing.Size(123, 23);
            this.RemoveBaselineButton.TabIndex = 14;
            this.RemoveBaselineButton.Text = "Remove baseline";
            this.RemoveBaselineButton.UseVisualStyleBackColor = true;
            this.RemoveBaselineButton.Click += new System.EventHandler(this.RemoveBaselineButton_Click);
            // 
            // CalculateBaselineButton
            // 
            this.CalculateBaselineButton.Location = new System.Drawing.Point(12, 259);
            this.CalculateBaselineButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CalculateBaselineButton.Name = "CalculateBaselineButton";
            this.CalculateBaselineButton.Size = new System.Drawing.Size(123, 23);
            this.CalculateBaselineButton.TabIndex = 15;
            this.CalculateBaselineButton.Text = "Calculate baseline";
            this.CalculateBaselineButton.UseVisualStyleBackColor = true;
            this.CalculateBaselineButton.Click += new System.EventHandler(this.CalculateBaselineButton_Click);
            // 
            // BaseLineOrderTextBox
            // 
            this.BaseLineOrderTextBox.Location = new System.Drawing.Point(12, 314);
            this.BaseLineOrderTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BaseLineOrderTextBox.Name = "BaseLineOrderTextBox";
            this.BaseLineOrderTextBox.Size = new System.Drawing.Size(124, 20);
            this.BaseLineOrderTextBox.TabIndex = 16;
            // 
            // CalculatePeakValuesButton
            // 
            this.CalculatePeakValuesButton.Location = new System.Drawing.Point(12, 373);
            this.CalculatePeakValuesButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CalculatePeakValuesButton.Name = "CalculatePeakValuesButton";
            this.CalculatePeakValuesButton.Size = new System.Drawing.Size(123, 23);
            this.CalculatePeakValuesButton.TabIndex = 17;
            this.CalculatePeakValuesButton.Text = "Calculate peak values";
            this.CalculatePeakValuesButton.UseVisualStyleBackColor = true;
            this.CalculatePeakValuesButton.Click += new System.EventHandler(this.CalculatePeakValuesButton_Click);
            // 
            // AddPeakPointsCheckBox
            // 
            this.AddPeakPointsCheckBox.AutoSize = true;
            this.AddPeakPointsCheckBox.Location = new System.Drawing.Point(12, 351);
            this.AddPeakPointsCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.AddPeakPointsCheckBox.Name = "AddPeakPointsCheckBox";
            this.AddPeakPointsCheckBox.Size = new System.Drawing.Size(103, 17);
            this.AddPeakPointsCheckBox.TabIndex = 18;
            this.AddPeakPointsCheckBox.Text = "Add peak points";
            this.AddPeakPointsCheckBox.UseVisualStyleBackColor = true;
            // 
            // RemovePeakPointsButton
            // 
            this.RemovePeakPointsButton.Location = new System.Drawing.Point(12, 401);
            this.RemovePeakPointsButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RemovePeakPointsButton.Name = "RemovePeakPointsButton";
            this.RemovePeakPointsButton.Size = new System.Drawing.Size(123, 23);
            this.RemovePeakPointsButton.TabIndex = 19;
            this.RemovePeakPointsButton.Text = "Remove peak points";
            this.RemovePeakPointsButton.UseVisualStyleBackColor = true;
            this.RemovePeakPointsButton.Click += new System.EventHandler(this.RemovePeakPointsButton_Click);
            // 
            // SeriesListBox
            // 
            this.SeriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SeriesListBox.CheckOnClick = true;
            this.SeriesListBox.FormattingEnabled = true;
            this.SeriesListBox.Location = new System.Drawing.Point(12, 429);
            this.SeriesListBox.Name = "SeriesListBox";
            this.SeriesListBox.Size = new System.Drawing.Size(148, 244);
            this.SeriesListBox.TabIndex = 20;
            this.SeriesListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.SeriesListBox_ItemCheck);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 682);
            this.Controls.Add(this.SeriesListBox);
            this.Controls.Add(this.RemovePeakPointsButton);
            this.Controls.Add(this.AddPeakPointsCheckBox);
            this.Controls.Add(this.CalculatePeakValuesButton);
            this.Controls.Add(this.BaseLineOrderTextBox);
            this.Controls.Add(this.CalculateBaselineButton);
            this.Controls.Add(this.RemoveBaselineButton);
            this.Controls.Add(this.AddBaselinePointsCheckBox);
            this.Controls.Add(this.IntervalTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CorrectDataButton);
            this.Controls.Add(this.LoadCorrectionDataButton);
            this.Controls.Add(this.AnalyzeFolderButton);
            this.Controls.Add(this.YAxisLowerBoundTextBox);
            this.Controls.Add(this.YAxisUpperBoundTextBox);
            this.Controls.Add(this.RemoveFittedCurveButton);
            this.Controls.Add(this.DoFittingButton);
            this.Controls.Add(this.LoadAsciiFileButton);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button LoadAsciiFileButton;
        private System.Windows.Forms.Button DoFittingButton;
        private System.Windows.Forms.Button RemoveFittedCurveButton;
        private System.Windows.Forms.TextBox YAxisUpperBoundTextBox;
        private System.Windows.Forms.TextBox YAxisLowerBoundTextBox;
        private System.Windows.Forms.Button AnalyzeFolderButton;
        private System.Windows.Forms.Button LoadCorrectionDataButton;
        private System.Windows.Forms.Button CorrectDataButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox IntervalTextBox;
        private System.Windows.Forms.CheckBox AddBaselinePointsCheckBox;
        private System.Windows.Forms.Button RemoveBaselineButton;
        private System.Windows.Forms.Button CalculateBaselineButton;
        private System.Windows.Forms.TextBox BaseLineOrderTextBox;
        private System.Windows.Forms.Button CalculatePeakValuesButton;
        private System.Windows.Forms.CheckBox AddPeakPointsCheckBox;
        private System.Windows.Forms.Button RemovePeakPointsButton;
        private System.Windows.Forms.CheckedListBox SeriesListBox;
    }
}

