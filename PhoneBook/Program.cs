using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;


namespace PhoneBook
{   
    class Program
    {
       
     //Class Program which has the helper and semi helper functions used in the whole application 
        public void WriteXMLFileUsingValues(String name, String address, String phoneno, String XMLFile)
        {
            try
            {
                //Check if file does not exist create a new one, and if it does exist just append the data to the already existing one
                if (File.Exists(XMLFile) == false)
                {

                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();   //XMLWriter with settings to make the "new line on attribute" property false
                    xmlWriterSettings.Indent = true;
                    xmlWriterSettings.NewLineOnAttributes = false;
                    using (XmlWriter xmlWriter = XmlWriter.Create(XMLFile, xmlWriterSettings)) //Using the abstract XMLWriter class to write data
                    {
                        //Write document with enteries after being validated with starting id 1
                        //Demostrating XMLWriter use 
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("PhoneBookEntries");
                        xmlWriter.WriteStartElement("PhoneBookEntry");
                        xmlWriter.WriteAttributeString("EntryID", "1");
                        xmlWriter.WriteElementString("Name", name);
                        xmlWriter.WriteElementString("Address", address);
                        xmlWriter.WriteElementString("PhoneNo", phoneno);
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                        xmlWriter.Close();
                    }
                   
                }
                else
                {
                    //Start inserting at the after the last row element of the XML file
                    IEnumerable<XElement> rows = MainWindow.xDocument.Root.Descendants("PhoneBookEntry");
                    XElement lastRow = rows.Last();
                    lastRow.AddAfterSelf(
                       new XElement("PhoneBookEntry",
                       new XAttribute("EntryID", PhoneBook.Properties.Settings.Default.XMLCounter+1),
                       new XElement("Name", name),
                       new XElement("Address", address),
                       new XElement("PhoneNo", phoneno)));
                    MainWindow.xDocument.Save(XMLFile); //Save the file
                    PhoneBook.Properties.Settings.Default.XMLCounter += 1; //Update settings with the last count reached and save it 
                    PhoneBook.Properties.Settings.Default.Save();
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("WriteXMLFileUsingValues: Writing to an XML File Error");
            }

        }

        public void WriteToTextFile(String XMLFile, String TextFileName)
        {
            try

            {

                /*LINQ to XML  is better than XMLDocument regarding code maintainability but is slower than xmldocument which has xpath approach which good for security as well, but in small documents like one the LINQ to XML's XDocument outperforms all.
                Also LINQ to XML is supposed to use XmlReaders "under cover" by calling XDocument.Load does read the whole document into memory before returning. So if we are looking for data at the top of middle of a very large document this could be a concern.  */
               
                //Creating a new XDocument to have the sorted entries
                XDocument xNewDoc = new XDocument();
                
                //Add the roots in there
                xNewDoc.Add(MainWindow.xDocument.Root);

                //Remove the nodes
                xNewDoc.Root.RemoveNodes();

                //Add the Elements of the old unsorted files after sorting it by name alphabitically
                xNewDoc.Root.Add(MainWindow.xDocument.Root.Elements().OrderBy(e => e.Element("Name").Value));
                //String builder to build the output with each entry on a new line in the form of ex: EntryID, Name, Address, PhoneNo (Special characters has been disabeld from entries while doing form validation to avoid overflows and errors)
                StringBuilder sortedPhoneBookEntriesSB = new StringBuilder();

                //Using LINQ is making things easier
                var phonebookentries = from phonebookentry in xNewDoc.Descendants("PhoneBookEntry")
                select new
                {
                  EntryID = phonebookentry.Attribute("EntryID").Value,
                  Name = phonebookentry.Element("Name").Value,
                  Address = phonebookentry.Element("Address").Value,
                  PhoneNo = phonebookentry.Element("PhoneNo").Value,
                 };

                //C# Strong foreach
                foreach (var phonebookentry in phonebookentries)
                {  

                    sortedPhoneBookEntriesSB.AppendLine(phonebookentry.EntryID + "," + phonebookentry.Name + "," + phonebookentry.Address + "," + phonebookentry.PhoneNo);
                }

                //Manage if text file already exists, delete it and create a new one
                try{
                    if (File.Exists(TextFileName))
                    {
                        File.Delete(TextFileName);
                    }
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(TextFileName, true))
                    {
                        file.Write(sortedPhoneBookEntriesSB);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is IOException)
                    {
                        System.Windows.MessageBox.Show("WriteToTextFile: File IO Error");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("WriteToTextFile Exception : " + ex.Message);
                    }
                }


            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("WriteToTextFile Error");
            }
        }


        //isPhone Number Unique which is used for form validation
        public bool isPhoneNoUnique(String search)
        {

            if (isElementUnique(search, "PhoneBookEntry", "PhoneNo"))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        // isElement Unique Helper Function
        public bool isElementUnique(String search,String Decendands,String Element)
        {

            if (File.Exists(MainWindow.XMLFile) && MainWindow.validXMLFlag==1) //Read Notes in MainWindow Regarding the flag
            {

                IEnumerable<XElement> rows = MainWindow.xDocument.Root.Descendants(Decendands);
                foreach (XElement element in rows.Elements(Element))
                {
                    if (element.Value.Equals(search))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //isDigit Helper function
        public bool isDigit(String input)
        {
            if (input.All(char.IsDigit))
            { //Digit in order to make sure that it is single digits for example 0.5 would not pass the validation, if we used char.IsNumber also not to say 1E3 as yes
                return true;
            }
            else
            {
                return false;
            }
        }

        //isNumeric Helper function
        public bool isNumeric(String input)
        {
            if (input.Any(char.IsNumber))
            { 
                return true;
            }
            else
            {
                return false;
            }
        }

        //hasSpecialChrs Helper function
        public bool hasSpecialChrs(String input)
        {
            Regex rgx = new Regex("[^A-Za-z0-9() ]");

            if (rgx.IsMatch(input))
            {
                return true; 
            }
            else
            {
                return false;
            }
        }

        //validatePhoneNo
        public bool validatePhoneNo(String input)
        {
            if (!string.IsNullOrEmpty(input) && input.Length == 9 && isDigit(input) && isPhoneNoUnique(input)) //validate to a swiss number without 0 so 766683551 && unique phone No
            { 
                return true;
            }
            else
            {
                return false;
            }
        }

        //validateName
        public bool validateName(String input)
        {
            if (!string.IsNullOrEmpty(input) && !hasSpecialChrs(input) && !isNumeric(input) && !isDigit(input) && input.Length <= 50) //validate to a text without special characters and no numbers in names  and length less than or equal 50 chrs
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //validateAddress
        public bool validateAddress(String input)
        {
            if (!string.IsNullOrEmpty(input) && !hasSpecialChrs(input) && input.Length<=50) //validate to a text without special characters and length less than or equal 50 chrs
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //validateForm
        public bool validateForm(String name, String address, String phoneno)
        {
            if (validatePhoneNo(phoneno) && validateName(name) && validateAddress(address)) //validate form
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //isValidXML File which cals the helper class XML Validator
        public bool isValidXML(String XMLFile, String XSDFile) {
        XMLValidator objclsSValidator = new XMLValidator(XMLFile, XSDFile);
            if (!objclsSValidator.ValidateXMLFile())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //getElementFromXMLFileConditioned Helper function
        public String getElementFromXMLFileConditioned(String getElement, String Descendants, String searchElement, String key)
        {
            //LINQ is making things easier here as well
            //Ex: Get the name from the decendands of phonebookentry when the phoneno = key
            var resultname = (from c in MainWindow.xDocument.Descendants(Descendants)
                              where ((String)c.Element(searchElement)).Equals(key)
                              select (String)c.Element(getElement)).FirstOrDefault().Trim();
             return resultname;

        }

    }
}
