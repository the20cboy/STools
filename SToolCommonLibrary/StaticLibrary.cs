using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace SToolCommonLibrary
{
    public class XmlControl
    {
        public static void Serialize(string filePath, Type type, object obj)
        {
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            if (fs != null)
            {
                XmlSerializer serializer = new XmlSerializer(type);

                try
                {
                    serializer.Serialize(fs, obj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        public static Object DeSerialize(string filePath, Type type)
        {
            XmlTextReader xmlReader = null;
            XmlSerializer xmlSerializer = null;
            Object obj = null;

            try
            {
                xmlReader = new XmlTextReader(filePath);
                xmlSerializer = new XmlSerializer(type);

                obj = xmlSerializer.Deserialize(xmlReader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                xmlReader.Close();
            }

            return obj;
        }
    }

    public class DataConverter
    {
        public static string StringArrayToString(string[] array)
        {
            string temp = string.Empty;
            foreach (string str in array)
            {
                temp += str;
                temp += ",";
            }

            return temp.TrimEnd(new char[] { ',' });
        }

        public static List<ushort> StringListToUshortList(List<string> strList)
        {
            List<ushort> reportList = new List<ushort>();
            foreach (string id in strList)
            {
                ushort rptid = 0;
                if (ushort.TryParse(id, out rptid))
                {
                    reportList.Add(rptid);
                }
            }

            return reportList;
        }

        public static string GetSystemBytes(UInt32 id)
        {
            byte[] systembytes = BitConverter.GetBytes(id);
            Array.Reverse(systembytes);
            return BitConverter.ToString(systembytes);
        }
    }
}
