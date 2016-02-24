using iabi.BCF.BCFv2;
using iabi.BCF.BCFv2.Schemas;
using System;
using System.Collections.Generic;

namespace iabi.BCF.Test.BCFTestCases.CreateAndExport.Factory
{
    public static class DocumentReferenceExternalTestCase
    {
        public static BCFv2Container CreateContainer()
        {
            var Container = new BCFv2Container();
            Container.Topics.Add(CreateTopic());
            return Container;
        }

        public static BCFTopic CreateTopic()
        {
            var ReturnTopic = new BCFTopic();
            ReturnTopic.Markup = CreateMarkup();
            return ReturnTopic;
        }

        private static Markup CreateMarkup()
        {
            var Markup = new Markup();
            Markup.Topic = new Topic
            {
                CreationAuthor = "dangl@iabi.eu",
                CreationDate = new DateTime(2015, 10, 11, 12, 13, 14, DateTimeKind.Utc),
                Description = "This topic has a document reference to an external file via an absolute URL. It references the old BCFv1 Markup Schema.",
                Guid = BCFTestCaseData.DocumentReferenceExternal_TopicGuid,
                Index = "0",
                Title = "Document Reference External",
                DocumentReferences = new List<TopicDocumentReferences>
                {
                    new TopicDocumentReferences
                    {
                        Description = "BCFv1 Markup Schema",
                        isExternal = true,
                        ReferencedDocument = "http://www.buildingsmart-tech.org/specifications/bcf-releases/bcfxml-v1/markup.xsd/at_download/file"
                    }
                }
            };
            return Markup;
        }
    }
}