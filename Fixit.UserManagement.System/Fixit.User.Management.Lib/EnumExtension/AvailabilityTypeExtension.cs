using System;
using System.Collections.Generic;
using System.Linq;
using Fixit.Core.DataContracts.Users.Availabilities;
using Fixit.Core.DataContracts.Users.Enums;

namespace Fixit.User.Management.Lib.EnumExtension
{
  public static class AvailabilityTypeExtension
  {
    private static Dictionary<AvailabilityType, IList<DayAvailabilityDto>> _availabilities = new Dictionary<AvailabilityType, IList<DayAvailabilityDto>>();

    static AvailabilityTypeExtension()
    {
      _availabilities.Add(AvailabilityType.Always, GetContinuousAvailability());
      _availabilities.Add(AvailabilityType.BusinessHours, GetStandardBusinessHoursAvailability());
    }

    private static IList<DayAvailabilityDto> GetContinuousAvailability()
    {
      var availabilities = new List<DayAvailabilityDto>();
      var standardWorkingHours = new BusinessHoursRangeDto
      {
        StartTimestampUtc = new TimeSpan(0, 0, 0).Ticks,
        EndTimestampUtc = new TimeSpan(24, 0, 0).Ticks
      };
      var dayNames = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
      foreach (var dayName in dayNames)
      {
        availabilities.Add(new DayAvailabilityDto
        {
          DayName = dayName,
          BusinessHours = new List<BusinessHoursRangeDto> { standardWorkingHours }
        });
      }
      return availabilities;
    }

    private static IList<DayAvailabilityDto> GetStandardBusinessHoursAvailability()
    {
      var availabilities = new List<DayAvailabilityDto>();
      var standardWorkingHours = new BusinessHoursRangeDto
      {
        StartTimestampUtc = new TimeSpan(9, 0, 0).Ticks,
        EndTimestampUtc = new TimeSpan(17, 0, 0).Ticks
      };
      var dayNames = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Where(day => day != DayOfWeek.Saturday || day != DayOfWeek.Sunday);
      foreach (var dayName in dayNames)
      {
        availabilities.Add(new DayAvailabilityDto
        {
          DayName = dayName,
          BusinessHours = new List<BusinessHoursRangeDto> { standardWorkingHours }
        });
      }
      return availabilities;
    }

    public static IList<DayAvailabilityDto> GetAvailability(this AvailabilityType availabilityType)
    {
      return _availabilities[availabilityType];
    }
  }
}
