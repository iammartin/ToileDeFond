using System;
using System.Collections.Generic;

namespace ToileDeFond.Tests.Common
{
    public static class TestsConstants
    {
        public static class ContentType
        {
            public const string FolderTypeName = "Folder";
            public const string SubFolderTypeName = "SubFolder";
            public const string NamePropertyModifiedValue = "Modified";
            public const string ReleaseDatePropertyName = "ReleaseDate";
            public const string NamePropertyName = "Name";
            public const string NamePropertyDefaultValue = "Alberto";
            public static DateTime ReleaseDatePropertyDefaultValue = DateTime.Now;
            public const string ParentFolderPropertyName = "ParentFolder";
            public static Guid ParentFolderPropertyDefaultValue = new Guid("1CFE174E-6BDD-4E15-ABF1-D244F6E68C99");
            public const string RelatedFoldersPropertyName = "RelatedFoldersPropertyName";
            public static List<Guid> RelatedFoldersPropertyDefaultValue = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            public static string AggregatedFolderPropertyName = "AggregatedFolderPropertyName";
            public static string AggregatedFoldersPropertyName = "AggregatedFoldersPropertyName";
        }
        public static class Module
        {
            public const string FullName = "ToileDeFond.TestModule";
            public const string Version = "1.0.0.0";
        }
    }
}
