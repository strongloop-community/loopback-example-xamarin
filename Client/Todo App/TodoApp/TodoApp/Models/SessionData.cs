using System;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp.Models
{
	public class SessionData
	{
		static object locker = new object ();
		static SessionData instance;

		private Dictionary<Type, Lazy<object>> Items { get; set; }

		private static SessionData Instance
		{
			get
			{
				lock (locker) {
					if (instance == null)
						instance = new SessionData ();
					return instance;
				}
			}
		}

		public SessionData ()
		{
			Items = new Dictionary<Type, Lazy<object>> ();
		}

		public static void Register<T> (T obj)
		{
			Instance.Items [typeof (T)] = new Lazy<object> (() => obj);
		}

		public static void Register<T> ()
			where T : new ()
		{
			if (Instance.Items.Any (i => i is T))
				return;

			Instance.Items [typeof (T)] = new Lazy<object> (() => new T ());
		}

		public static void Register<T> (Func<object> function)
		{
			Instance.Items [typeof (T)] = new Lazy<object> (function);
		}

		public static T Resolve<T> ()
		{
			Lazy<object> service;
			if (Instance.Items.TryGetValue (typeof (T), out service)) {
				return (T)service.Value;
			} else {
				throw new KeyNotFoundException (string.Format ("Service not found for type '{0}'", typeof (T)));
			}
		}

	}
}
