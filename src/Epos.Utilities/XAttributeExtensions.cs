namespace System.Xml.Linq;

/// <summary>
/// Provides extension methods for working with XML attributes in LINQ to XML.
/// </summary>
public static class XAttributeExtensions
{
    /// <summary>
    /// Retrieves the specified attribute from the given XML element. Throws an exception if the attribute is not found.
    /// </summary>
    /// <param name="element">The XML element to search for the attribute.</param>
    /// <param name="name">The name of the attribute to retrieve.</param>
    /// <returns>The requested <see cref="XAttribute"/> if found.</returns>
    /// <exception cref="ArgumentException">Thrown if the attribute is not found in the element.</exception>
    public static XAttribute RequiredAttribute(this XElement element, XName name)
        => element.Attribute(name) ??
           throw new ArgumentException($"Required attribute '{name}' not found in element '{element.Name}'.");

    /// <summary>
    /// Retrieves the specified element from the given XML element. Throws an exception if the element is not found.
    /// </summary>
    /// <param name="element">The XML element to search for the child element.</param>
    /// <param name="name">The name of the child element to retrieve.</param>
    /// <returns>The requested <see cref="XElement"/> if found.</returns>
    /// <exception cref="ArgumentException">Thrown if the child element is not found in the element.</exception>
    public static XElement RequiredElement(this XElement element, XName name)
        => element.Element(name) ??
           throw new ArgumentException($"Required element '{name}' not found in element '{element.Name}'.");

    /// <summary>
    /// Retrieves the root element from the given XML document. Throws an exception if the root element is not found.
    /// </summary>
    /// <param name="document">The XML document to retrieve the root element from.</param>
    /// <returns>The root <see cref="XElement"/> of the document.</returns>
    /// <exception cref="ArgumentException">Thrown if the root element is not found in the document.</exception>
    public static XElement RequiredRoot(this XDocument document)
        => document.Root ??
           throw new ArgumentException($"Required root element not found in document '{document.Declaration}'.");
}
