/*
   Copyright 2014-2019 Zumero, LLC

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

public static class projects
{
	// Each item in these Lists corresponds to a project file that will be
	// generated.
	//
	public static List<config_csproj> items_csproj = new List<config_csproj>();

	// nuspec files only
	public static List<config_esqlite3> items_esqlite3 = new List<config_esqlite3>();

	// This function is called by Main to initialize the project lists.
	//
	public static void init()
	{
		init_esqlite3();
	}

	private static void init_esqlite3()
	{
		items_esqlite3.Add(new config_esqlite3 { toolset="v110_xp" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v110" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v110_wp80" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v120" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v120_wp81" });
		items_esqlite3.Add(new config_esqlite3 { toolset="v140" });
	}

	public static string get_nuget_target_path(string env)
	{
		if (config_cs.env_is_portable(env))
		{
			return string.Format("lib\\{0}\\", projects.get_portable_nuget_target_string(env));
		}
		else if (env == "wp80")
		{
            // this goes into build/wp80/cpu, but the cpu isn't
            // a param to this function, so the wp80 case has to
            // be handled another way
            throw new NotImplementedException();
		}
		else
		{
			return string.Format("lib\\{0}\\", config_cs.get_nuget_framework_name(env));
		}
	}

    public static string rid_front_half(string toolset)
    {
        switch (toolset)
        {
            case "v110_xp":
				// for our builds, v110_xp should always correspond to a win-whatever RID
                return "win";
            case "v110":
                return "win8";
            case "v110_wp80":
                return "wp80";
            case "v120":
                return "win81";
            case "v120_wp81":
                return "wpa81";
            case "v140":
                return "win10";
            default:
                throw new Exception();
        }
    }

    public static string cs_env_to_toolset(string env)
    {
	    switch (env) {
		case "net45":
			return "v110_xp";
		case "net40":
			return "v110_xp";
		case "net35":
			return "v110_xp";
		case "wp80":
			return "v110_wp80";
		case "wp81_sl":
			return "v120";
		case "wpa81":
			return "v120_wp81";
		case "uwp10":
			return "v140";
		case "win8":
			return "v110";
		case "win81":
			return "v120";
		default:
			throw new Exception(env);
	    }
    }

	public static string get_portable_nuget_target_string(string env)
	{
		switch (env)
		{
			case "profile78":
				return "portable-net45+netcore45+wp8+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			case "profile259":
				return "portable-net45+netcore45+wpa81+wp8+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			case "profile111":
				return "portable-net45+netcore45+wpa81+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			case "profile158":
				return "portable-net45+sl5+netcore45+wp8+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			case "profile136":
				return "portable-net40+sl5+netcore45+wp8+MonoAndroid10+MonoTouch10+Xamarin.iOS10";
			default:
				throw new Exception(env);
		}
	}

	public static config_csproj find(string area, string env)
    {
		foreach (config_csproj cfg in projects.items_csproj)
		{
			if (cfg.area == area && cfg.env == env)
			{
				return cfg;
			}
		}
        return null;
    }

	public static config_csproj find(string name)
    {
		foreach (config_csproj cfg in projects.items_csproj)
		{
			if (cfg.name == name)
			{
				return cfg;
			}
		}
        return null;
    }

	public static config_csproj find(string area, string what, string env, string cpu)
    {
		foreach (config_csproj cfg in projects.items_csproj)
		{
			if (cfg.area == area && cfg.what == what && cfg.env == env && cfg.cpu == cpu)
			{
				return cfg;
			}
		}
        return null;
    }

    public static config_csproj find_name(string name)
    {
        config_csproj cfg = find(name);
        if (cfg != null)
        {
            return cfg;
        }
		throw new Exception(string.Format("csproj not found {0}", name));
    }

}

public interface config_info
{
	string get_name();
}

public class config_esqlite3 : config_info
{
	public string guid;
	public string toolset;

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_name(), "bin", "release", s));
	}

	public string get_name()
	{
        // TODO could include the word dynamic here
		return string.Format("lib.e_sqlite3.{0}", toolset);
	}

	public string get_title()
	{
		return string.Format("Native code only (e_sqlite3, compiled with {0}) for SQLitePCLRaw", toolset);
	}

	public string get_id()
	{
		return string.Format("{0}.{1}", gen.ROOT_NAME, get_name());
	}

}

public static class config_cs
{
	public static bool env_is_portable(string env)
	{
		return env.StartsWith("profile");
	}

	public static string get_nuget_framework_name(string env)
	{
		switch (env)
		{
			case "ios_unified":
				return "Xamarin.iOS10";
			case "macos":
				return "Xamarin.Mac20";
			case "watchos":
				return "Xamarin.WatchOS";
			case "android":
				return "MonoAndroid";
			case "net45":
				return "net45";
			case "net40":
				return "net40";
			case "net35":
				return "net35";
			case "wp80":
				return "wp8";
			case "wp81_sl":
				return "wp81";
			case "wpa81":
				return "wpa81";
			case "uwp10":
				return "uap10.0";
			case "win8":
				return "win8";
			case "win81":
				return "win81";
			case "netstandard11":
				return "netstandard1.1";
			case "netstandard10":
				return "netstandard1.0";
            case "profile111":
            case "profile136":
            case "profile259":
                return projects.get_portable_nuget_target_string(env);
            case "netcoreapp":
                return "netcoreapp";
			default:
				throw new Exception(env);
		}
	}
					
}

public class config_csproj : config_info
{
    public string area;
    public string what; // TODO call this provider_name ?
    public string name;
	public string guid;
	public string assemblyname;
	public string env;
    public string nuget_override_target_env;
    public bool CopyNuGetImplementations;
	public string cpu = "anycpu";
	public List<string> csfiles_src = new List<string>();
	public List<string> csfiles_bld = new List<string>();
	public List<string> defines = new List<string>();
	public List<string> runtimes = new List<string>();
	public Dictionary<string,string> deps = new Dictionary<string,string>();

    string root_name
    {
        get
        {
            return gen.ROOT_NAME;
        }
    }

    public string get_name()
    {
        return name;
    }

	public string fixed_cpu()
	{
		if (cpu == "anycpu")
		{
			return "Any CPU";
		}
		else
		{
			return cpu;
		}
	}

	private void add_product(List<string> a, string s)
	{
		a.Add(Path.Combine(get_name(), "bin", "release", s));
	}

	public void get_products(List<string> a)
	{
		add_product(a, string.Format("{0}.dll", assemblyname));
	}

    // TODO rm this func
	public string get_id()
	{
		return get_name();
	}

}

public static class gen
{
    public const string ROOT_NAME = "SQLitePCLRaw";

	private static void gen_assemblyinfo(config_csproj cfg, string root, string top)
	{
		string cs = File.ReadAllText(Path.Combine(root, "src/cs/AssemblyInfo.cs"));
		using (TextWriter tw = new StreamWriter(Path.Combine(top, string.Format("AssemblyInfo.{0}.cs", cfg.assemblyname))))
		{
			string cs1 = cs
				.Replace("REPLACE_WITH_ASSEMBLY_NAME", '"' + cfg.assemblyname + '"')
				.Replace("REPLACE_WITH_ASSEMBLY_VERSION", '"' + ASSEMBLY_VERSION + '"')
				;
			tw.Write(cs1);
		}
	}

	private static void write_nuspec_file_entry(config_csproj cfg, XmlWriter f)
    {
        if (cfg.nuget_override_target_env != null)
        {
            write_nuspec_file_entry(cfg, cfg.nuget_override_target_env, f);
        }
        else
        {
            write_nuspec_file_entry(cfg, cfg.env, f);
        }
    }

	private static void write_nuspec_file_entry(config_csproj cfg, string target_env, XmlWriter f)
	{
        // note that target_env may not be the same as cfg.env
        // for example we may want to build with netstandard11
        // settings, and then drop that assembly into more than
        // one place in the nuget file
		f.WriteComment(string.Format("{0}", cfg.get_name()));
		var a = new List<string>();
		cfg.get_products(a);

		foreach (string s in a)
		{
			f.WriteStartElement("file");
			f.WriteAttributeString("src", s);
			f.WriteAttributeString("target", projects.get_nuget_target_path(target_env));
			f.WriteEndElement(); // file
		}
	}

	private static void write_empty(XmlWriter f, string top, string tfm)
    {
		f.WriteComment("empty directory in lib to avoid nuget adding a reference");

		Directory.CreateDirectory(Path.Combine(Path.Combine(top, "empty"), tfm));

		f.WriteStartElement("file");
		f.WriteAttributeString("src", string.Format("empty\\{0}\\", tfm));
		f.WriteAttributeString("target", string.Format("lib\\{0}", tfm));
		f.WriteEndElement(); // file
    }

	public const int MAJOR_VERSION = 1;
	public const int MINOR_VERSION = 1;
	public const int PATCH_VERSION = 14;
	public static string NUSPEC_VERSION_PRE = string.Format("{0}.{1}.{2}-pre{3}", 
		MAJOR_VERSION,
		MINOR_VERSION,
		PATCH_VERSION,
		DateTime.Now.ToString("yyyyMMddHHmmss")
		); 
	public static string NUSPEC_VERSION_RELEASE = string.Format("{0}.{1}.{2}",
		MAJOR_VERSION,
		MINOR_VERSION,
		PATCH_VERSION
		);
	public static string NUSPEC_VERSION = NUSPEC_VERSION_PRE;
	public static string ASSEMBLY_VERSION = string.Format("{0}.{1}.{2}.{3}", 
		MAJOR_VERSION,
		MINOR_VERSION,
		PATCH_VERSION,
		(int) ((DateTime.Now - new DateTime(2018,1,1)).TotalDays) 
		); 

	private const string NUSPEC_RELEASE_NOTES = "1.1.13:  fix problems with unofficial sqlcipher builds for Android and iOS.  use new license tag for nuspecs.  1.1.12:  update e_sqlite3 builds to 3.26.0.  bug fix for bundle_zetetic on iOS.  1.1.11:  put a copy of alpine-x64/e_sqlite3 into linux-musl-x64, for .NET Core 2.1.  1.1.10:  improve bundle_zetetic.  update e_sqlite3 to 3.22.0 and turn on FTS5.  fix bundled sqlcipher build for UWP.  AssemblyVersion now being updated properly.  attempt fix crash involving CLR finalizer.  add e_sqlite3 builds for linux-arm64 and alpine-x64.  change generic Windows builds to use win-foo instead of win7-foo.  add support for SQLITE_DETERMINISTIC.  added sqlite3_blob_open overload to support higher perf in certain cases.  fix problem with Mac-but-not-Xamarin and targets file.   1.1.9:  bug fixes for Xamarin.Mac.  add a sqlcipher build for UWP.  1.1.8:  SQLite builds for .NET Core ARM, linux and Windows IoT.  Finalizers.  Fix Xam.Mac issue with bundle_green.  Fix edge case in one of the sqlite3_column_blob() overloads.  New 'bundle_zetetic' for use with official SQLCipher builds from Zetetic.  1.1.7:  Drop SQLite down to 3.18.2.  1.1.6:  AssetTargetFallback fixes.  update sqlite builds to 3.19.3.  1.1.5:  bug fix path in lib.foo.linux targets file.  1.1.4:  tweak use of nuget .targets files for compat with .NET Core.  1.1.3:  add SQLITE_CHECKPOINT_TRUNCATE symbol definition.  add new blob overloads to enable better performance in certain cases.  chg winsqlite3 to use StdCall.  fix targets files for better compat with VS 2017 nuget pack.  add 32-bit linux build for e_sqlite3.  update to latest libcrypto builds from couchbase folks.  1.1.2:  ability to FreezeProvider().  update e_sqlite3 builds to 3.16.1.  1.1.1:  add support for config_log.  update e_sqlite3 builds to 3.15.2.  fix possible memory corruption when using prepare_v2() with multiple statements.  better errmsg from ugly.step().  add win8 dep groups in bundles.  fix batteries_v2.Init() to be 'last call wins' like the v1 version is.  chg raw.SetProvider() to avoid calling sqlite3_initialize() so that sqlite3_config() can be used.  better support for Xamarin.Mac.  1.1.0:  fix problem with winsqlite3 on UWP.  remove iOS Classic support.  add sqlite3_enable_load_extension.  add sqlite3_config/initialize/shutdown.  add Batteries_V2.Init().  1.0.1:  fix problem with bundle_e_sqlite3 on iOS.  fix issues with .NET Core.  add bundle_sqlcipher.  1.0.0 release:  Contains minor breaking changes since 0.9.x.  All package names now begin with SQLitePCLRaw.  Now supports netstandard.  Fixes for UWP and Android N.  Change all unit tests to xunit.  Support for winsqlite3.dll and custom SQLite builds.";

    private static void add_dep_core(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.core", gen.ROOT_NAME));
        f.WriteAttributeString("version", NUSPEC_VERSION);
        f.WriteEndElement(); // dependency
    }

    private static void add_dep_ugly(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", string.Format("{0}.ugly", gen.ROOT_NAME));
        f.WriteAttributeString("version", NUSPEC_VERSION);
        f.WriteEndElement(); // dependency
    }

    private static void add_dep_netstandard(XmlWriter f)
    {
        f.WriteStartElement("dependency");
        f.WriteAttributeString("id", "NETStandard.Library");
        f.WriteAttributeString("version", "1.6.0");
        f.WriteEndElement(); // dependency
    }

	private static void gen_nuspec_core(string top, string root)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

        string id = string.Format("{0}.core", gen.ROOT_NAME);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
            f.WriteElementString("title", id);
			f.WriteElementString("description", "SQLitePCL.raw is a Portable Class Library (PCL) for low-level (raw) access to SQLite.  This package does not provide an API which is friendly to app developers.  Rather, it provides an API which handles platform and configuration issues, upon which a friendlier API can be built.  In order to use this package, you will need to also add one of the SQLitePCLRaw.provider.* packages and call raw.SetProvider().  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Eric Sink, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite pcl database xamarin monotouch ios monodroid android wp8 wpa netstandard uwp");

			f.WriteStartElement("dependencies");

            write_dependency_group(f, "android", DEP_NONE);
            write_dependency_group(f, "ios_unified", DEP_NONE);
            write_dependency_group(f, "macos", DEP_NONE);
            // TODO write_dependency_group(f, "watchos", DEP_NONE);
            write_dependency_group(f, "net35", DEP_NONE);
            write_dependency_group(f, "net40", DEP_NONE);
            write_dependency_group(f, "net45", DEP_NONE);
            write_dependency_group(f, "win81", DEP_NONE);
            write_dependency_group(f, "wpa81", DEP_NONE);
            write_dependency_group(f, "wp80", DEP_NONE);
            write_dependency_group(f, "uwp10", DEP_NONE);
            write_dependency_group(f, "profile111", DEP_NONE);
            write_dependency_group(f, "profile136", DEP_NONE);
            write_dependency_group(f, "profile259", DEP_NONE);
            write_dependency_group(f, "netstandard11", DEP_NONE);
            write_dependency_group(f, null, DEP_NONE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
                if (cfg.area == "core")
                {
                    write_nuspec_file_entry(
                            cfg, 
                            f
                            );
                }
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_esqlite3(string top, string cb_bin, config_esqlite3 cfg)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = cfg.get_id();
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Eric Sink, D. Richard Hipp, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			Action<string,string,string,string> write_file_entry = (toolset, flavor, arch, rid) =>
			{
				f.WriteStartElement("file");
				f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "win", toolset, flavor, arch, "e_sqlite3.dll"));
				f.WriteAttributeString("target", string.Format("runtimes\\{0}\\native\\", rid));
				f.WriteEndElement(); // file
			};

			switch (cfg.toolset)
			{
				case "v110_xp":
					write_file_entry("v110", "xp", "x86", "win-x86");
					write_file_entry("v110", "xp", "x64", "win-x64");
					write_file_entry("v140", "plain", "arm", "win8-arm");
					break;
				case "v110":
					write_file_entry("v110", "appcontainer", "arm", "win8-arm");
					write_file_entry("v110", "appcontainer", "x64", "win8-x64");
					write_file_entry("v110", "appcontainer", "x86", "win8-x86");
					break;
				case "v120":
					write_file_entry("v120", "appcontainer", "arm", "win81-arm");
					write_file_entry("v120", "appcontainer", "x64", "win81-x64");
					write_file_entry("v120", "appcontainer", "x86", "win81-x86");
					break;
				case "v140":
					write_file_entry("v140", "appcontainer", "arm", "win10-arm");
					write_file_entry("v140", "appcontainer", "x64", "win10-x64");
					write_file_entry("v140", "appcontainer", "x86", "win10-x86");
					break;
				case "v110_wp80":
					write_file_entry("v110", "wp80", "arm", "wp80-arm");
					write_file_entry("v110", "wp80", "x86", "wp80-x86");
					break;
				case "v120_wp81":
					write_file_entry("v120", "wp81", "arm", "wpa81-arm");
					write_file_entry("v120", "wp81", "x86", "wpa81-x86");
					break;
				default:
					throw new NotImplementedException(string.Format("esqlite3 nuspec: {0}", cfg.toolset));
			}

			string tname;
			switch (cfg.toolset) {
				case "v110_xp":
					tname = gen_nuget_targets_pinvoke_anycpu(top, cfg.get_id(), cfg.toolset);
                    if (tname != null) 
                    {
                        f.WriteStartElement("file");
                        f.WriteAttributeString("src", tname);
                        f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
                        f.WriteEndElement(); // file

                        write_empty(f, top, "net35");
                        write_empty(f, top, "netstandard1.0");
                        write_empty(f, top, "netstandard2.0");
                    }
					break;
				default:
					tname = gen_nuget_targets_sqlite3_itself(top, cfg.get_id(), cfg.toolset);
                    if (tname != null) 
                    {
                        f.WriteStartElement("file");
                        f.WriteAttributeString("src", tname);
                        f.WriteAttributeString("target", string.Format("build\\{0}.targets", id));
                        f.WriteEndElement(); // file
                    }
					break;
			}


			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_embedded(string top, config_csproj cfg)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = cfg.get_id();
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Eric Sink, D. Richard Hipp, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite xamarin");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

            write_nuspec_file_entry(
                    cfg, 
                    f
                    );

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_e_sqlite3(string top, string cb_bin, string plat)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("SQLitePCLRaw.lib.e_sqlite3.{0}", plat);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", string.Format("Native code only (e_sqlite3, {0}) for SQLitePCLRaw", plat));
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLite for use with SQLitePCL.raw.  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.e_sqlite3.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Eric Sink, D. Richard Hipp, et al");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			string tname = string.Format("{0}.targets", id);
            switch (plat)
            {
                case "osx":
                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "mac", "libe_sqlite3.dylib"));
                    f.WriteAttributeString("target", "runtimes\\osx-x64\\native\\libe_sqlite3.dylib");
                    f.WriteEndElement(); // file
                    gen_nuget_targets_osx(top, tname, "libe_sqlite3.dylib", forxammac: false);
                    break;
                case "linux":
                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "linux", "x64", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\linux-x64\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "linux", "x86", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\linux-x86\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "linux", "armhf", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\linux-arm\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "linux", "armsf", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\linux-armel\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "linux", "musl-x64", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\linux-musl-x64\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "linux", "musl-x64", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\alpine-x64\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    f.WriteStartElement("file");
                    f.WriteAttributeString("src", Path.Combine(cb_bin, "e_sqlite3", "linux", "arm64", "libe_sqlite3.so"));
                    f.WriteAttributeString("target", "runtimes\\linux-arm64\\native\\libe_sqlite3.so");
                    f.WriteEndElement(); // file

                    gen_nuget_targets_linux(top, tname, "libe_sqlite3.so");
                    break;
                default:
                    throw new Exception();
            }

            f.WriteStartElement("file");
            f.WriteAttributeString("src", tname);
            f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
            f.WriteEndElement(); // file

            if (plat == "osx")
            {
                write_empty(f, top, "Xamarin.Mac20");
                tname = string.Format("{0}.Xamarin.Mac20.targets", id);
                gen_nuget_targets_osx(top, tname, "libe_sqlite3.dylib", forxammac: true);

                f.WriteStartElement("file");
                f.WriteAttributeString("src", tname);
                f.WriteAttributeString("target", string.Format("build\\Xamarin.Mac20\\{0}.targets", id));
                f.WriteEndElement(); // file
            }

            write_empty(f, top, "net35");
            write_empty(f, top, "netstandard1.0");
            write_empty(f, top, "netstandard2.0");

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_sqlcipher(string top, string cb_bin, string plat)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string id = string.Format("SQLitePCLRaw.lib.sqlcipher.{0}", plat);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.8.1");

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", string.Format("Native code only (sqlcipher, {0}) for SQLitePCLRaw", plat));
			f.WriteElementString("description", "This package contains a platform-specific native code build of SQLCipher (see sqlcipher/sqlcipher on GitHub) for use with SQLitePCL.raw.  The build of SQLCipher packaged here is built and maintained by Couchbase (see couchbaselabs/couchbase-lite-libsqlcipher on GitHub).  To use this, you need SQLitePCLRaw.core as well as SQLitePCLRaw.provider.sqlcipher.net45 or similar.  Convenience packages are named SQLitePCLRaw.bundle_*.");
			f.WriteElementString("authors", "Couchbase, SQLite, Zetetic");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "A Portable Class Library (PCL) for low-level (raw) access to SQLite");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			string tname = string.Format("{0}.targets", id);
			switch (plat) {
				case "windows":
					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "win", "v140", "plain", "x86", "sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("runtimes\\win-x86\\native\\sqlcipher.dll"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "win", "v140", "plain", "x64", "sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("runtimes\\win-x64\\native\\sqlcipher.dll"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "win", "v140", "plain", "arm", "sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("runtimes\\win-arm\\native\\sqlcipher.dll"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "win", "v140", "appcontainer", "x64", "sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("runtimes\\win10-x64\\nativeassets\\uap10.0\\sqlcipher.dll"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "win", "v140", "appcontainer", "x86", "sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("runtimes\\win10-x86\\nativeassets\\uap10.0\\sqlcipher.dll"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "win", "v140", "appcontainer", "arm", "sqlcipher.dll"));
					f.WriteAttributeString("target", string.Format("runtimes\\win10-arm\\nativeassets\\uap10.0\\sqlcipher.dll"));
					f.WriteEndElement(); // file

					gen_nuget_targets_windows(top, tname, "sqlcipher.dll");
					break;
				case "osx":
					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "mac", "libsqlcipher.dylib"));
					f.WriteAttributeString("target", string.Format("runtimes\\osx-x64\\native\\libsqlcipher.dylib"));
					f.WriteEndElement(); // file

					gen_nuget_targets_osx(top, tname, "libsqlcipher.dylib", forxammac: false);
					break;
				case "linux":
					// TODO do we need amd64 version here?

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "linux", "x64", "libsqlcipher.so"));
					f.WriteAttributeString("target", string.Format("runtimes\\linux-x64\\native\\libsqlcipher.so"));
					f.WriteEndElement(); // file

					f.WriteStartElement("file");
					f.WriteAttributeString("src", Path.Combine(cb_bin, "sqlcipher", "linux", "x86", "libsqlcipher.so"));
					f.WriteAttributeString("target", string.Format("runtimes\\linux-x86\\native\\libsqlcipher.so"));
					f.WriteEndElement(); // file

					gen_nuget_targets_linux(top, tname, "libsqlcipher.so");
					break;
				default:
					throw new Exception();
			}
			f.WriteStartElement("file");
			f.WriteAttributeString("src", tname);
			f.WriteAttributeString("target", string.Format("build\\net35\\{0}.targets", id));
			f.WriteEndElement(); // file

            if (plat == "osx")
            {
                write_empty(f, top, "Xamarin.Mac20");
                tname = string.Format("{0}.Xamarin.Mac20.targets", id);
                gen_nuget_targets_osx(top, tname, "libsqlcipher.dylib", forxammac: true);

                f.WriteStartElement("file");
                f.WriteAttributeString("src", tname);
                f.WriteAttributeString("target", string.Format("build\\Xamarin.Mac20\\{0}.targets", id));
                f.WriteEndElement(); // file
            }

            write_empty(f, top, "net35");
            write_empty(f, top, "uap10.0");
            write_empty(f, top, "netstandard1.0");
            write_empty(f, top, "netstandard2.0");

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void write_license(XmlWriter f)
	{
		f.WriteStartElement("license");
		f.WriteAttributeString("type", "expression");
		f.WriteString("Apache-2.0");
		f.WriteEndElement();
	}

	private static void gen_nuspec_ugly(string top)
	{
		string id = string.Format("{0}.ugly", gen.ROOT_NAME);

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "These extension methods for SQLitePCL.raw provide a more usable API while remaining stylistically similar to the sqlite3 C API, which most C# developers would consider 'ugly'.  This package exists for people who (1) really like the sqlite3 C API, and (2) really like C#.  So far, evidence suggests that 100% of the people matching both criteria are named Eric Sink, but this package is available just in case he is not the only one of his kind.");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Extension methods for SQLitePCLRaw, providing an ugly-but-usable API");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

            write_dependency_group(f, "android", DEP_CORE);
            write_dependency_group(f, "ios_unified", DEP_CORE);
            write_dependency_group(f, "macos", DEP_CORE);
            // TODO write_dependency_group(f, "watchos", DEP_CORE);
            write_dependency_group(f, "net35", DEP_CORE);
            write_dependency_group(f, "net40", DEP_CORE);
            write_dependency_group(f, "net45", DEP_CORE);
            write_dependency_group(f, "win81", DEP_CORE);
            write_dependency_group(f, "wpa81", DEP_CORE);
            write_dependency_group(f, "wp80", DEP_CORE);
            write_dependency_group(f, "uwp10", DEP_CORE);
            write_dependency_group(f, "profile111", DEP_CORE);
            write_dependency_group(f, "profile136", DEP_CORE);
            write_dependency_group(f, "profile259", DEP_CORE);
            write_dependency_group(f, "netstandard11", DEP_CORE);
            write_dependency_group(f, null, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "ugly")
				{
					write_nuspec_file_entry(
							cfg, 
							f
							);
				}
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_bundle_winsqlite3(string top)
    {
		string id = string.Format("{0}.bundle_winsqlite3", gen.ROOT_NAME);

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: .no SQLite library included, uses winsqlite3.dll");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Batteries-included package to bring in SQLitePCL.raw and dependencies");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

			// --------
			f.WriteStartElement("group");
			f.WriteAttributeString("targetFramework", config_cs.get_nuget_framework_name("uwp10"));

			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", string.Format("{0}.core", gen.ROOT_NAME));
			f.WriteAttributeString("version", NUSPEC_VERSION);
			f.WriteEndElement(); // dependency

			f.WriteStartElement("dependency");
			f.WriteAttributeString("id", string.Format("{0}.provider.winsqlite3.uwp10", gen.ROOT_NAME));
			f.WriteAttributeString("version", NUSPEC_VERSION);
			f.WriteEndElement(); // dependency

			f.WriteEndElement(); // group

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "batteries_winsqlite3" && cfg.env != "wp80")
				{
					write_nuspec_file_entry(
							cfg, 
							f
							);
				}
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

    private const int DEP_NONE = 0;
    private const int DEP_CORE = 1;
    private const int DEP_UGLY = 2;

    private static void write_dependency_group(XmlWriter f, string env, int flags)
    {
        f.WriteStartElement("group");
        if (env != null)
        {
            f.WriteAttributeString("targetFramework", config_cs.get_nuget_framework_name(env));
            switch (env)
            {
                case "uwp10":
                case "netstandard11":
                    add_dep_netstandard(f);
                    break;
            }
        }
        if ((flags & DEP_CORE) != 0)
        {
            add_dep_core(f);
        }
        if ((flags & DEP_UGLY) != 0)
        {
            add_dep_ugly(f);
        }
        f.WriteEndElement(); // group
    }

    private static void write_bundle_dependency_group(XmlWriter f, string env, string what)
    {
        write_bundle_dependency_group(f, env, env, what, true);
    }

    private static void write_bundle_dependency_group(XmlWriter f, string env, string what, bool lib)
    {
        write_bundle_dependency_group(f, env, env, what, lib);
    }

    private static void write_bundle_dependency_group(XmlWriter f, string env_target, string env_deps, string what, bool lib)
    {
        // --------
        f.WriteStartElement("group");
        f.WriteAttributeString("targetFramework", config_cs.get_nuget_framework_name(env_target));

        add_dep_core(f);

        if (
                ((env_deps == "ios_unified") || (env_deps == "watchos"))
                && (what != "sqlite3")
           )
        {
            f.WriteStartElement("dependency");
            f.WriteAttributeString("id", string.Format("{0}.provider.{1}.{2}", gen.ROOT_NAME, "internal", env_deps));
            f.WriteAttributeString("version", NUSPEC_VERSION);
            f.WriteEndElement(); // dependency
        }
        else
        {
            f.WriteStartElement("dependency");
            f.WriteAttributeString("id", string.Format("{0}.provider.{1}.{2}", gen.ROOT_NAME, what, env_deps));
            f.WriteAttributeString("version", NUSPEC_VERSION);
            f.WriteEndElement(); // dependency
        }

        if (lib)
        {
        if (what == "e_sqlite3")
        {
            switch (env_deps)
            {
                case "android":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.android", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "macos":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "ios_unified":
                case "watchos":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.{1}.static", gen.ROOT_NAME, env_deps));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "net35":
                case "net40":
                case "net45":
                case "netstandard11": // TODO because this is used for netcoreapp, kinda hackish
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.v110_xp", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency

                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency

                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.linux", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                default:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.e_sqlite3.{1}", gen.ROOT_NAME, projects.cs_env_to_toolset(env_deps)));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
            }
        }
        else if (what == "sqlcipher")
        {
            switch (env_deps)
            {
                case "android":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.android", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "macos":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "ios_unified":
                case "watchos":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.{1}.static", gen.ROOT_NAME, env_deps));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "net35":
                case "net40":
                case "net45":
                case "netstandard11": // TODO because this is used for netcoreapp, kinda hackish
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.windows", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency

                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.osx", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency

                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.linux", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                case "uwp10":
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.windows", gen.ROOT_NAME));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
                default:
                    f.WriteStartElement("dependency");
                    f.WriteAttributeString("id", string.Format("{0}.lib.sqlcipher.{1}", gen.ROOT_NAME, projects.cs_env_to_toolset(env_deps)));
                    f.WriteAttributeString("version", NUSPEC_VERSION);
                    f.WriteEndElement(); // dependency
                    break;
            }
        }
        }

        f.WriteEndElement(); // group
    }

	enum SQLCipherBundleKind
	{
		Unofficial,
		Zetetic,
	}

	private static void gen_nuspec_bundle_sqlcipher(string top, SQLCipherBundleKind kind)
    {
		string id;
		switch (kind)
		{
			case SQLCipherBundleKind.Unofficial:
				id = string.Format("{0}.bundle_sqlcipher", gen.ROOT_NAME);
				break;
			case SQLCipherBundleKind.Zetetic:
				id = string.Format("{0}.bundle_zetetic", gen.ROOT_NAME);
				break;
			default:
				throw new NotImplementedException();
		}

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			switch (kind)
			{
				case SQLCipherBundleKind.Unofficial:
					f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: unofficial open source sqlcipher builds included.  Note that these sqlcipher builds are unofficial and unsupported.  For official sqlcipher builds, contact Zetetic.");
					break;
				case SQLCipherBundleKind.Zetetic:
					f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: reference the official SQLCipher builds from Zetetic, which are not included in this package");
					break;
				default:
					throw new NotImplementedException();
			}
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Batteries-included package to bring in SQLitePCL.raw and dependencies");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

			bool lib_deps;
			switch (kind)
			{
				case SQLCipherBundleKind.Unofficial:
					lib_deps = true;
					break;
				case SQLCipherBundleKind.Zetetic:
					lib_deps = false;
					break;
				default:
					throw new NotImplementedException();
			}

            write_bundle_dependency_group(f, "android", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "ios_unified", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "macos", "sqlcipher", lib_deps);
            // TODO write_bundle_dependency_group(f, "watchos", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "net35", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "net40", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "net45", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "netcoreapp", "netstandard11", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "wpa81", "wpa81", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "win8", "win8", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "win81", "win81", "sqlcipher", lib_deps);
            write_bundle_dependency_group(f, "uwp10", "sqlcipher");
            
            write_dependency_group(f, "profile111", DEP_CORE);
            write_dependency_group(f, "profile136", DEP_CORE);
            write_dependency_group(f, "profile259", DEP_CORE);
            write_dependency_group(f, "netstandard11", DEP_CORE);
            write_dependency_group(f, null, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "batteries_sqlcipher")
				{
					write_nuspec_file_entry(
							cfg, 
							f
							);
				}
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_bundle_e_sqlite3(string top)
	{
		string id = string.Format("{0}.bundle_e_sqlite3", gen.ROOT_NAME);

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: e_sqlite3 included");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Batteries-included package to bring in SQLitePCL.raw and dependencies");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, "android", "e_sqlite3");
            write_bundle_dependency_group(f, "ios_unified", "e_sqlite3");
            write_bundle_dependency_group(f, "macos", "e_sqlite3");
            // TODO write_bundle_dependency_group(f, "watchos", "e_sqlite3");
            write_bundle_dependency_group(f, "wpa81", "e_sqlite3");
            write_bundle_dependency_group(f, "wp80", "e_sqlite3");
            write_bundle_dependency_group(f, "win8", "e_sqlite3");
            write_bundle_dependency_group(f, "win81", "e_sqlite3");
            write_bundle_dependency_group(f, "uwp10", "e_sqlite3");
            write_bundle_dependency_group(f, "net35", "e_sqlite3");
            write_bundle_dependency_group(f, "net40", "e_sqlite3");
            write_bundle_dependency_group(f, "net45", "e_sqlite3");
            write_bundle_dependency_group(f, "netcoreapp", "netstandard11", "e_sqlite3", true);
            
            write_dependency_group(f, "profile111", DEP_CORE);
            write_dependency_group(f, "profile136", DEP_CORE);
            write_dependency_group(f, "profile259", DEP_CORE);
            write_dependency_group(f, "netstandard11", DEP_CORE);
            write_dependency_group(f, null, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "batteries_e_sqlite3" && cfg.env != "wp80")
				{
					write_nuspec_file_entry(
							cfg, 
							f
							);
				}
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static void gen_nuspec_bundle_green(string top)
	{
		string id = string.Format("{0}.bundle_green", gen.ROOT_NAME);

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, string.Format("{0}.nuspec", id)), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");

			f.WriteStartElement("metadata");
			f.WriteAttributeString("minClientVersion", "2.5"); // TODO 2.8.3 for unified

			f.WriteElementString("id", id);
			f.WriteElementString("version", NUSPEC_VERSION);
			f.WriteElementString("title", id);
			f.WriteElementString("description", "This 'batteries-included' bundle brings in SQLitePCLRaw.core and the necessary stuff for certain common use cases.  Call SQLitePCL.Batteries.Init().  Policy of this bundle: iOS=system SQLite, others=e_sqlite3 included");
			f.WriteElementString("authors", "Eric Sink");
			f.WriteElementString("owners", "Eric Sink");
			f.WriteElementString("copyright", "Copyright 2014-2019 Zumero, LLC");
			f.WriteElementString("requireLicenseAcceptance", "false");
			write_license(f);
			f.WriteElementString("projectUrl", "https://github.com/ericsink/SQLitePCL.raw");
			f.WriteElementString("releaseNotes", NUSPEC_RELEASE_NOTES);
			f.WriteElementString("summary", "Batteries-included package to bring in SQLitePCL.raw and dependencies");
			f.WriteElementString("tags", "sqlite pcl database monotouch ios monodroid android wp8 wpa");

			f.WriteStartElement("dependencies");

            write_bundle_dependency_group(f, "android", "e_sqlite3");
            write_bundle_dependency_group(f, "ios_unified", "sqlite3");
            write_bundle_dependency_group(f, "macos", "e_sqlite3");
            // TODO write_bundle_dependency_group(f, "watchos", "sqlite3");
            write_bundle_dependency_group(f, "wpa81", "e_sqlite3");
            write_bundle_dependency_group(f, "wp80", "e_sqlite3");
            write_bundle_dependency_group(f, "win8", "e_sqlite3");
            write_bundle_dependency_group(f, "win81", "e_sqlite3");
            write_bundle_dependency_group(f, "uwp10", "e_sqlite3");
            write_bundle_dependency_group(f, "net35", "e_sqlite3");
            write_bundle_dependency_group(f, "net40", "e_sqlite3");
            write_bundle_dependency_group(f, "net45", "e_sqlite3");
            write_bundle_dependency_group(f, "netcoreapp", "netstandard11", "e_sqlite3", true);

            write_dependency_group(f, "profile111", DEP_CORE);
            write_dependency_group(f, "profile136", DEP_CORE);
            write_dependency_group(f, "profile259", DEP_CORE);
            write_dependency_group(f, "netstandard11", DEP_CORE);
            write_dependency_group(f, null, DEP_CORE);

			f.WriteEndElement(); // dependencies

			f.WriteEndElement(); // metadata

			f.WriteStartElement("files");

			foreach (config_csproj cfg in projects.items_csproj)
			{
				if (cfg.area == "batteries_green" && cfg.env != "wp80")
				{
					write_nuspec_file_entry(
							cfg, 
							f
							);
				}
			}

			f.WriteEndElement(); // files

			f.WriteEndElement(); // package

			f.WriteEndDocument();
		}
	}

	private static string gen_nuget_targets_sqlite3_itself(string top, string id, string toolset)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		// TODO should we put the cpu check code here, like the original version of this function (below)?

		string tname = string.Format("{0}.targets", id);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			switch (toolset)
			{
				case "v110_xp":
					// statically linked
					break;
				case "v110":
					f.WriteStartElement("ItemGroup");
					f.WriteAttributeString("Condition", " '$(Platform.Trim().Substring(0,3).ToLower())' != 'any' ");
					f.WriteStartElement("SDKReference");
					f.WriteAttributeString("Include", "Microsoft.VCLibs, Version=11.0");
					f.WriteEndElement(); // SDKReference
					f.WriteEndElement(); // ItemGroup
					break;
				case "v120":
					f.WriteStartElement("ItemGroup");
					f.WriteAttributeString("Condition", " '$(Platform.Trim().Substring(0,3).ToLower())' != 'any' ");
					f.WriteStartElement("SDKReference");
					f.WriteAttributeString("Include", "Microsoft.VCLibs, Version=12.0");
					f.WriteEndElement(); // SDKReference
					f.WriteEndElement(); // ItemGroup
					break;
				case "v140":
#if not // TODO do we need this?  we should, but testing says we don't.
					f.WriteStartElement("ItemGroup");
					f.WriteStartElement("SDKReference");
					f.WriteAttributeString("Include", "Microsoft.VCLibs, Version=14.0");
					f.WriteEndElement(); // SDKReference
					f.WriteEndElement(); // ItemGroup
#endif
					break;
			}

			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", Guid.NewGuid().ToString()));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");

			var front = projects.rid_front_half(toolset);
			Action<string> write_item = (cpu) =>
			{
				f.WriteStartElement("ItemGroup");
				f.WriteAttributeString("Condition", string.Format(" '$(Platform.ToLower())' == '{0}' ", cpu.ToLower()));

				f.WriteStartElement("Content");
				// TODO call other.get_products() instead of hard-coding the sqlite3.dll name here
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\runtimes\\{0}-{1}\\native\\e_sqlite3.dll", front, cpu));
				// TODO link
				// TODO condition/exists ?
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
				f.WriteEndElement(); // Content

				f.WriteEndElement(); // ItemGroup
			};

			switch (toolset)
			{
				case "v110_xp":
					write_item("x86");
					write_item("x64");
					write_item("arm");
					break;
				case "v110":
					write_item("arm");
					write_item("x64");
					write_item("x86");
					break;
				case "v120":
					write_item("arm");
					write_item("x64");
					write_item("x86");
					break;
				case "v140":
					write_item("arm");
					write_item("x64");
					write_item("x86");
					break;
				case "v110_wp80":
					write_item("arm");
					write_item("x86");
					break;
				case "v120_wp81":
					write_item("arm");
					write_item("x86");
					break;
				default:
					throw new NotImplementedException();
			}

			f.WriteEndElement(); // Target

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
		return tname;
	}

	// TODO change the name of this to something like dual arch
	private static string gen_nuget_targets_pinvoke_anycpu(string top, string id, string toolset)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		string tname = string.Format("{0}.targets", id);
		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			var guid = Guid.NewGuid().ToString();
			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", guid));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");
			{
				f.WriteStartElement("Content");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}", Path.Combine("runtimes\\win-x86\\native", "e_sqlite3.dll")));
				// TODO condition/exists ?
				f.WriteElementString("Link", string.Format("{0}\\e_sqlite3.dll", "x86"));
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
				f.WriteEndElement(); // Content
			}
			{
				f.WriteStartElement("Content");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\{0}", Path.Combine("runtimes\\win-x64\\native", "e_sqlite3.dll")));
				// TODO condition/exists ?
				f.WriteElementString("Link", string.Format("{0}\\e_sqlite3.dll", "x64"));
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
				f.WriteEndElement(); // Content
			}
			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Target

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ResolveAssemblyReferencesDependsOn", 
					string.Format("$(ResolveAssemblyReferencesDependsOn);InjectReference_{0}", guid));
			f.WriteEndElement(); // PropertyGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
		return tname;
	}

	private static void gen_nuget_targets_windows(string top, string tname, string filename)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			var guid = Guid.NewGuid().ToString();
			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", guid));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Windows_NT' ");

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\win-x86\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x86", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\win-x64\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x64", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content

			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Target

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ResolveAssemblyReferencesDependsOn", 
					string.Format("$(ResolveAssemblyReferencesDependsOn);InjectReference_{0}", guid));
			f.WriteEndElement(); // PropertyGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void gen_nuget_targets_osx(string top, string tname, string filename, bool forxammac)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			var guid = Guid.NewGuid().ToString();
			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", guid));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Unix' AND Exists('/Library/Frameworks') ");

			if (forxammac)
			{
				f.WriteStartElement("NativeReference");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\osx-x64\\native\\{0}", filename));
				f.WriteElementString("Kind", "Dynamic");
				f.WriteElementString("SmartLink", "False");
				f.WriteEndElement(); // NativeReference
			}
			else
			{
				f.WriteStartElement("Content");
				f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\osx-x64\\native\\{0}", filename));
				f.WriteElementString("Link", filename);
				f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
				f.WriteElementString("Pack", "false");
				f.WriteEndElement(); // Content
			}

			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Target

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ResolveAssemblyReferencesDependsOn", 
					string.Format("$(ResolveAssemblyReferencesDependsOn);InjectReference_{0}", guid));
			f.WriteEndElement(); // PropertyGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void gen_nuget_targets_linux(string top, string tname, string filename)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = false;

		using (XmlWriter f = XmlWriter.Create(Path.Combine(top, tname), settings))
		{
			f.WriteStartDocument();
			f.WriteComment("Automatically generated");

			f.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
			f.WriteAttributeString("ToolsVersion", "4.0");

			var guid = Guid.NewGuid().ToString();
			f.WriteStartElement("Target");
			f.WriteAttributeString("Name", string.Format("InjectReference_{0}", guid));
			f.WriteAttributeString("BeforeTargets", "ResolveAssemblyReferences");

			f.WriteStartElement("ItemGroup");
			f.WriteAttributeString("Condition", " '$(OS)' == 'Unix' AND !Exists('/Library/Frameworks') ");

#if TODO // load library before dllimport doesn't seem to work on linux
			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\linux-x64\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x64", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content

			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\linux-x86\\native\\{0}", filename));
			f.WriteElementString("Link", string.Format("{0}\\{1}", "x86", filename));
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content
#else
			f.WriteStartElement("Content");
			f.WriteAttributeString("Include", string.Format("$(MSBuildThisFileDirectory)..\\..\\runtimes\\linux-x64\\native\\{0}", filename));
			f.WriteElementString("Link", filename);
			f.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
            f.WriteElementString("Pack", "false");
			f.WriteEndElement(); // Content
#endif

			f.WriteEndElement(); // ItemGroup

			f.WriteEndElement(); // Target

			f.WriteStartElement("PropertyGroup");
			f.WriteElementString("ResolveAssemblyReferencesDependsOn", 
					string.Format("$(ResolveAssemblyReferencesDependsOn);InjectReference_{0}", guid));
			f.WriteEndElement(); // PropertyGroup

			f.WriteEndElement(); // Project

			f.WriteEndDocument();
		}
	}

	private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

    static void replace(string path, string oldstr, string newstr)
    {
        string txt = File.ReadAllText(path);
        using (TextWriter tw = new StreamWriter(path))
        {
            string cs1 = txt.Replace(oldstr, newstr);
            tw.Write(cs1);
        }
    }

    static void fix_version(string path)
    {
        replace(path, "1.0.0-PLACEHOLDER", NUSPEC_VERSION);
    }

    static void fix_guid(string path, string guid)
    {
        var a = File.ReadAllLines(path);
        using (TextWriter tw = new StreamWriter(path))
        {
            foreach (var s in a)
            {
                if (s.Contains("ProjectGuid"))
                {
                    tw.WriteLine("<ProjectGuid>{0}</ProjectGuid>", guid);
                }
                else
                {
                    tw.WriteLine(s);
                }
            }
        }
    }

	public static void Main(string[] args)
	{
		projects.init();

		string root = Directory.GetCurrentDirectory(); // assumes that gen_build.exe is being run from the root directory of the project
		string top = Path.Combine(root, "bld");
		var cb_bin = Path.GetFullPath(Path.Combine(root, "..", "cb", "bld", "bin"));

		// --------------------------------
		// create the bld directory
		Directory.CreateDirectory(top);

		// --------------------------------
		// generate all the AssemblyInfo files

		foreach (config_csproj cfg in projects.items_csproj)
		{
			gen_assemblyinfo(cfg, root, top);
		}

		// --------------------------------

        gen_nuspec_core(top, root);
        gen_nuspec_ugly(top);
        gen_nuspec_bundle_green(top);
        gen_nuspec_bundle_e_sqlite3(top);
        gen_nuspec_bundle_winsqlite3(top);
        gen_nuspec_bundle_sqlcipher(top, SQLCipherBundleKind.Unofficial);
        gen_nuspec_bundle_sqlcipher(top, SQLCipherBundleKind.Zetetic);

		foreach (config_csproj cfg in projects.items_csproj)
		{
            if (cfg.area == "lib")
            {
                gen_nuspec_embedded(top, cfg);
            }
		}

		foreach (config_esqlite3 cfg in projects.items_esqlite3)
		{
			gen_nuspec_esqlite3(top, cb_bin, cfg);
		}

		gen_nuspec_e_sqlite3(top, cb_bin, "osx");
		gen_nuspec_e_sqlite3(top, cb_bin, "linux");

		gen_nuspec_sqlcipher(top, cb_bin, "windows");
		gen_nuspec_sqlcipher(top, cb_bin, "osx");
		gen_nuspec_sqlcipher(top, cb_bin, "linux");

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "build.ps1")))
		{
			// our build configs for ancient PCL profiles require an old version of nuget
			tw.WriteLine("../nuget_old.exe restore sqlitepcl.sln");
			tw.WriteLine("msbuild /p:Configuration=Release sqlitepcl.sln");
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "pack.ps1")))
		{
            tw.WriteLine("../nuget pack {0}.core.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.ugly.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_green.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_e_sqlite3.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_sqlcipher.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_zetetic.nuspec", gen.ROOT_NAME);
            tw.WriteLine("../nuget pack {0}.bundle_winsqlite3.nuspec", gen.ROOT_NAME);

			tw.WriteLine("../nuget pack {0}.lib.e_sqlite3.osx.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../nuget pack {0}.lib.e_sqlite3.linux.nuspec", gen.ROOT_NAME);

			tw.WriteLine("../nuget pack {0}.lib.sqlcipher.windows.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../nuget pack {0}.lib.sqlcipher.osx.nuspec", gen.ROOT_NAME);
			tw.WriteLine("../nuget pack {0}.lib.sqlcipher.linux.nuspec", gen.ROOT_NAME);

			foreach (config_csproj cfg in projects.items_csproj)
			{
                if (cfg.area == "lib")
                {
                    string id = cfg.get_id();
                    tw.WriteLine("../nuget pack {0}.nuspec", id);
                }
			}
			foreach (config_esqlite3 cfg in projects.items_esqlite3)
			{
				string id = cfg.get_id();
				tw.WriteLine("../nuget pack {0}.nuspec", id);
			}
			tw.WriteLine("ls *.nupkg");
		}

		using (TextWriter tw = new StreamWriter(Path.Combine(top, "push.ps1")))
		{
            const string src = "https://www.nuget.org/api/v2/package";

			tw.WriteLine("ls *.nupkg");
			tw.WriteLine("../nuget push -Source {2} {0}.core.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.ugly.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_green.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_e_sqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_sqlcipher.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_zetetic.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.bundle_winsqlite3.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			tw.WriteLine("../nuget push -Source {2} {0}.lib.e_sqlite3.osx.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.lib.e_sqlite3.linux.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			tw.WriteLine("../nuget push -Source {2} {0}.lib.sqlcipher.windows.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.lib.sqlcipher.osx.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);
			tw.WriteLine("../nuget push -Source {2} {0}.lib.sqlcipher.linux.{1}.nupkg", gen.ROOT_NAME, NUSPEC_VERSION, src);

			foreach (config_csproj cfg in projects.items_csproj)
			{
                if (cfg.area == "lib")
                {
                    string id = cfg.get_id();
                    tw.WriteLine("../nuget push -Source {2} {0}.{1}.nupkg", id, NUSPEC_VERSION, src);
                }
			}
			foreach (config_esqlite3 cfg in projects.items_esqlite3)
			{
				string id = cfg.get_id();
				tw.WriteLine("../nuget push -Source {2} {0}.{1}.nupkg", id, NUSPEC_VERSION, src);
			}
		}
	}
}

