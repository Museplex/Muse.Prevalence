#region license
// Bamboo.Prevalence - a .NET object prevalence engine
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Based on the original concept and implementation of Prevayler (TM)
// by Klaus Wuestefeld. Visit http://www.prevayler.org for details.
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// http://bbooprevalence.sourceforge.net
// mailto:rodrigobamboo@users.sourceforge.net
#endregion

using System;
using System.IO;

namespace Bamboo.Prevalence.Configuration
{
	/// <summary>
	/// Stores configuration settings for the Bamboo.Prevalence engine.
	/// This class allows you to use .NET config files to automatically
	/// set up prevalent systems and make them available to your
	/// code at runtime.
	/// </summary>
	/// <example>
	/// <code>
	/// <![CDATA[
	/// <!--
	///		This is a sample configuration file that demonstrates
	///		the use of Bamboo.Prevalence.Configuration.PrevalenceSettings
	///	-->
	/// <configuration>
	///		<!--
	///			First you need to declare the section bamboo.prevalence,
	///			note that you have to spell the section name exactly
	///			like this: bamboo.prevalence
	///		-->
	///		<configSections>
	///			<section name="bamboo.prevalence" type="Bamboo.Prevalence.Configuration.PrevalenceSettings, Bamboo.Prevalence" />
	///		</configSections>
	///
	///		<!-- Add the section to your .config file -->		
	///		<bamboo.prevalence>
	///		
	///			<!--
	///			Add any engines you want. The configuration
	///			handler will automatically set the engines up
	///			for you and make them available to your code
	///			through PrevalenceSettings.GetEngine.
	///			-->
	///			<engines>
	///			
	///				<add id="tasks"
	///					type="Bamboo.Prevalence.Demo.TaskSystem, TaskAssembly"
	///					base="c:\tasks" />
	///					
	///				<add id="documents"
	///					type="Bamboo.Prevalence.Demo.DocumentSystem, DocumentAssembly"
	///					base="c:\documents"
	///					autoVersionMigration="true" />
	///					
	///				<add id="intranet"
	///					type="Bamboo.Prevalence.Demo.IntranetSystem, IntranetAssembly"
	///					base="c:\intranet"
	///					engineType="Transparent" />
	///					
	///			</engines>
	///		</bamboo.prevalence>
	///	</configuration>
	///	]]>
	///	
	///	The application code:
	///
	///	using Bamboo.Prevalence;
	///	using Bamboo.Prevalence.Configuration;
	///	
	///	public class App
	///	{
	///		public static void Main(string[] args)
	///		{
	///			PrevalenceEngine tasksEngine = PrevalenceSettings.GetEngine("tasks");
	///			// use the engine...
	///		}
	///	}
	///	</code>
	/// </example>
	public class PrevalenceSettings : System.Configuration.IConfigurationSectionHandler
	{	
		/// <summary>
		/// Should the engines flush data to disk after every command?
		/// </summary>
		public static bool FlushAfterCommand = true;

		/// <summary>
		/// Type of engine to be configured.
		/// </summary>
		public enum EngineType
		{
			/// <summary>
			/// Bamboo.Prevalence.PrevalenceEngine
			/// </summary>
			Normal,

			/// <summary>
			/// Bamboo.Prevalence.TransparentEngine
			/// </summary>
			Transparent
		}

		/// <summary>
		/// Config file section name that <b>MUST</b> be used for 
		/// Bamboo.Prevalence.Configuration.PrevalenceSettings.
		/// </summary>
		public const string SectionName = "bamboo.prevalence";		

		/// <summary>
		/// Returns the current ConfigurationSettings instance.
		/// </summary>
		/// <exception cref="System.Configuration.ConfigurationException">in the case Bamboo.Prevalence
		/// was not correctly configured</exception>		
		public static PrevalenceSettings Current
		{
			get
			{
				PrevalenceSettings current = (PrevalenceSettings)System.Configuration.ConfigurationSettings.GetConfig(SectionName);
				if (null == current)
				{
					throw new System.Configuration.ConfigurationException("Bamboo.Prevalence is not configured for the current application!");
				}
				return current;
			}
		}
		
		/// <summary>
		/// Same as PrevalenceSettings.Current[id]. See
		/// <see cref="PrevalenceSettings.Current"/>.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static PrevalenceEngine GetEngine(string id)
		{
			return PrevalenceSettings.Current[id];
		}

		private System.Collections.IDictionary _engines;

		/// <summary>
		/// Creates a ConfigurationSettings from a XML
		/// configuration section node.
		/// </summary>
		/// <param name="section">configuration section node</param>
		private PrevalenceSettings(System.Xml.XmlNode section)
		{
			_engines = SetUpEngines(section);
		}	

		/// <summary>
		/// Do not call this constructor! it will be called
		/// by the system at configuration loading time.
		/// </summary>
		public PrevalenceSettings()
		{
		}

		/// <summary>
		/// Gets a configured engine.
		/// </summary>
		/// <param name="id">engine id</param>		
		/// <exception cref="System.Configuration.ConfigurationException">in the case Bamboo.Prevalence
		/// was not correctly configured</exception>
		/// <exception cref="ArgumentException">if the engine was not found</exception>
		public PrevalenceEngine this[string id]
		{
			get
			{
				if (null == _engines)
				{
					throw new System.Configuration.ConfigurationException("Bamboo.Prevalence was not configured for this application!");
				}

				PrevalenceEngine engine = (PrevalenceEngine)_engines[id];
				if (null == engine)
				{
					throw new ArgumentException(string.Format("Engine {0} not found!", id), "id");
				}
				return engine;
			}
		}

		#region Implementation of IConfigurationSectionHandler
		object System.Configuration.IConfigurationSectionHandler.Create(object parent, object configContext, System.Xml.XmlNode section)
		{			
			if (!section.LocalName.Equals(SectionName))
			{
				InvalidSectionNameError(section);
			}
			return new PrevalenceSettings(section);
		}	
		#endregion			

		private System.Collections.IDictionary SetUpEngines(System.Xml.XmlNode section)
		{		
			System.Xml.XmlNodeList list = section.SelectNodes("engines/add");
			System.Collections.Hashtable engines = new System.Collections.Hashtable(list.Count);
			foreach (System.Xml.XmlElement item in list)
			{
				SetUpEngine(engines, item);
			}			
			return engines;
		}

		private void SetUpEngine(System.Collections.Hashtable engines, System.Xml.XmlElement item)
		{
			string id = GetRequiredAttribute(item, "id");
			string type = GetRequiredAttribute(item, "type");
			string prevalenceBase = GetOptionalPrevalenceBase(item, id);
			bool autoVersionMigration = bool.Parse(GetOptionalAttribute(item, "autoVersionMigration", "false"));
			EngineType engineType = (EngineType) EngineType.Parse(typeof(EngineType), 
				GetOptionalAttribute(item, "engineType", "Normal"));

			System.Type systemType = System.Type.GetType(type);
			if (null == systemType)
			{
				TypeNotFoundError(type, item);
			}

			try
			{
				switch (engineType)
				{
					case EngineType.Transparent:
						engines[id] = PrevalenceActivator.CreateTransparentEngine(systemType, prevalenceBase, autoVersionMigration);
						break;

					case EngineType.Normal:
						engines[id] = PrevalenceActivator.CreateEngine(systemType, prevalenceBase, autoVersionMigration, null);
						break;

					default:
						throw new PrevalenceException(prevalenceBase, "Invalid value for engineType attribute. Please specify one of [Normal, Transparent].");
				}
			}
			catch (System.Exception x)
			{
				SetUpError(x, item);
			}			
		}

		private string GetOptionalAttribute(System.Xml.XmlElement element, string name, string defaultValue)
		{
			if (element.HasAttribute(name))
			{
				return element.GetAttribute(name);
			}
			return defaultValue;
		}

		private string GetOptionalPrevalenceBase(System.Xml.XmlElement element, string id)
		{
			if (element.HasAttribute("base"))
			{
				return element.GetAttribute("base");
			}
			return Path.Combine(GetPersonalPrevalenceFolder(), id);
		}

		private string GetPersonalPrevalenceFolder()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "prevalence");
		}

		private string GetRequiredAttribute(System.Xml.XmlElement element, string name)
		{
			string value = element.GetAttribute(name);
			if (null == value || 0 == value.Length)
			{
				AttributeRequiredError(name, element);
			}
			return value;
		}

		private void InvalidSectionNameError(System.Xml.XmlNode section)
		{
			throw new System.Configuration.ConfigurationException(
				"Bamboo.Prevalence configuration section must be named " + SectionName + "!", section
				);
		}	

		private void TypeNotFoundError(string type, System.Xml.XmlNode section)
		{
			throw new System.Configuration.ConfigurationException(
				"Type " + type + " not found!", section
				);
		}

		private void SetUpError(Exception inner, System.Xml.XmlNode section)
		{
			throw new System.Configuration.ConfigurationException(
				"Error setting up prevalent engine! Source Error: " + inner, inner, section
				);
		}

		private void AttributeRequiredError(string name, System.Xml.XmlNode section)
		{
			throw new System.Configuration.ConfigurationException(
				"Attribute " + name + " expected!", section
				);
		}
	}
}
