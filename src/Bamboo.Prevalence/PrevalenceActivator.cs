// Bamboo.Prevalence - a .NET object prevalence engine
// Copyright (C) 2002 Rodrigo B. de Oliveira
//
// Based on the original concept and implementation of Prevayler (TM)
// by Klaus Wuestefeld. Visit http://www.prevayler.org for details.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// http://bbooprevalence.sourceforge.net
// mailto:rodrigobamboo@users.sourceforge.net

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Bamboo.Prevalence.Implementation;
using Bamboo.Prevalence.Serialization;

namespace Bamboo.Prevalence
{
	/// <summary>
	/// Class factory for prevalence engines.
	/// </summary>
	public class PrevalenceActivator
	{
		/// <summary>
		/// Creates a new prevalence engine for the prevalent system
		/// type specified by the systemType argument.
		/// </summary>
		/// <remarks>
		/// The prevalence log files will be read from/written to the
		/// prevalenceBase directory.<br />
		/// If the directory does not exist it will be created.<br />
		/// If there are any log files already in the directory they will
		/// be used to restore the state of the system.<br />
		/// </remarks>
		/// <param name="systemType">prevalent system type, must be serializable</param>
		/// <param name="prevalenceBase">directory where to store log files</param>
		/// <returns>a new prevalence engine</returns>
		public static PrevalenceEngine CreateEngine(System.Type systemType, string prevalenceBase)
		{
			return CreateEngine(systemType, prevalenceBase, new BinaryFormatter());
		}

		/// <summary>
		/// Creates a new prevalence engine for the prevalent system
		/// type specified by the systemType argument with the option
		/// to automatically support the migration from older serialization
		/// layout versions.
		/// </summary>
		/// <param name="systemType">prevalent system type, must be serializable</param>
		/// <param name="prevalenceBase">directory where to store log files</param>
		/// <param name="autoVersionMigration">include support for auto version migration</param>
		/// <returns>a new prevalence engine</returns>
		public static PrevalenceEngine CreateEngine(System.Type systemType, string prevalenceBase, bool autoVersionMigration)
		{
			CheckEngineParameters(systemType, prevalenceBase);

			BinaryFormatter formatter;

			if (autoVersionMigration)
			{
				formatter = CreateAutoVersionMigrationFormatter(systemType);
			}
			else
			{
				formatter = new BinaryFormatter();
			}

			return new PrevalenceEngine(systemType, prevalenceBase, formatter);
		}

		/// <summary>
		/// Creates a new prevalence engine for the prevalent system
		/// type specified by the systemType argument.
		/// </summary>
		/// <param name="systemType">prevalent system type, must be serializable</param>
		/// <param name="prevalenceBase">directory where to store log files</param>
		/// <param name="formatter">serialization formatter that should be used for reading from/writing to the logs</param>
		/// <returns>a new prevalence engine</returns>
		public static PrevalenceEngine CreateEngine(System.Type systemType, string prevalenceBase, BinaryFormatter formatter)
		{
			CheckEngineParameters(systemType, prevalenceBase);
			Assertion.AssertParameterNotNull("formatter", formatter);

			return new PrevalenceEngine(systemType, prevalenceBase, formatter);
		}

		/// <summary>
		/// Creates a new transparent prevalence engine for the prevalent
		/// system type specified by the systemType argument.
		/// </summary>
		/// <remarks>
		/// A transparent prevalence engine automatically intercepts the
		/// calls made to the prevalent system and creates command objects
		/// as needed.
		/// </remarks>
		/// <param name="systemType">prevalent system type, must be serializable and inherited from
		/// System.MarshalByRefObject</param>
		/// <param name="prevalenceBase">directory where to store log files</param>
		/// <returns>a new prevalence engine</returns>
		public static PrevalenceEngine CreateTransparentEngine(System.Type systemType, string prevalenceBase)
		{
			CheckEngineParameters(systemType, prevalenceBase);
			return new TransparentPrevalenceEngine(systemType, prevalenceBase, new BinaryFormatter());
		}

		private static void CheckEngineParameters(System.Type systemType, string prevalenceBase)
		{
			Assertion.AssertParameterNotNull("systemType", systemType);
			Assertion.AssertParameter("systemType", "Prevalent system type must be serializable!", systemType.IsSerializable);
			Assertion.AssertParameterNotNull("prevalenceBase", prevalenceBase);
		}

		private static BinaryFormatter CreateAutoVersionMigrationFormatter(System.Type type)
		{
			ISurrogateSelector autoVersionMigrationSurrogate = new AutoVersionMigrationSurrogate(type.Assembly);
			BinaryFormatter formatter = new BinaryFormatter(autoVersionMigrationSurrogate, new StreamingContext(StreamingContextStates.All));
			return formatter;
		}
	}
}
