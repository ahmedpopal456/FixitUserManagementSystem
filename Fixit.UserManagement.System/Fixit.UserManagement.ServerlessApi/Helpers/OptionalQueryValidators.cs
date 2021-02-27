using System;
using System.ComponentModel;

namespace Fixit.User.Management.ServerlessApi.Helpers
{
  public static class OptionalQueryValidators
  {
    public static bool TryParseTimestampUtc(string timestampUtcString, out long? timestampUtcLong)
    {
      timestampUtcLong = null;
      var typeConverter = TypeDescriptor.GetConverter(typeof(long));
      var isParsable = !string.IsNullOrEmpty(timestampUtcString) && typeConverter != null && typeConverter.IsValid(timestampUtcString);
      if (isParsable)
      {
        timestampUtcLong = Convert.ToInt64(timestampUtcString);
      }

      return isParsable;
    }
  }
}
