﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Reflection;

namespace GitGetter2
{
    class Program
    {
    
      
        private static String fileType;
        private static readonly HttpClient client = new HttpClient();



        public static void Main(string[] projectId)
        {
            //client.DefaultRequestHeaders.Add("PRIVATE-TOKEN" ,"ZqpfJzg-n9-qQNv2z1N2");
            fileType = ".java"; // controlls what type of files it will get.
                              // String url = "dit341/express-template"; //Used for testing
            Console.WriteLine(projectId[0].ToString());
            String dirPath = gitTreeRetriever(projectId[0].ToString());
            activateParser(projectId[0].ToString());


        }

        public static bool gitFileRetriever(String projectId, String filepath, String name)
        { // This method is meant to take the id/filepath of a single file. 
          //String address = "https://gitlab.com/"+ projectId +"/raw/master/"+filepath;
            String address = "https://gitlab.com/api/v4/projects/" + WebUtility.UrlEncode(projectId) + "/repository/files/" + WebUtility.UrlEncode(filepath) + "/raw?ref=master";
            Console.WriteLine(address);

            // The part where the request is actually made
            try
            {
                Console.WriteLine("Sending request");
                Task<String> responseTask = client.GetStringAsync(address);
                while (responseTask.IsCompleted == false)
                {
                }
                Console.WriteLine("Request done");
                String responseString = "";
                try
                {
                    responseString = responseTask.Result;
                    Console.WriteLine("Response string has been made");

                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e.Message);
                };

                try
                {
                    String dirPath = "../../../../GitGetter2/FileStorer/" + projectId;
                    File.WriteAllText(dirPath+"/"+ name, responseString);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Making file went wrong"); Console.WriteLine(e);
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong with HTTP request :Defaultdance");
                Console.WriteLine(e);
                return false;
                //Write stuff incase the git repository was not found. 
            }

            return true;
        }
        private static String gitTreeRetriever(String projectId)
        {
            //urlEncoder(projectId);
            String address = "https://gitlab.com/api/v4/projects/" + urlEncoder(projectId) + "/repository/tree";
            Console.WriteLine(address);
            // String address = "https://gitlab.com/api/v4/projects/dit341%2Fexpress-template/repository/tree";
            // The part where the request is actually made
            try
            {
                Console.WriteLine("Sending request");
                Task<String> responseTask = client.GetStringAsync(address);
                while (responseTask.IsCompleted != true) { }

                String responseString = responseTask.Result;

                List<TreeObject> noPathFolder = makeTreeList(responseString);

                //Makes a directory for this project
                String dirPath = "../../../../GitGetter2/FileStorer/" + projectId;
                System.IO.Directory.CreateDirectory(dirPath);
                System.IO.Directory.CreateDirectory(dirPath+ "_srcml");

                foreach (TreeObject tree in noPathFolder)
                {
                    TreeNavigator(projectId, tree);
                }
                return dirPath;

            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong with HTTP request :Defaultdance");
                Console.WriteLine(e);
                //Write stuff incase the git repository was not found. 
                return "ERROR";
            }
        }

        private static bool gitTreeRetriever(String projectId, String filepath)
        { // intended to be used recursivly to get the things in all the folders
            String address = "https://gitlab.com/api/v4/projects/" + urlEncoder(projectId) + "/repository/tree?path=" + WebUtility.UrlEncode(filepath);
            Console.WriteLine(address);
            // String address = "https://gitlab.com/api/v4/projects/dit341%2Fexpress-template/repository/tree";
            // The part where the request is actually made
            try
            {
                Console.WriteLine("Sending request");
                Task<String> responseTask = client.GetStringAsync(address);
                while (responseTask.IsCompleted != true) { }

                String responseString = responseTask.Result;

                List<TreeObject> noPathFolder = makeTreeList(responseString);

                foreach (TreeObject tree in noPathFolder)
                {
                    TreeNavigator(projectId, tree);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong with HTTP request :Defaultdance");
                Console.WriteLine(e);
                return false;
                //Write stuff incase the git repository was not found. 
            }
            return true;
        }
        private static bool TreeNavigator(String projectId, TreeObject map)
        { // Checks if the tree object is a file or folder and calls the appropriate method. 


            if (map.type == "tree")
            {
                return gitTreeRetriever(projectId, map.path);
            }
            else if (map.type == "blob" && map.name.Contains(fileType))
            {
                return gitFileRetriever(projectId, map.path, map.name);
                //return true; //Added for the testing of enumerator. Remember to uncomment gitFileRetriever and remove this. 
            }
            else
            {
                return false;
            }
        }
        private static List<TreeObject> makeTreeList(String populationMaker)
        {
            String populationString = populationMaker;
            //Console.WriteLine(populationString);

            List<TreeObject> tempTreeObject = new List<TreeObject>();

            JsonConvert.PopulateObject(populationString, tempTreeObject);

            return tempTreeObject;
        }
        public static String urlEncoder(String url)
        {
            String encoded_url = "";

            char[] letters = url.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == '/')
                {
                    String tempUrl = "";
                    for (int x = 0; x < i; x++)
                    {
                        tempUrl += letters[x];
                    }
                    tempUrl += "%2F";
                    for (int y = i + 1; y < letters.Length; y++)
                    {
                        tempUrl += letters[y];
                    }

                    letters = tempUrl.ToCharArray();
                }

            }

            encoded_url = new string(letters);

            return encoded_url;
        }

        private static void activateParser(String projectId){
        {

             string PathP = System.AppDomain.CurrentDomain.BaseDirectory + "../../../../../parser/parser/obj/x86/Debug/Parser.exe";

                

                String dirPath = globalFolderGetter(4) +"/GitGetter2/FileStorer/" + projectId+ "/";
                String batAddress = ".SrcmlStarter.bat";
                String storageAddress = globalFolderGetter(4) + "/GitGetter2/FileStorer/";
                storageAddress = storageAddress.Replace( "/" ,"\\ ");

                Console.WriteLine("Enumerator and srcml beginning");
                // Part that activates srcml
                Console.WriteLine("Before enum");
                IEnumerator<String> enumerator = System.IO.Directory.EnumerateFiles(dirPath).GetEnumerator();
                Console.WriteLine("after enum");
                do
                {
                    try
                    {
                        Console.WriteLine("Current enum location: "+enumerator.Current);
                        Process srcml = new Process();
                        //This is intended to start sourceml and then do the thing on each file in the folder.
                        //String batAddress = "../../../../GitGetter2/FileStorer/.SrcmlStarter.bat"; Not working right now

                        

                        char[] charArray = enumerator.Current.ToCharArray();
                        
                        String fileName = "";
                        for (int i = charArray.Length - 1; i > 0; i--)
                        {
                            if (charArray[i] == '/')
                            {
                                break;
                            }
                            fileName += charArray[i];
                        }
                        char[] charArray2 = fileName.ToCharArray();

                        fileName = "";

                        for (int i = charArray2.Length-1; i>=0; i--)
                        {
                            fileName += charArray2[i];
                        }

                        Console.WriteLine("Before srcml calls");
                        Console.WriteLine(storageAddress + "Srcml/SrcmlTextDoc.xml");

                        srcml.StartInfo.FileName = batAddress; // IF YOU WANT TO CHANGE WHERE THE OUTPUT FILES GOES THEN CHANGE IT IN THE BAT FILE

                        // srcml.StartInfo.Arguments = enumerator.Current+" "+dirPath+ "_srcml/"+ fileName;
                        srcml.StartInfo.Arguments = enumerator.Current+ " "+ fileName;
                        // What arguments the file will take when it starts
                        srcml.Start();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }


                } while (enumerator.MoveNext() == true);
             //End of srcml part. 


                // This part should start the parser but I'm too lazy to test it right now. 
                /* Process Project = new Process();
                 try
                 {
                     //so it know where to find the file it should use to start the proccess
                     //if no actual file is specified it will just open the specified folder
                     Project.StartInfo.FileName = PathP;
                     Project.StartInfo.Arguments = dirPath;
                     // What arguments the file will take when it starts
                     Project.Start();
                 }
                 catch (Exception e)
                 {
                     Console.WriteLine(e.Message);
                 }*/
        }

        }
        private static String globalFolderGetter(int backwardsSteps) { // Used to move backwards in the folders
            
            String endlocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //Console.WriteLine("This is the exelocation = "+ endlocation);
            for (int i = 0; i<backwardsSteps; i++)
            {
                endlocation = Path.GetDirectoryName(endlocation);
                //Console.WriteLine("This is the endlocation = " + endlocation);
                //Console.WriteLine("Current iteration: " + i);
            }


            return endlocation;
        }

    }
}

