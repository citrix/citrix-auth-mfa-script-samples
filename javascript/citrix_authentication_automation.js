// Copyright © Citrix Systems, Inc.  All rights reserved.

const otplib = require('otplib');

const webdriver = require('selenium-webdriver'),
    By = webdriver.By,
    until = webdriver.until;

const driver = new webdriver.Builder()
    .forBrowser('chrome')
    .build();

// Simple way to add delays
const delay = ms => new Promise(resolve => setTimeout(resolve, ms))

var URL = "https://citrix.cloud.com"

var USERNAME = "<<Account Username>>";
var PASSWORD = "<<Account Password>>";
var MFA_SECRET = "<<MFA Secret>>";

driver.get(URL)
    // First screen: Enter Username and password
    .then(function(){
        driver.findElement(webdriver.By.name("username")).sendKeys(USERNAME);
        driver.findElement(webdriver.By.name("password")).sendKeys(PASSWORD);
        driver.findElement(webdriver.By.id("submit")).click();

    // Second Screen: Generate the OTP code and enter it on the next page
    }).then(async function(){
        var otpVerification = By.id("verify-totp-code");
        
        // wait for up to 10 seconds for the page to load
        await driver.wait(until.elementLocated(otpVerification), 10000); 

        // Generate the OTP code
        const token = otplib.authenticator.generate(MFA_SECRET);

        // Enter the code into each textbox
        var j = 1;
        for (var i of token) {
            var xpath = '//*[@class="ctx-input-digits"]/input[' + j + ']';
            driver.findElement(By.xpath(xpath)).sendKeys(i);
            j++;
        }

    // Once the code was entered, click the button to verify the OTP code
    }).then(async function(){
        var otpVerification = By.id("verify-totp-code");
        driver.findElement(otpVerification).click();

    // Handle any other synthtics that are needed.
    }).then(async function(){
        await delay(5000)
    }).then(function(){
        driver.quit();
    })
    
