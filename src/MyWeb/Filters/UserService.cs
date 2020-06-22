namespace MyWeb {
    public class UserService {
        public bool IsValidUser(string user, string password) {
            return user == "admin" && password == "1234";
        }
    }
}