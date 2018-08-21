/*
 * Создано в SharpDevelop.
 * Пользователь: Zaika
 * Дата: 07.12.2017
 * Время: 22:21
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ParcerOlxSite.Common;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ParcerOlxSite
{
	class Program
	{
		private static void ShowBalloon(string title, string body)
		{
			NotifyIcon notifyIcon = new NotifyIcon();
			notifyIcon.Icon = SystemIcons.Exclamation;
			notifyIcon.Visible = true;
			notifyIcon.ShowBalloonTip(20000, title, body, ToolTipIcon.Info);
		}
		private static string GetShortInfoAD(IEnumerable<AD> list, int maxCount)
		{
			string s = "";
			int i = 0;
			foreach (AD ad in list)
			{
				s += ad.title + Environment.NewLine + "Цена: " + ad.price + " " + ad.currency;
				if (maxCount != -1 && ++i >= maxCount)
					break;				
			}
			return s;
		}
		public static void Parce(IConnectionStringBuilder conn, ICPath path)
		{
			List<AD> listAD = new List<AD>();
			IDataParser paserProduct = new ParcerProduct(listAD);
			paserProduct.SetConnection(conn);
			paserProduct.Select(path);
			HashSet<AD> distinctAD = new HashSet<AD>(listAD, new SameADComparer());

			ADMapper mapper = new ADMapper(Directory.GetCurrentDirectory() + "\\storage.xml");
			HashSet<AD> storedListAD = new HashSet<AD>(new SameADComparer());
			
			foreach (AD o in mapper.ReadAll())
				storedListAD.Add(o);
			
			distinctAD = new HashSet<AD>(distinctAD.Except(storedListAD, new SameADComparer()), new SameADComparer());
			
			if (storedListAD.SequenceEqual(storedListAD.Union(distinctAD, new SameADComparer()), new SameADComparer()) == false)
			{			
//				ShowBalloon("Новые товары", GetShortInfoAD(distinctAD, 3));
				foreach (AD ad in distinctAD)
					Console.WriteLine(ad);
				listAD = storedListAD.Union(distinctAD).ToList();
				mapper.WriteAll(listAD);
			}
		}
		
		public static void Main(string[] args)
		{
			while (true)
			{	
				IConnectionStringBuilder conn = new Connection("https://www.olx.ua");
				ICPath path1 = new CPath("/elektronika/kompyutery-i-komplektuyuschie/komplektuyuschie-i-aksesuary/protsessory/dnepr/q-5450/" +
				                         "?search%5Bfilter_float_price%3Afrom%5D=500&search%5Bfilter_float_price%3Ato%5D=1000"
				                        );
				ICPath path2 = new CPath("/elektronika/kompyutery-i-komplektuyuschie/komplektuyuschie-i-aksesuary/protsessory/dnepr/q-5460/" +
				                         "?search%5Bfilter_float_price%3Afrom%5D=500&search%5Bfilter_float_price%3Ato%5D=1000");
				ICPath path3 = new CPath("/elektronika/kompyutery-i-komplektuyuschie/komplektuyuschie-i-aksesuary/videokarty/dnepr/" +
				                         "?search%5Bfilter_float_price%3Afrom%5D=500&search%5Bfilter_float_price%3Ato%5D=2000");
				Parce(conn, path1);
				Thread.Sleep(2000);
				Parce(conn, path2);
				Thread.Sleep(3600);
//				Parce(conn, path3);
				Thread.Sleep(3600000);
//				Parce(new CPath("/elektronika/kompyutery-i-komplektuyuschie/komplektuyuschie-i-aksesuary/videokarty/dnepr/q-450/?search%5Bfilter_float_price%3Afrom%5D=1500&search%5Bfilter_float_price%3Ato%5D=1500"));
//				Thread.Sleep(3000);
			}
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}