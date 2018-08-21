/*
 * Created by SharpDevelop.
 * User: Zaika
 * Date: 16.11.2017
 * Time: 22:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace ParcerOlxSite.Common
{
	abstract public class Parcer : IDataParser
	{
		protected IDataValidator dataValidator;
		protected IConnectionStringBuilder connectionStringBuilder;
		public static string getRequest(string url)
		{
			try
			{
				var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
				httpWebRequest.AllowAutoRedirect = false;//Запрещаем автоматический реддирект
				httpWebRequest.Method = "GET"; //Можно не указывать, по умолчанию используется GET.
				httpWebRequest.Referer = "https://www.yandex.ua/"; // Реферер. Тут можно указать любой URL
				httpWebRequest.UseDefaultCredentials = true;
				httpWebRequest.UserAgent = "null";
				using (var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse())
				{
					using (var stream = httpWebResponse.GetResponseStream())
					{
						using (var reader = new StreamReader(stream, Encoding.GetEncoding(httpWebResponse.CharacterSet)))
						{
							return reader.ReadToEnd();
						}
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(url + " :");
				Console.WriteLine(e);
				return String.Empty;
			}
		}
		
		protected HtmlDocument GetDocument(ref string content)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(content);
			return doc;
		}
		
		abstract public void Select(ICPath cPath);
		protected void SaveToLog(string content, string logFileName)
		{
			FileOperation.WriteToFile(Directory.GetCurrentDirectory() + "\\" + logFileName, content, false);
		}
		public void SetConnection(IConnectionStringBuilder connectionStringBuilder)
		{
			this.connectionStringBuilder = connectionStringBuilder;
		}
		public void SetValidator(IDataValidator dataValidator)
		{
			this.dataValidator = dataValidator;
		}
	}
	
	public class DataValidator : IDataValidator
	{
		public bool CheckCSD<T>(IContainer<T> container)
		{			
			return true;
		}
		public void SetCSD(ICsd CSD)
		{
			
		}
	}
	
	public class Connection : IConnectionStringBuilder
	{
		private readonly string value;
		public string GetCeonnectionString()
		{
			return value;
		}
		public Connection(string value_)
		{
			value = value_;
		}
	}

	public class CPath : ICPath
	{
		private readonly string value;
		public string GetPath()
		{
			return value;
		}
		public CPath(string value_)
		{
			value = value_;
		}
	}
}
