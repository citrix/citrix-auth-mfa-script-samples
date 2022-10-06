// Copyright © Citrix Systems, Inc.  All rights reserved.

using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools.V102.Profiler;
using TwoStepsAuthenticator;

namespace CitrixMfaAuthenticationAutomation
{
    internal class Program
    {
        private const string URL = "https://citrix.cloud.com";
        private const string USERNAME = "<<Account Username>>";
        private const string PASSWORD = "<<Account Password>>";
        private const string MFA_SECRET = "<<MFA Secret>>";

        static async Task Main(string[] args)
        {
            await AuthenticateWithMfa().ConfigureAwait(false);
        }

        public static async Task AuthenticateWithMfa()
        {
            var webdriver = new ChromeDriver();
            try
            {
                // Enter the username and password
                webdriver.NavigateToUrl(URL);
                webdriver.EnterUsernameAndPassword(USERNAME, PASSWORD);

                // Generate the TOTP code
                var mfaDevice = new TimeAuthenticator();
                var otpCode = mfaDevice.GetCode(MFA_SECRET);

                // Submits the TOTP code
                webdriver.EnterAuthenticationOtp(otpCode);
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            finally
            {
                webdriver?.Quit();
            }
        }

        public static async Task AuthenticateAndEnrollInMFA()
        {
            var webdriver = new ChromeDriver();
            try
            {
                // Navigate to the page and enter the username and password
                webdriver.NavigateToUrl(URL);
                webdriver.EnterUsernameAndPassword(USERNAME, PASSWORD);

                // Accept the prompt to enroll in MFA
                webdriver.ClickAcceptMfaEnrollmentPrompt();

                // Trigger verification email
                webdriver.ClickSendVerificationEmail();

                // Read in the verification code
                Console.WriteLine("Please Enter the Email Verification Code:");
                var verificationCode = Console.ReadLine();

                // Submit the verification code and the password
                webdriver.ClickSubmitVerificationCodeAndPassword(verificationCode, PASSWORD);

                // Retrieve the MFA secret and display it on the console
                var mfaSecret = webdriver.GetMfaDeviceSecret();
                Console.WriteLine("MFA Secret: "+mfaSecret);

                // Generate the TOTP code and submit it
                var mfaDevice = new TimeAuthenticator();
                var otpCode = mfaDevice.GetCode(mfaSecret);
                webdriver.EnterEnrollmentOtp(otpCode);
                webdriver.ClickFinishVerifyingDeviceDuringEnrollment();

                // Add a recovery phone
                webdriver.AddRecoveryPhone("555-555-5555");

                // Download the backup codes
                var backupCodes = webdriver.DownloadBackupCodes();

                // finish MFA enrollment
                webdriver.ClickFinishMfaEnrollment();

                await Task.Delay(5)
            }
            finally
            {
                webdriver?.Quit();
            }
        }
    }
}