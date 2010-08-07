using System;
using System.Net;
using System.Text;

namespace fefe
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try
			{
				WebClient wClient = new WebClient();
				wClient.Encoding = Encoding.UTF8;
				wClient.Headers.Add("user-agent", "fefe.exe - command line client");
				string strSource = wClient.DownloadString("http://blog.fefe.de");
			
				string divOpen = "<h3>";
				string divClose = "</h3>";
				
				bool found = true;
				int counter = 0;
				
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
						
						//Write day
						Console.WriteLine(current);
						
						{
							pos = strSource.IndexOf("<ul>");
							strSource = strSource.Remove(0, pos);
							
							pos = strSource.IndexOf("</ul>");
							string entries = strSource.Substring(0,pos);
							
							entries = entries.Replace("<li>", "\t");
							entries = StripTags(entries);
							Console.WriteLine(entries);
						}
										
						counter++;
					
						//limit to 3 days
						if (counter >= 3) found = false;
					}
					
				}		
				
				Console.ReadKey();
			} catch
			{
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
	}
}

