﻿using System;
using org.kalima.kalimamq.nodelib;
using org.kalima.kalimamq.message;
using org.kalima.kalimamq.crypto;
using org.kalima.util;
using org.kalima.cache.lib;
using ikvm.extensions;
using System.Text;

namespace KalimaCSharpExample
{
    public class Client : KalimaNode
    {

        private Node node;
        private Clone clone;
        private Logger logger;
        private KalimaClientCallBack kalimaClientCallback;
        private ClonePreferences clonePreferences;

        public static void Main(string[] args)
        {
            try
            {
                Client client = new Client(args);
                Console.Write("\nEnd of the console program...");
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
        }

        public Client(string[] args)
        {
            clonePreferences = new ClonePreferences(args[0]);
            logger = clonePreferences.getLoadConfig().getLogger();
            initComponents();
            System.Threading.Thread.Sleep(2000);
            ConsoleKeyInfo pressedKeyLeftExample = new ConsoleKeyInfo();
            string cachepath = "";
            // Global loop for the example
            do
            {
                ConsoleKeyInfo pressedKeyAskCachePathChoice = new ConsoleKeyInfo();
                ConsoleKeyInfo pressedKeyStayInCachePathChoice = new ConsoleKeyInfo();

                // Verify if this is the first loop in the example
                //Then if this is not the first ask if the user want to stay in the same cache path
                if (pressedKeyLeftExample.KeyChar == '\r')
                {
                    do
                    {
                        Console.WriteLine("\nDo you want to stay in the same cache path? y/n");
                        pressedKeyStayInCachePathChoice = Console.ReadKey();
                    } while (pressedKeyStayInCachePathChoice.KeyChar != 'Y' && pressedKeyStayInCachePathChoice.KeyChar != 'y' && pressedKeyStayInCachePathChoice.KeyChar != 'N' && pressedKeyStayInCachePathChoice.KeyChar != 'n');
                }
                if (pressedKeyStayInCachePathChoice.KeyChar != 'Y' && pressedKeyStayInCachePathChoice.KeyChar != 'y')
                {
                    string[] listCachePathExplorer = clone.getCacheList();
                    string[] listCachePath = new string[listCachePathExplorer.Length];
                    int i = 1;
                    foreach (string cache in listCachePathExplorer)
                    {
                        if (!cache.Contains("hdr") && !cache.Contains("fmt") && !cache.Contains("json") && !cache.Contains("val") && !cache.Contains("Kalima"))
                        {
                            Console.WriteLine("\n" + i + "." + cache);
                            listCachePath.SetValue(cache, i - 1);
                            i++;
                        }
                    }
                    cachepath = askForCachePath(listCachePath, i);


                }
                //Now the user will say if he wants to add or delete a data
                do
                {
                    Console.WriteLine("\nPress 'A' to add something to the cache path and 'D' to delete");
                    pressedKeyAskCachePathChoice = Console.ReadKey();
                } while (pressedKeyAskCachePathChoice.KeyChar != 'A' && pressedKeyAskCachePathChoice.KeyChar != 'a' && pressedKeyAskCachePathChoice.KeyChar != 'D' && pressedKeyAskCachePathChoice.KeyChar != 'd');

                string key = "";
                string body = "";
                do
                {
                    if (pressedKeyAskCachePathChoice.KeyChar == 'A' || pressedKeyAskCachePathChoice.KeyChar == 'a')
                    {
                        Console.WriteLine("\nEnter the key of the data you want to add");
                        key = Console.ReadLine();
                        Console.WriteLine("\nEnter the value of your data");
                        body = Console.ReadLine();
                    }
                    else if (pressedKeyAskCachePathChoice.KeyChar == 'D' || pressedKeyAskCachePathChoice.KeyChar == 'd')
                    {
                        Console.WriteLine("\nEnter the key of the data you want to delete");
                        key = Console.ReadLine();
                    }
                } while (key == "");

                //Send message to the blockchain with the cache path 
                clone.put(cachepath, key, Encoding.ASCII.GetBytes(body));
                System.Threading.Thread.Sleep(1000);

                Console.WriteLine("\nPress ENTER to continue or 'E' to exit");
                pressedKeyLeftExample = Console.ReadKey();
            } while (pressedKeyLeftExample.KeyChar != 'e' && pressedKeyLeftExample.KeyChar != 'E');
        }

        public void initComponents()
        {
            node = new Node(clonePreferences.getLoadConfig());
            clone = new Clone(clonePreferences, node);

            kalimaClientCallback = new KalimaClientCallBack(this);

            node.connect(null, kalimaClientCallback);
        }

        public Node getNode()
        {
            return node;
        }

        public Logger getLogger()
        {
            return logger;
        }

        public Clone getClone()
        {
            return clone;
        }

        //Function to ask in what cache path the user want to work in.

        public string askForCachePath(string[] listCachePath,int lengthList)
        {
            ConsoleKeyInfo pressedKeyChoiceCachePath = new ConsoleKeyInfo();
            Console.WriteLine("\nChoose a cache path");
            pressedKeyChoiceCachePath = Console.ReadKey();
            for(int i = 1; i < lengthList + 1 ; i++)
            {
                if (pressedKeyChoiceCachePath.KeyChar.ToString().Equals(i.ToString()))
                {
                    if(i - 1 >= 0)
                    {
                        return listCachePath[i - 1];
                    }
                }
            }
            askForCachePath(listCachePath, lengthList);
            return "";
        }
    }
}
