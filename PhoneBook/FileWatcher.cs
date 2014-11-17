using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Threading;

namespace PhoneBook
{ //XMLFile Watcher Class
    public class FileWatcher
    {
        private FileSystemWatcher fileWatcher = new FileSystemWatcher(); //To Disable/Enable Raising Events when something is triggered in the MainWindow class

        public bool EnableRaisingEvents
        {

            get { return fileWatcher.EnableRaisingEvents; }

            set
            {
                fileWatcher.EnableRaisingEvents = value;
            }

        }

        public FileWatcher (String File) {
            fileWatcher.Path = System.IO.Directory.GetCurrentDirectory();
            //Filter Attributes to monitor any activities with the files
            fileWatcher.NotifyFilter = NotifyFilters.Attributes |
            NotifyFilters.CreationTime |
            NotifyFilters.DirectoryName |
            NotifyFilters.FileName |
            NotifyFilters.LastAccess |
            NotifyFilters.LastWrite |
            NotifyFilters.Security |
            NotifyFilters.Size;

            fileWatcher.Filter = File;
            fileWatcher.Changed += new FileSystemEventHandler(OnChanged);
            fileWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            //fileWatcher.Created += new FileSystemEventHandler(OnCreated);
            //fileWatcher.Deleted += new FileSystemEventHandler(OnDeleted);

            fileWatcher.EnableRaisingEvents = true;

        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            
                try
                {
                    fileWatcher.EnableRaisingEvents = false;

                }

                finally
                {
                    fileWatcher.EnableRaisingEvents = true;
                    MessageBoxResult result = MessageBox.Show("XMLFile /XSDFile has been changed externally. Close and restart application to restart the validation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK)
                    {
                        //For Updating UI Elements, or doing application related method with out the main class is not poassible in C#, so the solution is to invoke the dispatcher of the application to the mainclass in the form of communication between two threads 
                        //If i had many UI intensive updates i would have used this approach to update in background using threading the UI of the main class from a different one
                        ThreadStart xmlFileWatcherThreadStart = delegate()
                        {
                            Application.Current.Dispatcher.BeginInvoke((Action)delegate()
                            {
                                Application.Current.Shutdown();
                            });
                        };
                        Thread xmlFileWatcherThread = new Thread(xmlFileWatcherThreadStart);
                        xmlFileWatcherThread.Start();
                    }
                   
                }
            

        }

        /*private void OnCreated(object source, FileSystemEventArgs e)
        {
            try
            {
                fileWatcher.EnableRaisingEvents = false;

                System.Console.Write("OnCreated");
            }

            finally
            {
                fileWatcher.EnableRaisingEvents = true;
            }

        }*/

        /*private void OnDeleted(object source, FileSystemEventArgs e)
        {
            try
            {
                fileWatcher.EnableRaisingEvents = false;

                System.Console.Write("OnDeleted");
            }

            finally
            {
                fileWatcher.EnableRaisingEvents = true;
            }

        }*/

        private void OnRenamed(object source, FileSystemEventArgs e)
        {
            try
            {
                fileWatcher.EnableRaisingEvents = false;

            }

            finally
            {
                fileWatcher.EnableRaisingEvents = true;
                MessageBoxResult result = MessageBox.Show("XMLFile / XSDFile has been changed externally. Close and restart application to restart the validation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {

                    ThreadStart xmlFileWatcherThreadStart = delegate()
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)delegate()
                        {
                            Application.Current.Shutdown();
                        });
                    };
                    Thread xmlFileWatcherThread = new Thread(xmlFileWatcherThreadStart);
                    xmlFileWatcherThread.Start();
                }

            }

        }


    }
}
