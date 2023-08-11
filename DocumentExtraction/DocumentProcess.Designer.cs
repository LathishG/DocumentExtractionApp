
namespace DocumentProcess
{
    partial class DocumentProcess
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentProcess));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.imgEdit = new AxImgeditLibCtl.AxImgEdit();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDomain = new System.Windows.Forms.Label();
            this.lbl = new System.Windows.Forms.Label();
            this.lblVmName = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblImageFile = new System.Windows.Forms.Label();
            this.lblAnnotationFile = new System.Windows.Forms.Label();
            this.lblLogFile = new System.Windows.Forms.Label();
            this.lblArchive = new System.Windows.Forms.Label();
            this.lblArchiveFiles = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblDocCount = new System.Windows.Forms.Label();
            this.lblCompleted = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.lblBurned = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblOtherType = new System.Windows.Forms.Label();
            this.lblOther = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkbMultiThread = new System.Windows.Forms.CheckBox();
            this.btn_pdf = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgEdit)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.imgEdit, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(32, 271);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(76, 42);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // imgEdit
            // 
            this.imgEdit.Location = new System.Drawing.Point(3, 3);
            this.imgEdit.Name = "imgEdit";
            this.imgEdit.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("imgEdit.OcxState")));
            this.imgEdit.Size = new System.Drawing.Size(70, 35);
            this.imgEdit.TabIndex = 0;
            this.imgEdit.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.2521F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.41457F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.btnProcess, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btn_pdf, 2, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(639, 335);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(218, 54);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(68, 3);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(73, 46);
            this.btnProcess.TabIndex = 0;
            this.btnProcess.Text = "Start Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(59, 46);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Location = new System.Drawing.Point(-82, -278);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(554, 35);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.lblStatus, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(6, 25);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(826, 35);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(64, 20);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "[Status]";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 6;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.lblUser, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.lblDomain, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.lbl, 4, 0);
            this.tableLayoutPanel5.Controls.Add(this.lblVmName, 5, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(9, 25);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(824, 38);
            this.tableLayoutPanel5.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "User";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(140, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(51, 20);
            this.lblUser.TabIndex = 1;
            this.lblUser.Text = "[User]";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(277, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Domain";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.Location = new System.Drawing.Point(414, 0);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(72, 20);
            this.lblDomain.TabIndex = 3;
            this.lblDomain.Text = "[Domain]";
            this.lblDomain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Location = new System.Drawing.Point(552, 0);
            this.lbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(117, 20);
            this.lbl.TabIndex = 4;
            this.lbl.Text = "Instance Name";
            // 
            // lblVmName
            // 
            this.lblVmName.AutoSize = true;
            this.lblVmName.Location = new System.Drawing.Point(689, 0);
            this.lblVmName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVmName.Name = "lblVmName";
            this.lblVmName.Size = new System.Drawing.Size(41, 20);
            this.lblVmName.TabIndex = 5;
            this.lblVmName.Text = "[VM]";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.98172F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.01828F));
            this.tableLayoutPanel6.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.lblImageFile, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.lblAnnotationFile, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.lblLogFile, 1, 2);
            this.tableLayoutPanel6.Controls.Add(this.lblArchive, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.lblArchiveFiles, 1, 3);
            this.tableLayoutPanel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel6.Location = new System.Drawing.Point(6, 38);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 4;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(423, 125);
            this.tableLayoutPanel6.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Image File";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Annotation";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Log File";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblImageFile
            // 
            this.lblImageFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblImageFile.AutoSize = true;
            this.lblImageFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImageFile.Location = new System.Drawing.Point(125, 0);
            this.lblImageFile.Name = "lblImageFile";
            this.lblImageFile.Size = new System.Drawing.Size(295, 20);
            this.lblImageFile.TabIndex = 3;
            this.lblImageFile.Text = "[ImageFile]";
            this.lblImageFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAnnotationFile
            // 
            this.lblAnnotationFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAnnotationFile.AutoSize = true;
            this.lblAnnotationFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAnnotationFile.Location = new System.Drawing.Point(125, 31);
            this.lblAnnotationFile.Name = "lblAnnotationFile";
            this.lblAnnotationFile.Size = new System.Drawing.Size(295, 20);
            this.lblAnnotationFile.TabIndex = 4;
            this.lblAnnotationFile.Text = "[Annotation]";
            this.lblAnnotationFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLogFile
            // 
            this.lblLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLogFile.AutoSize = true;
            this.lblLogFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogFile.Location = new System.Drawing.Point(125, 62);
            this.lblLogFile.Name = "lblLogFile";
            this.lblLogFile.Size = new System.Drawing.Size(295, 20);
            this.lblLogFile.TabIndex = 5;
            this.lblLogFile.Text = "[Log File]";
            this.lblLogFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblArchive
            // 
            this.lblArchive.AutoSize = true;
            this.lblArchive.Location = new System.Drawing.Point(4, 93);
            this.lblArchive.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblArchive.Name = "lblArchive";
            this.lblArchive.Size = new System.Drawing.Size(55, 17);
            this.lblArchive.TabIndex = 6;
            this.lblArchive.Text = "Archive";
            // 
            // lblArchiveFiles
            // 
            this.lblArchiveFiles.AutoSize = true;
            this.lblArchiveFiles.Location = new System.Drawing.Point(126, 93);
            this.lblArchiveFiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblArchiveFiles.Name = "lblArchiveFiles";
            this.lblArchiveFiles.Size = new System.Drawing.Size(63, 17);
            this.lblArchiveFiles.TabIndex = 7;
            this.lblArchiveFiles.Text = "[Archive]";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel6);
            this.groupBox1.Location = new System.Drawing.Point(12, 154);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(435, 171);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output File Location";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel4);
            this.groupBox2.Location = new System.Drawing.Point(12, 88);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(843, 66);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel5);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(844, 69);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "WorkDesk Details";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel7);
            this.groupBox4.Location = new System.Drawing.Point(454, 155);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(404, 171);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Process Summary";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.label10, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.lblDocCount, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.lblCompleted, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.lblError, 1, 2);
            this.tableLayoutPanel7.Controls.Add(this.lblBurned, 1, 3);
            this.tableLayoutPanel7.Controls.Add(this.label9, 0, 5);
            this.tableLayoutPanel7.Controls.Add(this.lblTime, 1, 5);
            this.tableLayoutPanel7.Controls.Add(this.lblOtherType, 0, 4);
            this.tableLayoutPanel7.Controls.Add(this.lblOther, 1, 4);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(14, 26);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 6;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(376, 138);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Total";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 17);
            this.label7.TabIndex = 1;
            this.label7.Text = "Completed";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 17);
            this.label8.TabIndex = 2;
            this.label8.Text = "Error";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(3, 66);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 17);
            this.label10.TabIndex = 4;
            this.label10.Text = "Has Annotation";
            // 
            // lblDocCount
            // 
            this.lblDocCount.AutoSize = true;
            this.lblDocCount.Location = new System.Drawing.Point(191, 0);
            this.lblDocCount.Name = "lblDocCount";
            this.lblDocCount.Size = new System.Drawing.Size(39, 20);
            this.lblDocCount.TabIndex = 5;
            this.lblDocCount.Text = "[cnt]";
            // 
            // lblCompleted
            // 
            this.lblCompleted.AutoSize = true;
            this.lblCompleted.Location = new System.Drawing.Point(191, 22);
            this.lblCompleted.Name = "lblCompleted";
            this.lblCompleted.Size = new System.Drawing.Size(39, 20);
            this.lblCompleted.TabIndex = 6;
            this.lblCompleted.Text = "[cnt]";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(191, 44);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(39, 20);
            this.lblError.TabIndex = 7;
            this.lblError.Text = "[cnt]";
            // 
            // lblBurned
            // 
            this.lblBurned.AutoSize = true;
            this.lblBurned.Location = new System.Drawing.Point(191, 66);
            this.lblBurned.Name = "lblBurned";
            this.lblBurned.Size = new System.Drawing.Size(39, 20);
            this.lblBurned.TabIndex = 8;
            this.lblBurned.Text = "[cnt]";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(3, 110);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 17);
            this.label9.TabIndex = 3;
            this.label9.Text = "Time Elapsed";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(191, 110);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(47, 20);
            this.lblTime.TabIndex = 9;
            this.lblTime.Text = "[time]";
            // 
            // lblOtherType
            // 
            this.lblOtherType.AutoSize = true;
            this.lblOtherType.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOtherType.Location = new System.Drawing.Point(4, 88);
            this.lblOtherType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOtherType.Name = "lblOtherType";
            this.lblOtherType.Size = new System.Drawing.Size(113, 17);
            this.lblOtherType.TabIndex = 10;
            this.lblOtherType.Text = "Other Type Files";
            // 
            // lblOther
            // 
            this.lblOther.AutoSize = true;
            this.lblOther.Location = new System.Drawing.Point(192, 88);
            this.lblOther.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOther.Name = "lblOther";
            this.lblOther.Size = new System.Drawing.Size(39, 20);
            this.lblOther.TabIndex = 11;
            this.lblOther.Text = "[cnt]";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkbMultiThread);
            this.groupBox5.Location = new System.Drawing.Point(18, 328);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(429, 54);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            // 
            // chkbMultiThread
            // 
            this.chkbMultiThread.AutoSize = true;
            this.chkbMultiThread.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkbMultiThread.Location = new System.Drawing.Point(10, 22);
            this.chkbMultiThread.Name = "chkbMultiThread";
            this.chkbMultiThread.Size = new System.Drawing.Size(154, 21);
            this.chkbMultiThread.TabIndex = 0;
            this.chkbMultiThread.Text = "Run As Multithread";
            this.chkbMultiThread.UseVisualStyleBackColor = true;
            // 
            // btn_pdf
            // 
            this.btn_pdf.Location = new System.Drawing.Point(147, 3);
            this.btn_pdf.Name = "btn_pdf";
            this.btn_pdf.Size = new System.Drawing.Size(68, 45);
            this.btn_pdf.TabIndex = 2;
            this.btn_pdf.Text = "PDF";
            this.btn_pdf.UseVisualStyleBackColor = true;
            this.btn_pdf.Click += new System.EventHandler(this.btn_pdf_Click);
            // 
            // DocumentProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 395);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(852, 425);
            this.Name = "DocumentProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Document Processor";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgEdit)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private AxImgeditLibCtl.AxImgEdit imgEdit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblImageFile;
        private System.Windows.Forms.Label lblAnnotationFile;
        private System.Windows.Forms.Label lblLogFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblDocCount;
        private System.Windows.Forms.Label lblCompleted;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label lblBurned;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkbMultiThread;
        private System.Windows.Forms.Label lblOtherType;
        private System.Windows.Forms.Label lblArchive;
        private System.Windows.Forms.Label lblArchiveFiles;
        private System.Windows.Forms.Label lblOther;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Label lblVmName;
        private System.Windows.Forms.Button btn_pdf;
    }
}

