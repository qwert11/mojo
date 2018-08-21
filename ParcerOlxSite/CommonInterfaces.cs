/*
 * Created by SharpDevelop.
 * User: Zaika
 * Date: 16.11.2017
 * Time: 22:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ParcerOlxSite
{
	public interface IContainer <T>
	{
		T GetContainer();
	}
	
	public interface ICsd
	{
		string GetShemaString();
	}
	
	/// <summary>
	/// проверка данных по схеме
	/// </summary>
	public interface IDataValidator
	{
		bool CheckCSD<T>(IContainer<T> container);
		void SetCSD(ICsd CSD);
	}
	
	// проверка данных по схеме
	public interface IDataMapper : IDataValidator
	{
		bool CheckCSD<T>(IContainer<T> container);
	}
	
	// маппер данных
	public interface IDataParser
	{
		void Select(ICPath cPath);
		void SetConnection(IConnectionStringBuilder connectionStringBuilder);
		void SetValidator(IDataValidator dataValidator);
	}
	
	public interface ICPath
	{
		string GetPath();
	}
	
	public interface IConnectionStringBuilder
	{
		string GetCeonnectionString();
	}	
}
