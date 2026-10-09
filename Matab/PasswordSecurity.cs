using System;
using System.Security.Cryptography;
namespace Matab {
 internal static class PasswordSecurity {
  internal static string Hash(string password) {
   using(var rng=RandomNumberGenerator.Create()) { var salt=new byte[16]; rng.GetBytes(salt);
    using(var pbkdf=new Rfc2898DeriveBytes(password,salt,120000))
     return "PBKDF2$120000$"+Convert.ToBase64String(salt)+"$"+Convert.ToBase64String(pbkdf.GetBytes(32));
   }
  }
  internal static bool Verify(string password,string stored) {
   if(string.IsNullOrEmpty(stored)) return false;
   var parts=stored.Split('$');
   if(parts.Length!=4 || parts[0]!="PBKDF2") return false;
   int iterations; if(!int.TryParse(parts[1],out iterations) || iterations<10000 || iterations>1000000) return false;
   try { var salt=Convert.FromBase64String(parts[2]); var expected=Convert.FromBase64String(parts[3]);
    using(var pbkdf=new Rfc2898DeriveBytes(password,salt,iterations)) { var actual=pbkdf.GetBytes(expected.Length); int diff=0; for(int i=0;i<actual.Length;i++) diff|=actual[i]^expected[i];return diff==0; }
   } catch(FormatException) { return false; }
  }
 }
}
