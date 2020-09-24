using System;
using System.Collections;
using System.IO;
using Korus.TestApplication.Common;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Korus.TestApplication.ContractLoader.Common;

namespace Korus.TestApplication.ContractLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to contract creation system!");

            Tracer.Trace("program started");

            string url;
            do
            {
                url = GetUrlFromUserInput();
            }
            while (!StartAppication(url));

            Tracer.Trace("program exiting");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void EnsureLogsDirectory()
        {
            var logsDir = "Logs";
            if (!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
            }
        }

        private static bool StartAppication(string url)
        {
            try
            {
                Console.WriteLine("Trying to connect");
                using (var site = new SPSite(url))
                {
                    using (var web = site.OpenWeb())
                    {
                        var f = site.Features[new Guid("0d067f3e-2800-49ad-9708-3e6c4402252b")];

                        if (f == null)
                        {
                            Console.WriteLine("Specified site is not a contract management system!");
                            return false;
                        }

                        Console.WriteLine("Successfully connected!");

                        Tracer.Trace($"Connected to site {url}");

                        var countToCreate = GetCountToCreate();

                        Tracer.Trace($"Received command to create {countToCreate} contracts");

                        CreateContracts(web, countToCreate);

                        return true;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Sharepoint site not found at '{url}'");
                return false;
            }

        }

        private static void CreateContracts(SPWeb web, int countToCreate)
        {
            Console.WriteLine($"Starting to create {countToCreate} contracts");

            var contractsLib = web.GetList(SPUtility.ConcatUrls(web.Url, Urls.ContractsList));

            for (int i = 0; i < countToCreate; i++)
            {
                try
                {
                    var ct = contractsLib.ContentTypes.BestMatch(new SPContentTypeId(ContentTypeIds.Contracts));

                    var docSet = DocumentSet.Create(contractsLib.RootFolder, "0", ct, null, true);

                    var msgCreated = $"Succesfully created contract #{docSet.Item.Title}";
                    Console.WriteLine(msgCreated);
                    Tracer.Trace(msgCreated);
                }
                catch (Exception exception)
                {
                    Tracer.Trace($"Unexpected error while creating contract: {exception}");
                }

            }
        }

        private static int GetCountToCreate()
        {
            Console.Write("Please enter number of contracts to create: ");

            int countToCreate = 0;
            bool countreceived = false;
            while (!countreceived)
            {
                countreceived = int.TryParse(Console.ReadLine(), out countToCreate);

                if (countToCreate <= 0)
                {
                    countreceived = false;
                }
                if (!countreceived)
                {
                    Console.WriteLine("Value not recognized as valid number");
                }
            }
            return countToCreate;
        }

        private static string GetUrlFromUserInput()
        {
            bool urlReceived = false;

            string input = null;
            while (!urlReceived)
            {
                Console.WriteLine(Environment.NewLine + "Please enter sitecollection URL: ");

                input = Console.ReadLine();
                

                if (!Uri.IsWellFormedUriString(input, UriKind.Absolute))
                {
                    Console.WriteLine("You enter inncorrect URL!");
                    continue;
                }

                urlReceived = true;
            }
        
            return input;
        }
    }
}
