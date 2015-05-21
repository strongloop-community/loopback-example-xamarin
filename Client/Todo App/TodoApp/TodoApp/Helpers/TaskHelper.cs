using System;
using LBXamarinSDK;
using LBXamarinSDK.LBRepo;
using System.Threading.Tasks;
using System.Collections.Generic;
using TodoApp.Models;
using System.Linq;

namespace TodoApp.Helpers
{
	
	public static class TaskHelper
	{
		private static IEnumerable<TodoTask> tasksCache;
		private static DateTime tasksCacheTime = DateTime.Now;

		private const int CACHE_TIMEOUT = 10;

		public async static Task<IEnumerable<TodoTask>> GetAllTasks(){
			var token = SessionData.Resolve<AccessToken>();
			if (tasksCache == null || DateTime.Now.Subtract(tasksCacheTime).TotalSeconds > CACHE_TIMEOUT) {
				tasksCache = await Users.getTodoTask (token.userID, @"{""where"":{""isDeleted"":false}}");
				tasksCacheTime = DateTime.Now;
			}
			return tasksCache;
		}

		private static void ResetCache(){
			tasksCache = null;
		}

		public async static Task<IEnumerable<TodoTask>> GetTaskByDay(DateTime date){
			var tasks = await GetAllTasks();
			return tasks.Where (task => task.date.Date == date.Date);
		}

		public async static Task DeleteTask(TodoTask task){
			var token = SessionData.Resolve<AccessToken>();
			task.isDeleted = true;
			await Users.updateByIdTodoTask (task, token.userID, task.id);
			ResetCache();
		}

		public async static Task ToggleDone(TodoTask task){
			var token = SessionData.Resolve<AccessToken>();
			task.isDone = !task.isDone;
			await Users.updateByIdTodoTask (task, token.userID, task.id);
			ResetCache();
		}

		public async static Task ToggleFavorite(TodoTask task){
			var token = SessionData.Resolve<AccessToken>();
			task.isFavorite = !task.isFavorite;
			await Users.updateByIdTodoTask (task, token.userID, task.id);
			ResetCache();
		}

		public static async Task SaveTask (TodoTask task)
		{
			var token = SessionData.Resolve<AccessToken>();
			if (task.id == null) {
				await Users.createTodoTask (task, token.userID);
			} else {
				await Users.updateByIdTodoTask (task, token.userID, task.id);
			}
			ResetCache ();
		}

		public static Category GetCategory(this TodoTask task){
			return CategoryHelper.GetByName (task.category);
		}

		public static Boolean IsValid(this TodoTask task){
			return !(
			    String.IsNullOrEmpty (task.title) ||
			    String.IsNullOrEmpty (task.category)
			);
		}
	}
}

