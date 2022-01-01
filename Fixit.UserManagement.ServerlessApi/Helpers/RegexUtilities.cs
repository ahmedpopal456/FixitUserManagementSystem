using System;
using System.Text.RegularExpressions;

namespace Fixit.User.Management.ServerlessApi.Helpers
{
  class RegexUtilities
  {
    public static bool IsValidEmail(string email)
    {
      bool isValid = false;
      string emailRegexPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))";

      if (string.IsNullOrWhiteSpace(email)) {
        isValid = false;
      }

      try
      {
        isValid = Regex.IsMatch(email, emailRegexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
      }
      catch (RegexMatchTimeoutException)
      {
        isValid = false;
      }

      return isValid;
    }
  }
}
