using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PhoneBook
{   //Model class for enteries to be serialized
    public class PhoneBookEntries
    {
        private Program programInstance = new Program();
    

        private string phoneNo;  
        public string PhoneNo
        {

            get { return phoneNo; }

            set
            {
                phoneNo = value;
                //validate phone no and throw exception if false to be handled in the ontextchanged to let the UI interactive
                if (programInstance.validatePhoneNo(phoneNo) == false)
                {
                    throw new ApplicationException("Must be a swiss number without 0, ex:766683551");
                }
            }

        }

        private string name;
        public string Name
        {

            get { return name; }

            set
            {
                name = value;
                //validate name and throw exception if false to be handled in the ontextchanged to let the UI interactive
                if (programInstance.validateName(name) == false)
                {
                    throw new ApplicationException("Must be Letters and with out special characters");
                }
            }

        }

        private string address;  
        public string Address
        {

            get { return address; }

            set
            {
                address = value;
                //validate address and throw exception if false to be handled in the ontextchanged to let the UI interactive
                if (programInstance.validateAddress(address) == false)
                {
                    throw new ApplicationException("Must be Letters and Digits with out special characters");
                }
            }

        }


    }

    
}
