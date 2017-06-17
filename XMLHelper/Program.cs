//*Author:linwrui
//*DateTime:2017-06-10

using System;
using System.Text;
using XMLHelper.xml;

namespace XMLHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateXmlFile("D:\\TestExportFile\\TestXml.xml");
            //LoadXmlFile("D:\\TestExportFile\\TestXml.xml");
           // LoadXmlString();
            TestCreateXmlString();
            Console.ReadLine();
        }

        private static void LoadXmlFile(string filePath)
        {
            using (var xmlOperator = new XmlOperator(filePath))
            {
                //Console.WriteLine(xmlOperator.GetNodeAttributeValue("Books","Category"));
                //Console.WriteLine(xmlOperator.GetXmlNodeValue("detail"));
                var node=xmlOperator.GetXmlNode(x => x.Name == "Books");
                Console.WriteLine(node.OuterXml);
            }
        }

        private static void TestCreateXmlString()
        {
            using (var xmloper = new XmlOperator())
            {
                xmloper.CreateRootElement("Envelope", "soap", "http://schemas.xmlsoap.org/soap/envelope/");
                var bodyNode= xmloper.RootNode.AppendChild(xmloper.CreateNode("Body", true));
                var faultNode = bodyNode.AppendChild(xmloper.CreateNode("Fault", true));
                var faultCodeNode = faultNode.AppendChild(xmloper.CreateNode("faultcode"));
                var faultStringNode = faultNode.AppendChild(xmloper.CreateNode("faultstring"));
                var detailNode = faultNode.AppendChild(xmloper.CreateNode("detail"));
                faultCodeNode.AppendChild(xmloper.XmlDoc.CreateTextNode("soap:Client"));
                faultStringNode.AppendChild(xmloper.XmlDoc.CreateTextNode("Error message."));
                detailNode.AppendChild(xmloper.XmlDoc.CreateTextNode("Error details."));
                xmloper.SaveAs("D:\\TestExportFile\\TestXml.xml", true);
            }
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
                xmlOperator.LoadFromString(errorMessageFormatter.ToString());
                var node = xmlOperator.GetXmlNode(x => x.Name == "faultcode");
                Console.WriteLine(xmlOperator.GetXmlNode(x=>x.Name=="faultstring")?.FirstChild.Value);
                Console.WriteLine(xmlOperator.GetXmlNode(x=>x.Name=="detail")?.FirstChild.Value);
                xmlOperator.SaveAs("D:\\TestExportFile\\TestXml.xml",true);
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
