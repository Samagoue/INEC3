using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INEC3.Helper
{
    public class Base
    {
        public string access_token
        {
            get { return EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("access_token"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("access_token"), EncryptDecrypt.Encrypt(value)); }
        }
        public string EmailAddress
        {
            get { return EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("EmailAddress"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("EmailAddress"), EncryptDecrypt.Encrypt(value)); }
        }
        public string RoleCode
        {
            get { return string.IsNullOrEmpty(EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("RoleCode")))) ? "0" : EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("RoleCode"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("RoleCode"), EncryptDecrypt.Encrypt(value.ToString())); }
        }
        public string UserCode
        {
            get { return string.IsNullOrEmpty(EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("UserCode")))) ? "0" : EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("UserCode"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("UserCode"), EncryptDecrypt.Encrypt(value.ToString())); }
        }
        public string Password
        {
            get { return EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("Password"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("Password"), EncryptDecrypt.Encrypt(value)); }
        }
        //public string RememberMe
        //{
        //    get { return EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("RememberMe"))); }
        //    set { SaveCookie(EncryptDecrypt.Encrypt("RememberMe"), EncryptDecrypt.Encrypt(value)); }
        //}
        public string First_name
        {
            get { return EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("First_name"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("First_name"), EncryptDecrypt.Encrypt(value)); }
        }
        public string Last_name
        {
            get { return EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("Last_name"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("Last_name"), EncryptDecrypt.Encrypt(value)); }
        }
        public string UserNameCookies
        {
            get { return EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("UserNameCookies"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("UserNameCookies"), EncryptDecrypt.Encrypt(value)); }
        }
        public string UserPasswordCookies
        {
            get { return EncryptDecrypt.Decrypt(GetCookie(EncryptDecrypt.Encrypt("UserPasswordCookies"))); }
            set { SaveCookie(EncryptDecrypt.Encrypt("UserPasswordCookies"), EncryptDecrypt.Encrypt(value)); }
        }
        public string ReturnUrl
        {
            get { return HttpUtility.UrlDecode(GetCookie("ReturnUrl")); }
            set { SaveCookie("ReturnUrl", HttpUtility.UrlEncode(value)); }
        }
        public void SaveCookie(string strKey, string strValue)
        {
            if (HttpContext.Current.Request.Cookies[strKey] != null)
            {
                HttpCookie cookie = new HttpCookie(strKey);
                cookie.Value = strValue;
                var response = HttpContext.Current.Response;
                response.Cookies.Remove(strKey);
                response.Cookies.Add(cookie);
            }
            else
            {
                HttpCookie cookie = new HttpCookie(strKey);
                cookie.Value = strValue;
                cookie.HttpOnly = false;
                cookie.Expires = DateTime.Now.AddHours(8);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        public string GetCookie(string strKey)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request.Cookies[strKey] != null)
                {
                    return HttpContext.Current.Request.Cookies[strKey].Value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public void ExpireCookie()
        {
        }
    }
}