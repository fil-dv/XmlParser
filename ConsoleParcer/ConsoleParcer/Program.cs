using DbLayer;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleParcer
{
    class Program
    {        
        static void Main(string[] args)
        {
            OracleConnect con = new OracleConnect("User ID=import_user;password=sT7hk9Lm;Data Source=CD_WORK");
            con.OpenConnect();

            //       for write to file       //
            //List<XmlData> dataList = new List<XmlData>();
            ReadDataToList(con/*, ref dataList*/);
            //WriteToBinaryFile("XML_test.bin", dataList);

            //       for read from file       //
            //  List<XmlData> dataList = ReadFromBinaryFile<List<XmlData>>("XML_test.bin");

        }

        static void Parser(XmlData item)
        {
            string str = item.Data;

        }

        static void ReadDataToList(OracleConnect con/*, ref List<XmlData> dataList*/)
        {
            string query = "select t.id, t.project_id, t.data from SUVD.PROJECT_XML t";
            OracleDataReader reader = con.GetReader(query);
           // Console.WriteLine("Старт. " + DateTime.Now);
            int limit = 0;
            while (reader.Read() && limit < 1)
            {
                limit++;
          //      if (limit < 1500000) continue;
                XmlData xmlData = new XmlData();
                xmlData.Id = Convert.ToDecimal(reader[0].ToString());
                xmlData.ProjectId = Convert.ToDecimal(reader[1].ToString());
                xmlData.Data = reader[2].ToString();

                Parser(xmlData);

                //dataList.Add(xmlData);                
                //Console.WriteLine(limit.ToString());
            }
            reader.Close();
            //Console.WriteLine("Готово. Прочитано " + dataList.Count + " записей.");
            //Console.WriteLine(DateTime.Now);
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<XmlData>(string filePath, XmlData objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
          //  Console.WriteLine("Запись в файл закончена. " + DateTime.Now);
          //  Console.ReadKey();
        }


        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static XmlData ReadFromBinaryFile<XmlData>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (XmlData)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
