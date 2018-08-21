/*
 * Создано в SharpDevelop.
 * Пользователь: zaikay
 * Дата: 25.09.2017
 * Время: 17:00
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyCollections
{
	/// <summary>
	/// Description of Tree.
	/// </summary>
	class Tree<T> 
	{
		private readonly List<Node<T>> _roots = new List<Node<T>>();
		
		public IEnumerable<Node<T>> Roots() { return _roots; }
		
		public void AddRoot(Node<T> root)
		{
			_roots.Add(root);
		}
		
		private IEnumerable<Node<T>> GetEnumerator(IEnumerable<Node<T>> list)
		{
			foreach (Node<T> node in list)
			{
				yield return node;
				if (node.IsLeaf() == false)
					foreach (Node<T> chaild in GetEnumerator(node.Children()))
						yield return chaild;
			}
		}
		public IEnumerable<Node<T>> GetEnumerator()
		{
			return GetEnumerator( Roots() );
		}
	}

	class Node<T>
	{
		private readonly List<Node<T>> _children = new List<Node<T>>();
		private readonly Node<T> _parent;
		
		public Node(T value, Node<T> parent)
		{
			Value = value;
			_parent = parent;
		}
		
		public T Value { get; set; }
		
		public IEnumerable<Node<T>> Children () { return _children; }
		
		public Node<T> Parent () { return _parent; }
		
		public bool IsRoot() { return _parent == null; }
		
		public bool IsLeaf() { return _children.Count == 0; }
		
		public void AddChild(Node<T> node)
		{
			_children.Add(node);
		}
	}
}
