using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace PhoneBook
{
	public class XMLValidator
	{
		// Variables declarations 
		private string m_sXMLFileName ;
		private string m_sSchemaFileName ;
		private XmlSchemaCollection m_objXmlSchemaCollection ;
		private bool m_bIsFailure=false ;

		public XMLValidator()
		{
			
		}
        public XMLValidator(string sXMLFileName, string sSchemaFileName)
        {
            try
            {
                m_sXMLFileName = sXMLFileName;
                m_sSchemaFileName = sSchemaFileName;
                m_objXmlSchemaCollection = new XmlSchemaCollection();
                //adding the schema file to the newly created schema collection
                m_objXmlSchemaCollection.Add(null, m_sSchemaFileName);
            }
            catch (Exception io) {  //io
            }
		} 
		public bool ValidateXMLFile()
		{
			XmlTextReader objXmlTextReader =null;
			XmlValidatingReader objXmlValidatingReader=null ;

            try
            {
                //creating a text reader for the XML file already picked by the 
                //overloaded constructor above viz..XMLValidator
                objXmlTextReader = new XmlTextReader(m_sXMLFileName);
                //creating a validating reader for that objXmlTextReader just created
                objXmlValidatingReader = new XmlValidatingReader(objXmlTextReader);
                //For validation we are adding the schema collection in 
                //ValidatingReaders Schema collection.
                objXmlValidatingReader.Schemas.Add(m_objXmlSchemaCollection);
                //Attaching the event handler now in case of failures
                objXmlValidatingReader.ValidationEventHandler += new ValidationEventHandler
                    (ValidationFailed);
                //Actually validating the data in the XML file with a empty while.
                //which would fire the event ValidationEventHandler and invoke 
                //our ValidationFailed function
                while (objXmlValidatingReader.Read())
                {

                }
                //Note:- If any issue is faced in the above while it will invoke ValidationFailed
                //which will in turn set the module level m_bIsFailure boolean variable to false
                //thus returning true as a signal to the calling function that the ValidateXMLFile 
                //function(this function) has encountered failure
                return m_bIsFailure;
            }
            catch (IOException io)
            {
                System.Windows.MessageBox.Show("There is no XML File already, It is OK. Start filling yours now!");
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Exception : " + ex.Message);
                return true;
            }
			finally 
			{
				// close the readers, no matter what.
				objXmlValidatingReader.Close (); 
				objXmlTextReader.Close ();
			}
		} 
		private void ValidationFailed (object sender, ValidationEventArgs args)
		{
			m_bIsFailure = true;
            System.Windows.MessageBox.Show("Invalid XML File: " + args.Message);
		} 
	}
}
