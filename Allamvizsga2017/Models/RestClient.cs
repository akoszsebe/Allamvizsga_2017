using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Allamvizsga2017.Models
{
    class RestClient
    {
        static string ip = "allamvizsga-akoszsebe.c9users.io";//"192.168.0.106"//fekete feher szurke
        static string port = "";//":8080";


        public static bool Login(LoginUser user)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/login?user_email=" + user.Email +
                "&password=" + user.Password);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 8000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<bool>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Register(LoginUser user)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/setuser?user_email=" + user.Email +
                "&password=" + user.Password);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 12000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<bool>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool AddUserHouse(string user_email, string house_id)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/setuserhouse?user_email=" + user_email +
                "&house_id=" + house_id);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 12000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<bool>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool AddUserSmartWatch(string user_email, string smartwatch_id)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/setusersmartwatch?user_email=" + user_email +
                "&smartwatch_id=" + smartwatch_id);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 12000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<bool>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteUserHouse(string user_email, string house_id)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/deleteuserhouse?user_email=" + user_email +
                "&house_id=" + house_id);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 12000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<bool>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<House> GetHouses()
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/gethouses");
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 3000;
            
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return null;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<List<House>>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<House> GetUserHouses(string user_email)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/getuserhouses"
            +"?user_email="+user_email);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 3000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return null;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<List<House>>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<House> GetHousesById(string house_id)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/gethousesbyid" +
                "?house_id=" + house_id);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 3000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return null;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<List<House>>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Amper> GetActualDevices(string house_id)
        {
            var request = WebRequest.Create("http://" + ip + "" + port + "/getamper?" +
            //var request = WebRequest.Create("http://" + "192.168.1.210" + "" + ":8081" + "/getamper?" +
                "house_id=" + house_id.ToString());
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 2000;
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return null;
                        }
                        else
                        {

                            response.Close();
                            return JsonConvert.DeserializeObject<List<Amper>>(content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Amper> GetAmpers(string house_id, long datefrom, long dateto)
        {
            var request = WebRequest.Create("http://" + ip + "" + port + "/getAmpers?" +
                "house_id=" + house_id.ToString() +
                "&date_from=" + datefrom+
                "&date_to=" + dateto);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 3000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return null;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<List<Amper>>(content);
                        }
                    }                    
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool SetDeviceSetting(string house_id,string name, int icon_id,int value,int valuedelay)
        {
            var request = WebRequest.Create("http://" + ip + "" + port + "/setdevicesetting?" +
                "house_id=" + house_id +
                "&name=" + name +
                "&icon_id=" + icon_id + "&value=" + value + "&valuedelay=" + valuedelay);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 2000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteDeviceSetting(string house_id, int value)
        {
            var request = WebRequest.Create("http://" + ip + "" + port + "/deletedevicesetting?" +
                "house_id=" + house_id +
                "&value=" + value);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 2000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return true;
                        }
                    }                    
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<Device> GetDeviceSetting(string house_id)
        {
            var request = WebRequest.Create("http://" + ip + "" + port + "/getdevicesetting?" +
                "house_id=" + house_id.ToString());
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 3000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return null;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<List<Device>>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Device> GetActualDevicesWithSetting(string house_id)
        {
            var request = WebRequest.Create("http://" + ip + "" + port + "/getdeviceswithsettings?" +
                "house_id=" + house_id.ToString());
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 3000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return null;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<List<Device>>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool RegisterHouse(string house_id, string house_name)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/sethouse?house_id=" + house_id +
                "&house_name=" + house_name);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 12000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<bool>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<SmartWatch> GetUserSmartWatches(string user_email)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/getusersmartwatch"
            + "?user_email=" + user_email);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 3000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return null;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<List<SmartWatch>>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool RequestResetCode(string user_email)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/resetuserpassword"
            + "?user_email=" + user_email);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 3000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<bool>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static bool VerifyResetCode(string user_email,string reset_code)
        {
            var request = WebRequest.Create(@"http://" + ip + "" + port + "/getresetuserpassword"
            + "?user_email=" + user_email
            + "&reset_code=" + reset_code);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Timeout = 4000;

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        return false;
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            response.Close();
                            return false;
                        }
                        else
                        {
                            response.Close();
                            return JsonConvert.DeserializeObject<bool>(content);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}