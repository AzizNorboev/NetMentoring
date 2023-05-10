using System.Security.Cryptography;
using System.Text;


//Original method
byte[] salt = new byte[32];
var watch = System.Diagnostics.Stopwatch.StartNew();
Console.WriteLine(GeneratePasswordHashUsingSalt("Password", salt));
watch.Stop();
var elapsedMs = watch.ElapsedMilliseconds;
Console.WriteLine(elapsedMs);

Console.WriteLine();

//Optimized method
var watch1 = System.Diagnostics.Stopwatch.StartNew();
Console.WriteLine(GeneratePasswordHashUsingSaltOptimized("Password", salt));
watch1.Stop();
var elapsedMs1 = watch1.ElapsedMilliseconds;
Console.WriteLine(elapsedMs1);
static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
{
    var iterate = 10000;
    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);

    byte[] hash = pbkdf2.GetBytes(20);
    byte[] hashBytes = new byte[36];

    Array.Copy(salt, 0, hashBytes, 0, 16);
    Array.Copy(hash, 0, hashBytes, 16, 20);

    var passwordHash = Convert.ToBase64String(hashBytes);

    return passwordHash;
}

/*
Currently, the method generates a 20-byte hash using the PBKDF2 algorithm. By increasing the hash size, 
the resulting hash would be stronger and more secure, and the performance impact would likely be minimal.
Another improvement could be to avoid creating a new byte array for the hashBytes and instead, use a 
StringBuilder to concatenate the salt and hash bytes. This would avoid creating unnecessary objects and improve performance.
*/
static string GeneratePasswordHashUsingSaltOptimized(string passwordText, byte[] salt)
{
    var iterate = 10000;
    var hashSize = 32; // use a larger hash size for increased security
    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);

    byte[] hash = pbkdf2.GetBytes(hashSize);
    StringBuilder hashBuilder = new StringBuilder();

    hashBuilder.Append(Convert.ToBase64String(salt));
    hashBuilder.Append(Convert.ToBase64String(hash));

    var passwordHash = hashBuilder.ToString();

    return passwordHash;
}
