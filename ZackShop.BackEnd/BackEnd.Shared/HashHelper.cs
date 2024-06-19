using System.Text;

namespace BackEnd.Shared
{
    public interface IHashHelper: IService
    {
        public string SHA256Hash(string value);
    }

    public class HashHelper: IHashHelper
    {
       
        public string SHA256Hash(string value)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
