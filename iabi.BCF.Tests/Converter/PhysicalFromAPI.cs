using System;
using System.Linq;
using iabi.BCF.APIObjects.Extensions;
using iabi.BCF.BCFv2.Schemas;
using iabi.BCF.Converter;
using Xunit;

namespace iabi.BCF.Tests.Converter
{
     
    public class PhysicalFromAPI
    {
         
        public class SnapshotConversion
        {
            [Fact]
            public void ReadSnapshotInfo()
            {
                var APIContainerInstance = new APIContainer();
                APIContainerInstance.Topics.Add(new TopicContainer());
                APIContainerInstance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                APIContainerInstance.Topics.First().Viewpoints.Add(new ViewpointContainer());
                APIContainerInstance.Topics.First().Viewpoints.First().Viewpoint = new BCF.APIObjects.Viewpoint.viewpoint_GET();
                APIContainerInstance.Topics.First().Viewpoints.First().Snapshot = new byte[] { 15, 15, 15, 15, 15, 15 };

                var ReadContainer = iabi.BCF.Converter.PhysicalFromAPI.Convert(APIContainerInstance);

                // Viewpoint present
                Assert.NotNull(ReadContainer.Topics.First().Viewpoints.First());
                Assert.True(ReadContainer.Topics.First().ViewpointSnapshots.ContainsKey(ReadContainer.Topics.First().Viewpoints.First().GUID));

                // Viewpoint set in markup
                var MarkupEntry = ReadContainer.Topics.First().Markup.Viewpoints.FirstOrDefault();
                Assert.NotNull(MarkupEntry);
                Assert.Equal(ReadContainer.Topics.First().Viewpoints.First().GUID, MarkupEntry.Guid);
            }
        }

         
        public class ExtensionsConversion
        {
            [Fact]
            public void DontCreateExtensionsWhenNotPresent()
            {
                APIContainer Instance = new APIContainer();
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                Assert.Null(BCFv2Instance.ProjectExtensions);
            }

            [Fact]
            public void DontCreateExtensionsWhenPresent_ButEmpty()
            {
                APIContainer Instance = new APIContainer();
                Instance.Extensions = new BCF.APIObjects.Extensions.extensions_GET();
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                Assert.Null(BCFv2Instance.ProjectExtensions);
            }

            [Fact]
            public void CreateExtensionsWhenPresent()
            {
                APIContainer Instance = new APIContainer();
                Instance.Extensions = new BCF.APIObjects.Extensions.extensions_GET();
                Instance.Extensions.user_id_type.Add("Some value");
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                Assert.NotNull(BCFv2Instance.ProjectExtensions);
            }

            [Fact]
            public void CreatedExtensionsMatch()
            {
                APIContainer Instance = new APIContainer();
                Instance.Extensions = new BCF.APIObjects.Extensions.extensions_GET();

                Instance.Extensions.priority.Add("Lorem");
                Instance.Extensions.priority.Add("ipsum");
                Instance.Extensions.snippet_type.Add("dolor");
                Instance.Extensions.snippet_type.Add("sit");
                Instance.Extensions.topic_label.Add("amet");
                Instance.Extensions.topic_label.Add("consetetur");
                Instance.Extensions.topic_status.Add("sadipscing");
                Instance.Extensions.topic_status.Add("elitr");
                Instance.Extensions.topic_type.Add("sed");
                Instance.Extensions.topic_type.Add("diam");
                Instance.Extensions.user_id_type.Add("nonumy");
                Instance.Extensions.user_id_type.Add("eirmod");

                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);

                CompareExtensions(Instance.Extensions, BCFv2Instance.ProjectExtensions);
            }

            private void CompareExtensions(extensions_GET ExpectedExtensions, Extensions_XSD ActualExtensions)
            {
                Assert.True(ExpectedExtensions.priority.SequenceEqual(ActualExtensions.Priority));
                Assert.True(ExpectedExtensions.snippet_type.SequenceEqual(ActualExtensions.SnippetType));
                Assert.True(ExpectedExtensions.topic_label.SequenceEqual(ActualExtensions.TopicLabel));
                Assert.True(ExpectedExtensions.topic_status.SequenceEqual(ActualExtensions.TopicStatus));
                Assert.True(ExpectedExtensions.topic_type.SequenceEqual(ActualExtensions.TopicType));
                Assert.True(ExpectedExtensions.user_id_type.SequenceEqual(ActualExtensions.UserIdType));
            }
        }

         
        public class GeneralTests
        {
            [Fact]
            public void ConvertEmptyContainer()
            {
                var APIContainerInstance = new APIContainer();
                var ReadContainer = iabi.BCF.Converter.PhysicalFromAPI.Convert(APIContainerInstance);
                //var ReadContainer = new iabi.BCF.Converter.PhysicalFromAPI(APIContainerInstance).BCFv2;
                Assert.NotNull(ReadContainer);
            }

            [Fact]
            public void ConvertContainerWithEmptyViewpoint()
            {
                var APIContainerInstance = new APIContainer();
                APIContainerInstance.Topics.Add(new TopicContainer());
                APIContainerInstance.Topics.First().Viewpoints.Add(new ViewpointContainer());

                Assert.Throws(typeof (ArgumentNullException), () =>
                {
                    var ReadContainer = iabi.BCF.Converter.PhysicalFromAPI.Convert(APIContainerInstance);
                });
            }
        }

         
        public class TopicInformation
        {
            [Fact]
            public void NoModifiedInfoWhenNotPresent()
            {
                APIContainer Instance = new APIContainer();
                Instance.Topics.Add(new TopicContainer());
                Instance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                //var BCFv2Instance = new iabi.BCF.Converter.PhysicalFromAPI(Instance).BCFv2;
                Assert.True(string.IsNullOrWhiteSpace(BCFv2Instance.Topics.First().Markup.Topic.ModifiedAuthor));
                Assert.False(BCFv2Instance.Topics.First().Markup.Topic.ModifiedDateSpecified);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_01()
            {
                APIContainer Instance = new APIContainer();
                Instance.Topics.Add(new TopicContainer());
                Instance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                Instance.Topics.First().Topic.modified_author = "Georg";
                Instance.Topics.First().Topic.modified_date = new DateTime(2015, 06, 06, 15, 47, 18);
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                //var BCFv2Instance = new iabi.BCF.Converter.PhysicalFromAPI(Instance).BCFv2;
                Assert.Equal("Georg", BCFv2Instance.Topics.First().Markup.Topic.ModifiedAuthor);
                Assert.Equal(new DateTime(2015, 06, 06, 15, 47, 18), BCFv2Instance.Topics.First().Markup.Topic.ModifiedDate);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_02()
            {
                APIContainer Instance = new APIContainer();
                Instance.Topics.Add(new TopicContainer());
                Instance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                Instance.Topics.First().Topic.modified_date = new DateTime(2015, 06, 06, 15, 47, 18);
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                //var BCFv2Instance = new iabi.BCF.Converter.PhysicalFromAPI(Instance).BCFv2;
                Assert.True(string.IsNullOrWhiteSpace(BCFv2Instance.Topics.First().Markup.Topic.ModifiedAuthor));
                Assert.Equal(new DateTime(2015, 06, 06, 15, 47, 18), BCFv2Instance.Topics.First().Markup.Topic.ModifiedDate);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_03()
            {
                APIContainer Instance = new APIContainer();
                Instance.Topics.Add(new TopicContainer());
                Instance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                Instance.Topics.First().Topic.modified_author = "Georg";
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                //var BCFv2Instance = new iabi.BCF.Converter.PhysicalFromAPI(Instance).BCFv2;
                Assert.Equal("Georg", BCFv2Instance.Topics.First().Markup.Topic.ModifiedAuthor);
                Assert.False(BCFv2Instance.Topics.First().Markup.Topic.ModifiedDateSpecified);
            }
        }

         
        public class CommentInformation
        {
            [Fact]
            public void NoModifiedInfoWhenNotPresent()
            {
                APIContainer Instance = new APIContainer();
                Instance.Topics.Add(new TopicContainer());
                Instance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                Instance.Topics.First().Comments.Add(new BCF.APIObjects.Comment.comment_GET());
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                //var BCFv2Instance = new iabi.BCF.Converter.PhysicalFromAPI(Instance).BCFv2;
                Assert.True(string.IsNullOrWhiteSpace(BCFv2Instance.Topics.First().Markup.Comment.First().ModifiedAuthor));
                Assert.False(BCFv2Instance.Topics.First().Markup.Comment.First().ModifiedDateSpecified);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_01()
            {
                APIContainer Instance = new APIContainer();
                Instance.Topics.Add(new TopicContainer());
                Instance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                Instance.Topics.First().Comments.Add(new BCF.APIObjects.Comment.comment_GET());
                Instance.Topics.First().Comments.First().modified_author = "Georg";
                Instance.Topics.First().Comments.First().modified_date = new DateTime(2015, 06, 06, 15, 47, 18);
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                //var BCFv2Instance = new iabi.BCF.Converter.PhysicalFromAPI(Instance).BCFv2;
                Assert.Equal("Georg", BCFv2Instance.Topics.First().Markup.Comment.First().ModifiedAuthor);
                Assert.Equal(new DateTime(2015, 06, 06, 15, 47, 18), BCFv2Instance.Topics.First().Markup.Comment.First().ModifiedDate);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_02()
            {
                APIContainer Instance = new APIContainer();
                Instance.Topics.Add(new TopicContainer());
                Instance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                Instance.Topics.First().Comments.Add(new BCF.APIObjects.Comment.comment_GET());
                Instance.Topics.First().Comments.First().modified_date = new DateTime(2015, 06, 06, 15, 47, 18);
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                //var BCFv2Instance = new iabi.BCF.Converter.PhysicalFromAPI(Instance).BCFv2;
                Assert.True(string.IsNullOrWhiteSpace(BCFv2Instance.Topics.First().Markup.Comment.First().ModifiedAuthor));
                Assert.Equal(new DateTime(2015, 06, 06, 15, 47, 18), BCFv2Instance.Topics.First().Markup.Comment.First().ModifiedDate);
            }

            [Fact]
            public void ModifiedInfoWhenSpecified_03()
            {
                APIContainer Instance = new APIContainer();
                Instance.Topics.Add(new TopicContainer());
                Instance.Topics.First().Topic = new BCF.APIObjects.Topic.topic_GET();
                Instance.Topics.First().Comments.Add(new BCF.APIObjects.Comment.comment_GET());
                Instance.Topics.First().Comments.First().modified_author = "Georg";
                var BCFv2Instance = iabi.BCF.Converter.PhysicalFromAPI.Convert(Instance);
                //var BCFv2Instance = new iabi.BCF.Converter.PhysicalFromAPI(Instance).BCFv2;
                Assert.Equal("Georg", BCFv2Instance.Topics.First().Markup.Comment.First().ModifiedAuthor);
                Assert.False(BCFv2Instance.Topics.First().Markup.Comment.First().ModifiedDateSpecified);
            }
        }
    }
}
