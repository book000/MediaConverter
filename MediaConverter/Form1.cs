using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListBox;

namespace MediaConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addLogBox("Application Start");

            comboBox1.SelectedIndex = 0;

            if (!File.Exists("ffmpeg.exe"))
            {
                MessageBox.Show("同じディレクトリ内にffmpeg.exeを配置してください。",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            int i = 0;
            foreach (string file in files)
            {
                if(!listBox1.Items.Contains(file)){
                    listBox1.Items.Add(file);
                    i++;
                }
            }
            addLogBox("Added " + i);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            addLogBox("Cleared List");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            addLogBox("Process Start");

            addLogBox("Files Count: " + listBox1.Items.Count);

            int success = 0;
            int error = 0;

            List<string> removes = new List<string>();

            progressBar1.Maximum = listBox1.Items.Count;
            progressBar1.Value = 0;

            foreach (string path in listBox1.Items)
            {
                string filePath = @"output\" + Path.GetFileNameWithoutExtension(path) + "." + comboBox1.SelectedItem;
                addLogBox(Path.GetFileNameWithoutExtension(path));
                addLogBox(Path.GetExtension(path) + " ->" + comboBox1.SelectedItem);

                Process p = Process.Start(
                    "ffmpeg.exe",
                    "-y -i \"" + path + "\" \"" + filePath + "\"");

                p.WaitForExit();
                removes.Add(path);
                
                if(p.ExitCode == 0)
                {
                    addLogBox("Success!");
                    success++;
                }
                else
                {
                    addLogBox("Error...");
                    error++;
                }
                progressBar1.Value++;
            }
            foreach(string remove in removes)
            {
                listBox1.Items.Remove(remove);
            }
            
            MessageBox.Show("処理終了" + Environment.NewLine + "成功数: " + success + Environment.NewLine + "失敗数: " + error,
                    "Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            addLogBox("成功数: " + success);
            addLogBox("失敗数: " + error);
            button2.Enabled = true;
        }

        void addLogBox(string text)
        {
            string date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            textBox1.Text = textBox1.Text + "[" + date + "] " + text + Environment.NewLine;
        }
    }
}
