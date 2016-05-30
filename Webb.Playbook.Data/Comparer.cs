using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Webb.Playbook.Data
{
    public enum CharType
    {
        Number = 0,
        Normal = 1
    }

    public class Model
    {
        private string _oValue;
        public string oValue
        {
            set { _oValue = value; }
            get { return _oValue; }
        }

        private CharType _cType;
        public CharType cType
        {
            set { _cType = value; }
            get { return _cType; }
        }

        public Model(string o, CharType c)
        {
            _oValue = o;
            _cType = c;
        }
    }

    // 09-29-2011 Scott
    public class FileNameComparer : IComparer
    {
        public int Compare(object a, object b)
        {
            string s1 = a.ToString();
            string s2 = b.ToString();

            if (s1.LastIndexOf('.') == -1 && s2.LastIndexOf('.') == -1)
            {
                return SubCompare(s1, s2);
            }
            else if (s1.LastIndexOf('.') != -1 && s2.LastIndexOf('.') != -1)
            {
                int pos1 = s1.LastIndexOf(".");
                int pos2 = s2.LastIndexOf(".");
                string ss1 = s1.Substring(0, pos1);
                string ss2 = s2.Substring(0, pos2);
                int result = SubCompare(ss1, ss2);
                if (result == 0)
                {
                    string is1 = s1.Substring(pos1 + 1);
                    string is2 = s2.Substring(pos2 + 1);
                    return SubCompare(is1, is2);
                }
                return result;
            }
            else
            {
                int pos1 = s1.LastIndexOf(".");
                int pos2 = s2.LastIndexOf(".");
                return pos1 > pos2 ? 1 : -1;
            }
        }

        int SubCompare(string s1, string s2)
        {
            List<Model> q1 = null;
            List<Model> q2 = null;

            q1 = StoreQueue(s1);
            q2 = StoreQueue(s2);
            if (q1 == null)
                return -1;
            if (q2 == null)
                return 1;

            int len = q1.Count;
            if (len > q2.Count)
                len = q2.Count;
            for (int i = 0; i < len; i++)
            {
                if (q1[i].cType != q2[i].cType)
                {
                    return q1[i].oValue[0] > q2[i].oValue[0] ? 1 : -1;
                }
                else
                {
                    if (q1[i].oValue == q2[i].oValue)
                    {
                        continue;
                    }
                    if (q1[i].cType == CharType.Number)
                    {
                        if (q1[i].oValue != q2[i].oValue)
                        {
                            Int64 num1 = Int64.Parse(q1[i].oValue);
                            Int64 num2 = Int64.Parse(q2[i].oValue);
                            return num1 > num2 ? 1 : -1;
                        }
                    }
                    else
                    {
                        if (q1[i].oValue != q2[i].oValue)
                        {
                            int rlen = q1[i].oValue.Length;
                            if (rlen > q2[i].oValue.Length)
                                rlen = q2[i].oValue.Length;
                            for (int j = 0; j < rlen; j++)
                            {
                                if (q1[i].oValue[j] != q2[i].oValue[j])
                                    return q1[i].oValue[j] > q2[i].oValue[j] ? 1 : -1;
                            }
                            return q1[i].oValue.Length > q2[i].oValue.Length ? 1 : -1;
                        }
                    }
                }
            }
            if (q1.Count != q2.Count)
                return q1.Count > q2.Count ? 1 : -1;
            else
                return 0;
        }

        CharType GetCharType(char c)
        {
            if (c >= 48 && c <= 57)
                return CharType.Number;
            else return CharType.Normal;
        }

        public List<Model> StoreQueue(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 0)
            {
                return null;
            }

            List<Model> sl = new List<Model>();
            Model m = null;
            CharType ctype = GetCharType(str[0]);
            if (str.Length == 1)
            {
                m = new Model(str, ctype);
                sl.Add(m);
                return sl;
            }
            int start = 0;
            for (int i = 1; i < str.Length; i++)
            {
                if (GetCharType(str[i]) != ctype)
                {
                    m = new Model(str.Substring(start, i - start), ctype);
                    sl.Add(m);
                    if (i == str.Length - 1)
                    {
                        CharType sType = GetCharType(str[i]);
                        m = new Model(str[i].ToString(), sType);
                        sl.Add(m);
                    }
                    else
                    {
                        ctype = GetCharType(str[i]);
                        start = i;
                    }
                }
                else
                {
                    if (i == str.Length - 1)
                    {
                        CharType sType = GetCharType(str[i]);
                        m = new Model(str.Substring(start, i + 1 - start), sType);
                        sl.Add(m);
                    }
                }
            }
            return sl;
        }
    }
}
