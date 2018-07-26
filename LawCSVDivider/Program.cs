using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace LawCSVDivider
{
    public class CompanyViolations
    {
        public CompanyViolations()
        {
            this.ViolationType = new List<LawInfo>();
        }
        public int ID { get; set; }
        public string CompanyState { get; set; }
        public string AnnounceDate { get; set; }
        public string CompanyName { get; set; }
        public string Date { get; set; }
        public string ViolateID { get; set; }
        public string ViolateTypes { get; set; }
        public string Description { get; set; }
        public string Annotation { get; set; }      
        public List<LawInfo> ViolationType { get; set; }

    }   
    public class LawInfo
    {
        public LawInfo() { }
        public string Law { get; set; }
        public string LawLine { get; set; }
        public string LawElephant { get; set; }
    }
    public class CompanyViolationsMap : ClassMap<CompanyViolations>
    {
        public CompanyViolationsMap()
        {
            Map(m => m.ID).Name("編號").Index(0);
            Map(m => m.CompanyState).Name("縣市別").Index(1);
            Map(m => m.AnnounceDate).Name("公告日期").Index(2);
            Map(m => m.CompanyName).Name("公司名稱(負責人) / 自然人姓名").Index(3);
            Map(m => m.Date).Name("處分日期").Index(4);
            Map(m => m.ViolateID).Name("處分字號").Index(5);
            Map(m => m.ViolateTypes).Name("違反法規條款").Index(6);
            Map(m => m.Description).Name("法條敘述").Index(7);
            Map(m => m.Annotation).Name("備註").Index(8);
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            string path = @"C:\Users\wish\Desktop\CSVTP.csv";
            List<CompanyViolations> list = new List<CompanyViolations>();
            using (var reader = new StreamReader(path))
            {                
                reader.ReadLine();                
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.RegisterClassMap<CompanyViolationsMap>();
                    var record = csv.GetRecords<CompanyViolations>();
                    //CsvHelper.BadDataException;
                    csv.Configuration.BadDataFound = null;
                    List<CompanyViolations> result = record.ToList();
                    foreach(CompanyViolations element in result)
                    {
                        string[] types = element.ViolateTypes.Replace("\n", String.Empty).Split(';');
                        foreach(string substring in types)
                        {
                            LawInfo newinfo = new LawInfo() { };
                            try  //政府有些檔案少打關鍵字
                            {
                                string[] info = substring.Split('第');
                                newinfo.Law = info[0];
                                newinfo.LawLine = '第' + info[1];
                                if (info.Length >= 3)
                                    newinfo.LawElephant = '第' + info[2];
                                element.ViolationType.Add(newinfo);
                            }
                            catch
                            {

                            }
                        }
                        list.Add(element);
                    }
                }              
            }

            using (var textWriter = new StreamWriter(@"C:\Users\wish\Desktop\Law.csv",false,Encoding.UTF8))
            {
                var csv = new CsvWriter(textWriter);
                foreach (CompanyViolations record in list)
                {
                    csv.WriteRecord(record);
                    foreach (LawInfo law in record.ViolationType)
                    {
                        csv.WriteRecord(law);
                    }
                    csv.NextRecord();
                }
            }
        }
    }
}
