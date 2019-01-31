using System;
using System.IO;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Txt2Voi
{
    public partial class Form1 : Form
    {
        private SpeechSynthesizer speech;
        private string speakerName = "";
        /// <summary>
        /// 音量
        /// </summary>
        private int value = 100;
        /// <summary>
        /// 语速
        /// </summary>
        private int rate;

        public Form1()
        {
            InitializeComponent();
            speech = new SpeechSynthesizer();
            //获取本机上所安装的所有的Voice的名称
            speaker.Items.Clear();
            foreach (InstalledVoice iv in speech.GetInstalledVoices())
            {
                speaker.Items.Add(iv.VoiceInfo.Name);
            }
            speaker.SelectedIndex = 0;
            comboBox1.SelectedIndex = 3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            if (text.Trim().Length == 0)
            {
                MessageBox.Show("不能阅读空内容!", "错误提示");
                return;
            }

            if (button1.Text == "语音试听")
            {
                speakerName = speaker.Text;
                try
                {
                    new Thread(Speak).Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }

                button1.Text = "停止试听";
            }
            else if (button1.Text == "停止试听")
            {
                speech.SpeakAsyncCancelAll();//停止阅读
                button1.Text = "语音试听";
            }
        }

        private void Speak()
        {
            speech.Rate = rate;
            speech.SelectVoice(speakerName);//设置播音员（中文）
            speech.Volume = value;
            speech.SpeakAsync(textBox1.Text);//语音阅读方法
            speech.SpeakCompleted += speech_SpeakCompleted;//绑定事件
        }

        /// <summary>
        /// 语音阅读完成触发此事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            button1.Text = "语音试听";
        }


        /// <summary>
        /// 拖动进度条事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //因为trackBar1的值为（0-10）之间而音量值为（0-100）所以要乘10；
            value = trackBar1.Value * 10;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            if (text.Trim().Length == 0)
            {
                MessageBox.Show("空内容无法生成!", "错误提示");
                return;
            }
            this.SaveFile(text);
        }

        /// <summary>
        /// 生成语音文件的方法
        /// </summary>
        /// <param name="text"></param>
        private void SaveFile(string text)
        {
            speech = new SpeechSynthesizer();
            var dialog = new SaveFileDialog();
            dialog.Filter = "*.wav|*.wav|*.mp3|*.mp3";
            dialog.ShowDialog();
            string path = dialog.FileName;
            if (path.Trim().Length == 0)
            {
                return;
            }
            speech.SetOutputToWaveFile(path);
            speech.Volume = value;
            speech.Rate = rate;
            speech.Speak(text);
            speech.SetOutputToNull();
            MessageBox.Show("生成成功!在" + path + "路径中！", "提示");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            rate = Int32.Parse(comboBox1.Text);
        }

        /// <summary>
        /// 读取本地文本文件的方法
        /// </summary>
        private void ReadlocalFile()
        {
            var open = new OpenFileDialog();

            open.ShowDialog();

            //得到文件路径
            string path = open.FileName;

            if (path.Trim().Length == 0)
            {
                return;
            }

            var os = new StreamReader(path, Encoding.UTF8);
            string str = os.ReadToEnd();
            textBox1.Text = str;
        }
        private void 清空内容ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.ReadlocalFile();
        }
    }

}

