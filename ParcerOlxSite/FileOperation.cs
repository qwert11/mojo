/*
 * Created by SharpDevelop.
 * User: Zaika
 * Date: 18.11.2017
 * Time: 21:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ParcerOlxSite
{
	/// <summary>
	/// Description of FileOperation.
	/// </summary>
	public static class FileOperation
	{
		public static bool WriteToFile(string path, string line, bool append)
		{
			try
			{
				using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, append))
					file.WriteLine(line);
				return true;
			}
			catch(Exception) { }
			
			return false;
		}
	}
}
