
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace cg
{
    public class args
    {
        public args() { sb = new StringBuilder(); lst = new List<string>(); }
        StringBuilder sb;
        List<string> lst;
        public args Add(string s)
        {
            lst.Add(s);
            return this;
        }
        public args Add(params object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                sb.Remove(0, sb.Length); 
                sb.AppendFormat("{0}", objs[i]);
                this.Add(sb.ToString());
            }
            return this;
        }
        public args AddFormat(string format, params object[] objs)
        {
            sb.Remove(0, sb.Length);            
            sb.AppendFormat(format, objs);
            lst.Add(sb.ToString());
            return this;
        }
        public args Clear()
        {
            sb.Remove(0, sb.Length);
            lst.Clear();
            return this;
        }
        public override string ToString()
        {
            sb.Remove(0, sb.Length);
            for (int i = 0; i < lst.Count; i++)
            {
                sb.Append(lst[i]);
                if (i != lst.Count - 1)
                    sb.Append(", ");
            }
            return sb.ToString();
        }
    }
}