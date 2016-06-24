<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="d0ed9acb-0435-4532-afdd-b5115bc4d562" namespace="aZaaS.KStar.Authentication" xmlSchemaNamespace="azaas.kstar" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="UserAuthProviderSection" namespace="aZaaS.KStar.Authentication" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="userAuthProvider">
      <attributeProperties>
        <attributeProperty name="DefaultProvider" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="defaultProvider" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="UserExistsValidation" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="userExistsValidation" isReadOnly="false" defaultValue="true">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="AuthProviders" isRequired="false" isKey="false" isDefaultCollection="true" xmlName="authProviders" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/AuthProviderCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="AuthProviderCollection" namespace="aZaaS.KStar.Authentication" xmlItemName="authProvider" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/AuthProviderElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="AuthProviderElement" namespace="aZaaS.KStar.Authentication">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="AssemblyType" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="assemblyType" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Parameters" isRequired="false" isKey="false" isDefaultCollection="true" xmlName="parameters" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/ParameterCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="ParameterElement" namespace="aZaaS.KStar.Authentication">
      <attributeProperties>
        <attributeProperty name="Key" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="key" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Value" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="ParameterCollection" namespace="aZaaS.KStar.Authentication" xmlItemName="parameter" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/ParameterElement" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>