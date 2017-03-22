using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace EnerlifeCN.Code.Util
{
    /**/

    /// <summary>
    ///     XML形式的字符串、XML文江转换成DataSet、DataTable格式
    /// </summary>
    public class XmlAndTdHelper
    {
        private static XmlAndTdHelper _instance;

        private XmlAndTdHelper()
        {
        }

        public static XmlAndTdHelper GetInstance()
        {
            if (_instance == null)
            {
                _instance = new XmlAndTdHelper();
            }
            return _instance;
        }

        public DataTable XmlToDataTable(string strXmlPath)
        {
            XmlTextReader reader = null;
            StreamReader sr = null;
            try
            {
                if (strXmlPath.Length <= 0)
                {
                    return new DataTable();
                }
                sr = new StreamReader(strXmlPath);
                var strXmlContent = sr.ReadToEnd();
                var stream = new StringReader(strXmlContent);
                reader = new XmlTextReader(stream);
                var ds = new DataSet();
                ds.ReadXml(reader);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                // MessageBox.Show(vErr.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (reader != null)
                    reader.Close();
            }
            return new DataTable();
        }

        public bool DataTableToXml(DataTable dtTable, string strXmlPath)
        {
            XmlTextWriter writer = null;
            StreamWriter sw = null;
            try
            {
                var stream = new MemoryStream();
                writer = new XmlTextWriter(stream, Encoding.UTF8);
                dtTable.WriteXml(writer, XmlWriteMode.WriteSchema);
                var nCount = (int) stream.Length;
                var arr = new byte[nCount];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, nCount);
                var utf = new UTF8Encoding();
                var strContent = utf.GetString(arr).Trim();
                sw = new StreamWriter(strXmlPath);
                sw.Write(strContent);

                return true;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(vErr.Message);

                return false;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
    }
}