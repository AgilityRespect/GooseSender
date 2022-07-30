using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PcapDotNet.Base;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.Gre;
using PcapDotNet.Packets.Http;
using PcapDotNet.Packets.Icmp;
using PcapDotNet.Packets.Igmp;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;
using PcapDotNet.Packets.Transport;

namespace GooseSender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
            treeView1.AfterSelect += TreeView1_AfterSelect;
            сетьToolStripMenuItem.MouseDown += сетьToolStripMenuItem_MouseDown;
        }

        //public async Task RunPeriodicallyAsync(Func<Task> func, TimeSpan interval, CancellationToken cancellationToken)
        //{
        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        await Task.Delay(interval, cancellationToken)
        //        await func();
        //    }
        //}

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //Button button = new Button();
            //splitContainer1.Panel2.Controls.Add(button);
            //splitContainer1.Panel2.Controls[0].Name = "Button" + "0";
        }

        private void Panel2_Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int fileNumber = treeView1.SelectedNode.Parent.Index;
            int gooseNumber = treeView1.SelectedNode.Index;

            if (fileList[fileNumber].Packets[gooseNumber].AllData.Substring(Convert.ToInt32(button.Name), 2) == "83")
            {
                if (fileList[fileNumber].Packets[gooseNumber].AllData.Substring(Convert.ToInt32(button.Name) + 4, 2) == "00")
                {
                    fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Remove(Convert.ToInt32(button.Name) + 4, 2);
                    fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Insert(Convert.ToInt32(button.Name) + 4, "01");
                }
                else
                {
                    fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Remove(Convert.ToInt32(button.Name) + 4, 2);
                    fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Insert(Convert.ToInt32(button.Name) + 4, "00");
                }
            }
            else if (fileList[fileNumber].Packets[gooseNumber].AllData.Substring(Convert.ToInt32(button.Name), 4) == "8402")
            {
                if (fileList[fileNumber].Packets[gooseNumber].AllData.Substring(Convert.ToInt32(button.Name) + 6, 2) == "00")
                {
                    fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Remove(Convert.ToInt32(button.Name) + 6, 2);
                    fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Insert(Convert.ToInt32(button.Name) + 6, "01");
                }
                else
                {
                    fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Remove(Convert.ToInt32(button.Name) + 6, 2);
                    fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Insert(Convert.ToInt32(button.Name) + 6, "00");
                }
            }
            //else if (fileList[fileNumber].Packets[gooseNumber].AllData.Substring(Convert.ToInt32(button.Name), 4) == "8402")
            //{
            //    if (fileList[fileNumber].Packets[gooseNumber].AllData.Substring(Convert.ToInt32(button.Name) + 6, 2) == "00")
            //    {
            //        fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Remove(Convert.ToInt32(button.Name) + 6, 2);
            //        fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Insert(Convert.ToInt32(button.Name) + 6, "01");
            //    }
            //    else if (fileList[fileNumber].Packets[gooseNumber].AllData.Substring(Convert.ToInt32(button.Name) + 6, 2) == "01")
            //    {
            //        fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Remove(Convert.ToInt32(button.Name) + 6, 2);
            //        fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Insert(Convert.ToInt32(button.Name) + 6, "10");
            //    }
            //    else if (fileList[fileNumber].Packets[gooseNumber].AllData.Substring(Convert.ToInt32(button.Name) + 6, 2) == "10")
            //    {
            //        fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Remove(Convert.ToInt32(button.Name) + 6, 2);
            //        fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Insert(Convert.ToInt32(button.Name) + 6, "11");
            //    }
            //    else
            //    {
            //        fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Remove(Convert.ToInt32(button.Name) + 6, 2);
            //        fileList[fileNumber].Packets[gooseNumber].AllData = fileList[fileNumber].Packets[gooseNumber].AllData.Insert(Convert.ToInt32(button.Name) + 6, "00");
            //    }
            //}
            else
            {
                return;
            }

            splitContainer1.Panel2.Controls.Clear();
            ButtonsGenerator(fileNumber, gooseNumber);
        }

        public void ButtonSendMany_Click(object sender, EventArgs e)
        {
            fileList[treeView1.SelectedNode.Parent.Index].Packets[treeView1.SelectedNode.Index].SendingNow = 
                !fileList[treeView1.SelectedNode.Parent.Index].Packets[treeView1.SelectedNode.Index].SendingNow;

            Control[] controls = splitContainer1.Panel2.Controls.Find("frequency", false);
            SendMany(treeView1.SelectedNode.Parent.Index, treeView1.SelectedNode.Index, Convert.ToInt32(controls[0].Text));
            
            Control[] controls2 = splitContainer1.Panel2.Controls.Find("sendMany", false);
            if (fileList[treeView1.SelectedNode.Parent.Index].Packets[treeView1.SelectedNode.Index].SendingNow)
            {
                controls2[0].Text = "Sending...";
            }
            else
            {
                controls2[0].Text = "Stopped";
            }
        }

        private void ButtonSendOnce_Click(object sender, EventArgs e)
        {
            SendOnce(treeView1.SelectedNode.Parent.Index, treeView1.SelectedNode.Index);
        }

        List<Program.BinaryFile> fileList = new List<Program.BinaryFile>();
        /* понятия не имею что это */
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void SendMany(int fileNumber, int gooseNumber, int frequency)
        {
            fileList[fileNumber].Packets[gooseNumber].SendingFrequency = frequency;

            if (fileList[fileNumber].Packets[gooseNumber].SendingNow)
            {
                Task.Run(async delegate
                {
                    await Task.Delay(frequency).ContinueWith(_ =>
                    {
                        SendMany(fileNumber, gooseNumber, frequency);
                    });
                    IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
                    if (allDevices.Count == 0)
                    {
                        MessageBox.Show("No interfaces found! Make sure WinPcap is installed.");
                        return;
                    }
                    PacketDevice selectedDevice = allDevices[0];
                    using (PacketCommunicator communicator = selectedDevice.Open(100, // name of the device
                                                                                     PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                                                                     1000)) // read timeout
                    {
                        if (fileList[fileNumber].Packets[gooseNumber].AllData != fileList[fileNumber].Packets[gooseNumber].AllDataLastModify)
                        {
                            fileList[fileNumber].Packets[gooseNumber].StNum = (Byte.Parse(fileList[fileNumber].Packets[gooseNumber].StNum,
                                System.Globalization.NumberStyles.HexNumber, null) + 1).ToString("X2"); // расширить для большИх чисел
                            fileList[fileNumber].Packets[gooseNumber].SqNum = "01"; // переопределить для бОльших чисел
                            fileList[fileNumber].Packets[gooseNumber].AllDataLastModify = fileList[fileNumber].Packets[gooseNumber].AllData;
                        }
                        else
                        {
                            fileList[fileNumber].Packets[gooseNumber].SqNum = (Byte.Parse(fileList[fileNumber].Packets[gooseNumber].SqNum,
                            System.Globalization.NumberStyles.HexNumber, null) + 1).ToString("X2");
                        }
                        communicator.SendPacket(Program.BuildEthernetPacket(Program.ConvertToByteArray(Program.BuildPacketString(fileList[fileNumber].Packets[gooseNumber]))));
                    }
                });
            }
        }

        private void TimerT_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SendOnce(int fileNumber, int gooseNumber)
        {
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
            if (allDevices.Count == 0)
            {
                MessageBox.Show("No interfaces found! Make sure WinPcap is installed.");
                return;
            }
            PacketDevice selectedDevice = allDevices[0];
            using (PacketCommunicator communicator = selectedDevice.Open(100, // name of the device
                                                                         PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                                                         1000)) // read timeout
            {
                if (fileList[fileNumber].Packets[gooseNumber].AllData != fileList[fileNumber].Packets[gooseNumber].AllDataLastModify)
                {
                    fileList[fileNumber].Packets[gooseNumber].StNum = (Byte.Parse(fileList[fileNumber].Packets[gooseNumber].StNum,
                        System.Globalization.NumberStyles.HexNumber, null) + 1).ToString("X2"); // расширить для большИх чисел
                    fileList[fileNumber].Packets[gooseNumber].SqNum = "01"; // переопределить для бОльших чисел

                    fileList[fileNumber].Packets[gooseNumber].AllDataLastModify = fileList[fileNumber].Packets[gooseNumber].AllData;
                }
                else
                {
                    fileList[fileNumber].Packets[gooseNumber].SqNum = (Byte.Parse(fileList[fileNumber].Packets[gooseNumber].SqNum,
                        System.Globalization.NumberStyles.HexNumber, null) + 1).ToString("X2");
                }
                communicator.SendPacket(Program.BuildEthernetPacket(Program.ConvertToByteArray(Program.BuildPacketString(fileList[fileNumber].Packets[gooseNumber]))));
                MessageBox.Show("Succeed");
            }
        }

        private void AddLabel(int vertical)
        {
            Label label = new Label();
            splitContainer1.Panel2.Controls.Add(label);
            label.Left = 270;
            label.Top = vertical + 5;
            label.Text = "Structure";
            label.Width = 50;
        }

        private void AddButton(int horizontal, int vertical, string name, string labelText, string text)
        {
            Label label = new Label();
            splitContainer1.Panel2.Controls.Add(label);
            label.Left = horizontal;
            label.Top = vertical + 5;
            label.Text = labelText;
            label.Width = 60;

            Button button = new Button();
            splitContainer1.Panel2.Controls.Add(button);
            button.Left = horizontal + 60;
            button.Top = vertical;
            button.Name = name;
            button.Text = text;
            button.Width += 50;

            button.Click += Panel2_Button_Click;
        }

        private void ButtonsGenerator(int fileNumber, int gooseNumber)
        {
            string allData = fileList[fileNumber].Packets[gooseNumber].AllData;
            int horizontal = 0;
            int vertical = 0;

            for (int i = 0; i < allData.Length; i += 2) // i is string char
            {
                switch (allData.Substring(i, 2))
                {
                    case "83":
                        if (!(allData.Substring(i + 2, 2) == "01"))
                        {
                            MessageBox.Show("Corrupted data");
                            return;
                        }
                        i += 4;
                        //MessageBox.Show("Boolean " + allData.Substring(i, 2));
                        {
                            AddButton(horizontal, vertical, (i - 4).ToString(), "Boolean", allData.Substring(i, 2));
                            vertical += 30;
                        }
                        break;
                    case "84":
                        i += 2;
                        if (allData.Substring(i, 2) == "03")
                        {
                            //MessageBox.Show("Bitstring " + allData.Substring(i + 2, 6));
                            AddButton(horizontal, vertical, (i - 2).ToString(), "BitString", allData.Substring(i + 2, 6));
                            vertical += 30;
                            i += 6;
                        }
                        else if (allData.Substring(i, 2) == "02")
                        {
                            //MessageBox.Show("dbPos " + allData.Substring(i + 2, 4));
                            AddButton(horizontal, vertical, (i - 2).ToString(), "dbPos", allData.Substring(i + 2, 4));
                            vertical += 30;
                            i += 4;
                        }
                        else
                        {
                            MessageBox.Show("Corrupted data");
                            return;
                        }
                        break;
                    case "91":
                        if (!(allData.Substring(i + 2, 2) == "08"))
                        {
                            MessageBox.Show("Corrupted data");
                            return;
                        }
                        i += 4;
                        //MessageBox.Show("Timestamp " + allData.Substring(i, 16));
                        AddButton(horizontal, vertical, (i - 4).ToString(), "Timestamp", allData.Substring(i, 16));
                        vertical += 30;
                        i += 14;
                        break;
                    case "A2":
                        //MessageBox.Show("STRUCTURE");
                        AddLabel(vertical);
                        vertical += 30;
                        for (int k = i + 4; k < i + 4 + 2 * Byte.Parse(allData.Substring(i + 2, 2), System.Globalization.NumberStyles.HexNumber, null); k += 2) // k is index for A2 tag
                        {
                            switch (allData.Substring(k, 2))
                            {
                                case "83":
                                    if (!(allData.Substring(k + 2, 2) == "01"))
                                    {
                                        MessageBox.Show("Corrupted data");
                                        return;
                                    }
                                    k += 4;
                                    //MessageBox.Show("    Boolean " + allData.Substring(k, 2));
                                    AddButton(horizontal + 200, vertical, (k - 4).ToString(), "Boolean", allData.Substring(k, 2));
                                    vertical += 30;
                                    break;
                                case "84":
                                    k += 2;
                                    if (allData.Substring(k, 2) == "03")
                                    {
                                        //MessageBox.Show("    Bitstring " + allData.Substring(k + 2, 6));
                                        AddButton(horizontal + 200, vertical, (k - 2).ToString(), "BitString", allData.Substring(k + 2, 6));
                                        vertical += 30;
                                        k += 6;
                                    }
                                    else if (allData.Substring(k, 2) == "02")
                                    {
                                        //MessageBox.Show("    dbPos " + allData.Substring(k + 2, 4));
                                        AddButton(horizontal + 200, vertical, (k - 2).ToString(), "dbPos", allData.Substring(k + 2, 4));
                                        vertical += 30;
                                        k += 4;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Corrupted data");
                                        return;
                                    }
                                    break;
                                case "91":
                                    if (!(allData.Substring(k + 2, 2) == "08"))
                                    {
                                        MessageBox.Show("Corrupted data");
                                        return;
                                    }
                                    k += 4;
                                    //MessageBox.Show("    Timestamp " + allData.Substring(k, 16));
                                    AddButton(horizontal + 200, vertical, (k - 4).ToString(), "Timestamp", allData.Substring(k, 16));
                                    vertical += 30;
                                    k += 14;
                                    break;
                                default:
                                    MessageBox.Show("Corrupted data");
                                    return;
                            }
                        }
                        i += 2 + 2 * Byte.Parse(allData.Substring(i + 2, 2), System.Globalization.NumberStyles.HexNumber, null);
                        break;
                    default:
                        MessageBox.Show("Corrupted data");
                        return;
                }
                //i += Convert.ToByte(Packets[number].AllData.Substring(i, 1)) + 1;
            }

            TextBox textBox = new TextBox();
            splitContainer1.Panel2.Controls.Add(textBox);
            textBox.Left = 425;
            textBox.Top = 10;
            textBox.Name = "frequency";
            textBox.Width = 145;
            textBox.Text = fileList[fileNumber].Packets[gooseNumber].SendingFrequency.ToString();
            //textBox.ForeColor = Color.Gray;

            Button buttonSendMany = new Button();
            splitContainer1.Panel2.Controls.Add(buttonSendMany);
            buttonSendMany.Left = 410;
            buttonSendMany.Top = 30;
            buttonSendMany.Name = "sendMany";
            buttonSendMany.Text = "Stopped";
            buttonSendMany.Width = 175;

            buttonSendMany.Click += ButtonSendMany_Click;

            Button buttonSendOnce = new Button();
            splitContainer1.Panel2.Controls.Add(buttonSendOnce);
            buttonSendOnce.Left = 460;
            buttonSendOnce.Top = 70;
            buttonSendOnce.Name = "sendOnce";
            buttonSendOnce.Text = "Send once";

            buttonSendOnce.Click += ButtonSendOnce_Click;

            ////////////////////////////////////
            //for (int i = 0; i < 3; i++)
            //{
            //    Button btn = new Button();
            //    btn.Text = "btn" + i;
            //    btn.Location = new Point(i * 50, i * 50);
            //    //listik.Controls.Add(btn);

            //    return;

            //    if (treeView1.SelectedNode.Level == 2 && treeView1.SelectedNode.IsSelected)
            //    {
            //        int gooseindex = treeView1.SelectedNode.Index;
            //        int fileindex = treeView1.SelectedNode.Parent.Index;

            //        MessageBox.Show("file " + fileindex + " goose " + gooseindex);
            //    }
            //}
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear(); // очищаем панель 2
            int fileNumber;
            int gooseNumber;
            if (treeView1.SelectedNode.Level == 2)
            {
                fileNumber = treeView1.SelectedNode.Parent.Index;
                gooseNumber = treeView1.SelectedNode.Index;
                //MessageBox.Show("file " + fileNumber.ToString() + " goose " + gooseNumber.ToString());
                ButtonsGenerator(fileNumber, gooseNumber);
            }
        }

        private void AddTreeBranch(Program.BinaryHeader1 header, List<Program.Packet1> gooseList, string fileName)
        {
            treeView1.Nodes[0].Nodes.Add(fileName);

            for (int i = 0; i < gooseList.Count; i++)
            {
                Program.Packet1 goose = gooseList[i];
                int lastNodeNumber = treeView1.Nodes[0].Nodes.Count - 1;
                treeView1.Nodes[0].Nodes[lastNodeNumber].Nodes.Add(Program.StringToCharString(goose.GoID));

                goose.AllDataLastModify = goose.AllData;
            }
        }

        private List<Program.Packet1> BinHandler(byte[] bin)
        {
            //string data = "";
            //foreach (byte b in bin)
            //{
            //    data += b.ToString("X2");
            //}
            ////Program.BinaryHeader1 header = new Program.BinaryHeader1();
            ////header = Program.HeaderReader(bin);
            ////List<Program.Packet1> gooseList = new List<Program.Packet1>();
            //List<Program.Packet1> gooseList = Program.FileReader(bin);
            //AddTreeNode(bin);
            return Program.FileReader(bin);
        }
        /// <summary>
        /// Кнопка меню Файл -> Открыть
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                byte[] bin = File.ReadAllBytes(ofd.FileName);
                Program.BinaryHeader1 header = Program.HeaderReader(bin);

                Program.BinaryFile binaryFile = new Program.BinaryFile();
                binaryFile.Packets = BinHandler(bin);
                binaryFile.Name = ofd.FileName;
                fileList.Add(binaryFile);

                AddTreeBranch(header, binaryFile.Packets, binaryFile.Name);
            }
        }

        private void сетьToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            //Program.ChooseNetwork();
        }

        //private void textBox_Enter(object sender, EventArgs e)//происходит когда элемент стает активным
        //{
        //    Control[] textBoxes = splitContainer1.Panel2.Controls.Find("frequency", false);
        //    var textBox = textBoxes[0];
        //    textBox.Text = null;
        //    textBox.ForeColor = Color.Black;
        //}

        //private void InitializeTreeView()
        //{
        //    TreeNode docNode = new TreeNode("Список файлов");

        //    treeView1.Nodes.Add("Список файлов");
        //    treeView1.Nodes[0].Nodes.Add("Child 1");
        //    treeView1.Nodes[0].Nodes.Add("Child 2");
        //    treeView1.Nodes[0].Nodes[1].Nodes.Add("Grandchild");
        //    treeView1.Nodes[0].Nodes[1].Nodes[0].Nodes.Add("Great Grandchild");
        //}

    }
}

