using System.Diagnostics;

namespace Unity.VisualScripting
{
    public class ProfiledSegment
    {
        public ProfiledSegment(ProfiledSegment parent, string name)
        {
            this.parent = parent;
            this.name = name;
            stopwatch = new Stopwatch();
            children = new ProfiledSegmentCollection();
        }

        public string name { get; private set; }
        public Stopwatch stopwatch { get; private set; }
        public long calls { get; set; }
        public ProfiledSegment parent { get; private set; }
        public ProfiledSegmentCollection children { get; private set; }
    }
}
