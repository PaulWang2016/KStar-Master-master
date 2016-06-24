//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace aZaaS.KStar.Authentication
{
    
    
    /// <summary>
    /// The UserAuthProviderSection Configuration Section.
    /// </summary>
    public partial class UserAuthProviderSection : global::System.Configuration.ConfigurationSection
    {
        
        #region Singleton Instance
        /// <summary>
        /// The XML name of the UserAuthProviderSection Configuration Section.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string UserAuthProviderSectionSectionName = "userAuthProvider";
        
        /// <summary>
        /// The XML path of the UserAuthProviderSection Configuration Section.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string UserAuthProviderSectionSectionPath = "userAuthProvider";
        
        /// <summary>
        /// Gets the UserAuthProviderSection instance.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public static global::aZaaS.KStar.Authentication.UserAuthProviderSection Instance
        {
            get
            {
                return ((global::aZaaS.KStar.Authentication.UserAuthProviderSection)(global::System.Configuration.ConfigurationManager.GetSection(global::aZaaS.KStar.Authentication.UserAuthProviderSection.UserAuthProviderSectionSectionPath)));
            }
        }
        #endregion
        
        #region Xmlns Property
        /// <summary>
        /// The XML name of the <see cref="Xmlns"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string XmlnsPropertyName = "xmlns";
        
        /// <summary>
        /// Gets the XML namespace of this Configuration Section.
        /// </summary>
        /// <remarks>
        /// This property makes sure that if the configuration file contains the XML namespace,
        /// the parser doesn't throw an exception because it encounters the unknown "xmlns" attribute.
        /// </remarks>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.UserAuthProviderSection.XmlnsPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public string Xmlns
        {
            get
            {
                return ((string)(base[global::aZaaS.KStar.Authentication.UserAuthProviderSection.XmlnsPropertyName]));
            }
        }
        #endregion
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region DefaultProvider Property
        /// <summary>
        /// The XML name of the <see cref="DefaultProvider"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string DefaultProviderPropertyName = "defaultProvider";
        
        /// <summary>
        /// Gets or sets the DefaultProvider.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.ComponentModel.DescriptionAttribute("The DefaultProvider.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.UserAuthProviderSection.DefaultProviderPropertyName, IsRequired=true, IsKey=false, IsDefaultCollection=false)]
        public virtual string DefaultProvider
        {
            get
            {
                return ((string)(base[global::aZaaS.KStar.Authentication.UserAuthProviderSection.DefaultProviderPropertyName]));
            }
            set
            {
                base[global::aZaaS.KStar.Authentication.UserAuthProviderSection.DefaultProviderPropertyName] = value;
            }
        }
        #endregion
        
        #region UserExistsValidation Property
        /// <summary>
        /// The XML name of the <see cref="UserExistsValidation"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string UserExistsValidationPropertyName = "userExistsValidation";
        
        /// <summary>
        /// Gets or sets the UserExistsValidation.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.ComponentModel.DescriptionAttribute("The UserExistsValidation.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.UserAuthProviderSection.UserExistsValidationPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false, DefaultValue=true)]
        public virtual bool UserExistsValidation
        {
            get
            {
                return ((bool)(base[global::aZaaS.KStar.Authentication.UserAuthProviderSection.UserExistsValidationPropertyName]));
            }
            set
            {
                base[global::aZaaS.KStar.Authentication.UserAuthProviderSection.UserExistsValidationPropertyName] = value;
            }
        }
        #endregion
        
        #region AuthProviders Property
        /// <summary>
        /// The XML name of the <see cref="AuthProviders"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string AuthProvidersPropertyName = "authProviders";
        
        /// <summary>
        /// Gets or sets the AuthProviders.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.ComponentModel.DescriptionAttribute("The AuthProviders.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.UserAuthProviderSection.AuthProvidersPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=true)]
        public virtual global::aZaaS.KStar.Authentication.AuthProviderCollection AuthProviders
        {
            get
            {
                return ((global::aZaaS.KStar.Authentication.AuthProviderCollection)(base[global::aZaaS.KStar.Authentication.UserAuthProviderSection.AuthProvidersPropertyName]));
            }
            set
            {
                base[global::aZaaS.KStar.Authentication.UserAuthProviderSection.AuthProvidersPropertyName] = value;
            }
        }
        #endregion
    }
}
namespace aZaaS.KStar.Authentication
{
    
    
    /// <summary>
    /// A collection of AuthProviderElement instances.
    /// </summary>
    [global::System.Configuration.ConfigurationCollectionAttribute(typeof(global::aZaaS.KStar.Authentication.AuthProviderElement), CollectionType=global::System.Configuration.ConfigurationElementCollectionType.BasicMapAlternate, AddItemName=global::aZaaS.KStar.Authentication.AuthProviderCollection.AuthProviderElementPropertyName)]
    public partial class AuthProviderCollection : global::System.Configuration.ConfigurationElementCollection
    {
        
        #region Constants
        /// <summary>
        /// The XML name of the individual <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> instances in this collection.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string AuthProviderElementPropertyName = "authProvider";
        #endregion
        
        #region Overrides
        /// <summary>
        /// Gets the type of the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <returns>The <see cref="global::System.Configuration.ConfigurationElementCollectionType"/> of this collection.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public override global::System.Configuration.ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return global::System.Configuration.ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        
        /// <summary>
        /// Gets the name used to identify this collection of elements
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        protected override string ElementName
        {
            get
            {
                return global::aZaaS.KStar.Authentication.AuthProviderCollection.AuthProviderElementPropertyName;
            }
        }
        
        /// <summary>
        /// Indicates whether the specified <see cref="global::System.Configuration.ConfigurationElement"/> exists in the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="elementName">The name of the element to verify.</param>
        /// <returns>
        /// <see langword="true"/> if the element exists in the collection; otherwise, <see langword="false"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        protected override bool IsElementName(string elementName)
        {
            return (elementName == global::aZaaS.KStar.Authentication.AuthProviderCollection.AuthProviderElementPropertyName);
        }
        
        /// <summary>
        /// Gets the element key for the specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="global::System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified <see cref="global::System.Configuration.ConfigurationElement"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        protected override object GetElementKey(global::System.Configuration.ConfigurationElement element)
        {
            return ((global::aZaaS.KStar.Authentication.AuthProviderElement)(element)).Name;
        }
        
        /// <summary>
        /// Creates a new <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        protected override global::System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new global::aZaaS.KStar.Authentication.AuthProviderElement();
        }
        #endregion
        
        #region Indexer
        /// <summary>
        /// Gets the <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public global::aZaaS.KStar.Authentication.AuthProviderElement this[int index]
        {
            get
            {
                return ((global::aZaaS.KStar.Authentication.AuthProviderElement)(base.BaseGet(index)));
            }
        }
        
        /// <summary>
        /// Gets the <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public global::aZaaS.KStar.Authentication.AuthProviderElement this[object name]
        {
            get
            {
                return ((global::aZaaS.KStar.Authentication.AuthProviderElement)(base.BaseGet(name)));
            }
        }
        #endregion
        
        #region Add
        /// <summary>
        /// Adds the specified <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> to the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="authProvider">The <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> to add.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public void Add(global::aZaaS.KStar.Authentication.AuthProviderElement authProvider)
        {
            base.BaseAdd(authProvider);
        }
        #endregion
        
        #region Remove
        /// <summary>
        /// Removes the specified <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> from the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="authProvider">The <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> to remove.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public void Remove(global::aZaaS.KStar.Authentication.AuthProviderElement authProvider)
        {
            base.BaseRemove(this.GetElementKey(authProvider));
        }
        #endregion
        
        #region GetItem
        /// <summary>
        /// Gets the <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public global::aZaaS.KStar.Authentication.AuthProviderElement GetItemAt(int index)
        {
            return ((global::aZaaS.KStar.Authentication.AuthProviderElement)(base.BaseGet(index)));
        }
        
        /// <summary>
        /// Gets the <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> with the specified key.
        /// </summary>
        /// <param name="name">The key of the <see cref="global::aZaaS.KStar.Authentication.AuthProviderElement"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public global::aZaaS.KStar.Authentication.AuthProviderElement GetItemByKey(string name)
        {
            return ((global::aZaaS.KStar.Authentication.AuthProviderElement)(base.BaseGet(((object)(name)))));
        }
        #endregion
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
    }
}
namespace aZaaS.KStar.Authentication
{
    
    
    /// <summary>
    /// The AuthProviderElement Configuration Element.
    /// </summary>
    public partial class AuthProviderElement : global::System.Configuration.ConfigurationElement
    {
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region Name Property
        /// <summary>
        /// The XML name of the <see cref="Name"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string NamePropertyName = "name";
        
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.ComponentModel.DescriptionAttribute("The Name.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.AuthProviderElement.NamePropertyName, IsRequired=true, IsKey=true, IsDefaultCollection=false)]
        public virtual string Name
        {
            get
            {
                return ((string)(base[global::aZaaS.KStar.Authentication.AuthProviderElement.NamePropertyName]));
            }
            set
            {
                base[global::aZaaS.KStar.Authentication.AuthProviderElement.NamePropertyName] = value;
            }
        }
        #endregion
        
        #region AssemblyType Property
        /// <summary>
        /// The XML name of the <see cref="AssemblyType"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string AssemblyTypePropertyName = "assemblyType";
        
        /// <summary>
        /// Gets or sets the AssemblyType.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.ComponentModel.DescriptionAttribute("The AssemblyType.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.AuthProviderElement.AssemblyTypePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public virtual string AssemblyType
        {
            get
            {
                return ((string)(base[global::aZaaS.KStar.Authentication.AuthProviderElement.AssemblyTypePropertyName]));
            }
            set
            {
                base[global::aZaaS.KStar.Authentication.AuthProviderElement.AssemblyTypePropertyName] = value;
            }
        }
        #endregion
        
        #region Parameters Property
        /// <summary>
        /// The XML name of the <see cref="Parameters"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string ParametersPropertyName = "parameters";
        
        /// <summary>
        /// Gets or sets the Parameters.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.ComponentModel.DescriptionAttribute("The Parameters.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.AuthProviderElement.ParametersPropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=true)]
        public virtual global::aZaaS.KStar.Authentication.ParameterCollection Parameters
        {
            get
            {
                return ((global::aZaaS.KStar.Authentication.ParameterCollection)(base[global::aZaaS.KStar.Authentication.AuthProviderElement.ParametersPropertyName]));
            }
            set
            {
                base[global::aZaaS.KStar.Authentication.AuthProviderElement.ParametersPropertyName] = value;
            }
        }
        #endregion
    }
}
namespace aZaaS.KStar.Authentication
{
    
    
    /// <summary>
    /// The ParameterElement Configuration Element.
    /// </summary>
    public partial class ParameterElement : global::System.Configuration.ConfigurationElement
    {
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
        
        #region Key Property
        /// <summary>
        /// The XML name of the <see cref="Key"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string KeyPropertyName = "key";
        
        /// <summary>
        /// Gets or sets the Key.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.ComponentModel.DescriptionAttribute("The Key.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.ParameterElement.KeyPropertyName, IsRequired=true, IsKey=true, IsDefaultCollection=false)]
        public virtual string Key
        {
            get
            {
                return ((string)(base[global::aZaaS.KStar.Authentication.ParameterElement.KeyPropertyName]));
            }
            set
            {
                base[global::aZaaS.KStar.Authentication.ParameterElement.KeyPropertyName] = value;
            }
        }
        #endregion
        
        #region Value Property
        /// <summary>
        /// The XML name of the <see cref="Value"/> property.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string ValuePropertyName = "value";
        
        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        [global::System.ComponentModel.DescriptionAttribute("The Value.")]
        [global::System.Configuration.ConfigurationPropertyAttribute(global::aZaaS.KStar.Authentication.ParameterElement.ValuePropertyName, IsRequired=false, IsKey=false, IsDefaultCollection=false)]
        public virtual string Value
        {
            get
            {
                return ((string)(base[global::aZaaS.KStar.Authentication.ParameterElement.ValuePropertyName]));
            }
            set
            {
                base[global::aZaaS.KStar.Authentication.ParameterElement.ValuePropertyName] = value;
            }
        }
        #endregion
    }
}
namespace aZaaS.KStar.Authentication
{
    
    
    /// <summary>
    /// A collection of ParameterElement instances.
    /// </summary>
    [global::System.Configuration.ConfigurationCollectionAttribute(typeof(global::aZaaS.KStar.Authentication.ParameterElement), CollectionType=global::System.Configuration.ConfigurationElementCollectionType.BasicMapAlternate, AddItemName=global::aZaaS.KStar.Authentication.ParameterCollection.ParameterElementPropertyName)]
    public partial class ParameterCollection : global::System.Configuration.ConfigurationElementCollection
    {
        
        #region Constants
        /// <summary>
        /// The XML name of the individual <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> instances in this collection.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        internal const string ParameterElementPropertyName = "parameter";
        #endregion
        
        #region Overrides
        /// <summary>
        /// Gets the type of the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <returns>The <see cref="global::System.Configuration.ConfigurationElementCollectionType"/> of this collection.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public override global::System.Configuration.ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return global::System.Configuration.ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        
        /// <summary>
        /// Gets the name used to identify this collection of elements
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        protected override string ElementName
        {
            get
            {
                return global::aZaaS.KStar.Authentication.ParameterCollection.ParameterElementPropertyName;
            }
        }
        
        /// <summary>
        /// Indicates whether the specified <see cref="global::System.Configuration.ConfigurationElement"/> exists in the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="elementName">The name of the element to verify.</param>
        /// <returns>
        /// <see langword="true"/> if the element exists in the collection; otherwise, <see langword="false"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        protected override bool IsElementName(string elementName)
        {
            return (elementName == global::aZaaS.KStar.Authentication.ParameterCollection.ParameterElementPropertyName);
        }
        
        /// <summary>
        /// Gets the element key for the specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="global::System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified <see cref="global::System.Configuration.ConfigurationElement"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        protected override object GetElementKey(global::System.Configuration.ConfigurationElement element)
        {
            return ((global::aZaaS.KStar.Authentication.ParameterElement)(element)).Key;
        }
        
        /// <summary>
        /// Creates a new <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/>.
        /// </returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        protected override global::System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new global::aZaaS.KStar.Authentication.ParameterElement();
        }
        #endregion
        
        #region Indexer
        /// <summary>
        /// Gets the <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public global::aZaaS.KStar.Authentication.ParameterElement this[int index]
        {
            get
            {
                return ((global::aZaaS.KStar.Authentication.ParameterElement)(base.BaseGet(index)));
            }
        }
        
        /// <summary>
        /// Gets the <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public global::aZaaS.KStar.Authentication.ParameterElement this[object key]
        {
            get
            {
                return ((global::aZaaS.KStar.Authentication.ParameterElement)(base.BaseGet(key)));
            }
        }
        #endregion
        
        #region Add
        /// <summary>
        /// Adds the specified <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> to the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> to add.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public void Add(global::aZaaS.KStar.Authentication.ParameterElement parameter)
        {
            base.BaseAdd(parameter);
        }
        #endregion
        
        #region Remove
        /// <summary>
        /// Removes the specified <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> from the <see cref="global::System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> to remove.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public void Remove(global::aZaaS.KStar.Authentication.ParameterElement parameter)
        {
            base.BaseRemove(this.GetElementKey(parameter));
        }
        #endregion
        
        #region GetItem
        /// <summary>
        /// Gets the <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public global::aZaaS.KStar.Authentication.ParameterElement GetItemAt(int index)
        {
            return ((global::aZaaS.KStar.Authentication.ParameterElement)(base.BaseGet(index)));
        }
        
        /// <summary>
        /// Gets the <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> with the specified key.
        /// </summary>
        /// <param name="key">The key of the <see cref="global::aZaaS.KStar.Authentication.ParameterElement"/> to retrieve.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public global::aZaaS.KStar.Authentication.ParameterElement GetItemByKey(string key)
        {
            return ((global::aZaaS.KStar.Authentication.ParameterElement)(base.BaseGet(((object)(key)))));
        }
        #endregion
        
        #region IsReadOnly override
        /// <summary>
        /// Gets a value indicating whether the element is read-only.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ConfigurationSectionDesigner.CsdFileGenerator", "2.0.1.7")]
        public override bool IsReadOnly()
        {
            return false;
        }
        #endregion
    }
}