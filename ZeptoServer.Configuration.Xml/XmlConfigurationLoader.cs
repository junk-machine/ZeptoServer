using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ZeptoServer.Configuration.Xml
{
    /// <summary>
    /// Instantiates classes based on XML definition.
    /// </summary>
    /// <remarks>
    /// This is drastically simplified version of XAML.
    /// </remarks>
    public sealed class XmlConfigurationLoader
    {
        /// <summary>
        /// Name of the attribute that indicates a NULL value.
        /// </summary>
        private const string NullAttribute = "Null";

        /// <summary>
        /// Name of the attribute to access static member of a type or enum value.
        /// </summary>
        private const string MemberAttribute = "Member";

        /// <summary>
        /// Name of the attribute to create a list of objects.
        /// </summary>
        private const string ListAttribute = "List";

        /// <summary>
        /// Regular expression to parse CLR namespace reference from namespace URI.
        /// </summary>
        private static readonly Regex NamespaceReferenceRegex =
            new Regex(
                @"clr-namespace:(?<namespace>[^;]+)(;\s*assembly=(?<assembly>.+))?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Creates an object based on the given XML string.
        /// </summary>
        /// <typeparam name="T">Type of the expected object</typeparam>
        /// <param name="inputXml">XML string that defines an object</param>
        /// <returns>Instance of the object from XML definition.</returns>
        public T Load<T>(string inputXml)
        {
            using (var stringReader = new StringReader(inputXml))
            {
                return Load<T>(stringReader);
            }
        }

        /// <summary>
        /// Creates an object based on the XML from the text reader.
        /// </summary>
        /// <typeparam name="T">Type of the expected object</typeparam>
        /// <param name="textReader">Text reader to read XML from</param>
        /// <returns>Instance of the object from XML definition.</returns>
        public T Load<T>(TextReader textReader)
        {
            using (XmlReader xmlReader = XmlReader.Create(textReader))
            {
                xmlReader.MoveToContent();

                var result = GetValueForElement(xmlReader);

                if (!(result is T))
                {
                    throw Errors.RootHasWrongType(result?.GetType(), typeof(T));
                }

                return (T)result;
            }
        }

        /// <summary>
        /// Creates a value based on the XML element.
        /// </summary>
        /// <param name="xmlReader">XML reader to read the element from</param>
        /// <returns>Value that corresponds to the next element in the reader.</returns>
        private object GetValueForElement(XmlReader xmlReader)
        {
            if (!xmlReader.IsStartElement())
            {
                throw Errors.ElementExpected(xmlReader.NodeType);
            }

            object result;
            string attributeValue;

            if (bool.TrueString.Equals(xmlReader.GetAttribute(NullAttribute), StringComparison.OrdinalIgnoreCase))
            {
                // 'Null' attribute is set to 'True'
                result = null;
            }
            else
            {
                var objectType = GetTypeForElement(xmlReader);

                if (!String.IsNullOrEmpty(attributeValue = xmlReader.GetAttribute(MemberAttribute)))
                {
                    // 'Member' attribute is defined
                    result = GetTypeMember(objectType, attributeValue);
                }
                else if (bool.TrueString.Equals(xmlReader.GetAttribute(ListAttribute), StringComparison.OrdinalIgnoreCase))
                {
                    // 'List' attribute is set to 'True'
                    result = CreateObjectsList(xmlReader, objectType);
                }
                else if (xmlReader.IsEmptyElement)
                {
                    // Self-closed element without any attributes maps to parameter-less constructor
                    result = CreateObject(objectType);
                }
                else
                {
                    // Process element contents
                    result = CreateObjectForElement(xmlReader, objectType);
                }
            }

            // Done reading current element, skip over it.
            // This takes care of self-closed elements and separate closing tags.
            xmlReader.Skip();

            return result;
        }

        /// <summary>
        /// Retrieves a value of static field or property, or enum value.
        /// </summary>
        /// <param name="objectType">Type that defines a member</param>
        /// <param name="memberName">Name of the member</param>
        /// <returns>The member value.</returns>
        private object GetTypeMember(Type objectType, string memberName)
        {
            if (objectType.IsEnum)
            {
                // Get enum value
                return Enum.Parse(objectType, memberName);
            }

            // Try get static field value
            var staticField =
                objectType.GetField(memberName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            if (staticField != null)
            {
                return staticField.GetValue(null);
            }

            // Try get static property value
            var staticProperty =
                objectType.GetProperty(memberName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            if (staticProperty != null)
            {
                return staticProperty.GetValue(null);
            }

            throw Errors.MissingMember(objectType, memberName);
        }

        /// <summary>
        /// Creates list of objects based on the XML definition.
        /// </summary>
        /// <param name="xmlReader">XML reader to read object definitions from</param>
        /// <param name="elementType">Type of the list element</param>
        /// <returns>List of objects that corresponds to the next element in the reader.</returns>
        private object CreateObjectsList(XmlReader xmlReader, Type elementType)
        {
            var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            
            var parentDepth = xmlReader.Depth;
            
            xmlReader.ReadStartElement();

            while (xmlReader.IsStartElement() && xmlReader.Depth > parentDepth)
            {
                result.Add(GetValueForElement(xmlReader));
            }

            return result;
        }

        /// <summary>
        /// Creates new instance of the object from the next element in the XML reader
        /// </summary>
        /// <param name="xmlReader">XML reader to read object definition from</param>
        /// <param name="objectType">Expected type of the object</param>
        /// <returns>Object that corresponds to the next element in the reader.</returns>
        private object CreateObjectForElement(XmlReader xmlReader, Type objectType)
        {
            var parentDepth = xmlReader.Depth;

            xmlReader.ReadStartElement();

            var content = xmlReader.HasValue ? xmlReader.ReadContentAsString() : null;

            if (!String.IsNullOrEmpty(content) && !xmlReader.IsStartElement())
            {
                // If there is text content, but no children - treat it as a literal value.
                // Indentation (whitespaces and line feeds) are treated as content too,
                // so we need to check for children.
                return Convert.ChangeType(content, objectType);
            }

            // Read children elements as constructor arguments
            var constructorArguments = new List<object>();

            while (xmlReader.IsStartElement() && xmlReader.Depth > parentDepth)
            {
                constructorArguments.Add(GetValueForElement(xmlReader));
            }

            // Create new instance of the object
            return CreateObject(objectType, constructorArguments);
        }

        /// <summary>
        /// Retrieves the CLR type that corresponds to the next element in the reader.
        /// </summary>
        /// <param name="xmlReader">XML reader to read an object type from</param>
        /// <returns>The CLR type for the element.</returns>
        private Type GetTypeForElement(XmlReader xmlReader)
        {
            var namespaceReference = NamespaceReferenceRegex.Match(xmlReader.NamespaceURI);

            if (!namespaceReference.Success)
            {
                throw Errors.InvalidNamespaceReference(xmlReader.NamespaceURI);
            }

            var typeName = new StringBuilder(namespaceReference.Groups["namespace"].Value);
            typeName.Append('.');
            typeName.Append(xmlReader.LocalName);

            var assemblyName = namespaceReference.Groups["assembly"].Value;
            if (!String.IsNullOrEmpty(assemblyName))
            {
                typeName.Append(", ");
                typeName.Append(assemblyName);
            }

            return Type.GetType(typeName.ToString())
                ?? throw Errors.CannotLoadType(typeName.ToString());
        }

        /// <summary>
        /// Creates a new instance of the object of the given <paramref name="objectType"/>
        /// using constructor that matches given set of arguments.
        /// </summary>
        /// <param name="objectType">Type of the object to create</param>
        /// <param name="constructorArguments">Constructor arguments</param>
        /// <returns>New instance of the object</returns>
        private static object CreateObject(Type objectType, List<object> constructorArguments = null)
        {
            try
            {
                return
                    constructorArguments != null
                        ? Activator.CreateInstance(objectType, constructorArguments.ToArray())
                        : Activator.CreateInstance(objectType);
            }
            catch (Exception error)
            {
                throw Errors.CannotCreateInstance(objectType, constructorArguments, error);
            }
        }
    }
}
