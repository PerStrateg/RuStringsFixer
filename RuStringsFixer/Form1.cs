using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuStringsFixer
{
    public partial class Form1 : Form
    {
        DataTable dataTable;

        public Form1()
        {
            InitializeComponent();

            textBox1.Text = "D:/";
            comboBox1.Text = comboBox1.Items[0].ToString();
            
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            dataTable = new DataTable();
            dataGridView1.DataSource = dataTable;
            dataTable.Columns.Add("Имя оригинального .STRINGS");
            dataTable.Columns.Add("Имя будущего .STRINGS");
            dataTable.Columns.Add("Тип");
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].FillWeight = 100;
            dataGridView1.Columns[1].FillWeight = 100;
            dataGridView1.Columns[2].FillWeight = 30;

            if (Program.Args.Length != 0)
            {
                foreach (string file in Program.Args)
                    if (StringsExe.CheckIfStringsExt(file) && !CheckAlreadyExistsInTable(file))
                        dataTable.LoadDataRow((String[])new ArrayList() { file, file, Path.GetExtension(file) }.ToArray(typeof(string)), true);
            }

            ChangeDestPaths();
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
                if (StringsExe.CheckIfStringsExt(file) && !CheckAlreadyExistsInTable(file))
                    dataTable.LoadDataRow((String[])new ArrayList() { file, file, Path.GetExtension(file) }.ToArray(typeof(string)), true);

            ChangeDestPaths();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //StringsExe.DeleteDirectory("\\temp\\", Application.StartupPath + "\\temp\\");
            //StringsExe.DeleteFile("StringsUnpacker.exe");
        }

        bool CheckAlreadyExistsInTable(string file)
        {
            bool flag = false;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.Equals(file))
                    flag = true;
            }

            return flag;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringsExe.ExtractResource("StringsPacker.exe", Application.StartupPath + "\\temp\\");
            StringsExe.ExtractResource("StringsUnpacker.exe", Application.StartupPath + "\\temp\\");

            string stringsPackerPath = "\\temp\\StringsPacker.exe";
            string stringsUnpackerPath = "\\temp\\StringsUnpacker.exe";
            string tempPath = "\\temp\\";
            string line = "";
            string orig = "";
            string dest = "";

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                orig = dataGridView1.Rows[i].Cells[0].Value.ToString();
                dest = dataGridView1.Rows[i].Cells[1].Value.ToString();

                ///////String to Csv
                line = "";

                if (dataGridView1.Rows[i].Cells[2].Value.Equals(".ILSTRINGS"))
                    line += "/IL ";
                else if (dataGridView1.Rows[i].Cells[2].Value.Equals(".DLSTRINGS"))
                    line += "/DL ";

                line += StringsExe.StringsToCsv(orig, Application.StartupPath + tempPath);

                StringsExe.StartProcess("\"" + Application.StartupPath + stringsUnpackerPath + "\"", line);

                ///////UTF-8ToAnsi
                if (comboBox1.Text.Equals(comboBox1.Items[0]))
                    StringsExe.ConvertUTF8ToAnsi(Application.StartupPath + tempPath + "file.csv", Application.StartupPath + tempPath + "file1.csv");
                else if (comboBox1.Text.Equals(comboBox1.Items[1]))
                    StringsExe.ConvertAnsiToUTF8(Application.StartupPath + tempPath + "file.csv", Application.StartupPath + tempPath + "file1.csv");

                ///////Csv to String
                line = "";

                if (dataGridView1.Rows[i].Cells[2].Equals(".ILSTRINGS"))
                    line += "/IL ";
                else if (dataGridView1.Rows[i].Cells[2].Equals(".DLSTRINGS"))
                    line += "/DL ";

                line += StringsExe.CsvToStrings(Application.StartupPath + tempPath + "file1.csv", dest);

                StringsExe.StartProcess("\"" + Application.StartupPath + stringsPackerPath + "\"", line);

                StringsExe.DeleteFile(Application.StartupPath + tempPath + "file.csv");
                StringsExe.DeleteFile(Application.StartupPath + tempPath + "file1.csv");
            }
            
            StringsExe.DeleteDirectory("\\temp\\", Application.StartupPath + "\\temp\\");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            dlg.SelectedPath = textBox1.Text;

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            textBox1.Text = dlg.SelectedPath;
            textBox1.Text = textBox1.Text.Replace("\\", "/");
            textBox1.Text += "/";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = !label3.Enabled;
            textBox1.Enabled = !textBox1.Enabled;
            button2.Enabled = !button2.Enabled;

            ChangeDestPaths();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ChangeDestPaths();
        }

        private void ChangeDestPaths()
        {
            if (!textBox1.Enabled)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Cells[1].Value = dataGridView1.Rows[i].Cells[0].Value;
                }
            }
            else
            {
                textBox1.Text = textBox1.Text.Replace("\\", "/");

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Cells[1].Value = textBox1.Text + Path.GetFileName(dataGridView1.Rows[i].Cells[0].Value.ToString());
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataTable.Clear();
            dataGridView1.DataSource = dataTable;
        }
    }
}
