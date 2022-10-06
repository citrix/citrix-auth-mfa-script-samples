# Copyright © Citrix Systems, Inc.  All rights reserved.

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as Condition
import pyotp
import time

URL = "https://citrix.cloud.com"

USERNAME = "<<Account Username>>";
PASSWORD = "<<Account Password>>";
MFA_SECRET = "<<MFA Secret>>";

# Create the Selenium web driver and navigate to Citrix
driver = webdriver.Chrome()
driver.get(URL)

try:
    # Enter username and password, then click the submit button
    driver.find_element(By.NAME, "username").send_keys(USERNAME)
    driver.find_element(By.NAME, "password").send_keys(PASSWORD)
    driver.find_element(By.ID,"submit").click()

    # Wait for the page to load
    submit = WebDriverWait(driver, 10).until(Condition.presence_of_element_located((By.ID, "verify-totp-code")))

    # Generate the TOTP code
    totpCode = pyotp.TOTP(MFA_SECRET).now()

    # Enter it in the TOTP boxes
    j = 1;
    for digit in totpCode :
        xpath = '//*[@class="ctx-input-digits"]/input[' + str(j)+ ']'
        driver.find_element(By.XPATH, xpath).send_keys(digit)
        j=j+1

    # Click the submit button for the TOTP code
    submit.click()

finally:
    time.sleep(5)
    driver.quit()