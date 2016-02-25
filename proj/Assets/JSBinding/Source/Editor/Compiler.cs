using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Reflection;

public class Compiler
{
	static bool CallSharpKitToCompile(string allInvokeOutputPath, string allInvokeWithLocationOutputPath, string YieldReturnTypeOutputPath)
	{
		string workingDir = Application.dataPath.Replace("/Assets", "").Replace("/", "\\");
		
		cg.args args = new cg.args();
		
		// working dir
		if (workingDir.Contains(" "))
			args.AddFormat("/dir:\"{0}\"", workingDir);
		else
			args.AddFormat("/dir:{0}", workingDir);
		
		// define		
		string define = "TRACE;DEBUG;UNITY_EDITOR;JS";

        #if UNITY_4_6
        define += ";UNITY_4_6";
        #endif
        
        #if UNITY_4_7
        define += ";UNITY_4_7";
        #endif
        
        #if UNITY_5
        define += ";UNITY_5";
        #endif
        #if UNITY_5_1
        define += ";UNITY_5_1";
        #endif
        #if UNITY_5_2
        define += ";UNITY_5_2";
        #endif

        // NOVA!
		if (PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).IndexOf("USEAB") >= 0)
			define += ";USEAB";
		
		args.AddFormat("/define:{0}", define);
		
		// references
		System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		
		foreach (var asm in assemblies)
		{
			string r = asm.Location;
			
			if (r.Contains(" "))
				args.AddFormat("/reference:\"{0}\"", r.Replace("/", "\\"));
			else
				args.AddFormat("/reference:{0}", r.Replace("/", "\\"));
		}
		
		// out, target, target framework version
		args.Add("/out:obj/Debug/SharpKitProj.dll");
		args.Add("/target:library");
		args.Add("/TargetFrameworkVersion:v3.5");
		args.AddFormat("/AllInvocationsOutput:\"{0}\"", allInvokeOutputPath);
		args.AddFormat("/AllInvocationsWithLocationOutput:\"{0}\"", allInvokeWithLocationOutputPath);
		args.AddFormat("/YieldReturnTypeOutput:\"{0}\"", YieldReturnTypeOutputPath);
		
		// source files
		string[] sources = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
		foreach (var s in sources)
		{
			args.Add(s.Replace("/", "\\"));
		}
		
		// 把参数写到文件中，然后把这个文件路径做为参数传递给 skc5.exe
		string argFile = JSAnalyzer.GetTempFileNameFullPath("skc_args.txt");
		string strArgs = args.Format(cg.args.ArgsFormat.Space);
		File.WriteAllText(argFile, strArgs);
		
		string exePath = workingDir + "\\Compiler\\skc5.exe ";
		System.Diagnostics.Process process = System.Diagnostics.Process.Start(exePath, "\"" + argFile + "\"");
		// 等待结束
		process.WaitForExit();
		
		int exitCode = process.ExitCode;
		if (exitCode != 0)
		{
			Debug.LogError("Compile failed. exit code = " + exitCode);
			return false;
		}
		else
		{
			Debug.Log("Compile success.");
			return true;
		}
	}

	public class Location
	{
		public string FileName;
		public int Line;
	}

	// 加载文本文件
	// 内容是所有 Logic 调用 Framework 的代码信息
	// Dict: className -> (memberName -> locations)
	static Dictionary<string, Dictionary<string, List<Location>>> LoadAllInvoked(string path)
	{
		var D = new Dictionary<string, Dictionary<string, List<Location>>>();

		string content = File.ReadAllText(path);
		string[] lines = content.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

		Dictionary<string, List<Location>> E = null;
		List<Location> L = null;
		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;

			if (line[0] == '[')
			{
				string typeName = line.Substring(1, line.Length - 2);
				E = new Dictionary<string, List<Location>>();
				D.Add(typeName, E);
				continue;
			}

			bool b4 = line.Length >= 4 && line.Substring(0, 4) == "    ";
			bool b8 = line.Length >= 8 && line.Substring(0, 8) == "        ";

			if (b4 && !b8)
			{
				L = new List<Location>();
				E.Add(line.Substring(4), L);
			}
			else if (b8)
			{
				string[] loc = line.Substring(8).Split(',');
				//int index = int.Parse(loc[0]);
				L.Add(new Location { FileName = loc[1], Line = int.Parse(loc[2]) });
			}
			else 
				throw new Exception("Line is invalid: '" + line + "'");
		}

		return D;
	}
	// 加载文本文件
	// 内容是所有 导出到JS的
	// Dict: className -> member names
	static Dictionary<string, HashSet<string>> LoadAllExported(string path)
	{
		var D = new Dictionary<string, HashSet<string>>();
		
		string content = File.ReadAllText(path);
		string[] lines = content.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

		HashSet<string> L = null;
		foreach (var line in lines)
		{
			if (string.IsNullOrEmpty(line))
				continue;
			
			if (line[0] == '[')
			{
				string typeName = line.Substring(1, line.Length - 2);
				L = new HashSet<string>();
				D.Add(typeName, L);
			}
			else if (line.Substring(0, 4) == "    ")
			{
				L.Add(line.Substring(4));
			}
			else
				throw new Exception("Line is invalid: " + line);
		}
		
		return D;
	}

	static Dictionary<string, List<string>> typesImpByJs = null;
	static void CheckError_Invocation(string allInvokeWithLocationOutputPath)
	{
		if (typesImpByJs == null)
		{
			typesImpByJs = new Dictionary<string, List<string>>();
			typesImpByJs["T"] = new List<string> { "T" };
			typesImpByJs["System.Action"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Action$1"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Action$2"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Action$3"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Action$4"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Func$1"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Func$2"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Func$3"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Func$4"] = new List<string> { ""/* 调用 action */ };
			typesImpByJs["System.Exception"] = new List<string> { "ctor$$String" };
			typesImpByJs["System.NotImplementedException"] = new List<string> { "ctor" };
			typesImpByJs["System.Array"] = new List<string> { "length", "CopyTo", "Static_Sort$1$$T$Array" };
			typesImpByJs["System.Collections.Generic.List$1"] = new List<string> 
			{
				"ctor", "ctor$$IEnumerable$1", "ctor$$Int32", "RemoveRange", "Clear",
				"get_Item$$Int32", "set_Item$$Int32",
				"get_Count", "GetEnumerator", "ToArray",
				"AddRange", "Add", "Remove", "Contains",
				"SetItems", "IndexOf", "Exists", "IndexOf$$T", "Insert",
				"RemoveAt", "RemoveAll",
				//"TryRemove",
				//"CopyTo",
				//"get_IsReadOnly",
				"Reverse", "Sort", "Sort$$Comparison$1", "ForEach", "Find",
			};
			typesImpByJs["System.Collections.Generic.Dictionary$2"] = new List<string> 
			{
				"ctor", "Add", "Remove", "get_Item$$TKey", "set_Item$$TKey", "ContainsKey", "GetEnumerator",
				"Clear", "TryGetValue", "get_Count", "get_Keys", "get_Values"
			};
			typesImpByJs["System.Collections.Generic.KeyValuePair$2"] = new List<string>
			{
				"get_Key", "get_Value", "ctor$$TKey$$TValue",
			};
			typesImpByJs["System.Collections.Generic.Dictionary.ValueCollection$2"] = new List<string> { "CopyTo"};  // 特殊！
			typesImpByJs["System.Collections.Generic.Dictionary.KeyCollection$2"] = new List<string> { "CopyTo"};  // 特殊！
			typesImpByJs["System.Linq.Enumerable"] = new List<string> { "Static_ToArray$1" };
			typesImpByJs["System.Collections.Generic.HashSet$1"] = new List<string>
			{
				"ctor", "Add", "get_Count", "Clear", "Contains", "Remove"
			};
			typesImpByJs["System.Collections.Generic.Queue$1"] = new List<string>
			{
				"ctor", "ctor$$Int32", "Clear", "get_Count", "Enqueue", "Dequeue", "Peek", "Contains", "ToArray",
			};
			typesImpByJs["System.String"] = new List<string>
			{
				// native
				"toString", "length", "replace", "split", "indexOf","substr", "charAt", 
				/// static
				"Static_Empty", "Static_Format$$String$$Object", "Static_Format$$String$$Object$$Object", "Static_Format$$String$$Object$$Object$$Object",
				"Static_IsNullOrEmpty",
				// instance
				"Insert", "Substring$$Int32", "Substring$$Int32$$Int32", "Substring",
				"ToLower", "ToUpper", "getItem", "IndexOf$$String", "IndexOf$$Char",
				"LastIndexOf", "LastIndexOf$$Char", "LastIndexOf$$String", "Remove$$Int32", "Remove$$Int32$$Int32", "StartsWith$$String",
				"EndsWith$$String", "Contains", "get_Length", "Split$$Char$Array",
				"trim", "Trim", "ltrim", "rtrim",
				"Static_Format$$String$$Object$Array",
				"Replace$$String$$String", "Replace$$Char$$Char",
				"PadLeft$$Int32$$Char", "PadRight$$Int32$$Char"
			};
			typesImpByJs["System.Char"] = new List<string> { "toString", "Static_IsNumber$$Char" };
			typesImpByJs["System.Int32"] = new List<string> { "toString", "Static_Parse$$String", "Static_TryParse$$String$$Int32"  };
			typesImpByJs["System.UInt64"] = new List<string> { "toString", "Static_Parse$$String", "Static_TryParse$$String$$UInt64"  };
			typesImpByJs["System.Int64"] = new List<string> { "toString", "Static_Parse$$String", "Static_TryParse$$String$$Int64"  };
			typesImpByJs["System.Boolean"] = new List<string> { "toString",  };
			typesImpByJs["System.Double"] = new List<string> { "toString",  };
			typesImpByJs["System.Single"] = new List<string> { "toString", "Static_Parse$$String", };
//			typesImpByJs["System.Int32"] = new List<string> { "toString", "Static_Parse$$String",  };
			typesImpByJs["System.Enum"] = new List<string> { "toString" };
			typesImpByJs["System.MulticastDelegate"] = new List<string>();
		}


		string allExportedMembersFile = JSAnalyzer.GetAllExportedMembersFile();
		
		Dictionary<string, Dictionary<string, List<Location>>> allInvoked = LoadAllInvoked(allInvokeWithLocationOutputPath);
		Dictionary<string, HashSet<string>> allExported = LoadAllExported(allExportedMembersFile);
		foreach (var KV in typesImpByJs)
		{
			HashSet<string> HS = null;
			if (!allExported.TryGetValue(KV.Key, out HS))
			{
				HS = new HashSet<string>();
				allExported.Add(KV.Key, HS);
			}
			if (KV.Value == null)
				continue;

			foreach (var m in KV.Value)
			{
				if (!HS.Contains(m))
					HS.Add(m);
			}
		}
		
		StringBuilder sbError = new StringBuilder();
		
		int errCount = 0;
		foreach (var KV in allInvoked)
		{
			string typeName = KV.Key;
			HashSet<string> hsExported;
			Dictionary<string, List<Location>> DInvoked = KV.Value;
			
			// 类有导出吗？
			if (!allExported.TryGetValue(typeName, out hsExported))
			{
				errCount++;
				sbError.AppendFormat("[{0}] not exported.", typeName);
				sbError.AppendLine();
				foreach (var KV2 in DInvoked)
				{
					string methodName = KV2.Key;
					sbError.AppendFormat("      {0}", methodName);
					sbError.AppendLine();

					foreach (var loc in KV2.Value)
					{
						sbError.AppendFormat("        {0} {1}", loc.FileName, loc.Line);
                        sbError.AppendLine();
                  	}
                }
			}
			else
			{
				foreach (var KV2 in DInvoked)
				{
					string methodName = KV2.Key;
					// 函数可用/有导出吗
					if (hsExported == null || !hsExported.Contains(methodName))
					{
						errCount++;
						sbError.AppendFormat("[{0}].{1} not valid.", typeName, methodName);
						sbError.AppendLine();

						foreach (var loc in KV2.Value)
						{
							sbError.AppendFormat("        {0} {1}", loc.FileName, loc.Line);
							sbError.AppendLine();
						}
					}
				}
			}
		}
		
		string fullpath = JSAnalyzer.GetTempFileNameFullPath("CompilerCheckErrorResult.txt");
		File.Delete(fullpath);

		if (errCount > 0)
		{
			File.WriteAllText(fullpath, sbError.ToString());

			string relPath = fullpath.Replace("\\", "/").Substring(fullpath.IndexOf("Assets/"));
			UnityEngine.Object context = Resources.LoadAssetAtPath<UnityEngine.Object>(relPath);
			Debug.LogError("Check invocation error result: (" + errCount + " errors) （点击此条可定位文件）", context);
			Debug.LogError(sbError);
		}
		else
		{
			Debug.Log("Check invocation error result: 0 error");
		}
	}

	static void CheckError_Inheritance()
	{
		StringBuilder sb = new StringBuilder();
		int errCount = 0;
		foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (a.FullName.Contains("Assembly-CSharp")) // 可能是 Assembly-CSharp-Editor
			{
				Type[] types = a.GetTypes();
				foreach (var type in types)
				{
					bool toJS = JSSerializerEditor.WillTypeBeTranslatedToJavaScript(type);
					if (!toJS)
						continue;

					Type baseType = type.BaseType;
					if (baseType == null || 
					    baseType == typeof(System.Object) ||
					    baseType == typeof(System.Enum) ||
					    baseType == typeof(System.ValueType) ||
					    baseType == typeof(UnityEngine.MonoBehaviour))
					{
						continue;
					}

					// 特赦
//					if (baseType == typeof(Swift.Component) ||
//					    baseType == typeof(Swift.Port) ||
//					    baseType == typeof(Swift.PortAgent))
//					{
//						continue;
//					}


					bool baseToJS = JSSerializerEditor.WillTypeBeTranslatedToJavaScript(baseType);
					if (!baseToJS)
					{
						errCount++;
						sb.AppendFormat("[{0}] 继承自 [{1}]，是否应该放 Framework？", type.FullName, baseType.FullName);
						sb.AppendLine();
					}
				}
			}
		}
		if (errCount > 0)
			Debug.LogError(sb.ToString());
	}

	[MenuItem("JSB/Compile Cs to Js", false, 130)]
	public static void CompileCsToJs()
	{
		// 这个用于查看
		string allInvokeOutputPath = JSAnalyzer.GetTempFileNameFullPath("AllInvocations.txt");
		// 这个用于分析
		string allInvokeWithLocationOutputPath = JSAnalyzer.GetTempFileNameFullPath("AllInvocationsWithLocation.txt");
		// 
		string YieldReturnTypeOutputPath = JSAnalyzer.GetTempFileNameFullPath("YieldReturnTypes.txt");

		// 编译
		if (!CallSharpKitToCompile(allInvokeOutputPath, allInvokeWithLocationOutputPath, YieldReturnTypeOutputPath))
		{
			return;
		}

		// 纠正协程代码
		//JSAnalyzer.CorrectJavaScriptYieldCode();
		
		// 查错
		CheckError_Invocation(allInvokeWithLocationOutputPath);
		CheckError_Inheritance();

		// 生成JSC
#if UNITY_EDITOR_WIN
		JSCGenerator.ConvertJavaScriptToBytecode();
		JSCGenerator.ChangeExtensionJsc2Bytes();
#endif

		AssetDatabase.Refresh();

		// 提示生成 yield 结果
		string relPath = YieldReturnTypeOutputPath.Replace("\\", "/").Substring(YieldReturnTypeOutputPath.IndexOf("Assets/"));
		UnityEngine.Object context = Resources.LoadAssetAtPath<UnityEngine.Object>(relPath);
		Debug.Log("生成了文件 " + relPath + "，请检查（点击此条可定位文件）", context);
	}
}
