/*
 * Создано в SharpDevelop.
 * Пользователь: Zaika
 * Дата: 25.12.2017
 * Время: 18:53
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ParcerOlxSite.Common
{
	/// <summary>
	/// Description of AD.
	/// </summary>
	[Serializable]
	public class AD
	{
		public int id {get; set;}
		public string title {get; set;}
		public string href {get; set;}
		public double price { get { return ParcePrice(dirtPrice); } }
		private double ParcePrice(string sPrice)
		{
			string s = Regex.Match(sPrice, "[0-9]*[.,]?[0-9]+").Value;
			double price_ = 0;
			double.TryParse(s, out price_);
			return price_;
		}
		private string ParceCurrency(string sPrice)
		{			
			return Regex.Match(sPrice, "[^.\\d]\\D+").Value;
		}
		public string dirtPrice {get; set;}
		public string location {get; set;}
		public string currency { get { return ParceCurrency(dirtPrice); } }
		public AD(int id_, string title_, string href_, string price_, string location_)
		{
			id = id_; title = title_; href = href_; dirtPrice = price_; location = location_;
		}
		public AD() {}
		public override string ToString()
		{
			return string.Format("[AD \n\tId={0}, \n\tTitle={1}, \n\tHref={2}, \n\tPrice={3}{4}, \n\tLocation={5}]", id, title, href, price, currency, location);
		}
	}
	
	class SameADComparer : EqualityComparer<AD>
	{
		public override bool Equals(AD ad1, AD ad2)
		{
			return ad1.id.Equals(ad2.id);
		}
		
		public override int GetHashCode(AD ad)
		{
			int hCode = ad.id; //ad.id ^ ad.href ^ ad.price ^ ad.title;
			return hCode.GetHashCode();
		}
	}
}
