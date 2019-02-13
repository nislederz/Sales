﻿namespace Sales.Helpers
{
    using System;
    using System.Net.Mail;
    class RegexHelper
    {
        public static bool IsValidEmailAddress(string emailaddress)
        {
            try
            {
                var email = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }

        }
    }
}
