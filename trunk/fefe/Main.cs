/* fefeclient - Main.cs 
 * 
 * Copyright (c) 2010 Hugo Scholz
 * 
 * This software may be used and redistributed under the terms of the Mozilla Public License 
 * 
 * See http://www.mozilla.org/MPL/MPL-1.1.html for details
 */



using System;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace fefe
{
	class MainClass
	{
		
		private static bool filterBullshit = false;
		
			
    	private static void PrintUsage()
		{
			Console.WriteLine("Usage: fefe.exe [-[f|h]]" + Environment.NewLine);
			Console.WriteLine("Options:");
			Console.Write    (" -f (filter bullshit):  Filters irrelevant blog posts using a scoring system ");
			Console.WriteLine("based on complex AI algorithms and sophisticated heuristics");
			Console.WriteLine(" -h (help)           :  Displays this text");
		}
		
		public static void Main (string[] args)
		{

			
			// Parse Command Line Args
			if ( args.Length == 1)
			{
				if ( args[0] == "-f")
				   filterBullshit = true;
				else 
				{
					PrintUsage();
					return;
				}
			}
			else if ( args.Length > 1 )
			{
				PrintUsage();
				return;
			}
	        		
		
			try
			{
				ServicePointManager.ServerCertificateValidationCallback 
					+= new System.Net.Security.RemoteCertificateValidationCallback(bypassAllCertificateStuff);
				
				
				WebClient wClient = new WebClient();
				wClient.Proxy =  WebRequest.DefaultWebProxy;
				wClient.Proxy.Credentials = CredentialCache.DefaultCredentials;
				wClient.UseDefaultCredentials = true;				
				
				wClient.Encoding = Encoding.UTF8;
				wClient.Headers.Add("user-agent", "fefe.exe - command line client");
				string strSource = wClient.DownloadString("https://blog.fefe.de");
			
				string divOpen = "<h3>";
				string divClose = "</h3>";
				
				bool found = true;
				int counter = 0;
				
				Console.WriteLine();
				
				while(found)
				{				
					int pos = strSource.IndexOf(divOpen);
					
					if (pos != -1)
					{				
						//Remove everything in front of posStart
						strSource = strSource.Remove(0, pos + divOpen.Length);
						
						//Search for closing </td>
						pos = strSource.IndexOf(divClose);
						
						string current = strSource.Substring(0, pos);
						current = StripTags(current);
						
						
						
						
						{
							pos = strSource.IndexOf("<ul>");
							strSource = strSource.Remove(0, pos);
							
							pos = strSource.IndexOf("</ul>");
							string entries = strSource.Substring(0,pos);
							
							// entries = entries.Replace("<li>", "\t");
							// entries = entries.Replace("<br>", Environment.NewLine);
							
							entries = entries.Replace("<li>", "");
							entries = entries.Replace("<br>", "");

							
							entries = StripTags(entries);
							
        					string[] delimiter = new string[]{"[l]"};
							string[] blogPosts = entries.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
								
							// print blog posts to console
							bool datePrinted = false;
							for (int i = 1; i < blogPosts.Length; i++)
							{
							   // Check for bullshit blogpost, use heuristics
						       bool isBullshit = ! (blogPosts[i].Contains("Wikipedia") || blogPosts[i].Contains("Zensuradmin") || blogPosts[i].Contains("Löschtroll"));
							   if (! ( isBullshit && filterBullshit ) )
							   {
							      if ( ! datePrinted )
							      {
									 Console.WriteLine(current + Environment.NewLine);
							         datePrinted = true;
							      }    		 
							  	  
							  	  Console.WriteLine("\t[l]" + blogPosts[i]);		
									
							  }    
							}
									
		
							
						}
										
						counter++;
					
						//limit to 3 days
						if (counter >= 3) found = false;
					}
					
				}		
				
				//Console.ReadKey();
			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Sorry this is not working");	
			}

		}
		
				
		public static string StripTags(string src)
    	{
	        char[] a = new char[src.Length];
	        int ai = 0;
	        bool ins = false;
	
	        for (int i = 0; i < src.Length; i++)
	        {
	            char let = src[i];
	            if (let == '<')
	            {
	                ins = true;continue;
	            }
	            if (let == '>')
	            {
	                ins = false;continue;
	            }
	            if (!ins)
	            {
	                a[ai] = let;ai++;
	            }
	        }
	        return new string(a, 0, ai);
    	}
		
		private static bool bypassAllCertificateStuff(object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error)
		{
   			return true;
		}

	}
}

