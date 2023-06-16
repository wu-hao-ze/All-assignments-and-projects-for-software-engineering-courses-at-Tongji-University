// Copyright Christophe Bertrand.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.VisualScripting.AssemblyQualifiedNameParser
{
    public class ParsedAssemblyQualifiedName
    {
        public string AssemblyDescriptionString { get; }

        public string TypeName { get; private set; }

        public string ShortAssemblyName { get; }

        public string Version { get; }

        public string Culture { get; }

        public string PublicKeyToken { get; }

        public List<ParsedAssemblyQualifiedName> GenericParameters { get; } = new List<ParsedAssemblyQualifiedName>();

        public int GenericParameterCount { get; }

        public ParsedAssemblyQualifiedName(string AssemblyQualifiedName)
        {
            var typeNameLength = AssemblyQualifiedName.Length;
            var hasAssemblyDescription = false;

            var rootBlock = new Block();
            {
                var depth = 0;
                var currentBlock = rootBlock;

                for (var index = 0; index < AssemblyQualifiedName.Length; ++index)
                {
                    var c = AssemblyQualifiedName[index];

                    if (c == '[')
                    {
                        if (AssemblyQualifiedName[index + 1] == ']') // Array type // TODO (LAZLO): This won't detect multidimensional array, but FS can't handle them anyway
                        {
                            index++;
                        }
                        else
                        {
                            if (depth == 0)
                            {
                                typeNameLength = index;
                            }

                            ++depth;

                            var innerBlock = new Block
                            {
                                startIndex = index + 1,
                                level = depth,
                                parentBlock = currentBlock
                            };

                            currentBlock.innerBlocks.Add(innerBlock);

                            currentBlock = innerBlock;
                        }
                    }
                    else if (c == ']')
                    {
                        currentBlock.endIndex = index - 1;

                        if (AssemblyQualifiedName[currentBlock.startIndex] != '[')
                        {
                            currentBlock.parsedAssemblyQualifiedName = new ParsedAssemblyQualifiedName(AssemblyQualifiedName.Substring(currentBlock.startIndex, index - currentBlock.startIndex));

                            if (depth == 2)
                            {
                                GenericParameters.Add(currentBlock.parsedAssemblyQualifiedName);
                            }
                        }

                        currentBlock = currentBlock.parentBlock;
                        --depth;
                    }
                    else if (depth == 0 && c == ',')
                    {
                        typeNameLength = index;
                        hasAssemblyDescription = true;

                        break;
                    }
                }
            }

            TypeName = AssemblyQualifiedName.Substring(0, typeNameLength);

            var tickIndex = TypeName.IndexOf('`');

            if (tickIndex >= 0)
            {
                TypeName = TypeName.Substring(0, tickIndex);
                GenericParameterCount = GenericParameters.Count;
            }

            if (hasAssemblyDescription)
            {
                AssemblyDescriptionString = AssemblyQualifiedName.Substring(typeNameLength + 2);

                var parts = AssemblyDescriptionString.Split(',')
                    .Select(x => x.Trim())
                    .ToList();

                Version = LookForPairThenRemove(parts, "Version");
                Culture = LookForPairThenRemove(parts, "Culture");
                PublicKeyToken = LookForPairThenRemove(parts, "PublicKeyToken");

                if (parts.Count > 0)
                {
                    ShortAssemblyName = parts[0];
                }
            }
        }

        private class Block
        {
            internal int startIndex;

            internal int endIndex;

            internal int level;

            internal Block parentBlock;

            internal readonly List<Block> innerBlocks = new List<Block>();

            internal ParsedAssemblyQualifiedName parsedAssemblyQualifiedName;
        }

        private static string LookForPairThenRemove(List<string> strings, string Name)
        {
            for (var istr = 0; istr < strings.Count; istr++)
            {
                var s = strings[istr];
                var i = s.IndexOf(Name);

                if (i == 0)
                {
                    var i2 = s.IndexOf('=');

                    if (i2 > 0)
                    {
                        var ret = s.Substring(i2 + 1);
                        strings.RemoveAt(istr);

                        return ret;
                    }
                }
            }

            return null;
        }

        public void Replace(string oldTypeName, string newTypeName)
        {
            if (TypeName == oldTypeName)
            {
                TypeName = newTypeName;
            }

            foreach (var genericParameter in GenericParameters)
            {
                genericParameter.Replace(oldTypeName, newTypeName);
            }
        }

        private string ToString(bool includeAssemblyDescription)
        {
            var sb = new StringBuilder();

            sb.Append(TypeName);

            if (GenericParameters.Count > 0)
            {
                sb.Append("`");

                sb.Append(GenericParameterCount);

                sb.Append("[[");

                foreach (var genericParameter in GenericParameters)
                {
                    sb.Append(genericParameter.ToString(true));
                }

                sb.Append("]]");
            }

            if (includeAssemblyDescription)
            {
                sb.Append(", ");

                sb.Append(AssemblyDescriptionString);
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(false);
        }
    }
}
