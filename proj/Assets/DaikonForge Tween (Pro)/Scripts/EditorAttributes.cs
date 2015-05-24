/* Copyright 2013-2014 Daikon Forge */
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using DaikonForge.Tween;
using DaikonForge.Tween.Components;

namespace DaikonForge.Editor
{

	using Object = UnityEngine.Object;
	using UnityEngine;

	public delegate bool InspectorConditionCallback( object target );

	/// <summary>
	/// Used to specify the order of property groups in the inspector
	/// </summary>
	[AttributeUsage( AttributeTargets.Class, Inherited = true, AllowMultiple = false )]
	public class InspectorGroupOrderAttribute : System.Attribute
	{

		public List<string> Groups = new List<string>();

		public InspectorGroupOrderAttribute( params string[] groups )
			: base()
		{
			this.Groups.AddRange( groups );
		}

	}

	/// <summary>
	/// Used to cotnrol how a field or property is displayed in the inspector
	/// </summary>
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
	public class InspectorAttribute : System.Attribute, IComparable<InspectorAttribute>
	{

		#region Public properties 

		/// <summary>
		/// Defines the group heading that the property should be included in
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Defines the property's order within the inspector group
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// If supplied, this value will be used to generate a custom label for the field in the inspector
		/// </summary>
		public string Label { get; set; }

		/// <summary>
		/// May be used to specify the backing field for a property
		/// </summary>
		public string BackingField { get; set; }

		/// <summary>
		/// May be used to specify a tooltip for the field 
		/// </summary>
		public string Tooltip { get; set; }

		#endregion 

		#region Constructors 

		public InspectorAttribute( string group )
			: base()
		{
			this.Group = group;
			this.Order = int.MaxValue;
		}

		public InspectorAttribute( string category, int order )
			: base()
		{
			this.Group = category;
			this.Order = order;
		}

		#endregion 

		#region System.Object overrides 

		public override string ToString()
		{
			return string.Format( "{0} {1} - {2}", Group, Order, Label ?? BackingField ?? "(Unknown)" );
		}

		#endregion 

		#region IComparable<InspectorAttribute> Members

		public int CompareTo( InspectorAttribute other )
		{

			if( !string.Equals( this.Group, other.Group ) )
				return this.Group.CompareTo( other.Group );

			if( this.Order != other.Order )
				return this.Order.CompareTo( other.Order );

			var thisField = this.Label ?? this.BackingField;
			var otherField = other.Label ?? other.BackingField;

			if( !string.IsNullOrEmpty( thisField ) && !string.IsNullOrEmpty( otherField ) )
				return thisField.CompareTo( otherField );

			return 0;

		}

		#endregion

	}

}