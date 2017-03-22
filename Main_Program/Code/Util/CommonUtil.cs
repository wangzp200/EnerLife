using System;
using System.IO;
using System.Text;
using SAPbouiCOM;

namespace EnerlifeCN.Code.Util
{
    public class CommonUtil
    {
        public static int GetMaxLineId(DBDataSource lsDbs, string lsEntry)
        {
            if (string.IsNullOrEmpty(lsEntry))
            {
                lsEntry = "-1";
            }
            var lssql = "SELECT ISNULL(MAX(t10.LineId), 0) AS lineid  FROM ( SELECT * FROM [@A" +
                        lsDbs.TableName.Remove(0, 1) + "]" +
                        " WHERE DocEntry = " + lsEntry +
                        " UNION ALL " +
                        " SELECT * FROM [@" + lsDbs.TableName.Remove(0, 1) + "]  WHERE DocEntry = " + lsEntry +
                        " ) t10";
            Global.ORecordSet.DoQuery(lssql);
            var liMaxLine = 0;
            while (Global.ORecordSet.EoF == false)
            {
                liMaxLine = (int) Global.ORecordSet.Fields.Item(0).Value;
                for (var i = 0; i < lsDbs.Size; i++)
                {
                    if (!string.IsNullOrEmpty(lsDbs.GetValue("LineId", i)))
                    {
                        if (liMaxLine < int.Parse(lsDbs.GetValue("LineId", i)))
                        {
                            liMaxLine = int.Parse(lsDbs.GetValue("LineId", i));
                        }
                    }
                }
                Global.ORecordSet.MoveNext();
            }
            return liMaxLine;
        }

        public static void MtxAddRow(Matrix mtx, DBDataSource db, bool flg)
        {
            mtx.FlushToDataSource();
            if (!flg)
                db.InsertRecord(db.Size);
            db.SetValue("LineId", db.Size - 1, (GetMaxLineId(db, db.GetValue("DocEntry", 0)) + 1).ToString());
            mtx.LoadFromDataSource();
        }

        public static void SeriesValidValues(ValidValues validValues, Form oForm)
        {
            while (validValues.Count > 0)
            {
                validValues.Remove(0, BoSearchKey.psk_Index);
            }
            var lsObjectCode = oForm.TypeEx;
            var sql =
                "SELECT CAST(Series AS NVARCHAR(10)),SeriesName FROM dbo.NNM1 WHERE  Locked='N' AND ObjectCode='" +
                lsObjectCode + "'";
            Global.ORecordSet.DoQuery(sql);
            while (Global.ORecordSet.EoF == false)
            {
                var key = Global.ORecordSet.Fields.Item(0).Value as string;
                var value = Global.ORecordSet.Fields.Item(1).Value as string;
                validValues.Add(key, value);
                Global.ORecordSet.MoveNext();
            }
        }

        public static void DeleteFolder(string dir)
        {
            foreach (var entry in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(entry))
                {
                    var fi = new FileInfo(entry);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(entry);
                }
                else
                {
                    var directoryInfo = new DirectoryInfo(entry);
                    if (directoryInfo.GetFiles().Length != 0)
                    {
                        DeleteFolder(directoryInfo.FullName);
                    }
                    Directory.Delete(entry);
                }
            }
        }

        public static void SaveAsFile(string content, string path)
        {
            var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            var streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(content);
            streamWriter.Close();
            fileStream.Close();
        }

        public static string ReadText(string path)
        {
            var content = string.Empty;
            if (File.Exists(path))
            {
                var sr = new StreamReader(path, Encoding.UTF8);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    content = content + line;
                }
                sr.Close();
            }
            return content;
        }
    }
}