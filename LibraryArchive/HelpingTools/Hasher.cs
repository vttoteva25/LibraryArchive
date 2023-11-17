using System.Text;
using XSystem.Security.Cryptography;

namespace LibraryArchive.HelpingTools
{
    public static class Hasher
    {
        public static string Hash(string password)
        {
            var data = Encoding.ASCII.GetBytes(password);
            var md5 = new MD5CryptoServiceProvider();
            var md5data = md5.ComputeHash(data);
            string hashedPassword = new ASCIIEncoding().GetString(md5data);
            md5.Clear();
            return hashedPassword;
        }
    }
}
