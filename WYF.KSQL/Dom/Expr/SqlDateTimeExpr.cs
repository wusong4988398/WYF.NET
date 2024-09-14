using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlDateTimeExpr : SqlExpr
    {
        // Fields
        private int _timeType;
        private string dataTimeWord;
        private int date;
        private int hour;
        private int millisecond;
        private int minute;
        private int month;
        private int second;
        public const int TYPE_DATE = -19001;
        public const int TYPE_TIME = -19002;
        public const int TYPE_TIMESTAMP = -19000;
        private int year;

        // Methods
        public SqlDateTimeExpr() : base(0x15)
        {
            this._timeType = -19000;
            this.dataTimeWord = "TS";
        }

        public SqlDateTimeExpr(int year, int month, int date) : base(0x15)
        {
            this._timeType = -19000;
            this.dataTimeWord = "TS";
            this.year = year;
            this.month = month;
            this.date = date;
        }

        public SqlDateTimeExpr(int year, int month, int date, int hour, int minute, int second) : base(0x15)
        {
            this._timeType = -19000;
            this.dataTimeWord = "TS";
            this.year = year;
            this.month = month;
            this.date = date;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }

        public SqlDateTimeExpr(int year, int month, int date, int hour, int minute, int second, int millisecond) : base(0x15)
        {
            this._timeType = -19000;
            this.dataTimeWord = "TS";
            this.year = year;
            this.month = month;
            this.date = date;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.millisecond = millisecond;
        }

        public override object Clone()
        {
            SqlDateTimeExpr expr = new SqlDateTimeExpr
            {
                year = this.year,
                month = this.month,
                date = this.date,
                hour = this.hour,
                minute = this.minute,
                second = this.second,
                millisecond = this.millisecond
            };
            expr.setDataTimeWord(this.getDataTimeWord());
            return expr;
        }

        public string getDataTimeWord()
        {
            return this.dataTimeWord;
        }

        public int getDate()
        {
            return this.date;
        }

        public int getHour()
        {
            return this.hour;
        }

        public DateTime getJavaDate()
        {
            return new DateTime(this.year, this.month, this.date, this.hour, this.minute, this.second);
        }

        public int getMillisecond()
        {
            return this.millisecond;
        }

        public int getMinute()
        {
            return this.minute;
        }

        public int getMonth()
        {
            return this.month;
        }

        public int getSecond()
        {
            return this.second;
        }

        public int getYear()
        {
            return this.year;
        }

        public void setDataTimeWord(string dataTimeWord)
        {
            this.dataTimeWord = dataTimeWord;
        }

        public void setDate(int value)
        {
            this.date = value;
        }

        public void setHour(int value)
        {
            this.hour = value;
        }

        public void setMillisecond(int millisecond)
        {
            this.millisecond = millisecond;
        }

        public void setMinute(int value)
        {
            this.minute = value;
        }

        public void setMonth(int value)
        {
            this.month = value;
        }

        public void setSecond(int value)
        {
            this.second = value;
        }

        public void setTimeType(int type)
        {
            this._timeType = type;
        }

        public void setYear(int value)
        {
            this.year = value;
        }

        public int timeType()
        {
            return this._timeType;
        }
    }





}
