using System;

namespace Unity.VisualScripting
{
    public interface IAnalyticsIdentifiable
    {
        AnalyticsIdentifier GetAnalyticsIdentifier();
    }

    public class AnalyticsIdentifier
    {
        public string Identifier;
        public string Namespace;
        public int Hashcode;
    }
}
