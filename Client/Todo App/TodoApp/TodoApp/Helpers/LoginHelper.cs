using System;
using LBXamarinSDK;
using LBXamarinSDK.LBRepo;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp
{
	public static class LoginHelper
	{
		private const int UNAUTHORIZED = 401;
		private const int UNPROCESSABLE_ENTITY = 422;

		public static async Task<bool> Login(string email, string password){
			var user = new User () {
				email = email,
				password = password
			};
			try{
				var accessToken = (AccessToken)await Users.login (user);
				Gateway.SetAccessToken(accessToken);
				SessionData.Register<AccessToken>(accessToken);
			}catch(RestException ex){
				if (ex.StatusCode == UNAUTHORIZED) {
					return false;
				}
				throw;
			}

			return true;
		}

		public static async Task<bool> Signup (string email, string password)
		{
			var user = new User () {
				email = email,
				password = password
			};
			try{
				await Users.Create (user);
			}catch(RestException ex){
				if (ex.StatusCode == UNPROCESSABLE_ENTITY) {
					return false;
				}
				throw;
			}
			await Login (email, password);

			return true;
		}
	}
}

