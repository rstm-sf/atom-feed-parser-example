using System;
using System.Collections.Generic;

namespace atom_feed_parser_example
{
    internal class Package
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public override string ToString() => Id + " " + Version;
    }

    internal class Dependency
    {
        public string Framework { get; set; }

        public List<Package> Packages { get; set; }
    }

    internal class FeedPackage
    {
        public Package Package { get; set; }

        public string Description { get; set; }

        public List<Dependency> Dependencies { get; set; }

        public string GalleryDetailsUrl { get; set; }

        public DateTime? LastEdited { get; set; }
    }
}
