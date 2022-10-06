// Copyright © Citrix Systems, Inc.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CitrixMfaAuthenticationAutomation
{
    internal static class SeleniumDriverExtentions
    {
        #region Finding and Waiting for Elements To Be Ready

        public static void NavigateToUrl(this IWebDriver webDriver, string fullUrlString)
        {
            webDriver.Navigate().GoToUrl(fullUrlString);
        }

        public static IWebElement GetElementById(this IWebDriver webDriver, string id, bool isClickable = false)
            => webDriver.GetElement(By.Id(id), isClickable);

        public static IWebElement GetElementByClass(this IWebDriver webDriver, string className, bool isClickable = false)
            => webDriver.GetElement(By.ClassName(className), isClickable);

        public static IWebElement GetElementByXPath(this IWebDriver webDriver, string xPath, bool isClickable = false)
            => webDriver.GetElement(By.XPath(xPath), isClickable);

        public static IWebElement GetElement(this IWebDriver webDriver, By selector, bool isClickable = false)
            => webDriver.WaitForElement(selector, isClickable);

        public static IWebElement WaitForElement(this IWebDriver webDriver, By expression, bool isClickable = false)
        {
            var wait = new WebDriverWait(webDriver, DefaultElementWait);
            var element = wait.Until(_ => _.FindElement(expression));

            if (isClickable)
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(expression));
            }
            return element;
        }

        public static IList<IWebElement> GetElementsByXPath(this IWebDriver webDriver, string xPath)
            => webDriver.GetElements(By.XPath(xPath));

        public static IList<IWebElement> GetElements(this IWebDriver webDriver, By selector)
            => webDriver.WaitForElements(selector);

        public static IList<IWebElement> WaitForElements(this IWebDriver webDriver, By expression)
        {
            var wait = new WebDriverWait(webDriver, DefaultElementWait);
            return wait.Until(_ => _.FindElements(expression));
        }

        #endregion

        #region Entering Text and Click Links/Buttons
        public static void EnterText(this IWebDriver webDriver, string id, string text)
            => webDriver.GetElementById(id).EnterText(text);

        public static void EnterTextByXPath(this IWebDriver webDriver, string xPath, string text)
            => webDriver.GetElementByXPath(xPath).EnterText(text);

        public static void EnterText(this IWebElement element, string text)
            => element.SendKeys(text);

        public static void Click(this IWebDriver webDriver, string id)
            => Click(webDriver.GetElementById(id, true));

        public static void ClickByXPath(this IWebDriver webDriver, string xPath)
            => Click(webDriver.GetElementByXPath(xPath, true));

        public static void ClickByClass(this IWebDriver webDriver, string className)
            => Click(webDriver.GetElementByClass(className, true));

        public static void Click(this IWebElement element)
            => element.Click();
        #endregion

        #region UserName and password Authentication

        public static void EnterUsernameAndPassword(this IWebDriver webDriver, string username, string password)
        {
            webDriver.EnterText("username", username);
            webDriver.EnterText("password", password);
            webDriver.Click("submit");
        }
        #endregion

        #region MFA Enrollment

        public static void ClickAcceptMfaEnrollmentPrompt(this IWebDriver driver)
            => driver.ClickByXPath("//button[contains(@class, \"btn-default\")]");

        public static void ClickCancelMfaEnrollmentPrompt(this IWebDriver driver)
            => driver.ClickByXPath("//a[@class=\"mfa-enrollment-prompt-close\"]");

        public static void UpdateEmailDuringMfaEnrollment(this IWebDriver webDriver, string newEmail)
            => webDriver.EnterText("account-verification-email-input", newEmail);

        public static void ClickSendVerificationEmail(this IWebDriver webDriver)
            => webDriver.Click("account-verification-resend");

        public static void ClickSubmitVerificationCodeAndPassword(this IWebDriver webDriver, string emailVerificationCode, string password)
        {
            webDriver.EnterText("account-verification-code-input", emailVerificationCode);
            webDriver.EnterText("account-verification-password-input", password);
            webDriver.Click("account-verification-submit");
        }

        public static string GetMfaDeviceSecret(this IWebDriver webDriver)
            => webDriver.GetElementByClass("app-verification__qr-container__block__key")?.Text;

        public static void EnterEnrollmentOtp(this IWebDriver webDriver, string otpCode)
            => webDriver.EnterText("app-verification-code-input", otpCode);

        public static void ClickFinishVerifyingDeviceDuringEnrollment(this IWebDriver webDriver)
            => webDriver.ClickByClass("app-verification__button");

        #region Recovery Page

        public static void AddRecoveryPhone(this IWebDriver webDriver, string phoneNumber)
        {
            webDriver.ClickByXPath("//a[contains(text(), \"recovery phone\")]");
            webDriver.EnterTextByXPath("//input[@placeholder=\"Enter phone number\"]", phoneNumber);
            webDriver.EnterTextByXPath("//input[@placeholder=\"Verify phone number\"]", phoneNumber);
            webDriver.ClickByXPath("//button[contains(@class, \"recoveryphone__form__submit\")]");
        }

        public static void StartAddingRecoveryEmail(this IWebDriver webDriver, string recoveryEmail)
        {
            webDriver.ClickByXPath("//a[contains(text(), \"recovery email\")]");
            webDriver.EnterTextByXPath("//input[@placeholder=\"Enter recovery email\"]", recoveryEmail);
            webDriver.ClickByXPath("//form[@class=\"recoveryemail__form\"]//button[text()=\"Send verification email\"]");
        }

        public static void FinishAddingRecoveryEmail(this IWebDriver webDriver, string recoveryEmailVerificationCode)
        {
            webDriver.EnterTextByXPath("//form[@class=\"recoveryemail__form\"]" +
                                       "//input[@class=\"recoveryemail__form__input\"][@placeholder=\"Enter 6-digit verification code\"]", 
                recoveryEmailVerificationCode);
            webDriver.ClickByXPath("//form[@class=\"recoveryemail__form\"]//button[text()=\"Verify code\"]");
        }

        public static IList<string> DownloadBackupCodes(this IWebDriver webDriver)
        {
            webDriver.ClickByXPath("//a[contains(text(), \"backup codes\")]");

            webDriver.Click("checkbox");

            var codeElements = webDriver.GetElementsByXPath("//div[@class=\"codes__backup-codes__codes\"]/div");
            var backupCodes = codeElements
                .Where(element => !string.IsNullOrWhiteSpace(element.Text))
                .Select(element => element.Text);

            webDriver.ClickByXPath("//button[@type=\"submit\"]");
            return backupCodes.ToList();
        }

        public static void DownloadMfaBackUpCodes(this IWebDriver webDriver)
            => Click(webDriver.GetElement(By.CssSelector("#backup-codes-download-button")));

        public static void ClickFinishMfaEnrollment(this IWebDriver webDriver)
            => webDriver.ClickByXPath("//button[@type=\"button\" and contains(@class, \"recoverymethods__submit\")]");

        #endregion

        #endregion

        #region MFA Authentication
        public static void EnterAuthenticationOtp(this IWebDriver webDriver, string otpCode)
        {
            var divForInputDigits = webDriver.GetElementByClass("ctx-input-digits");
            var inputBoxes = divForInputDigits.FindElements(By.CssSelector("input"));

            foreach (var (inputBox, digit) in inputBoxes.Zip(otpCode, Tuple.Create))
            {
                inputBox.SendKeys(digit.ToString());
            }

            webDriver.Click("verify-totp-code");
        }
        #endregion

        private readonly static TimeSpan DefaultElementWait = TimeSpan.FromSeconds(30);
    }
}