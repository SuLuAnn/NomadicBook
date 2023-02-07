using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NomadicBook.Utils
{
    public class Global
    {
        public enum SeekStatus : byte
        {
            Request = 1,
            Match = 2,
            Cancile = 3,
            Complete = 4
        }
        public enum TradeMode : byte 
        {
            StoreToStore=1,
            Delivery = 2,
            MailBox=3,
            FaceTrade=4
        }
        public enum MainCategory : byte
        {
            [Description("中文書")]
            Chinese =1,
            [Description("簡體書")]
            SimplifiedChinese =2,
            [Description("外文書")]
            ForeignLanguage =3
        }
        public enum StringLimit : int
        {
            [Description("留言")]
            Message =2000,
            [Description("ISBN")]
            ISBN =13,
            [Description("書名")]
            BookName =400,
            [Description("作者")]
            Author =200,
            [Description("出版社")]
            PublishingHouse =100,
            [Description("心得")]
            Experience =2000,
            [Description("簡介")]
            Introduction =536870912,
            [Description("書況")]
            Condition =100,
            [Description("地址")]
            Address =200,
            [Description("地址")]
            Path =50,
            [Description("備註")]
            Detail =50,
            [Description("姓名")]
            TrueName =15,
            [Description("手機")]
            CellphoneNumber =15,
            [Description("暱稱")]
            NickName =8,
            [Description("密碼")]
            Password =18,
            [Description("Email")]
            Email =64,
            [Description("自我介紹")]
            SelfIntroduction =100
        }
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        public static string ChangeTime(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        public static string ChangeTime(DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }
            var dateWord = (DateTime)date;
            return dateWord.ToString("yyyy-MM-dd");

        }
        public static DateTime ChangeTime(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", null);
        }
        public static string ChangeHourTime(DateTime date)
        {
            return date.ToString("yyyy-MM-dd hh:mm tt");
        }
        public enum MessengerNum : byte
        {
            Request = 1,
            Match = 2,
            OthersCancile = 3,
            Send = 4,
            Receive=5,
            LeaveMessage=6,
            SelfCancile=7
        }
        public static string ReplaceNull(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return value;
        }
        public static double ReplaceUndefined(double value,int num)
        {
            if (value == 0 || num==0)
            {
                return 0;
            }
            return value/num;
        }
        public static string IsCorrectLength(string word, StringLimit limit)
        {
            if (word == null)
            {
                return null;
            }
            int max = (int)limit;
            if (word.Length <= max)
            {
                return null;
            }
            return $"{GetDescription(limit)}不能超過{max}個字" ;
        }
        public static double ReplaceDouble(double? value)
        {
            if (value == null)
            {
                return 0;
            }
            return (double)value;
        }
    }
}
