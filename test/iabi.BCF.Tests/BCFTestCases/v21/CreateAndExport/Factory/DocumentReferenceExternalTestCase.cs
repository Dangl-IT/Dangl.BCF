﻿using System;
using System.Collections.Generic;
using iabi.BCF.BCFv21;
using iabi.BCF.BCFv21.Schemas;

namespace iabi.BCF.Tests.BCFTestCases.v21.CreateAndExport.Factory
{
    public static class DocumentReferenceExternalTestCase
    {
        public static BCFv21Container CreateContainer()
        {
            var Container = new BCFv21Container();
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
                Guid = BCFv21TestCaseData.DocumentReferenceExternal_TopicGuid,
                Index = 0,
                Title = "Document Reference External",
                DocumentReference = new List<TopicDocumentReference>
                {
                    new TopicDocumentReference
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