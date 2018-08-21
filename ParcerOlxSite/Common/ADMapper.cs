/*
 * Создано в SharpDevelop.
 * Пользователь: Zaika
 * Дата: 25.12.2017
 * Время: 22:16
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ParcerOlxSite.Common
{
	/// <summary>
	/// Description of ADMapper.
	/// </summary>
	public class ADMapper
	{
		private readonly string storageFile;
		public ADMapper(string source_)
		{
			storageFile = source_;
		}
		public IEnumerable ReadAll()
		{
			List<AD> list = new List<AD>();
			XmlSerializer formatter = new XmlSerializer(typeof(List<AD>));
			if (File.Exists(storageFile))
				using (FileStream fs = new FileStream(storageFile, FileMode.OpenOrCreate))
			{
				list = (List<AD>)formatter.Deserialize(fs);
			}
			return list;
		}
		public void WriteAll(List<AD> list)
		{
			XmlSerializer formatter = new XmlSerializer(typeof(List<AD>));
			
			// получаем поток, куда будем записывать сериализованный объект
			using (FileStream fs = new FileStream(storageFile, FileMode.Create))
			{
				formatter.Serialize(fs, list);
			}
		}
	}
}
