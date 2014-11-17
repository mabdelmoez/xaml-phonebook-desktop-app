using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace PhoneBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public  partial class MainWindow : Window
    {

        private Program programInstance = new Program(); //(protected by being private) to insure clean code on instances and as well as no code conflict
        private static string XSDFile = "PhoneBookEntries.xsd"; //XSD File Name (protected by being private)
        FileWatcher fileWatcherInstanceXSD = new FileWatcher(XSDFile); //(protected by being private)
        FileWatcher fileWatcherInstanceXML = new FileWatcher(XMLFile); //(protected by being private)


        //Files must lie in the same Application Folder if it is going to be used like this, Please feel free to change the variables (Path) and recompile and it would be updated every where but it is reccomended to use URI with Paths
        //Variables only declared at one place to be easy to use
        public static string XMLFile = "PhoneBookEntries.xml"; //XML File Name
        public static string TextFileName = "PhoneBookEntries.txt"; //Text File Name
        public static XElement XElementLIST;
        public static XDocument xDocument;
        public static int validXMLFlag = 0; // It is used to check if the XML Valid one to start inserting elements on updating the UI after form validation is done
        /*validXMLFlag might seem like it could be bypassed but given the fact that the program checks for uniqueness on each load, edit and change operation as well as there is an event to trigger external edits(which would be the main portal of hacks) in XMLFile to bypass data and non unique values, but the SW it is not tolerent with that, in addition revalidation is all through the app, on Load, on edits ..etc so this apporach would be better than the md5 apporach which is is gonna be an expensive operation to read all the files and calculate md5, and better than LastEditDate solution of XMLFile which could be hacked as well*/

        //public MainWindow Constructor to initiate the UI with its actions
        public MainWindow()
        {
            InitializeComponent();
            //Register Window Loaded Event
            Loaded += MailWindow_Loaded;
            //Bind DataContext with the PhoneBookEntry ViewModel in order to activate the threwed exceptions of for validations   
            PhoneBookEntries instancePhoneBookEntries = new PhoneBookEntries();
            this.DataContext = instancePhoneBookEntries;
        }

        private void MailWindow_Loaded(object sender, RoutedEventArgs e)
        {

            


            try
            {

                if (!File.Exists(XSDFile))
                {
                    MessageBoxResult result = MessageBox.Show("XSD file does not exist, Program will close, please locate it and add it to the directory", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK)
                    {
                        Application.Current.Shutdown();

                    }

                }

                else if (File.Exists(XMLFile) && !programInstance.isValidXML(XMLFile, XSDFile) )
                {
                    this.btnSaveXML.IsEnabled = false;
                    this.btnDisplay.IsEnabled = false;
                    this.btnExportToFile.IsEnabled = false;
                    this.btnErase.IsEnabled = true;
                    MessageBoxResult result = MessageBox.Show("XML File is not valid, Do you want to erase it ? \n Please note that if you click NO the application will be closed", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Yes)
                    {
                        eraseXMLFile();

                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }

                }
                else if (File.Exists(XMLFile) && programInstance.isValidXML(XMLFile, XSDFile))
                {
                    //Stream loading the document for faster access than XMLDocument
                    //Demonstration of an approach which might be a good one to let the document be loaded into the server only once, specially when we are not using XMLReader (which is obviously faster specially in UTF8 Endoding and large files) but XDocument(Linq to XML) still faster than XMLDocument and easier, maintainable code, So better performance and better code, in addition this is a small document and XDocument(LINQ to XML) outperforms all approaches even XMLReader on small documents
                    xDocument = XDocument.Load(XMLFile);
                    XElementLIST = XElement.Load(XMLFile);
                    PhoneBook.Properties.Settings.Default.XMLCounter = XElementLIST.Descendants("PhoneBookEntry").Count();
                    PhoneBook.Properties.Settings.Default.Save();
                    this.btnErase.IsEnabled = true;
                    this.btnDisplay.IsEnabled = true;
                    this.btnExportToFile.IsEnabled = true;
                    validXMLFlag = 1;
                }
                
                
            }
            catch (Exception) { 
                System.Windows.MessageBox.Show("MainWindow_Loaded"); 
            }
           
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)  //btnSaveXML
        {

            fileWatcherInstanceXML.EnableRaisingEvents = false; //Disable OnChange event of the XMLFile from being triggered as it is an inside operation
            programInstance.WriteXMLFileUsingValues(txtName.Text, txtAddress.Text, txtPhoneNo.Text, XMLFile);
            this.btnErase.IsEnabled = true;
            this.btnDisplay.IsEnabled = true;
            this.btnExportToFile.IsEnabled = true;
            //Clear values after successfully being added to prevent any in case error even though it would be handeled
            txtAddress.Clear();
            txtName.Clear();
            txtPhoneNo.Clear(); 
            xDocument = XDocument.Load(XMLFile);  //Update XML Streams
            XElementLIST = XElement.Load(XMLFile); //Update XML Streams
            fileWatcherInstanceXML.EnableRaisingEvents = true; //Enable OnChange event of the XMLFile once once after the operation is done
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (File.Exists(XSDFile) && programInstance.isValidXML(XMLFile, XSDFile))
            {
                programInstance.WriteToTextFile(XMLFile, TextFileName);
            }
            else {
                System.Windows.MessageBox.Show("Error in XSD File before writing to the text file"); 
            }
        }

        //Revalidate form in order to enable "Write to XML file button" or not to insert into the XMLFile a new entry
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.btnSaveXML.IsEnabled = programInstance.validateForm(this.txtName.Text, this.txtAddress.Text, this.txtPhoneNo.Text);
        }

        private void txtPhoneNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.btnSaveXML.IsEnabled = programInstance.validateForm(this.txtName.Text, this.txtAddress.Text, this.txtPhoneNo.Text);
        }

        private void txtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.btnSaveXML.IsEnabled = programInstance.validateForm(this.txtName.Text, this.txtAddress.Text, this.txtPhoneNo.Text);
        }

        private void txtBoxXMLList_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnDisplay_Click(object sender, RoutedEventArgs e)
        {
            //XLinq : Put feed the grid from the list
            try
            {
                    this.XMLDataGrid.DataContext = XElementLIST;
                    this.XMLDataGrid.IsReadOnly = true; //Read only to prevent any manipualation
            }
            catch (Exception) { 
                System.Windows.MessageBox.Show("Display XML Data: Error feeding the XML grid with data"); 
            }
            
        }

        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
                eraseXMLFile();
        }

        private void eraseXMLFile() {
             try
            {
                File.Delete(XMLFile);
                //After erasing disable the buttons which cannot be used while file is not present
                this.btnErase.IsEnabled = false;
                this.btnDisplay.IsEnabled = false;
                this.btnExportToFile.IsEnabled = false;
                validXMLFlag = 0;
                PhoneBook.Properties.Settings.Default.XMLCounter = 1; //To start incrementing from this counter as id in case of erasing the XMLFile because when we create an XML file with the first entry it will start with id 1 and hence settings has 0 as default (and it counts XML file elements on load to update the number) so it has to start from 1 when the program is still open so the recount is not being  trigger yet
                PhoneBook.Properties.Settings.Default.Save();

            }
           
            catch (Exception ex) {
                if (ex is IOException)
                {
                    System.Windows.MessageBox.Show("Erase XMLFile Error: File IO Error");
                }
                else
                {
                    System.Windows.MessageBox.Show("Erase XMLFile Error Exeption: " + ex.Message); 
                }
                
            }
        }

    }

}
