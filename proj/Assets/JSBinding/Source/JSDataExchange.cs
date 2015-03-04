using System;
using System.Text;
using UnityEngine;

public class JSDataExchange 
{
    public static string simpleGetArgvString = "JSApi.JSh_ArgvStringS(vc.cx, vc.vp, vc.currIndex++);";
    public static string simpleGetArgvBool = "JSApi.JSh_ArgvBool(vc.cx, vc.vp, vc.currIndex++);";
    public static string simpleGetArgvInt = "JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++);";

    public static string simpleReturnString = "JSApi.JSh_SetRvalString(vc.cx, vc.vp, {0});";
    public static string simpleReturnBool = "JSApi.JSh_SetRvalBool(vc.cx, vc.vp, {0});";
    public static string simpleReturnInt = "JSApi.JSh_SetRvalInt(vc.cx, vc.vp, {0});";

    // get value from param
    public virtual string Get_GetParam() { return string.Empty; }
    public virtual string Get_Return(string expVar) { return string.Empty; }

    JSVCall vc;

    enum eGetType { Param, };

    public double getNumberic(eGetType e = eGetType.Param) {
        switch (e) {
            
        // js has only int32 and double, so...
        int i = vc.currIndex++;
        if (JSApi.JSh_ArgvIsDouble(vc.cx, vc.vp, i))
            return (Double)JSApi.JSh_ArgvDouble(vc.cx, vc.vp, i);
        else
            return (Double)JSApi.JSh_ArgvInt(vc.cx, vc.vp, i);
        }
    }

    public Boolean getBool() { return JSApi.JSh_ArgvBool(vc.cx, vc.vp, vc.currIndex++); }
    public String getString() { return JSApi.JSh_ArgvStringS(vc.cx, vc.vp, vc.currIndex++); }
    public Char getChar() { return (Char)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public Byte getByte() { return (Byte)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public SByte getSByte() { return (SByte)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public UInt16 getUInt16() { return (UInt16)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public Int16 getInt16() { return (Int16)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public UInt32 getUInt32() { return (UInt32)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public Int32 getInt32() { return (Int32)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public UInt64 getUInt64() { return (UInt64)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public Int64 getInt64() { return (Int64)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public Int32 getEnum() { return (Int32)JSApi.JSh_ArgvInt(vc.cx, vc.vp, vc.currIndex++); }
    public Single getFloat(eGetType e = eGetType.Param)
    {
        return (Single)getNumberic(e);
    }
    public Double getDouble()
    {
        return (Double)getNumberic(e);
    }
}

public class JSDataExchange_String : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvString; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnString, expVar).ToString(); }
}

public class JSDataExchange_Bool : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvBool; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnBool, expVar).ToString(); }
}

public class JSDataExchange_Char : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_Byte : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_SByte : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_UInt16 : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_Int32 : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_UInt32 : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_Int64 : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_UInt64 : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_Enum : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_Float : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}

public class JSDataExchange_Double : JSDataExchange
{
    public override string Get_GetParam() { return simpleGetArgvInt; }
    public override string Get_Return(string expVar) { return (new StringBuilder()).AppendFormat(simpleReturnInt, expVar).ToString(); }
}