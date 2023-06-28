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

Console.ReadLine();
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
    var iterations = 10000;
    const int hashSize = 20;  //Instead of hardcoded values - better for code readability and maintainability
    using (var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterations))
    {
        //First of all you should get bytes (salt + hash) then to encode all these bytes to Base64
        var passwordHashBytes = salt.Concat(pbkdf2.GetBytes(hashSize)).ToArray();
        return Convert.ToBase64String(passwordHashBytes);
    }
}

