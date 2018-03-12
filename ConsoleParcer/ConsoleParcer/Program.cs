using DbLayer;
using Oracle.ManagedDataAccess.Client;
using Semaphore.Infrastructure.WorkWithFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleParcer
{
    class Program
    {        
        static void Main(string[] args)
        {
            OracleConnect con = new OracleConnect("User ID=import_user;password=sT7hk9Lm;Data Source=CD_WORK");
            con.OpenConnect();
            ReadDataToList(con);
        }

        static void ReadDataToList(OracleConnect con)
        {
            try
            {
                string query = "select x.id, x.project_id, x.data " +
                                 "from SUVD.PROJECTS t, suvd.creditor_dogovors d, suvd.project_xml x " +
                                "where d.id = t.dogovor_id " +
                                  "and t.archive_flag = 0" +
                                  "and nvl(d.stop_date, sysdate) >= sysdate " +
                                  "and x.project_id = t.id " +
                                  "and x.data is not null";
                OracleDataReader reader = con.GetReader(query);

                DateTime startDate = DateTime.Now;
                DateTime lastDate = DateTime.Now;
                Console.WriteLine("Старт. " + startDate);
                int count = 0;
                int bitStart = 0;
                while (reader.Read() && count < 100)
                {
                    count++;
                    //if (count < 77888) continue; 
                    XmlData xmlData = new XmlData();
                    xmlData.Id = Convert.ToDecimal(reader[0].ToString());
                    xmlData.ProjectId = Convert.ToDecimal(reader[1].ToString());
                    xmlData.Data = reader[2].ToString();
                    Parser(xmlData, con, count);
                    if (count % 25000 == 0)
                    {
                        double tmp = (DateTime.Now - lastDate).TotalMinutes;
                        Console.WriteLine("c " + bitStart.ToString() + " по " + count.ToString() + " залилось за " + tmp.ToString() + " минут.");
                        bitStart = count;
                        lastDate = DateTime.Now;
                        Console.WriteLine();
                    }
                    //Console.WriteLine(count.ToString());
                    FileHandler.WriteToFile(count.ToString());
                }
                reader.Close();
                DateTime endDate = DateTime.Now;
                double dif = (endDate - startDate).TotalMinutes;
                Console.WriteLine("Закончено за " + dif + " минут." + Environment.NewLine + "Распарсено  " + count + " записей.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.TargetSite + "\n" + ex.Message);
                FileHandler.WriteToFile(ex.TargetSite + "\n" + ex.Message);
            }            
        }

        static void Parser(XmlData item, OracleConnect con, int count)
        {
            try
            {
                if (item.Data == null || item.Data.Length < 12)
                {
                    return;
                }
                string str = item.Data.Trim();
                if (!str.Contains("</xml>"))
                {
                    str = str + (Environment.NewLine + "</xml>");
                }
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);

                List<string> list = new List<string>();
               
                XmlNodeList outNodes = doc.DocumentElement.ChildNodes;
                foreach (XmlNode node in outNodes)
                {
                    int num = 0;
                    string outNodeName = "";
                    string outNodeAttr = "";
                    XmlAttributeCollection attrsOut = node.Attributes;
                    
                    foreach (XmlAttribute attOut in attrsOut)
                    {                        
                        list.Add(node.Name);
                        outNodeName = node.Name;
                        list.Add(attOut.Value);
                        outNodeAttr = attOut.Value;
                    }
                    XmlNodeList inNodes = node.ChildNodes;
                    foreach (XmlNode nodeIn in inNodes)
                    {
                        
                        XmlAttributeCollection attrsIn = nodeIn.Attributes;
                        foreach (XmlAttribute attIn in attrsIn)
                        {
                            num++;
                            list.Add(nodeIn.Name);
                            list.Add(attIn.Value);
                            list.Add(nodeIn.InnerText);
                            Record rec = new Record
                            {
                                ProjectID = item.ProjectId,
                                IdInXml = item.Id,
                                BlockName = outNodeAttr,
                                BlockVariableName = outNodeName,
                                ItemName = attIn.Value,
                                ItemVariableName = nodeIn.Name,
                                ItemValue = nodeIn.InnerText,
                                Num = num
                            };
                            rec.BlockName = rec.BlockName.Replace("'", "`");
                            rec.BlockVariableName = rec.BlockVariableName.Replace("'", "`");
                            rec.ItemName = rec.ItemName.Replace("'", "`");
                            rec.ItemVariableName = rec.ItemVariableName.Replace("'", "`");
                            rec.ItemValue = rec.ItemValue.Replace("'", "`");
                            InsertToDB(con, rec, count);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.TargetSite + "\n" + ex.Message);
                FileHandler.WriteToFile(ex.TargetSite + "\n" + ex.Message);
            }            
        }

        private static void InsertToDB(OracleConnect con, Record rec, int count)
        {
            try
            {
                string query = "INSERT INTO SUVD.XML_DATA (ID, PROJECT_ID, ID_IN_XML, BLOCK_TITLE, BLOCK_NAME, ITEM_TITLE, ITEM_NAME, ITEM_VALUE, NUM)" +
                                         " VALUES(XML_SEQUENCE.NEXTVAL, " +
                                                rec.ProjectID + ", " +
                                                rec.IdInXml + ", '" +
                                                rec.BlockName + "', '" +
                                                rec.BlockVariableName + "', '" +
                                                rec.ItemName + "', '" +
                                                rec.ItemVariableName + "', '" +
                                                rec.ItemValue + "', '" +
                                                rec.Num + "')";
                con.ExecCommand(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.TargetSite + "\n" + ex.Message);
                FileHandler.WriteToFile(ex.TargetSite + "\n" + ex.Message);
            }
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
