/*
 * Создано в SharpDevelop.
 * Пользователь: zaikay
 * Дата: 20.11.2017
 * Время: 8:57
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ParcerOlxSite.Common
{
	/// <summary>
	/// Парсер меню
	/// </summary>
	public class ParcerMenu : Parcer
	{
		public SortedSet<string> menuList;

		public interface IStateParcerMenu
		{
			void SelectMenu(HtmlDocument doc);
		}
		
		class StateParcerMainMenu : IStateParcerMenu
		{
			private readonly ParcerMenu parser;
			public void SelectMenu(HtmlDocument doc)
			{
				HtmlNode node = doc.DocumentNode.SelectSingleNode(".//nav[@id='main-menu']");
				
				HtmlNodeCollection nodes = node.SelectNodes(".//li[@class='cat-item']");
				
				SortedSet<string> tmpMenuList = new SortedSet<string>();
				foreach (HtmlNode n in nodes)
				{
					try
					{
						string href = n.SelectSingleNode(".//a").Attributes["href"].Value;
						href = parser.ConcatHref(parser.connectionStringBuilder.GetCeonnectionString(), href);
						tmpMenuList.Add(href);
					}
					catch
					{
						
					}
				}
				
				parser.SetState(new StateParcerSubMenu(parser));
				
				tmpMenuList.ExceptWith(parser.menuList);
				foreach (string s in tmpMenuList)
					Console.WriteLine(s);
				
				parser.menuList.UnionWith(tmpMenuList);
				foreach(string href in tmpMenuList)
				{
					parser.Select(new CPath(href));
				}
			}
			public StateParcerMainMenu(ParcerMenu parser_)
			{
				parser = parser_;
			}
		}
		
		class StateParcerSubMenu : IStateParcerMenu
		{
			private readonly ParcerMenu parser;
			public void SelectMenu(HtmlDocument doc)
			{
				HtmlNode node = doc.DocumentNode.SelectSingleNode(".//ul[@class='category-nav']");
				
				if (node == null)
					return;
				
				HtmlNodeCollection nodes = node.SelectNodes(".//li//ul//li//a");
				
				if (nodes == null)
					return;
				
				SortedSet<string> tmpMenuList = new SortedSet<string>();
				
				foreach (HtmlNode n in nodes)
				{
					try
					{
						string href = n.Attributes["href"].Value;
						href = parser.ConcatHref(parser.connectionStringBuilder.GetCeonnectionString(), href);
						tmpMenuList.Add(href);
					}
					catch
					{
						
					}
				}
				
				tmpMenuList.ExceptWith(parser.menuList);
				foreach (string s in tmpMenuList)
					Console.WriteLine(s);
				
				parser.menuList.UnionWith(tmpMenuList);
				if (tmpMenuList.Any())
					Console.WriteLine("total: " + parser.menuList.Count + "; add: " + tmpMenuList.Count);
				foreach(string href in tmpMenuList)
				{
					parser.Select(new CPath(href));
				}
			}
			public StateParcerSubMenu(ParcerMenu parser_)
			{
				parser = parser_;
			}
		}
		
		IStateParcerMenu state;
		
		override public void Select(ICPath cPath)
		{
			string stringConnect = connectionStringBuilder.GetCeonnectionString();
			string href = cPath.GetPath();
			href = stringConnect + href.Replace(stringConnect, "");
			string content = getRequest(href);
			
			HtmlDocument doc = GetDocument(ref content);
			
			SaveToLog(href + "\n" + content, "logMenu.txt");
			
			state.SelectMenu(doc);
		}
		public ParcerMenu()
		{
			state = new StateParcerMainMenu(this);
			menuList = new SortedSet<string>();
		}
		public void SetState(IStateParcerMenu state_)
		{
			state = state_;
		}
		public string ConcatHref(string href1, string href2)
		{
			return href1 + href2.Replace(href1, "");
		}
	}
}
