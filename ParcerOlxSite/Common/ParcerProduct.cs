/*
 * Создано в SharpDevelop.
 * Пользователь: zaikay
 * Дата: 20.11.2017
 * Время: 8:58
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;

namespace ParcerOlxSite.Common
{
	/// <summary>
	/// Парсер товаров
	/// </summary>
	public class ParcerProduct : Parcer
	{
		private int count = 0;
		private List<AD> list;
		
		private void RemoveOffset(HtmlDocument doc)
		{
			HtmlNodeCollection nodesToRemove = doc.DocumentNode.SelectNodes(".//table[@class='fixed offers breakword no-results-table']");

			if (nodesToRemove != null)
				foreach (HtmlNode node in nodesToRemove)
					node.Remove();
		}
		
		private void PrintProduct(HtmlDocument doc)
		{
			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(".//tr[@class='wrap']");
//			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(".//[not(parent::table[@class='fixed offers breakword no-results-table'])]//tr[@class='wrap']");
			if (nodes == null)
				return;
			foreach (HtmlNode title in nodes)
			{
				string location = "";
				try
				{
					HtmlNode n = title.SelectSingleNode(".//td[@valign='bottom']");
					n = n.SelectSingleNode(".//div[@class='space rel']");
					location = n.SelectSingleNode(".//small [@class='breadcrumb x-normal']").InnerText.Trim();
				}
				catch{}
				
				try
				{
					int id = 0;
					try
					{
						id = int.Parse(title.SelectSingleNode(".//table").Attributes["data-id"].Value);
					} catch {}
					
					HtmlNode node = title.SelectSingleNode(".//h3[@class='x-large lheight20 margintop5']");
					string sTitle = node.SelectSingleNode(".//strong").InnerText;
					string href = node.SelectSingleNode(".//a").Attributes["href"].Value;
					node = title.SelectSingleNode(".//p[@class='price']").SelectSingleNode(".//strong");
					string sPrice = node.InnerText.Replace(" ", "");					
					list.Add(new AD(id, sTitle, href, sPrice, location));
					count++;
				}
				catch(Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}
		
		private bool TryParseNextPage(HtmlDocument doc, out string path)
		{
			path = "";
			HtmlNode node = doc.DocumentNode.SelectSingleNode(".//div[@class='pager rel clr']");
			if (node == null)
				return false;
			
			node = node.SelectSingleNode(".//span[@class='fbold next abs large']");
			if (node == null)
				return false;
			
			node = node.SelectSingleNode(".//a");
			if (node == null)
				return false;
						
			path = node.Attributes["href"].Value;
			
			return true;
		}
		
		override public void Select(ICPath cPath)
		{
			string conn = connectionStringBuilder.GetCeonnectionString() + cPath.GetPath().Replace(connectionStringBuilder.GetCeonnectionString(), "");
			string content = getRequest(conn);
			
			HtmlDocument doc = GetDocument(ref content);
			
			SaveToLog(content, "logProduct.txt");
			RemoveOffset(doc);
			PrintProduct(doc);
			
			string href = "";
			if (TryParseNextPage(doc, out href))
			{
				Thread.Sleep(new Random().Next(1000, 3000));
				Select(new CPath(href));
			}
		}
		public ParcerProduct(List<AD> list_)
		{
			list = list_;
		}
	}
}
