using System;
using System.IO;
using System.DirectoryServices;
using System.Globalization;
using System.Security.Principal;
using System.Collections.Generic;

namespace ldapsearch
{
    class Program
    {
        static void Main(string[] args)
        {
            if ((args.Length < 4 || args.Length > 5) || (args.Length == 1 && args[0].ToLower(CultureInfo.InvariantCulture).IndexOf("help", System.StringComparison.Ordinal) >= 0))
            {
                Console.Write("Usage: ldapsearch.exe <FQDN> <user> <pass> <ldap-filter> <optional-output-file>");
                System.Environment.Exit(0);
            }
            else
            {
                try
                {
                    DirectoryEntry myldapconnection = CreateDirectoryEntry(args[0], args[1], args[2]); // create LDAP connection object with specified destination server and creds
                    DirectorySearcher mysearcher = CreateDirectorySearcher(myldapconnection,args[3]);  //create LDAP searcher object
                    SearchResultCollection allresults = mysearcher.FindAll(); //Initiate search function
                    List<string> outputfile = new List<string>();
                    if (allresults.Count != 0)
                    {
                        foreach (SearchResult result in allresults)
                        {
                            if (result != null)
                            {
                                ResultPropertyCollection fields = result.Properties;
                                foreach (String ldapField in fields.PropertyNames)
                                {
                                    foreach (Object mycollection in fields[ldapField])
                                    {
                                        if (mycollection is byte[])
                                        {
                                            byte[] objectsidinbytes = (byte[])mycollection;
                                            if (ldapField == "objectsid")
                                            {
                                                string sid = new SecurityIdentifier(objectsidinbytes, 0).ToString();
                                                outputfile.Add(sid);
                                            }
                                            else if (ldapField == "objectguid")
                                            {
                                                string guid = new System.Guid(objectsidinbytes).ToString();
                                                outputfile.Add(guid);
                                            }
                                            else
                                            {
                                                string bytesinstring = BitConverter.ToString(objectsidinbytes);
                                                outputfile.Add(bytesinstring);
                                            }
                                        }
                                        else
                                        {
                                            outputfile.Add(mycollection.ToString());
                                        }
                                        if ( args.Length == 5)
                                        {
                                            try
                                            {
                                                using (StreamWriter testfile = new StreamWriter(args[4], true))
                                                {
                                                    testfile.WriteLine(String.Format("{0,-20} : {1}", ldapField, string.Join("", outputfile[outputfile.Count - 1])));
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.ToString());
                                                System.Environment.Exit(0);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine(String.Format("{0,-20} : {1}", ldapField, string.Join("", outputfile[outputfile.Count - 1])));
                                        }
                                        
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Nothing Found!");
                                System.Environment.Exit(0);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nothing Found!");
                        System.Environment.Exit(0);
                    }
                    myldapconnection.Dispose();
                    allresults.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caught:\n\n" + e.ToString());
                }
            }
        }
        
        static DirectoryEntry CreateDirectoryEntry(string destination, string user, string pass)
        {
            // define and return a new LDAP connection with desired settings
            DirectoryEntry ldapconnection = new DirectoryEntry("LDAP://"+ destination , user, pass, AuthenticationTypes.SecureSocketsLayer | AuthenticationTypes.Secure);
            return ldapconnection;
        }

        static DirectorySearcher CreateDirectorySearcher(DirectoryEntry myldapconnection, String filter)
        {
            DirectorySearcher mysearcher = new DirectorySearcher(myldapconnection);
            mysearcher.PageSize = int.MaxValue;
            mysearcher.Filter = filter;
            return mysearcher;
        }
    }
}
