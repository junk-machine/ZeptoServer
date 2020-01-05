using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ZeptoServer.Configuration.Xml
{
    /// <summary>
    /// Defines all errors thrown by the console host.
    /// </summary>
    internal static class Errors
    {
        /// <summary>
        /// Error that is being thrown when requested configuration node is of a wrong type.
        /// </summary>
        /// <param name="expected">Expected root node type</param>
        /// <param name="actual">Actual root node type</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception RootHasWrongType(Type expected, Type actual)
        {
            return new InvalidCastException(
                String.Format(Resources.RootHasWrongTypeFormat, actual, expected));
        }

        /// <summary>
        /// Error that is being thrown when XML element was expected, but not found.
        /// </summary>
        /// <param name="actualNodeType">Actual XML node type</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception ElementExpected(XmlNodeType actualNodeType)
        {
            return new InvalidDataException(
                String.Format(Resources.ElementExpectedFormat, actualNodeType));
        }

        /// <summary>
        /// Error that is being thrown when class member is not found.
        /// </summary>
        /// <param name="type">Object type that should define the member</param>
        /// <param name="memberName">Name of the missing member</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception MissingMember(Type type, string memberName)
        {
            return new MissingMemberException(type.FullName, memberName);
        }

        /// <summary>
        /// Error that is being thrown when CLR namespace reference cannot be parsed.
        /// </summary>
        /// <param name="namespaceReference">CLR namespace reference</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception InvalidNamespaceReference(string namespaceReference)
        {
            return new FormatException(
                String.Format(Resources.InvalidNamespaceReferenceFormat, namespaceReference));
        }

        /// <summary>
        /// Error that is being thrown when CLR type cannot be loaded.
        /// </summary>
        /// <param name="typeName">Full or partial name of the CLR type</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception CannotLoadType(string typeName)
        {
            return new FormatException(
                String.Format(Resources.CannotLoadTypeFormat, typeName));
        }

        /// <summary>
        /// Error that is being thrown when object cannot be created.
        /// </summary>
        /// <param name="objectType">Type of the object</param>
        /// <param name="constructorArguments">Constructor arguments</param>
        /// <param name="rootCause">Root cause of the error</param>
        /// <returns>An exception that should be thrown.</returns>
        public static Exception CannotCreateInstance(Type objectType, IEnumerable<object> constructorArguments, Exception rootCause)
        {
            return new InvalidOperationException(
                String.Format(
                    Resources.CannotCreateInstanceFormat,
                    objectType.FullName,
                    constructorArguments == null ? null : String.Join(", ", constructorArguments.Select(a => a?.GetType()))),
                rootCause);
        }
    }
}
