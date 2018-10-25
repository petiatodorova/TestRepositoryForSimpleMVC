namespace IRunesWebApp.Controllers
{
    using System;
    using System.Linq;
    using IRunesWebApp.Models;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;

    public class UsersController : BaseController
    {
        public IHttpResponse Register(IHttpRequest httpRequest)
        {
            return this.View();
        }

        public IHttpResponse DoRegister(IHttpRequest httpRequest)
        {
            var username = httpRequest.FormData["username"].ToString().Trim();
            var password = httpRequest.FormData["password"].ToString();
            var confirmPassword = httpRequest.FormData["confirmPassword"].ToString();
            var email = httpRequest.FormData["email"].ToString();

            // Validate username and password
            if (string.IsNullOrEmpty(username) || username.Length < 4)
            {
                return this.BadRequestError("Please provide valid username with length of 4 or more characters.");
            }

            if (this.Db.Users.Any(u => u.Username == username))
            {
                return this.BadRequestError("User with the same name already exists.");
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                return this.BadRequestError("Please provide password with length of 6 or more characters.");
            }

            if (password != confirmPassword)
            {
                return this.BadRequestError("Passwords do not match.");
            }

            // Hash password
            var hashedPassword = this.HashService.Hash(password);

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                Password = hashedPassword,
                Email = email
            };

            this.Db.Users.Add(user);

            try
            {
                this.Db.SaveChanges();
            }
            catch (Exception ex)
            {
                return this.ServerError(ex.Message);
            }

            // Redirect
            return SignInUser(httpRequest, user);
        }

        public IHttpResponse Login(IHttpRequest httpRequest)
        {
            return this.View();
        }

        public IHttpResponse DoLogin(IHttpRequest httpRequest)
        {
            var username = httpRequest.FormData["username"].ToString().Trim();
            var password = httpRequest.FormData["password"].ToString();

            var hashedPassword = this.HashService.Hash(password);

            var user = this.Db.Users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == hashedPassword);

            if (user == null)
            {
                this.BadRequestError("Invalid user name or password.");
            }

            return SignInUser(httpRequest, user);
        }

        private IHttpResponse SignInUser(IHttpRequest httpRequest, User user)
        {
            httpRequest.Session.AddParameter("username", user.Username);

            var cookieContent = this.UserCookieService.GetUserCookie(user.Username);

            var response = new RedirectResult("/");
            var cookie = new HttpCookie(".auth-IRunes", cookieContent, 7) { HttpOnly = true };
            response.Cookies.Add(cookie);

            return response;
        }

        public IHttpResponse Logout(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth-IRunes") && !request.Cookies.ContainsCookie("SIS_ID"))
            {
                return new RedirectResult("/");
            }

            var irunesCookie = request.Cookies.GetCookie(".auth-IRunes");
            irunesCookie.Delete();
            var sisCookie = request.Cookies.GetCookie("SIS_ID");
            sisCookie.Delete();
            var response = new RedirectResult("/");
            response.Cookies.Add(irunesCookie);
            response.Cookies.Add(sisCookie);
            return response;
        }
    }
}
