using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Unity.VisualScripting
{
    public class XmlDocumentationTags
    {
        public XmlDocumentationTags()
        {
            parameterTypes = new Dictionary<string, Type>();
            parameters = new Dictionary<string, string>();
            typeParameters = new Dictionary<string, string>();
        }

        public XmlDocumentationTags(string summary) : this()
        {
            this.summary = summary;
        }

        public XmlDocumentationTags(XElement xml) : this()
        {
            foreach (var childNode in xml.Elements())
            {
                if (childNode.Name.LocalName == "summary")
                {
                    summary = ProcessText(childNode.Value);
                }
                else if (childNode.Name.LocalName == "returns")
                {
                    returns = ProcessText(childNode.Value);
                }
                else if (childNode.Name.LocalName == "remarks")
                {
                    remarks = ProcessText(childNode.Value);
                }
                else if (childNode.Name.LocalName == "param")
                {
                    var paramText = ProcessText(childNode.Value);

                    var nameAttribute = childNode.Attribute("name");

                    if (paramText != null && nameAttribute != null)
                    {
                        if (parameters.ContainsKey(nameAttribute.Value))
                        {
                            parameters[nameAttribute.Value] = paramText;
                        }
                        else
                        {
                            parameters.Add(nameAttribute.Value, paramText);
                        }
                    }
                }
                else if (childNode.Name.LocalName == "typeparam")
                {
                    var typeParamText = ProcessText(childNode.Value);

                    var nameAttribute = childNode.Attribute("name");

                    if (typeParamText != null && nameAttribute != null)
                    {
                        if (typeParameters.ContainsKey(nameAttribute.Value))
                        {
                            typeParameters[nameAttribute.Value] = typeParamText;
                        }
                        else
                        {
                            typeParameters.Add(nameAttribute.Value, typeParamText);
                        }
                    }
                }
                else if (childNode.Name.LocalName == "inheritdoc")
                {
                    inherit = true;
                }
            }
        }

        private bool methodBaseCompleted = true;

        public string summary { get; set; }
        public string returns { get; set; }
        public string remarks { get; set; }
        public Dictionary<string, string> parameters { get; private set; }
        public Dictionary<string, string> typeParameters { get; private set; }
        public Dictionary<string, Type> parameterTypes { get; private set; }
        public Type returnType { get; set; }
        public bool inherit { get; private set; }

        public void CompleteWithMethodBase(MethodBase methodBase, Type returnType)
        {
            if (methodBaseCompleted)
            {
                return;
            }

            var parameterInfos = methodBase.GetParameters();

            foreach (var parameterInfo in parameterInfos)
            {
                parameterTypes.Add(parameterInfo.Name, parameterInfo.ParameterType);
            }

            // Remove parameter summaries if no matching parameter is found.
            // (Happens frequently in Unity methods)
            foreach (var parameter in parameters.ToArray())
            {
                if (parameterInfos.All(p => p.Name != parameter.Key))
                {
                    parameters.Remove(parameter.Key);
                }
            }

            methodBaseCompleted = true;
        }

        public string ParameterSummary(ParameterInfo parameter)
        {
            if (parameters.ContainsKey(parameter.Name))
            {
                return parameters[parameter.Name];
            }

            return null;
        }

        private static string ProcessText(string xmlText)
        {
            xmlText = string.Join(" ", xmlText.Trim().Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries));

            if (xmlText == string.Empty)
            {
                return null;
            }

            return xmlText;
        }
    }
}
