using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualClassroom.Services
{
    /// <summary>
    /// Utility class for checking EGNs 
    /// </summary>
    public class EgnValidator
    {
        /// <summary>
        /// Validates an EGN using the official ESGRAON algorithm
        /// </summary>
        /// <param name="egn">EGN to check</param>
        /// <returns>True if the EGN is valid, false otherwise</returns>
        public static bool IsEgnValid(string egn)
        {
            if (string.IsNullOrEmpty(egn))
            {
                return false;
            }

            if (egn.Length != 10)
            {
                return false;
            }

            double num;
            if (!double.TryParse(egn, out num))
            {
                return false;
            }

            int[] weights = { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            int[] days = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int[] digits = new int[10];

            for (int i = 0; i < 10; i++)
            {
                digits[i] = int.Parse(egn[i].ToString());
            }

            int dd = digits[4] * 10 + digits[5];
            int mm = digits[2] * 10 + digits[3];
            int yy = digits[0] * 10 + digits[1];
            int yyyy = 0;

            if (mm >= 1 && mm <= 12)
            {
                yyyy = 1900 + yy;
            }
            else if (mm >= 21 && mm <= 32)
            {
                mm -= 20; yyyy = 1800 + yy;
            }
            else if (mm >= 41 && mm <= 52)
            {
                mm -= 40; yyyy = 2000 + yy;
            }
            else
            {
                return false;
            }

            days[1] += DateTime.IsLeapYear(yyyy) ? 1 : 0;
            if (!(dd >= 1 && dd <= days[mm - 1]))
            {
                return false;
            }

            if (yyyy == 1916 && mm == 4 && (dd >= 1 && dd < 14))
            {
                return false;
            }

            int checksum = 0;

            for (int j = 0; j < weights.Length; j++)
            {
                checksum += digits[j] * weights[j];
            }

            checksum %= 11;
            if (10 == checksum)
            {
                checksum = 0;
            }

            if (digits[9] != checksum)
            {
                return false;
            }

            return true;
        }
    }
}
