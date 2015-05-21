using System;

namespace TodoApp.Helpers {
	public static class DelegateHelper {
		public static void Dispatch(this Delegate method, params object[] args){
			if (method != null)
				method.DynamicInvoke (args);
		}

		public static T Dispatch<T>(this Delegate method, params object[] args){
			return method != null 
				? (T)method.DynamicInvoke (args) 
				: default(T);
		}
	}
}