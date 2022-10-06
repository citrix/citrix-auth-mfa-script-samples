# Examples of scripts for authentication against Citrix MFA

Includes examples for Javascript, Python and C#

All examples use Selenium and an external library to calculate the TOTP code for MFA.

The MFA_SECRET in each example is the MFA key or secret generated at the time of registering an MFA device. 
Citrix displays the QR and the key. The QR code contains the key, but the key can be copied down and supplied to the scripts below.
The same key can be used in multiple devices and they will generate the same TOTP code so long as the client and server clocks are in sync.

## C#
TOTP Library: **TwoStepsAuthenticator** - (https://github.com/glacasa/TwoStepsAuthenticator)

Required Nuget Packages: 
- DotNetSeleniumExtras.WaitHelpers
- Selenium.WebDriver
- Selenium.WebDriver.ChromeDriver
- TwoStepsAuthenticator

TOTP code Example:
```
var authenticator = new TwoStepsAuthenticator.TimeAuthenticator();
var code = authenticator.GetCode(MFA_SECRET);
```

## Python
TOTP Library: **Pythoauth** - (https://github.com/pyauth/pyotp)

Python Packages:
- pyotp
- Selenium

TOTP code Example:
```
var totp = pyotp.TOTP(MFA_SECRET)
var code = totp.now()
```

## Javascript
TOTP Library: **OTPLib** - (https://github.com/yeojz/otplib)

Uses NodeJS. Requires the packages:
- selenium-webdriver
- otplib

TOTP code Example:
```
const otplib = require('otplib');

const secret = 'mfaSecret';
const token = otplib.authenticator.generate(MFA_SECRET);
```