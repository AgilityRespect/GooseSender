using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Linq;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GooseSender
{
    public static class Program
    {
        /// <summary>
        /// This function build an Ethernet with payload packet.
        /// </summary>
        public static Packet BuildEthernetPacket(byte[] bin)//string buffer)
        {
            EthernetLayer ethernetLayer =
                new EthernetLayer
                {
                    Source = new MacAddress(ConvertMacAddress(GetMacAddress())),//new MacAddress("01:01:01:01:01:01"),
                    Destination = new MacAddress("01:0c:cd:01:ff:ff"),
                    EtherType = EthernetType.VLanTaggedFrame,
                };

            PayloadLayer payloadLayer =
                new PayloadLayer
                {
                    Data = new Datagram(bin),
                };

            PacketBuilder builder = new PacketBuilder(ethernetLayer, payloadLayer);

            return builder.Build(DateTime.Now);
        }

        public struct BinaryHeader1
        {
            public string TagHeader;
            public byte TagGooseAmount;
        }

        public class Packet1
        {
            public System.Net.NetworkInformation.PhysicalAddress DestinationMAC = null;
            public System.Net.NetworkInformation.PhysicalAddress SourceMAC = null;
            public string EthernetType = "";
            public string AppID = "";
            public string Length = "";
            public string Reserved1 = "";
            public string Reserved2 = "";
            public string GoosePduLength = "";
            public string GoCBRef = "";
            public string TimeAllowedToLive = "";
            public string DatSet = "";
            public string GoID = "";
            public string Time = "";
            public string StNum = "";
            public string SqNum = "";
            public string Simulation = "";
            public string ConfRev = "";
            public string NdsCom = "";
            public string NumDatSetEntries = "";
            public string AllData = "";
            public string PrpProtocol = "";

            public bool FileIsCorrupted = false;

            public bool SendingNow = false;
            public int SendingFrequency = 1000;
            public string AllDataLastModify = "";

            //public System.Timers.Timer TimerT = new System.Timers.Timer(1000);
        }

        public static void OutputByteArray(byte[] byteArray)
        {
            for (int i = 0; i < byteArray.Length; i++)
            {
                if (byteArray[i] == 0x01)
                {
                    if (byteArray[i + 1] == 0x0C)
                    {
                        if (byteArray[i + 2] == 0xCD)
                        {
                            if (byteArray[i + 3] == 0x01)
                            {
                                Console.WriteLine();
                                Console.WriteLine();
                                Console.Write(byteArray[i].ToString("X2") + " ");
                                i++;

                                for (int j = i; j < byteArray.Length; j++)
                                {
                                    if (byteArray[j] == 0x01)
                                    {
                                        if (byteArray[j + 1] == 0x0C)
                                        {
                                            if (byteArray[j + 2] == 0xCD)
                                            {
                                                if (byteArray[j + 3] == 0x01)
                                                {
                                                    Console.WriteLine();
                                                    Console.WriteLine();
                                                    Console.Write(byteArray[j].ToString("X2") + " ");
                                                    j++;

                                                    for (int k = j; k < byteArray.Length; k++)
                                                    {
                                                        Console.Write(byteArray[k].ToString("X2") + " ");
                                                        if ((k - j + 2) % 16 == 0)
                                                        {
                                                            Console.WriteLine();
                                                        }
                                                        else if ((k - j + 2) % 8 == 0)
                                                        {
                                                            Console.Write(" ");
                                                        }
                                                        if (byteArray[k] == 0x01)
                                                        {
                                                            if (byteArray[k + 1] == 0x0C)
                                                            {
                                                                if (byteArray[k + 2] == 0xCD)
                                                                {
                                                                    if (byteArray[k + 3] == 0x01)
                                                                    {
                                                                        Console.WriteLine("WTF");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (k == byteArray.Length - 1)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    Console.Write(byteArray[j].ToString("X2") + " ");
                                    if ((j - i + 2) % 16 == 0)
                                    {
                                        Console.WriteLine();
                                    }
                                    else if ((j - i + 2) % 8 == 0)
                                    {
                                        Console.Write(" ");
                                    }
                                }
                            }
                        }
                    }
                }
                Console.Write(byteArray[i].ToString("X2") + " ");
                if ((i + 1) % 16 == 0)
                {
                    Console.WriteLine();
                }
                else if ((i + 1) % 8 == 0)
                {
                    Console.Write(" ");
                }
            }
        }

        public static void OutputString(string str)
        {
            for (int i = 0; i < str.Length; i += 2)
            {
                Console.Write(str[i] + "" + str[i + 1] + " ");
                if ((i + 2) % 32 == 0)
                {
                    Console.WriteLine();
                }
                else if ((i + 2) % 16 == 0)
                {
                    Console.Write(" ");
                }
            }
        }

        public static void OutputList(List<Packet1> Packets, int number)
        {
            Console.WriteLine("DestinationMAC    = " + Packets[number].DestinationMAC.ToString());
            Console.WriteLine("SourceMAC         = " + Packets[number].SourceMAC.ToString());
            Console.WriteLine("EthernetType      = " + Packets[number].EthernetType);
            Console.WriteLine("AppID             = " + Packets[number].AppID);
            Console.WriteLine("Length            = " + Packets[number].Length);
            Console.WriteLine("Reserved1         = " + Packets[number].Reserved1);
            Console.WriteLine("Reserved2         = " + Packets[number].Reserved2);
            Console.WriteLine("GoosePduLength    = " + Packets[number].GoosePduLength);
            Console.WriteLine("GoCBRef           = " + Packets[number].GoCBRef);
            Console.WriteLine("TimeAllowedToLive = " + Packets[number].TimeAllowedToLive);
            Console.WriteLine("DatSet            = " + Packets[number].DatSet);
            Console.WriteLine("GoID              = " + Packets[number].GoID + " " + StringToCharString(Packets[number].GoID));
            Console.WriteLine("Time              = " + Packets[number].Time);
            Console.WriteLine("StNum             = " + Packets[number].StNum);
            Console.WriteLine("SqNum             = " + Packets[number].SqNum);
            Console.WriteLine("Simulation        = " + Packets[number].Simulation);
            Console.WriteLine("ConfRev           = " + Packets[number].ConfRev);
            Console.WriteLine("NdsCom            = " + Packets[number].NdsCom);
            Console.WriteLine("NumDatSetEntries  = " + Packets[number].NumDatSetEntries);
            Console.WriteLine("AllData           = " + Packets[number].AllData);
            Console.WriteLine("PrpProtocol       = " + Packets[number].PrpProtocol);
        }

        public static List<Packet1> FileReader(byte[] bin)
        {
            List<Packet1> Packets = new List<Packet1>(); // create list to contain packets

            BinaryHeader1 binHead = new BinaryHeader1();
            binHead.TagHeader = "";

            for (int i = 0; i < bin.Length; i++) // read all bytes
            {
                //BinaryHeader1 binHead;
                //binHead.TagHeader = "";

                if (bin[i] == 0x30) // search for header tag
                {
                    for (int x = i + 2; x < i + 2 + bin[i + 1]; x++)
                    {
                        binHead.TagHeader += bin[x];
                    }

                    i += bin[i + 1] + 2;

                    if (bin[i] == 0x31) // search for goose amount tag
                    {
                        binHead.TagGooseAmount = bin[i + 1];
                        int packetCounter = -1;

                        for (; i < bin.Length; i++)
                        {
                            if ((bin[i] == 0x01) && (bin[i + 1] == 0x0c) && (bin[i + 2] == 0xcd) && (bin[i + 3] == 0x01)) // search for Destination MAC (goose start)
                            {
                                packetCounter++;
                                Packet1 packet = new Packet1();
                                Packets.Add(packet);

                                packet.DestinationMAC = System.Net.NetworkInformation.PhysicalAddress.Parse(bin[i].ToString("X2") + bin[i + 1].ToString("X2") + bin[i + 2].ToString("X2") + bin[i + 3].ToString("X2") + bin[i + 4].ToString("X2") + bin[i + 5].ToString("X2"));
                                i += 6;

                                packet.SourceMAC = System.Net.NetworkInformation.PhysicalAddress.Parse(bin[i].ToString("X2") + bin[i + 1].ToString("X2") + bin[i + 2].ToString("X2") + bin[i + 3].ToString("X2") + bin[i + 4].ToString("X2") + bin[i + 5].ToString("X2"));
                                i += 6;

                                packet.EthernetType = bin[i].ToString("X2") + bin[i + 1].ToString("X2") + bin[i + 2].ToString("X2") + bin[i + 3].ToString("X2") + bin[i + 4].ToString("X2") + bin[i + 5].ToString("X2");
                                i += 6;

                                packet.AppID = bin[i].ToString("X2") + bin[i + 1].ToString("X2");
                                i += 2;

                                packet.Length = bin[i].ToString("X2") + bin[i + 1].ToString("X2");
                                i += 2;

                                packet.Reserved1 = bin[i].ToString("X2") + bin[i + 1].ToString("X2");
                                i += 2;

                                packet.Reserved2 = bin[i].ToString("X2") + bin[i + 1].ToString("X2");
                                i += 2;

                                if (!(bin[i] == 0x61 && bin[i + 1] == 0x81))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 2;

                                packet.GoosePduLength = bin[i].ToString("X2");
                                i += 1;

                                if (!(bin[i] == 0x80))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.GoCBRef += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x81))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.TimeAllowedToLive += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x82))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.DatSet += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x83))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.GoID += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x84))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.Time += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x85))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.StNum += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x86))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.SqNum += (bin[j] + packetCounter).ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x87))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.Simulation += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x88))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.ConfRev += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x89))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.NdsCom += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0x8a))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.NumDatSetEntries += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                if (!(bin[i] == 0xab))
                                {
                                    Console.WriteLine("Fatal Error");
                                }
                                i += 1;
                                for (int j = i + 1; j < i + 1 + bin[i]; j++)
                                {
                                    packet.AllData += bin[j].ToString("X2");
                                }
                                i += bin[i] + 1;

                                //if (!(bin[i] == 0xab))
                                //{
                                //    Console.WriteLine("Fatal Error");
                                //}
                                //i += 1;
                                //for (int j = i + 1; j < i + 1 + bin[i]; j++) // j is index for AB tag
                                //{
                                //    switch (bin[j])
                                //    {
                                //        case 0x83:
                                //            if (!(bin[j + 1] == 0x01))
                                //            {
                                //                Console.WriteLine("Fatal Error");
                                //            }
                                //            j += 1;
                                //            // do smth
                                //            break;
                                //        case 0x84:
                                //            // do smth
                                //            break;
                                //        case 0x91:
                                //            if (!(bin[j + 1] == 0x08))
                                //            {
                                //                Console.WriteLine("Fatal Error");
                                //            }
                                //            j += 1;
                                //            // do smth
                                //            break;
                                //        case 0xa2:
                                //            for (int k = j + 1; k < j + 1 + bin[j]; k++) // k is index for A2 tag
                                //            {
                                //                switch (bin[k])
                                //                {
                                //                    case 0x83:
                                //                        if (!(bin[k + 1] == 0x01))
                                //                        {
                                //                            Console.WriteLine("Fatal Error");
                                //                        }
                                //                        k += 1;
                                //                        // do smth
                                //                        break;
                                //                    case 0x84:
                                //                        // do smth
                                //                        break;
                                //                    case 0x91:
                                //                        if (!(bin[k + 1] == 0x08))
                                //                        {
                                //                            Console.WriteLine("Fatal Error");
                                //                        }
                                //                        k += 1;
                                //                        // do smth
                                //                        break;
                                //                    default:
                                //                        Console.WriteLine("Fatal Error");
                                //                        break;
                                //                }
                                //            }
                                //            break;
                                //        default:
                                //            Console.WriteLine("Fatal Error");
                                //            break;
                                //    }
                                //}
                                //i += bin[i] + 1;



                                Packets[packetCounter] = packet;
                            }
                            //else
                            //{
                            //    Console.WriteLine(bin[i].ToString("X2") + " " + bin[i + 1].ToString("X2") + " " + bin[i + 2].ToString("X2") + " " + bin[i + 3].ToString("X2"));
                            //}
                        }
                        return Packets;
                    }
                }
                //else
                //{
                //    Console.WriteLine("Fatal Error");
                //    Console.ReadLine();
                //    return Packets; // null;
                //}
            }
            return Packets;
        }

        public static string BuildPacketString(Packet1 packet)
        {
            string data = "";

            data += "800088b8" + packet.AppID + packet.Length + packet.Reserved1 + packet.Reserved2 + "6181" + packet.GoosePduLength + "80" + ((byte)(packet.GoCBRef.Length / 2)).ToString("X2") +
                packet.GoCBRef + "81" + ((byte)((packet.TimeAllowedToLive.Length) / 2)).ToString("X2") + packet.TimeAllowedToLive + "82" + ((byte)((packet.DatSet.Length) / 2)).ToString("X2") +
                    packet.DatSet + "83" + ((byte)((packet.GoID.Length) / 2)).ToString("X2") + packet.GoID + "84" + ((byte)((packet.Time.Length) / 2)).ToString("X2") + packet.Time + "85" + ((byte)((packet.StNum.Length) / 2)).ToString("X2") + packet.StNum + "86" + ((byte)((packet.SqNum.Length) / 2)).ToString("X2") + packet.SqNum + "87" +
                        ((byte)((packet.Simulation.Length) / 2)).ToString("X2") + packet.Simulation + "88" + ((byte)((packet.ConfRev.Length) / 2)).ToString("X2") + packet.ConfRev + "89" +
                            ((byte)((packet.NdsCom.Length) / 2)).ToString("X2") + packet.NdsCom + "8a" + ((byte)((packet.NumDatSetEntries.Length) / 2)).ToString("X2") +
                                packet.NumDatSetEntries + "ab" + ((byte)((packet.AllData.Length) / 2)).ToString("X2") + packet.AllData;

            return data;
        }

        public static byte[] ToByteArray(string HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static void DisplayTypeAndAddress()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            Console.WriteLine("Interface information for {0}.{1}     ",
                    computerProperties.HostName, computerProperties.DomainName);
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                Console.WriteLine(adapter.Description);
                Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
                Console.WriteLine("  Physical Address ........................ : {0}",
                           adapter.GetPhysicalAddress().ToString());
                Console.WriteLine("  Is receive only.......................... : {0}", adapter.IsReceiveOnly);
                Console.WriteLine("  Multicast................................ : {0}", adapter.SupportsMulticast);
                Console.WriteLine();
            }
        }

        public static string GetMacAddress()
        {
            string address;
            try
            {
                address = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
                return address;
            }
            catch
            {
                throw new Exception("No interface found");
            }
        }

        public static string ConvertMacAddress(string address)
        {
            address = address.Insert(10, ":");
            address = address.Insert(8, ":");
            address = address.Insert(6, ":");
            address = address.Insert(4, ":");
            address = address.Insert(2, ":");
            return address;
        }

        public static byte[] ConvertToByteArray(string s)
        {
            int NumberChars = s.Length;
            string buffer;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                buffer = s.Substring(i, 2);
                bytes[i / 2] = Byte.Parse(buffer, System.Globalization.NumberStyles.HexNumber, null);
            }
            return bytes;
        }

        public static string StringToCharString(string s)
        {
            string retString = "";
            for (int i = 0; i < s.Length; i += 2)
            {
                retString += Convert.ToChar(Byte.Parse(s.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, null));
            }
            return retString;
        }

        public static BinaryHeader1 HeaderReader(byte[] bin)
        {
            BinaryHeader1 binHead = new BinaryHeader1();
            binHead.TagHeader = "";

            for (int i = 0; i < bin.Length; i++)
            {
                if (bin[i] == 0x30) // search for header tag
                {
                    for (int x = i + 2; x < i + 2 + bin[i + 1]; x++)
                    {
                        binHead.TagHeader += bin[x];
                    }

                    i += bin[i + 1] + 2;

                    if (bin[i] == 0x31) // search for goose amount tag
                    {
                        binHead.TagGooseAmount = bin[i + 1];
                        return binHead;
                    }
                    else
                    {
                        Console.WriteLine("file is corrupted");
                        return binHead;
                    }
                }
            }
            return binHead;
        }

        public static void OutputData(List<Packet1> Packets, int number)
        {
            for (int i = 0; i < Packets[number].AllData.Length; i += 2)
            {
                switch (Packets[number].AllData.Substring(i, 2))
                {
                    case "83":
                        if (!(Packets[number].AllData.Substring(i + 2, 2) == "01"))
                        {
                            Console.WriteLine("Corrupted data");
                            return;
                        }
                        i += 4;
                        Console.WriteLine("Boolean " + Packets[number].AllData.Substring(i, 2));
                        break;
                    case "84":
                        i += 2;
                        if (Packets[number].AllData.Substring(i, 2) == "03")
                        {
                            Console.WriteLine("Bitstring " + Packets[number].AllData.Substring(i + 2, 6));
                            i += 6;
                        }
                        else if (Packets[number].AllData.Substring(i, 2) == "02")
                        {
                            Console.WriteLine("dbPos " + Packets[number].AllData.Substring(i + 2, 4));
                            i += 4;
                        }
                        else
                        {
                            Console.WriteLine("Corrupted data");
                            return;
                        }
                        break;
                    case "91":
                        if (!(Packets[number].AllData.Substring(i + 2, 2) == "08"))
                        {
                            Console.WriteLine("Corrupted data");
                            return;
                        }
                        i += 4;
                        Console.WriteLine("Timestamp " + Packets[number].AllData.Substring(i, 16));
                        i += 14;
                        break;
                    case "A2":
                        Console.WriteLine("STRUCTURE");
                        for (int k = i + 4; k < i + 4 + 2 * Byte.Parse(Packets[number].AllData.Substring(i + 2, 2), System.Globalization.NumberStyles.HexNumber, null); k += 2) // k is index for A2 tag
                        {
                            switch (Packets[number].AllData.Substring(k, 2))
                            {
                                case "83":
                                    if (!(Packets[number].AllData.Substring(k + 2, 2) == "01"))
                                    {
                                        Console.WriteLine("Corrupted data");
                                        return;
                                    }
                                    k += 4;
                                    Console.WriteLine("    Boolean " + Packets[number].AllData.Substring(k, 2));
                                    break;
                                case "84":
                                    k += 2;
                                    if (Packets[number].AllData.Substring(k, 2) == "03")
                                    {
                                        Console.WriteLine("    Bitstring " + Packets[number].AllData.Substring(k + 2, 6));
                                        k += 6;
                                    }
                                    else if (Packets[number].AllData.Substring(k, 2) == "02")
                                    {
                                        Console.WriteLine("    dbPos " + Packets[number].AllData.Substring(k + 2, 4));
                                        k += 4;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Corrupted data");
                                        return;
                                    }
                                    break;
                                case "91":
                                    if (!(Packets[number].AllData.Substring(k + 2, 2) == "08"))
                                    {
                                        Console.WriteLine("Corrupted data");
                                        return;
                                    }
                                    k += 4;
                                    Console.WriteLine("    Timestamp " + Packets[number].AllData.Substring(k, 16));
                                    k += 14;
                                    break;
                                default:
                                    Console.WriteLine("Corrupted data");
                                    return;
                            }
                        }
                        i += 2 + 2 * Byte.Parse(Packets[number].AllData.Substring(i + 2, 2), System.Globalization.NumberStyles.HexNumber, null);
                        break;
                    default:
                        Console.WriteLine("Corrupted data");
                        return;
                }
                //i += Convert.ToByte(Packets[number].AllData.Substring(i, 1)) + 1;
            }
        }

        public struct BinaryFile
        {
            public string Name;
            public List<Packet1> Packets;
        }

        //public static void ChooseNetwork()
        //{
        //    // Retrieve the device list from the local machine
        //    IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

        //    if (allDevices.Count == 0)
        //    {
        //        MessageBox.Show("No interfaces found! Make sure WinPcap is installed.");
        //        return;
        //    }

        //    // Print the list
        //    string s = "";
        //    for (int i = 0; i != allDevices.Count; ++i)
        //    {
        //        LivePacketDevice device = allDevices[i];
        //        s += ((i + 1) + ". " + device.Name);
        //        if (device.Description != null)
        //            s += (" (" + device.Description + ")") + "\n";
        //        else
        //            s += (" (No description available)") + "\n";
        //    }
        //    //MessageBox.Show(s);
        //    s = "";

        //    int deviceIndex = 0;
        //    do
        //    {
        //        s += ("Enter the interface number (1-" + allDevices.Count + "):") + "\n";
        //        //MessageBox.Show(s);
        //        string deviceIndexString = "1";//Console.ReadLine();
        //        if (!int.TryParse(deviceIndexString, out deviceIndex) ||
        //            deviceIndex < 1 || deviceIndex > allDevices.Count)
        //        {
        //            deviceIndex = 0;
        //        }
        //    } while (deviceIndex == 0);

        //    // Take the selected adapter
        //    PacketDevice selectedDevice = allDevices[deviceIndex - 1];

        //    // Open the output device
        //    using (PacketCommunicator communicator = selectedDevice.Open(100, // name of the device
        //                                                                 PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
        //                                                                 1000)) // read timeout
        //    {
        //        communicator.SendPacket(BuildEthernetPacket(ConvertToByteArray(BuildPacketString(FileReader(bin)[0]))));//BuildPacketString(FileReader(bin)[0])));

        //        // console method
        //        while (true)
        //        {
        //            Console.WriteLine("input .bin file location...");
        //            @path = Console.ReadLine();
        //            try
        //            {
        //                FileStream fs = new FileStream(path, FileMode.Open);
        //                BinaryReader br = new BinaryReader(fs);
        //                //byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
        //                foreach (byte b in bin)
        //                {
        //                    data += b.ToString("X2");
        //                }
        //                fs.Close();
        //                br.Close();

        //                BinaryHeader1 header = new BinaryHeader1();
        //                header = HeaderReader(bin);
        //                List<Packet1> gooseList = new List<Packet1>();
        //                gooseList = FileReader(bin);

        //                Console.WriteLine("there are " + header.TagGooseAmount + " goose in the file");
        //                Console.WriteLine("input goose number from 0 to " + (header.TagGooseAmount - 1));
        //                int gooseNumber = 0;
        //                gooseNumber = Convert.ToInt32(Console.ReadLine());

        //                OutputList(gooseList, gooseNumber);
        //                OutputData(gooseList, gooseNumber);

        //                Console.WriteLine("there are " + header.TagGooseAmount + " goose in the file");
        //            }
        //            catch
        //            {
        //                Console.WriteLine("cant open file, check if path is correct");
        //                continue;
        //            }
        //        }

        //    }
        //}

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            string path = @"D:\gooseout.bin";
            string data = "";

            FileStream fs1 = new FileStream(path, FileMode.Open);
            BinaryReader br1 = new BinaryReader(fs1);
            byte[] bin = br1.ReadBytes(Convert.ToInt32(fs1.Length));
            foreach (byte b in bin)
            {
                data += b.ToString("X2");
            }
            fs1.Close();
            br1.Close();

            //OutputList(FileReader(bin));
            //OutputByteArray(bin);
            //OutputList(FileReader(ToByteArray(BuildPacket1(FileReader(bin)[0]))));
            //Console.WriteLine(BuildPacket1(FileReader(bin)[0]));
            //for (int i = 0; i < (BuildPacket1(FileReader(bin)[0])).Length; i++)
            //{
            //    Console.Write(ToByteArray(BuildPacket1(FileReader(bin)[0]))[i]);
            //}
            //Console.WriteLine(data);
            //OutputString(data);

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            ///                                 Tutorial starts from here                                    ///
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            // Retrieve the device list from the local machine
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            if (allDevices.Count == 0)
            {
                Console.WriteLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                Console.Write((i + 1) + ". " + device.Name);
                if (device.Description != null)
                    Console.WriteLine(" (" + device.Description + ")");
                else
                    Console.WriteLine(" (No description available)");
            }

            int deviceIndex = 0;
            do
            {
                Console.WriteLine("Enter the interface number (1-" + allDevices.Count + "):");
                string deviceIndexString = Console.ReadLine();
                if (!int.TryParse(deviceIndexString, out deviceIndex) ||
                    deviceIndex < 1 || deviceIndex > allDevices.Count)
                {
                    deviceIndex = 0;
                }
            } while (deviceIndex == 0);

            // Take the selected adapter
            PacketDevice selectedDevice = allDevices[deviceIndex - 1];

            // Open the output device
            using (PacketCommunicator communicator = selectedDevice.Open(100, // name of the device
                                                                         PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                                                         1000)) // read timeout
            {
                communicator.SendPacket(BuildEthernetPacket(ConvertToByteArray(BuildPacketString(FileReader(bin)[0]))));//BuildPacketString(FileReader(bin)[0])));

                // console method
                while (true)
                {
                    Console.WriteLine("input .bin file location...");
                    @path = Console.ReadLine();
                    try
                    {
                        FileStream fs = new FileStream(path, FileMode.Open);
                        BinaryReader br = new BinaryReader(fs);
                        //byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                        foreach (byte b in bin)
                        {
                            data += b.ToString("X2");
                        }
                        fs.Close();
                        br.Close();

                        BinaryHeader1 header = new BinaryHeader1();
                        header = HeaderReader(bin);
                        List<Packet1> gooseList = new List<Packet1>();
                        gooseList = FileReader(bin);

                        Console.WriteLine("there are " + header.TagGooseAmount + " goose in the file");
                        Console.WriteLine("input goose number from 0 to " + (header.TagGooseAmount - 1));
                        int gooseNumber = 0;
                        gooseNumber = Convert.ToInt32(Console.ReadLine());

                        OutputList(gooseList, gooseNumber);
                        OutputData(gooseList, gooseNumber);

                        Console.WriteLine("there are " + header.TagGooseAmount + " goose in the file");
                    }
                    catch
                    {
                        Console.WriteLine("cant open file, check if path is correct");
                        continue;
                    }
                }

            }

            Console.WriteLine("End of main");
            Console.ReadLine();
        }
    }
}