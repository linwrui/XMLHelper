﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLHelper.xml;

namespace XMLHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateXmlFile("D:\\TestExportFile\\TestXml.xml");
            LoadXmlString();
            Console.ReadLine();
        }

        private static void LoadXmlFile(string filePath)
        {

        }

        private static void LoadXmlString()
        {
            StringBuilder errorMessageFormatter = new StringBuilder();
            errorMessageFormatter.AppendLine("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            errorMessageFormatter.AppendLine("   <soap:Body>");
            errorMessageFormatter.AppendLine("      <soap:Fault>");
            errorMessageFormatter.AppendLine("         <faultcode>soap:Client</faultcode>");
            errorMessageFormatter.AppendLine("         <faultstring>Error message!</faultstring>");
            errorMessageFormatter.AppendLine("         <detail>Error details</detail>");
            errorMessageFormatter.AppendLine("      </soap:Fault>");
            errorMessageFormatter.AppendLine("   </soap:Body>");
            errorMessageFormatter.AppendLine("</soap:Envelope>");
            using (var xmlOperator = new XmlOperator())
            {
                xmlOperator.Load(errorMessageFormatter.ToString());
                Console.WriteLine(xmlOperator.GetXmlNodeValue("faultstring"));
                Console.WriteLine(xmlOperator.GetXmlNodeValue("detail"));
            }
        }

        private static void CreateXmlFile(string filePath)
        {
            using (var xmlOperator = new XmlOperator())
            {
                xmlOperator.CreateRootElement("XmlSimple","lwr","XmlHelper");
                var booksNode=xmlOperator.AppendNode("Books",
                    new Tuple<string, object>("Category", "Sicence"),
                    new Tuple<string, object>("DateTime", DateTime.Now.ToShortDateString()));

                for (int i = 0; i < 10; i++)
                {
                    var bookNode = xmlOperator.AppendNodeToExitsNode(booksNode, "Book",
                        new Tuple<string, object>("BookName", $"Book_{i}"),
                        new Tuple<string, object>("Author", $"Author_{i}")
                        );
                }
                xmlOperator.SaveAs(filePath);
                xmlOperator.AppendNode("Photos", new Tuple<string, object>("Name", "照片"));
                xmlOperator.Save();
            }
            System.Diagnostics.Process.Start(filePath);
        }
    }
}
